using ISynergy.Framework.Core.Events;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Stores messenger recipients as weak references in two indexes — one for derived/interface message
/// types and one for exact message types — and provides register, unregister and cleanup operations.
/// </summary>
/// <remarks>
/// Extracted from <see cref="MessengerService"/>, which now owns only the public surface and dispatch.
/// The index reads used by dispatch return snapshots so a caller can iterate without holding the lock.
/// </remarks>
internal sealed class MessengerRecipientRegistry
{
    private readonly object _recipientsLock = new();
    private readonly object _cleanupLock = new();

    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsOfSubclassesAction;
    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsStrictAction;

    /// <summary>
    /// Registers a recipient action.
    /// </summary>
    public void Register<TMessage>(
        object recipient,
        object? token,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive)
    {
        lock (_recipientsLock)
        {
            var messageType = typeof(TMessage);

            Dictionary<Type, List<WeakActionAndToken>> recipients;

            if (receiveDerivedMessagesToo)
            {
                _recipientsOfSubclassesAction ??= new Dictionary<Type, List<WeakActionAndToken>>();
                recipients = _recipientsOfSubclassesAction;
            }
            else
            {
                _recipientsStrictAction ??= new Dictionary<Type, List<WeakActionAndToken>>();
                recipients = _recipientsStrictAction;
            }

            if (!recipients.TryGetValue(messageType, out var list))
            {
                list = new List<WeakActionAndToken>();
                recipients.Add(messageType, list);
            }

            list.Add(new WeakActionAndToken
            {
                Action = new WeakAction<TMessage>(recipient, action, keepTargetAlive),
                Token = token
            });
        }
    }

    /// <summary>
    /// Marks every registration for <paramref name="recipient"/> for deletion.
    /// </summary>
    public void Unregister(object recipient)
    {
        UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
        UnregisterFromLists(recipient, _recipientsStrictAction);
    }

    /// <summary>
    /// Marks a specific registration for deletion.
    /// </summary>
    public void Unregister<TMessage>(object recipient, object? token, Action<TMessage>? action)
    {
        UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
        UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
    }

    /// <summary>
    /// Gets a value indicating whether any derived/interface recipients have been registered.
    /// </summary>
    public bool HasSubclassRecipients
    {
        get
        {
            lock (_recipientsLock)
            {
                return _recipientsOfSubclassesAction is not null;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether any exact-type recipients have been registered.
    /// </summary>
    public bool HasStrictRecipients
    {
        get
        {
            lock (_recipientsLock)
            {
                return _recipientsStrictAction is not null;
            }
        }
    }

    /// <summary>
    /// Snapshots the message types registered for derived/interface dispatch.
    /// </summary>
    public List<Type> GetSubclassIndexKeys()
    {
        lock (_recipientsLock)
        {
            return _recipientsOfSubclassesAction is null
                ? []
                : _recipientsOfSubclassesAction.Keys.ToList();
        }
    }

    /// <summary>
    /// Snapshots the recipient list registered for a derived/interface message type, or <see langword="null"/>.
    /// </summary>
    public List<WeakActionAndToken>? GetSubclassRecipients(Type type)
    {
        lock (_recipientsLock)
        {
            if (_recipientsOfSubclassesAction is not null &&
                _recipientsOfSubclassesAction.TryGetValue(type, out var sourceList) &&
                sourceList.Count > 0)
            {
                return sourceList.ToList();
            }

            return null;
        }
    }

    /// <summary>
    /// Snapshots the recipient list registered for an exact message type, or <see langword="null"/>.
    /// </summary>
    public List<WeakActionAndToken>? GetStrictRecipients(Type messageType)
    {
        lock (_recipientsLock)
        {
            if (_recipientsStrictAction is not null &&
                _recipientsStrictAction.TryGetValue(messageType, out var sourceList) &&
                sourceList.Count > 0)
            {
                return sourceList.ToList();
            }

            return null;
        }
    }

    /// <summary>
    /// Removes dead weak references from both indexes.
    /// </summary>
    public void Cleanup()
    {
        lock (_cleanupLock)
        {
            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
        }
    }

    private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (recipient is null || lists is null || lists.Count == 0)
        {
            return;
        }

        lock (lists)
        {
            foreach (var messageType in lists.Keys)
            {
                var items = lists[messageType];
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item?.Action is not null && item.Action.Target == recipient)
                    {
                        item.Action.MarkForDeletion();
                    }
                }
            }
        }
    }

    private static void UnregisterFromLists<TMessage>(
        object recipient,
        object? token,
        Action<TMessage>? action,
        Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        var messageType = typeof(TMessage);

        if (recipient is null
            || lists is null
            || lists.Count == 0
            || !lists.TryGetValue(messageType, out var items))
        {
            return;
        }

        lock (lists)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item?.Action is not null
                    && item.Action.Target == recipient
                    && (action is null || item.Action.MethodName == action.Method.Name)
                    && (token is null || item.Token is not null && item.Token.Equals(token)))
                {
                    item.Action.MarkForDeletion();
                }
            }
        }
    }

    private static void CleanupList(Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (lists is null)
        {
            return;
        }

        lock (lists)
        {
            var typesToRemove = new List<Type>();

            foreach (var kvp in lists)
            {
                var messageType = kvp.Key;
                var recipients = kvp.Value;

                // Remove dead references in reverse order to avoid index issues
                for (int i = recipients.Count - 1; i >= 0; i--)
                {
                    var item = recipients[i];
                    if (item?.Action is null || !item.Action.IsAlive)
                    {
                        recipients.RemoveAt(i);
                    }
                }

                if (recipients.Count == 0)
                {
                    typesToRemove.Add(messageType);
                }
            }

            foreach (var key in typesToRemove)
            {
                lists.Remove(key);
            }
        }
    }
}
