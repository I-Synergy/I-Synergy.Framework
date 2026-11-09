namespace ISynergy.Framework.CQRS.Queries;

/// <summary>
/// Query interface with specified result type
/// </summary>
/// <typeparam name="TResult">
/// The type of query result. The type parameter is a marker: dispatchers resolve the handler from
/// the closed query type, so no member has to use it.
/// </typeparam>
#pragma warning disable S2326 // marker type parameter resolved through the closed generic type
public interface IQuery<TResult>
{
}
#pragma warning restore S2326
