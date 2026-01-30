namespace ISynergy.Framework.Mathematics.Distances;

using ISynergy.Framework.Mathematics.Distances.Base;
using System;

/// <summary>
///   Jaccard (Index) distance.
/// </summary>
/// 
/// <remarks>
/// <para>
///   The Jaccard index, also known as the Jaccard similarity coefficient (originally
///   coined coefficient de communauté by Paul Jaccard), is a statistic used for comparing
///   the similarity and diversity of sample sets. The Jaccard coefficient measures 
///   similarity between finite sample sets, and is defined as the size of the intersection
///   divided by the size of the union of the sample sets.</para>
///   
/// <para>
///   References:
///   <list type="bullet">
///     <item><description><a href="https://en.wikipedia.org/wiki/Jaccard_index">
///       https://en.wikipedia.org/wiki/Jaccard_index </a></description></item>
///   </list></para>  
/// </remarks>
/// 
/// <typeparam name="T">The type of the elements in the arrays to be compared.</typeparam>
/// 
[Serializable]
public struct Jaccard<T> : ISimilarity<T[]>, IMetric<T[]>, ICloneable
    where T : IEquatable<T>
{
    /// <summary>
    ///   Computes the distance <c>d(x,y)</c> between points
    ///   <paramref name="x"/> and <paramref name="y"/>.
    /// </summary>
    /// 
    /// <param name="x">The first point <c>x</c>.</param>
    /// <param name="y">The second point <c>y</c>.</param>
    /// 
    /// <returns>
    ///   A double-precision value representing the distance <c>d(x,y)</c>
    ///   between <paramref name="x"/> and <paramref name="y"/> according 
    ///   to the distance function implemented by this class.
    /// </returns>
    /// 
    public double Distance(T[] x, T[] y)
    {
        int inter = 0;
        int union = 0;

        for (var i = 0; i < x.Length; i++)
        {
            if (!x[i].Equals(default(T)) || !y[i].Equals(default(T)))
            {
                if (x[i].Equals(y[i]))
                    inter++;
                union++;
            }
        }

        return (union == 0) ? 0 : 1.0 - (inter / (double)union);
    }

    /// <summary>
    ///   Gets a similarity measure between two points.
    /// </summary>
    /// 
    /// <param name="x">The first point to be compared.</param>
    /// <param name="y">The second point to be compared.</param>
    /// 
    /// <returns>A similarity measure between x and y.</returns>
    /// 
    public double Similarity(T[] x, T[] y)
    {
        int inter = 0;
        int union = 0;

        for (var i = 0; i < x.Length; i++)
        {
            if (!x[i].Equals(default(T)) || !y[i].Equals(default(T)))
            {
                if (x[i].Equals(y[i]))
                    inter++;
                union++;
            }
        }

        return (inter == 0) ? 0 : inter / (double)union;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new Jaccard<T>();
    }
}
