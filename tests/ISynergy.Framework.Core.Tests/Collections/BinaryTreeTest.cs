using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Core.Collections.Tests
{
    [TestClass]
    public class BinaryTreeTest
    {
        #region doc_ctor_1
        /// <summary>
        ///   Implement this class as you need. There are no fundamental properties
        ///   or methods that should be overriden by your class, except for any data
        ///   value that you might want your node to carry.
        /// </summary>
        /// 
        class MyTreeNode : BinaryNode<MyTreeNode>
        {
            /// <summary>
            ///   Gets or sets a custom value that you would like your nodes to have.
            /// </summary>
            /// 
            public string Value { get; set; }
        }
        #endregion

        [TestMethod]
        public void tree_test()
        {
            #region doc_ctor_2
            // Let's start by creating an empty tree 
            var tree = new BinaryTree<MyTreeNode>();

            // Now, we can proceed by placing elements on different positions of the tree. Note that this class 
            // does not  implement a search  tree, so it is not possible to place elements automatically using 
            // an ".Add()" method. Instead, this class offers functionality that is common to all Binary Trees,
            // such as traversing the tree breadth-first, depth-first, in order, in pre-order, and in post-order.

            // Let's start populating the tree:
            tree.Root = new MyTreeNode()
            {
                Left = new MyTreeNode()
                {
                    Left = new MyTreeNode()
                    {
                        Value = "a"
                    },

                    Value = "b",

                    Right = new MyTreeNode()
                    {
                        Value = "c"
                    },
                },

                Value = "d",

                Right = new MyTreeNode()
                {
                    Left = new MyTreeNode()
                    {
                        Value = "e"
                    },

                    Value = "f",

                    Right = new MyTreeNode()
                    {
                        Value = "g"
                    },
                }
            };

            // Now, let's traverse the tree in order:
            List<string> breadthFirst = tree.Traverse(TreeTraversal.BreadthFirst).Select(x => x.Value).ToList();
            // should return: "d", "b", "f", "a", "c", "e", "g"

            List<string> depthFirst = tree.Traverse(TreeTraversal.DepthFirst).Select(x => x.Value).ToList();
            // should return: "d", "f", "g", "e", "b", "c", "a"

            List<string> inOrder = tree.Traverse(TreeTraversal.InOrder).Select(x => x.Value).ToList();
            // should return: "a", "b", "c", "d", "e", "f", "g"

            List<string> postOrder = tree.Traverse(TreeTraversal.PostOrder).Select(x => x.Value).ToList();
            // should return: "a", "c", "b", "e", "g", "f", "d"

            List<string> preOrder = tree.Traverse(TreeTraversal.PreOrder).Select(x => x.Value).ToList();
            // should return: "d", "b", "a", "c", "f", "e", "g"
            #endregion

            CollectionAssert.AreEqual(new List<string> { "d", "b", "f", "a", "c", "e", "g" }, breadthFirst);
            CollectionAssert.AreEqual(new List<string> { "d", "f", "g", "e", "b", "c", "a" }, depthFirst);
            CollectionAssert.AreEqual(new List<string> { "a", "b", "c", "d", "e", "f", "g" }, inOrder);
            CollectionAssert.AreEqual(new List<string> { "a", "c", "b", "e", "g", "f", "d" }, postOrder);
            CollectionAssert.AreEqual(new List<string> { "d", "b", "a", "c", "f", "e", "g" }, preOrder);
        }

    }
}
