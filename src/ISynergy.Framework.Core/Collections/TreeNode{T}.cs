using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace ISynergy.Framework.Core.Collections
{
    public class TreeNode<T> : ObservableClass, IDisposable
        where T : class
    {
        /// <summary>
        /// Occurs when [ancestor changed].
        /// </summary>
        public event Action<NodeChangeTypes, TreeNode<T>> AncestorChanged;
        /// <summary>
        /// Occurs when [descendant changed].
        /// </summary>
        public event Action<NodeChangeTypes, TreeNode<T>> DescendantChanged;
        
        /// <summary>
        /// Gets or sets the Data property value.
        /// </summary>
        public T Data
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Parent property value.
        /// </summary>
        /// <value>The parent.</value>
        public TreeNode<T> Parent
        {
            get { return GetValue<TreeNode<T>>(); }
            set { SetParent(value); }
        }

        /// <summary>
        /// Gets or sets the Children property value.
        /// </summary>
        /// <value>The children.</value>
        public TreeNodeList<T> Children
        {
            get { return GetValue<TreeNodeList<T>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        public TreeNode()
        {
            Parent = null;
            Children = new TreeNodeList<T>(this);
            DisposeTraversal = UpDownTraversalTypes.BottomUp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <param name="data">The value.</param>
        public TreeNode(T data)
            : this()
        {
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <param name="data">The value.</param>
        /// <param name="parent">The parent.</param>
        public TreeNode(T data, TreeNode<T> parent)
            : this(data)
        {
            Parent = parent;
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="updateChildNodes">if set to <c>true</c> [update child nodes].</param>
        public void SetParent(TreeNode<T> node, bool updateChildNodes = true)
        {
            if (node == Parent)
                return;

            var oldParent = Parent;
            var oldParentHeight = Parent != null ? Parent.Height : 0;
            var oldDepth = Depth;

            // if oldParent isn't null
            // remove this node from its newly ex-parent's children
            if (oldParent != null && oldParent.Children.Contains(this))
                oldParent.Children.Remove(this, updateParent: false);

            // update the backing field
            SetValue(node, false, nameof(Parent));

            // add this node to its new parent's children
            if (Parent != null && updateChildNodes)
                Parent.Children.Add(this, updateParent: false);

            // signal the old parent that it has lost this child
            if (oldParent != null)
                oldParent.OnDescendantChanged(NodeChangeTypes.NodeRemoved, this);

            if (oldDepth != Depth)
                OnDepthChanged();

            // if this operation has changed the height of any parent, initiate the bubble-up height changed event
            if (Parent != null)
            {
                var newParentHeight = Parent != null ? Parent.Height : 0;
                if (newParentHeight != oldParentHeight)
                    Parent.OnHeightChanged();

                Parent.OnDescendantChanged(NodeChangeTypes.NodeAdded, this);
            }

            OnParentChanged(oldParent, Parent);
        }

        /// <summary>
        /// Called when [parent changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public virtual void OnParentChanged(TreeNode<T> oldValue, TreeNode<T> newValue)
        {
            OnPropertyChanged(nameof(Parent));
        }

        /// <summary>
        /// Called when [ancestor changed].
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="node">The node.</param>
        public virtual void OnAncestorChanged(NodeChangeTypes changeType, TreeNode<T> node)
        {
            if (AncestorChanged != null)
                AncestorChanged(changeType, node);

            foreach (var child in Children)
                child.OnAncestorChanged(changeType, node);
        }

        /// <summary>
        /// Called when [descendant changed].
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="node">The node.</param>
        public virtual void OnDescendantChanged(NodeChangeTypes changeType, TreeNode<T> node)
        {
            if (DescendantChanged != null)
                DescendantChanged(changeType, node);

            if (Parent != null)
                Parent.OnDescendantChanged(changeType, node);
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return Children.Count == 0 ? 0 : Children.Max(n => n.Height) + 1; }
        }

        /// <summary>
        /// Called when [height changed].
        /// </summary>
        public virtual void OnHeightChanged()
        {
            OnPropertyChanged(nameof(Height));

            foreach (var child in Children)
                child.OnHeightChanged();
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth
        {
            get { return (Parent == null ? 0 : Parent.Depth + 1); }
        }

        /// <summary>
        /// Called when [depth changed].
        /// </summary>
        public virtual void OnDepthChanged()
        {
            OnPropertyChanged(nameof(Depth));

            if (Parent != null)
                Parent.OnDepthChanged();
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <value>The child nodes.</value>
        public List<TreeNode<T>> ChildNodes
        {
            get
            {
                var result = new List<TreeNode<T>>();

                foreach (var node in Children)
                    result.Add(node);

                return result;
            }
        }

        /// <summary>
        /// Gets the descendants.
        /// </summary>
        /// <value>The descendants.</value>
        public List<TreeNode<T>> Descendants
        {
            get
            {
                var result = new List<TreeNode<T>>();

                foreach (var node in ChildNodes)
                {
                    result.Add(node);

                    foreach (var descendant in node.Descendants)
                        result.Add(descendant);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the subtree.
        /// </summary>
        /// <value>The subtree.</value>
        public List<TreeNode<T>> Subtree
        {
            get
            {
                var result = new List<TreeNode<T>>();
                result.Add(this);

                foreach (var node in Descendants)
                    result.Add(node);

                return result;
            }
        }

        /// <summary>
        /// Gets the ancestors.
        /// </summary>
        /// <value>The ancestors.</value>
        public List<TreeNode<T>> Ancestors
        {
            get
            {
                var result = new List<TreeNode<T>>();

                if (Parent is null)
                    return result;

                result.Add(Parent);

                foreach (var node in Parent.Ancestors)
                    result.Add(node);

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the DisposeTraversal property value.
        /// </summary>
        public UpDownTraversalTypes DisposeTraversal
        {
            get { return GetValue<UpDownTraversalTypes>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() =>
            $"Depth={Depth}, Height={Height}, Children={Children.Count}";

        #region IDisposable
        // The bulk of the clean-up code is implemented in Dispose(bool)
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Data is IDisposable)
                {
                    if (DisposeTraversal == UpDownTraversalTypes.BottomUp)
                        foreach (var node in Children)
                            node.Dispose();

                    (Data as IDisposable).Dispose();

                    if (DisposeTraversal == UpDownTraversalTypes.TopDown)
                        foreach (var node in Children)
                            node.Dispose();
                }
            }

            // free native resources if there are any.
        }
        #endregion
    }
}
