using ISynergy.Framework.Mathematics.Distances;

namespace ISynergy.Framework.Mathematics;

public static partial class Distance
{

    private static readonly Yule cacheYule = new Yule();

    private static readonly Jaccard cacheJaccard = new Jaccard();

    private static readonly Hellinger cacheHellinger = new Hellinger();

    private static readonly Euclidean cacheEuclidean = new Euclidean();

    private static readonly SquareMahalanobis cacheSquareMahalanobis = new SquareMahalanobis();

    private static readonly RusselRao cacheRusselRao = new RusselRao();

    private static readonly Chebyshev cacheChebyshev = new Chebyshev();

    private static readonly Dice cacheDice = new Dice();

    private static readonly SokalMichener cacheSokalMichener = new SokalMichener();

    private static readonly WeightedEuclidean cacheWeightedEuclidean = new WeightedEuclidean();

    private static readonly Angular cacheAngular = new Angular();

    private static readonly SquareEuclidean cacheSquareEuclidean = new SquareEuclidean();

    private static readonly Hamming cacheHamming = new Hamming();

    private static readonly ArgMax cacheArgMax = new ArgMax();

    private static readonly Modular cacheModular = new Modular();

    private static readonly Cosine cacheCosine = new Cosine();

    private static readonly Mahalanobis cacheMahalanobis = new Mahalanobis();

    private static readonly BrayCurtis cacheBrayCurtis = new BrayCurtis();

    private static readonly Minkowski cacheMinkowski = new Minkowski();

    private static readonly Levenshtein cacheLevenshtein = new Levenshtein();

    private static readonly SokalSneath cacheSokalSneath = new SokalSneath();

    private static readonly Matching cacheMatching = new Matching();

    private static readonly Canberra cacheCanberra = new Canberra();

    private static readonly RogersTanimoto cacheRogersTanimoto = new RogersTanimoto();

    private static readonly Manhattan cacheManhattan = new Manhattan();

    private static readonly Kulczynski cacheKulczynski = new Kulczynski();

    private static readonly WeightedSquareEuclidean cacheWeightedSquareEuclidean = new WeightedSquareEuclidean();
}
