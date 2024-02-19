using ISynergy.Framework.Geography.Common;
using ISynergy.Framework.Geography.Geodetic;
using ISynergy.Framework.Geography.Utm;

namespace ISynergy.Framework.Geography.Global;

/// <summary>
/// This class overlays the globe with a mesh of squares.
/// The algorithm is based on subdividing the UTM grids into
/// finer cell structures, so the coverage is for latitudes
/// between 80° South and 84° North.
/// </summary>
public class GlobalMesh
{
    /// <summary>
    /// The minimum mesh size
    /// </summary>
    private const int MinimumMeshSize = 1;

    // The maximum number of cells required for any UTM Grid 

    // The maximum vertical number of cells in any UTM Grid
    /// <summary>
    /// The maximum vertical meshes
    /// </summary>
    private readonly long _maxVerticalMeshes;

    /// <summary>
    /// Instantiate the Mesh with the given nuber of meters as the size
    /// of the mesh squares. We do not support squares less than 1m.
    /// Please note that the actual mesh size used is a derived value
    /// that approximates the requested mesh size in order to provide
    /// better computational efficiency.
    /// </summary>
    /// <param name="meshSizeinMeters">The size of the squares in meter. The defauklt value is 1000m.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public GlobalMesh(int meshSizeinMeters = 1000)
    {
        if (meshSizeinMeters < MinimumMeshSize)
            throw new ArgumentOutOfRangeException(Properties.Resources.MESHSIZE_MIN_VIOLATION);

        MeshSize = meshSizeinMeters;
        var dblSquareSize = (double)meshSizeinMeters;
        var maxWidth = double.MinValue;
        var maxHeight = double.MinValue;

        for (var ord = 0; ord < UtmGrid.NumberOfGrids; ord++)
        {
            if (UtmGrid.IsValidOrdinal(ord))
            {
                var theGrid = new UtmGrid(Projection, ord);
                maxWidth = Math.Max(maxWidth, theGrid.MapWidth);
                maxHeight = Math.Max(maxHeight, theGrid.MapHeight);
            }
        }

        var maxHorizontalMeshes =
            (long)Math.Round((maxWidth + dblSquareSize - 1.0) / dblSquareSize, MidpointRounding.AwayFromZero);

        _maxVerticalMeshes =
            (long)Math.Round((maxHeight + dblSquareSize - 1.0) / dblSquareSize, MidpointRounding.AwayFromZero);

        if (maxHorizontalMeshes < 2 || _maxVerticalMeshes < 2)
            throw new ArgumentOutOfRangeException(Properties.Resources.MESHSIZE_TOO_BIG);

        Count = maxHorizontalMeshes * _maxVerticalMeshes;
    }

    /// <summary>
    /// The size of the mesh squares in meters. We only support full meters.
    /// </summary>
    /// <value>The size of the mesh.</value>
    public int MeshSize { get; }

    /// <summary>
    /// The UTM Projection for the Globe we cover with the mesh.
    /// </summary>
    /// <value>The projection.</value>
    public UtmProjection Projection { get; } = new UtmProjection();

    /// <summary>
    /// The (maximum) total number of meshes used to cover an UTM Grid.
    /// Individual Grids may actually be covered by fewer mesh-cells.
    /// </summary>
    /// <value>The count.</value>
    public long Count { get; }

    /// <summary>
    /// The maximum number of a mesh
    /// </summary>
    /// <value>The global count.</value>
    public long GlobalCount => Count * UtmGrid.NumberOfGrids;

    /// <summary>
    /// Return the UtmGrid this mesh belongs to
    /// </summary>
    /// <param name="meshNumber">The mesh number.</param>
    /// <returns>The UtmGrid in which this mesh cell is located</returns>
    /// <exception cref="ArgumentOutOfRangeException">Raised if an invalid mesh number is specified</exception>
    public UtmGrid Grid(long meshNumber)
    {
        ValidateMeshNumber(meshNumber);
        var ord = (int)(meshNumber / Count);
        return new UtmGrid(Projection, ord);
    }

