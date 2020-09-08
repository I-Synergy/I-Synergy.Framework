using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support
{
    public static class PropertyComparer
    {
        public static IReadOnlyCollection<PropertyChangeItem> FindChanges(
            IReadOnlyCollection<XElement> left,
            IReadOnlyCollection<XElement> right,
            params XName[] propertiesToIgnore)
        {
            var propsToIgnore = new HashSet<XName>(propertiesToIgnore);
            var result = new List<PropertyChangeItem>();
            var rightItems = right.ToDictionary(x => x.Name);
            foreach (var leftItem in left.Where(x => !propsToIgnore.Contains(x.Name)))
            {
                XElement rightItem;
                if (rightItems.TryGetValue(leftItem.Name, out rightItem))
                {
                    var leftText = leftItem.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting);
                    var rightText = rightItem.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting);
                    if (leftText != rightText)
                    {
                        result.Add(PropertyChangeItem.Changed(leftItem, rightItem));
                    }

                    rightItems.Remove(leftItem.Name);
                }
                else
                {
                    result.Add(PropertyChangeItem.Removed(leftItem));
                }
            }

            foreach (var rightItem in rightItems.Values.Where(x => !propsToIgnore.Contains(x.Name)))
            {
                result.Add(PropertyChangeItem.Added(rightItem));
            }

            return result;
        }
    }
}
