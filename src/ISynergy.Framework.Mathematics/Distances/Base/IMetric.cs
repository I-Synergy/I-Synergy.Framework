namespace ISynergy.Framework.Mathematics.Distances
{

    /// <summary>
    ///   Common interface for Metric distance functions.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The framework distinguishes between metrics and distances by using different
    ///   types for them. This makes it possible to let the compiler figure out logic
    ///   problems such as the specification of a non-metric for a method that requires
    ///   a proper metric (i.e. that respects the triangle inequality).</para>
    ///   
    /// <para>
    ///   The objective of this technique is to make it harder to make some mistakes.
    ///   However, it is generally possible to bypass this mechanism by using named constructors
    ///   available at each of the classes, such as Minkowski's <see cref="Minkowski.Nonmetric"/> 
    ///   method, to create distances implementing the <see cref="IMetric{T}"/> interface that are not
    ///   really metrics. Use at your own risk.</para>
    /// </remarks>
    /// 
    /// <seealso cref="IDistance{T}"/>
    /// 
    public interface IMetric<in T> : IDistance<T>
    {
    }

}