    /// <summary>
    /// Get the globally unique Mesh number of a coordinate
    /// </summary>
    /// <param name="coord">The UTM coordinate to convert</param>
    /// <returns>The mesh number to which the coordinate belongs</returns>
    public long MeshNumber(UtmCoordinate coord)
    {
        var relX = (long)Math.Round(coord.X - (coord.Grid.Origin?.X ?? 0), MidpointRounding.AwayFromZero) / MeshSize;
        var relY = (long)Math.Round(coord.Y - (coord.Grid.Origin?.Y ?? 0), MidpointRounding.AwayFromZero) / MeshSize;
        var res = coord.Grid.Ordinal * Count + relX * _maxVerticalMeshes + relY;
        return res;
    }

    /// <summary>
    /// Get the globally unique Mesh number of a location given by
    /// latitude and longitude.
    /// </summary>
    /// <param name="coord">The location to convert</param>
    /// <returns>The mesh number to which the location belongs</returns>
    public long MeshNumber(GlobalCoordinates coord)
    {
        return MeshNumber((UtmCoordinate)Projection.ToEuclidian(coord));
    }

    /// <summary>
    /// Get the globally unique Mesh number of a location given by
    /// latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude (in degrees)</param>
    /// <param name="longitude">The longitude (in degrees)</param>
    /// <returns>The mesh number to which the location belongs</returns>
    public long MeshNumber(Angle latitude, Angle longitude)
    {
        return MeshNumber(new GlobalCoordinates(latitude, longitude));
    }

