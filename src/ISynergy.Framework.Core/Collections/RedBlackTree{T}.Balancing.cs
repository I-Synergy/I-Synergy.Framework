#pragma warning disable S907  // goto is required in ported numerical algorithms
#pragma warning disable S1199 // nested blocks required in algorithm implementation
#pragma warning disable S2589 // null checks after null-forgiving operator are intentional defensive runtime guards

namespace ISynergy.Framework.Core.Collections;

public partial class RedBlackTree<T>
{
    /// <summary>
    ///   The red-black balancing fix-ups applied after an insertion or a removal.
    /// </summary>
    /// <remarks>
    ///   Extracted from <see cref="RedBlackTree{T}"/> so that the structural operations
    ///   (<see cref="Add(RedBlackTreeNode{T})"/> and <see cref="Remove(RedBlackTreeNode{T})"/>) are not
    ///   interleaved with the recoloring/rotation algorithms. The algorithms are ported verbatim from the
    ///   original implementation (hence the <c>goto</c>-based control flow); the private rotations and the
    ///   node types remain on the containing type, which this nested type can reach.
    /// </remarks>
    private static class Balancing
    {
        /// <summary>
        ///   Restores the red-black properties after <paramref name="item"/> was inserted under
        ///   <paramref name="parent"/>.
        /// </summary>
        /// <param name="tree">The tree being modified.</param>
        /// <param name="item">The freshly inserted node.</param>
        /// <param name="parent">The node under which <paramref name="item"/> was inserted.</param>
        public static void FixAfterInsert(RedBlackTree<T> tree, RedBlackTreeNode<T> item, RedBlackTreeNode<T> parent)
        {
            var p = parent;

            while (true)
            {
                if (item.Parent!.Color != RedBlackTreeNodeTypes.Red)
                    return;

                // red cannot have red child
                var u = p == p.Parent!.Left ? p.Parent.Right : p.Parent.Left;

                if (u is null || u.Color != RedBlackTreeNodeTypes.Red)
                {
                    FixInsertRotations(tree, item, p);
                    return;
                }

                p.Color = u.Color = RedBlackTreeNodeTypes.Black;
                item = p.Parent;
                p = item.Parent;

                if (p is null)
                    return;

                item.Color = RedBlackTreeNodeTypes.Red;
            }
        }

        /// <summary>
        ///   Applies the rotations and recoloring for the case where the inserted node's uncle is black.
        /// </summary>
        /// <param name="tree">The tree being modified.</param>
        /// <param name="item">The node being fixed up.</param>
        /// <param name="p">The parent of <paramref name="item"/>.</param>
        private static void FixInsertRotations(RedBlackTree<T> tree, RedBlackTreeNode<T> item, RedBlackTreeNode<T> p)
        {
            if (item == p.Right && p == p.Parent!.Left)
            {
                tree.rotate_left(p);
                p = item; item = item.Left!;
            }
            else if (item == p.Left && p == p.Parent!.Right)
            {
                tree.rotate_right(p);
                p = item;
                item = item.Right!;
            }

            p.Color = RedBlackTreeNodeTypes.Black;
            p.Parent!.Color = RedBlackTreeNodeTypes.Red;

            if (item == p.Left && p == p.Parent.Left)
                tree.rotate_right(p.Parent);
            else if (item == p.Right && p == p.Parent.Right)
                tree.rotate_left(p.Parent);
        }

        /// <summary>
        ///   Restores the red-black properties after a black node was removed.
        /// </summary>
        /// <param name="tree">The tree being modified.</param>
        /// <param name="m">The node that replaced the removed node (may be <c>null</c>).</param>
        /// <param name="mp">The parent of <paramref name="m"/> (may be <c>null</c>).</param>
        public static void FixAfterRemove(RedBlackTree<T> tree, RedBlackTreeNode<T>? m, RedBlackTreeNode<T>? mp)
        {
            while (mp is not null)
            {
                var s = NormalizeRedSibling(tree, m, mp, m == mp.Left ? mp.Right : mp.Left);

                if (mp.Color == RedBlackTreeNodeTypes.Black
                  && s!.Color == RedBlackTreeNodeTypes.Black
                  && IsBlack(s.Left)
                  && IsBlack(s.Right))
                {
                    s.Color = RedBlackTreeNodeTypes.Red;

                    m = mp;
                    mp = m.Parent;

                    continue;
                }

                if (mp.Color == RedBlackTreeNodeTypes.Red
                    && s!.Color == RedBlackTreeNodeTypes.Black
                    && IsBlack(s.Left)
                    && IsBlack(s.Right))
                {
                    s.Color = RedBlackTreeNodeTypes.Red;
                    mp.Color = RedBlackTreeNodeTypes.Black;
                }
                else
                {
                    RedistributeAfterRemoval(tree, m, mp);
                }

                return;
            }
        }

