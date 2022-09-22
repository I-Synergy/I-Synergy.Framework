using ISynergy.Framework.Core.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Collections
{
    /// <summary>
    /// Class TreeNodeList.
    /// Implements the <see cref="List{TreeNode}" />
    /// Implements the <see cref="TreeNodeList{T}" />
    /// Implements the <see cref="INotifyPropertyChanged" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="List{TreeNode}" />
    /// <seealso cref="TreeNodeList{T}" />
    /// <seealso cref="INotifyPropertyChanged" />
    public class TreeNodeList<T> : List<TreeNode<T>>, INotifyPropertyChanged
        where T : class
    {
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public TreeNode<T> Parent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeList{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public TreeNodeList(TreeNode<T> parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        /// <returns>ITreeNode&lt;T&gt;.</returns>
        public TreeNode<T> Add(TreeNode<T> node, bool updateParent = true)
        {
            if (updateParent)
            {
                node.SetParent(Parent, true);
                return node;
            }

            base.Add(node);
            OnPropertyChanged(nameof(Count));
            return node;
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(TreeNode<T> node, bool updateParent = true)
        {
            Argument.IsNotNull(node);

            if (!base.Contains(node))
                return false;

            if (updateParent)
            {
                node.SetParent(null, false);
                return !base.Contains(node);
            }

            var result = base.Remove(node);
            OnPropertyChanged(nameof(Count));
            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Count={Count}";
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
