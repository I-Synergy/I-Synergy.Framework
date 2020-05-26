using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Live;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Local
{
    /// <summary>
    /// The base class of all existing targets
    /// </summary>
    public abstract class EntryTarget : IExistingTarget
    {
        private readonly IEntry _entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryTarget"/> class.
        /// </summary>
        /// <param name="targetActions">The target actions implementation to use</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="destinationUrl">The destination URL for this entry</param>
        /// <param name="entry">The underlying entry</param>
        protected EntryTarget(
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions,
            CollectionTarget parent,
            Uri destinationUrl,
            IEntry entry)
        {
            TargetActions = targetActions;
            _entry = entry;
            Name = entry.Name;
            Parent = parent;
            DestinationUrl = destinationUrl;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Gets the parent collection target
        /// </summary>
        public CollectionTarget Parent { get; }

        /// <inheritdoc />
        public Uri DestinationUrl { get; }

        /// <summary>
        /// Gets the target actions implementation to use
        /// </summary>
        protected ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> TargetActions { get; }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<XName>> SetPropertiesAsync(IEnumerable<IUntypedWriteableProperty> properties, CancellationToken cancellationToken)
        {
            var liveProperties = new List<ILiveProperty>();
            var deadProperties = new List<IDeadProperty>();
            foreach (var property in properties)
            {
                var liveProp = property as ILiveProperty;
                if (liveProp != null)
                {
                    liveProperties.Add(liveProp);
                }
                else
                {
                    var deadProp = (IDeadProperty)property;
                    deadProperties.Add(deadProp);
                }
            }

            var livePropertiesResult = await SetPropertiesAsync(liveProperties, cancellationToken);

            if (deadProperties.Count != 0)
                await SetPropertiesAsync(deadProperties, cancellationToken);

            return livePropertiesResult;
        }

        private async Task SetPropertiesAsync(IEnumerable<IDeadProperty> properties, CancellationToken cancellationToken)
        {
            var propertyStore = _entry.FileSystem.PropertyStore;
            if (propertyStore == null)
                return;

            var elements = new List<XElement>();
            foreach (var property in properties)
            {
                var propValue = await property.GetXmlValueAsync(cancellationToken);
                if (!property.IsDefaultValue(propValue))
                    elements.Add(propValue);
            }

            await propertyStore.SetAsync(_entry, elements, cancellationToken);
        }

        private async Task<IReadOnlyCollection<XName>> SetPropertiesAsync(IEnumerable<ILiveProperty> properties, CancellationToken cancellationToken)
        {
            var isPropUsed = new Dictionary<XName, bool>();
            var propNameToValue = new Dictionary<XName, XElement>();
            foreach (var property in properties)
            {
                var key = property.Name;
                propNameToValue[key] = await property.GetXmlValueAsync(cancellationToken);
                isPropUsed[key] = false;
            }

            if (propNameToValue.Count == 0)
            {
                return new XName[0];
            }

            await using (var propEnum = _entry.GetProperties(TargetActions.Dispatcher, returnInvalidProperties: true).GetAsyncEnumerator())
            {
                while (await propEnum.MoveNextAsync())
                {
                    var key = propEnum.Current.Name;
                    isPropUsed[key] = true;
                    var prop = propEnum.Current as IUntypedWriteableProperty;
                    XElement propValue;
                    if (prop != null && propNameToValue.TryGetValue(key, out propValue))
                    {
                        await prop.SetXmlValueAsync(propValue, cancellationToken);
                    }
                }
            }

            var hasUnsetLiveProperties = isPropUsed.Any(x => !x.Value);
            if (hasUnsetLiveProperties)
            {
                var unsetPropNames = isPropUsed.Where(x => !x.Value).Select(x => x.Key).ToList();
                return unsetPropNames;
            }

            return new XName[0];
        }
    }
}