        /// <summary>
        ///   Recolors and rotates when the sibling of the removed node is red.
        /// </summary>
        /// <param name="tree">The tree being modified.</param>
        /// <param name="m">The node that replaced the removed node.</param>
        /// <param name="mp">The parent of <paramref name="m"/>.</param>
        /// <param name="s">The sibling of <paramref name="m"/>.</param>
        /// <returns>The (possibly new) sibling of <paramref name="m"/>.</returns>
        private static RedBlackTreeNode<T>? NormalizeRedSibling(
            RedBlackTree<T> tree,
            RedBlackTreeNode<T>? m,
            RedBlackTreeNode<T> mp,
            RedBlackTreeNode<T>? s)
        {
            if (s is null || s.Color != RedBlackTreeNodeTypes.Red)
                return s;

            mp.Color = RedBlackTreeNodeTypes.Red;
            s.Color = RedBlackTreeNodeTypes.Black;

            if (m == mp.Left)
                tree.rotate_left(mp);
            else
                tree.rotate_right(mp);

            return m == mp.Left ? mp.Right : mp.Left;
        }

        /// <summary>
        ///   Recolors and rotates around the sibling when the sibling (or one of its children) is red.
        /// </summary>
        /// <param name="tree">The tree being modified.</param>
        /// <param name="m">The node that replaced the removed node.</param>
        /// <param name="mp">The parent of <paramref name="m"/>.</param>
        private static void RedistributeAfterRemoval(RedBlackTree<T> tree, RedBlackTreeNode<T>? m, RedBlackTreeNode<T> mp)
        {
            var s = m == mp.Left ? mp.Right : mp.Left;

            if (s!.Color == RedBlackTreeNodeTypes.Black
                && m == mp.Left
                && IsRed(s.Left)
                && IsBlack(s.Right))
            {
                s.Color = RedBlackTreeNodeTypes.Red;
                s.Left!.Color = RedBlackTreeNodeTypes.Black;

                tree.rotate_right(s);

                s = m == mp.Left ? mp.Right : mp.Left;
            }
            else if (s.Color == RedBlackTreeNodeTypes.Black
                && m == mp.Right
                && IsRed(s.Right)
                && IsBlack(s.Left))
            {
                s.Color = RedBlackTreeNodeTypes.Red;
                s.Right!.Color = RedBlackTreeNodeTypes.Black;
                tree.rotate_left(s);
                s = m == mp.Left ? mp.Right : mp.Left;
            }

            s!.Color = mp.Color;
            mp.Color = RedBlackTreeNodeTypes.Black;

            if (m == mp.Left)
            {
                s.Right!.Color = RedBlackTreeNodeTypes.Black;
                tree.rotate_left(mp);
            }
            else
            {
                s.Left!.Color = RedBlackTreeNodeTypes.Black;
                tree.rotate_right(mp);
            }
        }

        /// <summary>
        ///   Determines whether the node is red (a missing node is not red).
        /// </summary>
        /// <param name="node">The node to inspect.</param>
        private static bool IsRed(RedBlackTreeNode<T>? node) =>
            node is not null && node.Color == RedBlackTreeNodeTypes.Red;

        /// <summary>
        ///   Determines whether the node is black (a missing node counts as black).
        /// </summary>
        /// <param name="node">The node to inspect.</param>
        private static bool IsBlack(RedBlackTreeNode<T>? node) =>
            node is null || node.Color == RedBlackTreeNodeTypes.Black;
    }
}