    /// <summary>
    /// Validates the mesh number.
    /// </summary>
    /// <param name="meshNumber">The mesh number.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ValidateMeshNumber(long meshNumber)
    {
        if (meshNumber < 0 || meshNumber >= GlobalCount)
            throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_MESH_NUMBER);
    }

    /// <summary>
    /// Meshes the origin.
    /// </summary>
    /// <param name="meshNumber">The mesh number.</param>
    /// <param name="relX">The relative x.</param>
    /// <param name="relY">The relative y.</param>
    private void MeshOrigin(long meshNumber, out long relX, out long relY)
    {
        var local = meshNumber % Count;
        relX = local / _maxVerticalMeshes * MeshSize;
        relY = local % _maxVerticalMeshes * MeshSize;
    }

    /// <summary>
    /// Return the central coordinates of a Mesh given by its number.
    /// Please note that this center is on the UTM map, but at the borders
    /// of a grid this coordinate may actually overlap and belong to another
    /// UTM grid. So if you convert them to a Latitude/Longitude and then back
    /// to an UtmCoordinate, you may get different values.
    /// </summary>
    /// <param name="meshNumber">The number of the mesh</param>
    /// <returns>The UTM coordinates of the center of the square</returns>
    /// <exception cref="ArgumentOutOfRangeException">Raised if an invalid mesh number is specified</exception>
    public UtmCoordinate CenterOf(long meshNumber)
    {
        var theGrid = Grid(meshNumber);
        MeshOrigin(meshNumber, out var relX, out var relY);
        var nx = relX + 0.5 * MeshSize;
        var ny = relY + 0.5 * MeshSize;
        return new UtmCoordinate(theGrid, (theGrid.Origin?.X ?? 0) + nx, (theGrid.Origin?.Y ?? 0) + ny);
    }

    /// <summary>
    /// Return the lower left corner coordinates of a Mesh given by its number.
    /// Please note that this point is on the UTM map, but at the borders
    /// of a grid this coordinate may actually overlap and belong to another
    /// UTM grid. So if you convert them to a Latitude/Longitude and then back
    /// to an UtmCoordinate, you may get different values.
    /// </summary>
    /// <param name="meshNumber">The mesh number.</param>
    /// <returns>The UTM coordinates of the lower left corner of the Mesh</returns>
    /// <exception cref="ArgumentOutOfRangeException">Raised if an invalid mesh number is specified</exception>
    public UtmCoordinate LowerLeft(long meshNumber)
    {
        var theGrid = Grid(meshNumber);
        MeshOrigin(meshNumber, out var relX, out var relY);
        return new UtmCoordinate(theGrid, (theGrid.Origin?.X ?? 0) + relX, (theGrid.Origin?.Y ?? 0) + relY);
    }

    /// <summary>
    /// Return the lower right corner coordinates of a Mesh given by its number.
    /// Please note that this point is on the UTM map, but at the borders
    /// of a grid this coordinate may actually overlap and belong to another
    /// UTM grid. So if you convert them to a Latitude/Longitude and then back
    /// to an UtmCoordinate, you may get different values.
    /// </summary>
    /// <param name="meshNumber">The mesh number.</param>
    /// <returns>The UTM coordinates of the lower right corner of the Mesh</returns>
    /// <exception cref="ArgumentOutOfRangeException">Raised if an invalid mesh number is specified</exception>
    public UtmCoordinate LowerRight(long meshNumber)
    {
        var theGrid = Grid(meshNumber);
        MeshOrigin(meshNumber, out var relX, out var relY);
        relX += MeshSize;
        return new UtmCoordinate(theGrid, (theGrid.Origin?.X ?? 0) + relX, (theGrid.Origin?.Y ?? 0) + relY);
    }

    /// <summary>
    /// Return the upper left corner coordinates of a Mesh given by its number.
    /// Please note that this point is on the UTM map, but at the borders
    /// of a grid this coordinate may actually overlap and belong to another
    /// UTM grid. So if you convert them to a Latitude/Longitude and then back
    /// to an UtmCoordinate, you may get different values.
    /// </summary>
    /// <param name="meshNumber">The mesh number.</param>
    /// <returns>The UTM coordinates of the upper left corner of the Mesh</returns>
    /// <exception cref="ArgumentOutOfRangeException">Raised if an invalid mesh number is specified</exception>
    public UtmCoordinate UpperLeft(long meshNumber)
    {
        var theGrid = Grid(meshNumber);
        MeshOrigin(meshNumber, out var relX, out var relY);
        relY += MeshSize;
        return new UtmCoordinate(theGrid, (theGrid.Origin?.X ?? 0) + relX, (theGrid.Origin?.Y ?? 0) + relY);
    }

    /// <summary>
    /// Return the upper right corner coordinates of a Mesh given by its number.
    /// Please note that this point is on the UTM map, but at the borders
    /// of a grid this coordinate may actually overlap and belong to another
    /// UTM grid. So if you convert them to a Latitude/Longitude and then back
    /// to an UtmCoordinate, you may get different values.
    /// </summary>
    /// <param name="meshNumber">The mesh number.</param>
    /// <returns>The UTM coordinates of the upper right corner of the Mesh</returns>
    /// <exception cref="ArgumentOutOfRangeException">Raised if an invalid mesh number is specified</exception>
    public UtmCoordinate UpperRight(long meshNumber)
    {
        var theGrid = Grid(meshNumber);
        MeshOrigin(meshNumber, out var relX, out var relY);
        relX += MeshSize;
        relY += MeshSize;
        return new UtmCoordinate(theGrid, (theGrid.Origin?.X ?? 0) + relX, (theGrid.Origin?.Y ?? 0) + relY);
    }

    /// <summary>
    /// Get the list of neighbor meshes in a specified "distance". Distance 1 means
    /// direct neighbors, 2 means neighbors that are 2 meshes away etc.
    /// </summary>
    /// <param name="meshNumber">The mesh number</param>
    /// <param name="distance">The distance (0-3 currently supported)</param>
    /// <returns>The list of mesh numbers of the neighbors</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public List<long> Neighborhood(long meshNumber, int distance)
    {
        const int maxDistance = 3;

        if (distance < 0 || distance > maxDistance)
            throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_DISTANCE);

        if (distance == 0)
        {
            return [meshNumber];
        }

        var center = Projection.FromEuclidian(CenterOf(meshNumber));
        var calc = new GeodeticCalculator(Projection.ReferenceGlobe);
        var result = new List<long>();

        for (var y = -distance; y <= distance; y++)
        {
            var bearing = Math.Sign(y) < 0 ? 180.0 : 0.0;

            var vertical = y != 0 ?
                calc.CalculateEndingGlobalCoordinates(center, bearing, Math.Abs(y) * MeshSize)
                : center;

            for (var x = -distance; x <= distance; x++)
            {
                if (x != 0 || y != 0)
                {
                    var add = false;
                    if (Math.Abs(y) == distance)
                    {
                        add = true;
                    }
                    else
                    {
                        if (Math.Abs(x) == distance)
                            add = true;
                    }
                    if (add)
                    {
                        bearing = Math.Sign(x) < 0 ? 270.0 : 90.0;
                        var horizontal = x != 0 ?
                            calc.CalculateEndingGlobalCoordinates(vertical, bearing, Math.Abs(x) * MeshSize)
                            : vertical;

                        result.Add(MeshNumber(horizontal));
                    }
                }
            }
        }
        return result;
    }
}
