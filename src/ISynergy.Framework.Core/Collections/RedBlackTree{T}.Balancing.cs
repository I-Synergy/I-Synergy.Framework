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

        fixtree:

            if (item.Parent!.Color == RedBlackTreeNodeTypes.Red)
            {
                // red cannot have red child
                var u = p == p.Parent!.Left ? p.Parent.Right : p.Parent.Left;

                if (u is not null && u.Color == RedBlackTreeNodeTypes.Red)
                {
                    p.Color = u.Color = RedBlackTreeNodeTypes.Black;
                    item = p.Parent;

                    if ((p = item.Parent) is not null)
                    {
                        item.Color = RedBlackTreeNodeTypes.Red;
                        goto fixtree;
                    }
                }
                else
                {
                    if (item == p.Right && p == p.Parent.Left)
                    {
                        tree.rotate_left(p);
                        p = item; item = item.Left!;
                    }
                    else if (item == p.Left && p == p.Parent.Right)
                    {
                        tree.rotate_right(p);
                        p = item;
                        item = item.Right!;
                    }

                    p.Color = RedBlackTreeNodeTypes.Black;
                    p.Parent.Color = RedBlackTreeNodeTypes.Red;

                    if (item == p.Left && p == p.Parent.Left)
                        tree.rotate_right(p.Parent);
                    else if (item == p.Right && p == p.Parent.Right)
                        tree.rotate_left(p.Parent);
                }
            }
        }

        /// <summary>
        ///   Restores the red-black properties after a black node was removed.
        /// </summary>
        /// <param name="tree">The tree being modified.</param>
        /// <param name="m">The node that replaced the removed node (may be <c>null</c>).</param>
        /// <param name="mp">The parent of <paramref name="m"/> (may be <c>null</c>).</param>
        public static void FixAfterRemove(RedBlackTree<T> tree, RedBlackTreeNode<T>? m, RedBlackTreeNode<T>? mp)
        {
        deleteblack:
            if (mp is not null)
            {
                var s = m == mp.Left ? mp.Right : mp.Left;

                if (s!.Color == RedBlackTreeNodeTypes.Red)
                {
                    mp.Color = RedBlackTreeNodeTypes.Red;
                    s.Color = RedBlackTreeNodeTypes.Black;

                    if (m == mp.Left)
                        tree.rotate_left(mp);
                    else tree.rotate_right(mp);

                    s = m == mp.Left ? mp.Right : mp.Left;
                }

                if (mp.Color == RedBlackTreeNodeTypes.Black
                  && s!.Color == RedBlackTreeNodeTypes.Black
                  && (s.Left is null || s.Left.Color == RedBlackTreeNodeTypes.Black)
                  && (s.Right is null || s.Right.Color == RedBlackTreeNodeTypes.Black))
                {
                    if (s is not null) // NOSONAR - defensive check; s may be null depending on tree state after goto
                        s.Color = RedBlackTreeNodeTypes.Red;

                    m = mp;
                    mp = m.Parent;

                    goto deleteblack;
                }

                if (mp.Color == RedBlackTreeNodeTypes.Red
                    && s!.Color == RedBlackTreeNodeTypes.Black
                    && (s.Left is null || s.Left.Color == RedBlackTreeNodeTypes.Black)
                    && (s.Right is null || s.Right.Color == RedBlackTreeNodeTypes.Black))
                {
                    if (s is not null) // NOSONAR - defensive check; s may be null depending on tree state after goto
                        s.Color = RedBlackTreeNodeTypes.Red;

                    mp.Color = RedBlackTreeNodeTypes.Black;
                }
                else
                {
                    if (m == mp.Left && s!.Color == RedBlackTreeNodeTypes.Black
                                     && (s.Left is not null && s.Left.Color == RedBlackTreeNodeTypes.Red)
                                     && (s.Right is null || s.Right.Color == RedBlackTreeNodeTypes.Black))
                    {
                        s.Color = RedBlackTreeNodeTypes.Red;
                        s.Left.Color = RedBlackTreeNodeTypes.Black;

                        tree.rotate_right(s);

                        s = m == mp.Left ? mp.Right : mp.Left;
                    }
                    else if (m == mp.Right && s!.Color == RedBlackTreeNodeTypes.Black
                                           && (s.Right is not null && s.Right.Color == RedBlackTreeNodeTypes.Red)
                                           && (s.Left is null || s.Left.Color == RedBlackTreeNodeTypes.Black))
                    {
                        s.Color = RedBlackTreeNodeTypes.Red;
                        s.Right.Color = RedBlackTreeNodeTypes.Black;
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
            }
        }
    }
}
