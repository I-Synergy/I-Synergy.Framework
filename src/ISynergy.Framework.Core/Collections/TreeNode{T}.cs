using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Core.Collections;

/// <summary>
/// Class TreeNode.
/// Implements the <see cref="ObservableClass" />
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <typeparam name="TKey">The type of the t identifier.</typeparam>
/// <typeparam name="TModel">The type of the t model.</typeparam>
/// <seealso cref="ObservableClass" />
/// <seealso cref="IDisposable" />
public class TreeNode<TKey, TModel> : ObservableClass
    where TKey : struct
    where TModel : class
{
    /// <summary>
    /// Gets or sets the IsSelected property value.
    /// </summary>
    /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
    public bool IsSelected
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsExpanded property value.
    /// </summary>
    /// <value><c>true</c> if this instance is expanded; otherwise, <c>false</c>.</value>
    public bool IsExpanded
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Key property value.
    /// </summary>
    public TKey Key
    {
        get => GetValue<TKey>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Data property value.
    /// </summary>
    /// <value>The data.</value>
    public TModel? Data
    {
        get => GetValue<TModel>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Parent property value.
    /// </summary>
    /// <value>The parent.</value>
    public TreeNode<TKey, TModel>? Parent
    {
        get => GetValue<TreeNode<TKey, TModel>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the ParentKey property value.
    /// </summary>
    public TKey ParentKey
    {
        get => GetValue<TKey>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeNode{TKey, TModel}"/> class.
    /// </summary>
    public TreeNode()
    {
        PropertyChanged += TreeNode_PropertyChanged;
        IsSelected = false;
        DisposeTraversal = UpDownTraversalTypes.BottomUp;
        Parent = null;
        Children = new ObservableCollection<TreeNode<TKey, TModel>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeNode{TKey, TModel}"/> class.
    /// </summary>
    /// <param name="data">The data.</param>
    public TreeNode(TModel data)
        : this()
    {
        Argument.IsNotNull(data);

        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeNode{TKey, TModel}"/> class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="parent">The parent.</param>
    public TreeNode(TModel data, TreeNode<TKey, TModel> parent)
        : this(data)
    {
        Argument.IsNotNull(parent);

        Parent = parent;
        ParentKey = parent.Key;
    }

    /// <summary>
    /// Adds the child.
    /// </summary>
    /// <param name="node">The node.</param>
    private TreeNode<TKey, TModel> AddChild(TreeNode<TKey, TModel> node)
    {
        Argument.IsNotNull(node);

        node.Parent = this;
        Children.Add(node);
        return node;
    }

    /// <summary>
    /// Adds the child.
    /// </summary>
    /// <param name="model">The node.</param>
    public TreeNode<TKey, TModel> AddChild(TModel model)
    {
        Argument.IsNotNull(model);

        var node = new TreeNode<TKey, TModel>(model, this);
        Children.Add(node);
        return node;
    }

    /// <summary>
    /// Sets the parent.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="updateChildNodes">if set to <c>true</c> [update child nodes].</param>
    public void SetParent(TreeNode<TKey, TModel>? node, bool updateChildNodes = true)
    {
        if (node == Parent)
            return;

        var oldParent = Parent;

        // if oldParent isn't null
        // remove this node from its newly ex-parent's children
        if (oldParent is not null && oldParent.Children.Contains(this))
            oldParent.RemoveChild(this);

        // update the backing field
        Parent = node;

        // add this node to its new parent's children
        if (Parent is not null && updateChildNodes)
            Parent.AddChild(this);
    }

    /// <summary>
    /// Gets or sets the Children property value.
    /// </summary>
    /// <value>The children.</value>
    public ObservableCollection<TreeNode<TKey, TModel>> Children
    {
        get => GetValue<ObservableCollection<TreeNode<TKey, TModel>>>();
        set => SetValue(value);
    }

    private void TreeNode_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName?.Equals(nameof(Parent)) == true)
        {
            if (Parent is not null)
            {
                ParentKey = Parent.Key;
            }
            else
            {
                ParentKey = default;
            }

            if (Data is not null && Data.HasParentIdentityProperty())
                Data.GetParentIdentityProperty().SetValue(Data, ParentKey);
        }
        else if (e.PropertyName?.Equals(nameof(Data)) == true &&
            Data is not null && Data.HasIdentityProperty())
        {
            Key = Data.GetIdentityValue<TModel, TKey>();
        }
    }

    /// <summary>
    /// Removes the child.
    /// </summary>
    /// <param name="node">The node.</param>
    public bool RemoveChild(TreeNode<TKey, TModel> node)
    {
        if (Children.Contains(node))
        {
            node.Parent = null;
            return Children.Remove(node);
        }

        return false;
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => Data?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets or sets the DisposeTraversal property value.
    /// </summary>
    /// <value>The dispose traversal.</value>
    public UpDownTraversalTypes DisposeTraversal
    {
        get { return GetValue<UpDownTraversalTypes>(); }
        set { SetValue(value); }
    }

    #region IDisposable
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    /// The bulk of the clean-up code is implemented in Dispose(bool)
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Data is IDisposable disposable)
            {
                if (DisposeTraversal == UpDownTraversalTypes.BottomUp)
                    foreach (var node in Children.EnsureNotNull())
                        node.Dispose();

                disposable.Dispose();

                if (DisposeTraversal == UpDownTraversalTypes.TopDown)
                    foreach (var node in Children.EnsureNotNull())
                        node.Dispose();
            }

            PropertyChanged -= TreeNode_PropertyChanged;
        }

        // free native resources if there are any.
    }
    #endregion
}
