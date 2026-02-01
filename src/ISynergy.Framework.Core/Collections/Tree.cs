namespace ISynergy.Framework.Core.Collections;

/// <summary>
/// Base class for tree structures.
/// </summary>
/// <typeparam name="TKey">The type of the t identifier.</typeparam>
/// <typeparam name="TModel">The type of the t model.</typeparam>
public class Tree<TKey, TModel> : TreeNode<TKey, TModel>
    where TKey : struct
    where TModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tree{TKey, TModel}" /> class.
    /// </summary>
    public Tree()
        : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tree{TKey, TModel}" /> class.
    /// </summary>
    /// <param name="data">The data.</param>
    public Tree(TModel data)
        : base(data) { }
}
