using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI.Abstractions;
using System;
using Windows.Foundation.Collections;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Actions
{
    /// <summary>
    /// Represents a collection of IActions.
    /// </summary>
    public sealed class ActionCollection : DependencyObjectCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCollection"/> class.
        /// </summary>
        public ActionCollection()
        {
            this.VectorChanged += this.ActionCollection_VectorChanged;
        }

        private void ActionCollection_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs eventArgs)
        {
            CollectionChange collectionChange = eventArgs.CollectionChange;

            if (collectionChange == CollectionChange.Reset)
            {
                foreach (DependencyObject item in this)
                {
                    ActionCollection.VerifyType(item);
                }
            }
            else if (collectionChange == CollectionChange.ItemInserted || collectionChange == CollectionChange.ItemChanged)
            {
                DependencyObject changedItem = this[(int)eventArgs.Index];
                ActionCollection.VerifyType(changedItem);
            }
        }

        private static void VerifyType(DependencyObject item)
        {
            if (!(item is IAction))
            {
                throw new InvalidOperationException(ServiceLocator.Default.GetInstance<ILanguageService>().GetString("NonActionAddedToActionCollectionExceptionMessage"));
            }
        }
    }
}
