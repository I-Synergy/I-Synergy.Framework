using ISynergy.Framework.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Collections;

/// <summary>
/// Tests for the refactored <see cref="TreeNode{TKey, TModel}"/> dispose logic
/// (extracted DisposeChildren helper + BottomUp / TopDown traversal).
/// </summary>
[TestClass]
public class TreeNodeDisposeTests
{
    /// <summary>
    /// Model that records the order in which nodes are disposed.
    /// </summary>
    private sealed class DisposableModel : IDisposable
    {
        private readonly string _name;
        private readonly List<string> _log;

        public DisposableModel(string name, List<string> log)
        {
            _name = name;
            _log = log;
        }

        public void Dispose() => _log.Add(_name);
    }

    [TestMethod]
    public void Dispose_BottomUp_DisposesChildrenBeforeParent()
    {
        var log = new List<string>();

        var root = new TreeNode<Guid, DisposableModel>(new DisposableModel("root", log));
        root.DisposeTraversal = UpDownTraversalTypes.BottomUp;
        var child = root.AddChild(new DisposableModel("child", log));
        child.DisposeTraversal = UpDownTraversalTypes.BottomUp;

        root.Dispose();

        CollectionAssert.AreEqual(new[] { "child", "root" }, log);
    }

    [TestMethod]
    public void Dispose_TopDown_DisposesParentBeforeChildren()
    {
        var log = new List<string>();

        var root = new TreeNode<Guid, DisposableModel>(new DisposableModel("root", log));
        root.DisposeTraversal = UpDownTraversalTypes.TopDown;
        var child = root.AddChild(new DisposableModel("child", log));
        child.DisposeTraversal = UpDownTraversalTypes.TopDown;

        root.Dispose();

        CollectionAssert.AreEqual(new[] { "root", "child" }, log);
    }

    [TestMethod]
    public void Dispose_MultipleChildren_DisposesEveryChild()
    {
        var log = new List<string>();

        var root = new TreeNode<Guid, DisposableModel>(new DisposableModel("root", log));
        root.DisposeTraversal = UpDownTraversalTypes.BottomUp;
        root.AddChild(new DisposableModel("child1", log));
        root.AddChild(new DisposableModel("child2", log));

        root.Dispose();

        CollectionAssert.AreEqual(new[] { "child1", "child2", "root" }, log);
    }

    [TestMethod]
    public void Dispose_LeafNode_DisposesDataOnly()
    {
        var log = new List<string>();

        var leaf = new TreeNode<Guid, DisposableModel>(new DisposableModel("leaf", log));

        leaf.Dispose();

        CollectionAssert.AreEqual(new[] { "leaf" }, log);
    }

    [TestMethod]
    public void Dispose_NestedBottomUp_DeepestChildrenFirst()
    {
        var log = new List<string>();

        var root = new TreeNode<Guid, DisposableModel>(new DisposableModel("root", log));
        root.DisposeTraversal = UpDownTraversalTypes.BottomUp;
        var child = root.AddChild(new DisposableModel("child", log));
        child.DisposeTraversal = UpDownTraversalTypes.BottomUp;
        var grandchild = child.AddChild(new DisposableModel("grandchild", log));
        grandchild.DisposeTraversal = UpDownTraversalTypes.BottomUp;

        root.Dispose();

        CollectionAssert.AreEqual(new[] { "grandchild", "child", "root" }, log);
    }

    [TestMethod]
    public void Dispose_NonDisposableData_SkipsTraversalWithoutThrowing()
    {
        // Space is not IDisposable, so the DisposeChildren branch is skipped entirely:
        // child nodes must not be traversed/disposed and the call must not throw.
        var root = new TreeNode<Guid, ISynergy.Framework.Core.Data.Space>(
            new ISynergy.Framework.Core.Data.Space { Name = "root" });
        root.DisposeTraversal = UpDownTraversalTypes.BottomUp;
        var child = root.AddChild(new ISynergy.Framework.Core.Data.Space { Name = "child" });

        root.Dispose();

        // Children remain (never disposed) because Data is not IDisposable.
        Assert.HasCount(1, root.Children);
        Assert.AreSame(child, root.Children[0]);
    }

}
