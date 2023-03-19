using ISynergy.Framework.Geography.Common;
using ISynergy.Framework.Geography.Global;
using ISynergy.Framework.Geography.Projection;
using System.Globalization;

namespace ISynergy.Framework.Geography.Utm
{
    /// <summary>
    /// The globe is partioned into Grids by the UTM projection.
    /// This structure represents such a grid.
    /// </summary>
    public struct UtmGrid : IEquatable<UtmGrid>
    {
        /// <summary>
        /// The delta
        /// </summary>
        private const double Delta = 1e-12;
        /// <summary>
        /// The minimum zone
        /// </summary>
        private const int MinZone = 1;
        /// <summary>
        /// The maximum zone
        /// </summary>
        private const int MaxZone = 60;
        /// <summary>
        /// The minimum band
        /// </summary>
        private const int MinBand = 0;
        /// <summary>
        /// The maximum band
        /// </summary>
        private const int MaxBand = 19;
        /// <summary>
        /// The band chars
        /// </summary>
        private const string BandChars = "CDEFGHJKLMNPQRSTUVWX";

        /// <summary>
        /// The number of (horizontal) zones
        /// </summary>
        public const int NumberOfZones = MaxZone;

        /// <summary>
        /// The number of (vertical) bands.
        /// </summary>
        public const int NumberOfBands = 1 + MaxBand;

        /// <summary>
        /// The theoretical number of UTM Grids on the globe. Actually there are
        /// fewer ones, because there are exceptions.  <seealso cref="NumberOfUsedGrids" />
        /// </summary>
        public const int NumberOfGrids = MaxZone * (1 + MaxBand);

        /// <summary>
        /// The number of UTM Grids really used. There are 3 potential combinations
        /// in the X-Band that are an exception and not used (32X, 34X, 36X)
        /// </summary>
        public const int NumberOfUsedGrids = NumberOfGrids - 3;

        /// <summary>
        /// Horizontal stepwidth for the Grids
        /// </summary>
        public static readonly Angle Xstep = 6.0;

        /// <summary>
        /// Vertical stepwidth for the Grids
        /// </summary>
        public static readonly Angle Ystep = 8.0;

        /// <summary>
        /// The band
        /// </summary>
        private int _band;
        /// <summary>
        /// The ll coordinates
        /// </summary>
        private GlobalCoordinates _llCoordinates;
        /// <summary>
        /// The map height
        /// </summary>
        private double _mapHeight;
        /// <summary>
        /// The map width
        /// </summary>
        private double _mapWidth;
        /// <summary>
        /// The origin
        /// </summary>
        private UtmCoordinate _origin;
        /// <summary>
        /// The zone
        /// </summary>
        private int _zone;

        /// <summary>
        /// Initializes a new instance of the <see cref="UtmGrid"/> struct.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <exception cref="ArgumentNullException"></exception>
        private UtmGrid(UtmProjection projection)
        {
            // Assign default values
            Projection = projection ?? throw new ArgumentNullException(Properties.Resources.PROJECTION_NULL);
            _origin = null;
            _mapHeight = 0.0;
            _mapWidth = 0.0;

            Width = Xstep;
            Height = Ystep;

            _zone = 0;
            _band = 0;
            _llCoordinates = new GlobalCoordinates();
        }

        /// <summary>
        /// Instantiate a new UTM Grid object
        /// </summary>
        /// <param name="projection">The UTM projection this grid belongs to</param>
        /// <param name="zone">The zone of the grid</param>
        /// <param name="band">The band of the grid</param>
        /// <exception cref="ArgumentOutOfRangeException">Throw if zone or band are invalid</exception>
        /// <exception cref="ArgumentNullException">Thrown if the projection is null</exception>
        public UtmGrid(UtmProjection projection, int zone, int band) : this(projection)
        {
            SetZoneAndBandInConstructor(zone, band);
        }

        /// <summary>
        /// Instantiate a new UTM Grid object
        /// </summary>
        /// <param name="projection">The UTM projection this grid belongs to</param>
        /// <param name="zone">The zone of the grid</param>
        /// <param name="band">The band of the grid</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if zone or band are requested</exception>
        /// <exception cref="ArgumentNullException">Thrown if the projection is null</exception>
        public UtmGrid(UtmProjection projection, int zone, char band)
            : this(projection, zone, BandChars.IndexOf(band))
        {
        }

        /// <summary>
        /// Instantiate a grid by its ordinal number.
        /// </summary>
        /// <param name="projection">The UTM projection this Grid belongs to</param>
        /// <param name="ordinal">The unique ordinal number of the grid</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public UtmGrid(UtmProjection projection, int ordinal) : this(projection)
        {
            if (ordinal < 0 || ordinal >= NumberOfGrids)
                throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_ORDINAL);

            SetZoneAndBandInConstructor(1 + ordinal / NumberOfBands, ordinal % NumberOfBands);
        }

        /// <summary>
        /// The UTM Grid for a given latitude/longitude
        /// </summary>
        /// <param name="projection">The projection to use</param>
        /// <param name="coord">Latitude/Longitude of the location</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public UtmGrid(UtmProjection projection, GlobalCoordinates coord) : this(projection)
        {
            if (coord.Latitude < projection.MinLatitude || coord.Latitude > projection.MaxLatitude)
                throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_LATITUDE);

            var longitude = MercatorProjection.NormalizeLongitude(coord.Longitude).Degrees + 180.0;
            var latitude = projection.NormalizeLatitude(coord.Latitude);
            var band = (int)((latitude - projection.MinLatitude).Degrees / Ystep.Degrees);
            if (band == NumberOfBands)
            {
                var northernLimit = projection.MinLatitude + NumberOfBands * Ystep;
                if (latitude >= northernLimit && latitude <= projection.MaxLatitude)
                    band--;
            }
            var zone = (int)(longitude / Xstep.Degrees) + 1;
            SetZoneAndBandInConstructor(zone, band, true);

            if (_zone == 31 && Band == 'V')
            {
                var delta = coord.Longitude.Degrees - _llCoordinates.Longitude.Degrees - Width.Degrees;
                if (Math.Sign(delta) != -1)
                {
                    Zone = _zone + 1;
                }
            }
            else if (Band == 'X')
            {
                if (_zone == 32 || _zone == 34 || _zone == 36)
                {
                    var delta = coord.Longitude.Degrees - CenterMeridian.Degrees;
                    if (Math.Sign(delta) == -1)
                        Zone = _zone - 1;
                    else
                        Zone = _zone + 1;
                }
            }
        }

        /// <summary>
        /// The projection this grid belongs to
        /// </summary>
        /// <value>The projection.</value>
        public UtmProjection Projection { get; }

        /// <summary>
        /// The UTM coordinates of the left corner of the wider latitude of the zone
        /// which is the latitude closer to the aequator.
        /// </summary>
        /// <value>The origin.</value>
        public UtmCoordinate Origin
        {
            get
            {
                if (_origin is null)
                    ComputeFlatSize();
                return _origin;
            }
        }

        /// <summary>
        /// The width of this grid (in meters)
        /// </summary>
        /// <value>The width of the map.</value>
        public double MapWidth
        {
            get
            {
                if (_origin is null)
                    ComputeFlatSize();
                return _mapWidth;
            }
        }

        /// <summary>
        /// The height of this grid (in meters)
        /// </summary>
        /// <value>The height of the map.</value>
        public double MapHeight
        {
            get
            {
                if (_origin is null)
                    ComputeFlatSize();
                return _mapHeight;
            }
        }

        /// <summary>
        /// The UTM zone the point belongs to.
        /// </summary>
        /// <value>The zone.</value>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Zone
        {
            get { return _zone; }
            set
            {
                if (value < MinZone || value > MaxZone)
                    throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_ZONE);
                _zone = value;
                ComputeSizes();
                if (_origin is not null)
                    ComputeFlatSize();
            }
        }

        /// <summary>
        /// Get the numeric representation of the band (0 based)
        /// </summary>
        /// <value>The band nr.</value>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int BandNr
        {
            get { return _band; }
            private set
            {
                if (value < MinBand || value > MaxBand)
                    throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_BAND);
                _band = value;
                ComputeSizes();
                if (_origin is not null)
                    ComputeFlatSize();
            }
        }

        /// <summary>
        /// The UTM band the point belongs to.
        /// </summary>
        /// <value>The band.</value>
        /// <exception cref="ArgumentOutOfRangeException">If the band character is out of its limits</exception>
        //TODO Check the correct Exception type
        public char Band
        {
            get { return BandChars[_band]; }
            set { BandNr = BandChars.IndexOf(value); }
        }

        /// <summary>
        /// Width of the Grid (as an Angle)
        /// </summary>
        /// <value>The width.</value>
        public Angle Width { get; private set; }

        /// <summary>
        /// Height of the Grid (as an Angle)
        /// </summary>
        /// <value>The height.</value>
        public Angle Height { get; private set; }

        /// <summary>
        /// Unique numbering of the Grids. The most western, most southern
        /// gets #0. Then we go north continue counting, when reaching the
        /// northern limit we go to the lowest south of the next zone to the
        /// east of the current one.
        /// </summary>
        /// <value>The ordinal.</value>
        public int Ordinal => (_zone - 1) * BandChars.Length + _band;

        /// <summary>
        /// Return true is this is a northern band
        /// </summary>
        /// <value><c>true</c> if this instance is northern; otherwise, <c>false</c>.</value>
        public bool IsNorthern => _band >= NumberOfBands / 2;

        /// <summary>
        /// Computes the size of the flat.
        /// </summary>
        private void ComputeFlatSize()
        {
            UtmCoordinate other;
            UtmCoordinate right;

            if (IsNorthern)
            {
                _origin = (UtmCoordinate)Projection.ToEuclidian(LowerLeftCorner);
                other = (UtmCoordinate)Projection.ToEuclidian(UpperLeftCorner);
                right = (UtmCoordinate)Projection.ToEuclidian(LowerRightCorner);
            }
            else
            {
                _origin = (UtmCoordinate)Projection.ToEuclidian(UpperLeftCorner);
                other = (UtmCoordinate)Projection.ToEuclidian(LowerLeftCorner);
                right = (UtmCoordinate)Projection.ToEuclidian(UpperRightCorner);
            }
            _mapHeight = Math.Abs(_origin.Y - other.Y);
            _mapWidth = Math.Abs(_origin.X - right.X);
        }

        /// <summary>
        /// Check wether or not an Ordinal number is valid
        /// </summary>
        /// <param name="ordinal">The ordinal to check</param>
        /// <returns>True if this is a valid ordinal number</returns>
        public static bool IsValidOrdinal(int ordinal)
        {
            if (ordinal < 0 || ordinal >= NumberOfGrids)
                return false;

            var zone = 1 + ordinal / NumberOfBands;
            var band = ordinal % NumberOfBands;
            if (band == MaxBand && (zone == 32 || zone == 34 || zone == 36))
                return false;

            return true;
        }

        /// <summary>
        /// Computes the sizes.
        /// </summary>
        private void ComputeSizes()
        {
            // Intial position of the grid
            _llCoordinates = new GlobalCoordinates(
                _band * Ystep + Projection.MinLatitude,
                (_zone - 1) * Xstep + MercatorProjection.MinLongitude);

            if (_band == MaxBand)
                Height = Ystep + 4.0;

            if (_zone == 32 && Band == 'V')
            {
                Width += 3.0;
                _llCoordinates.Longitude -= 3.0;
            }
            else if (_zone == 31 && Band == 'V')
            {
                Width -= 3.0;
            }
            else if (_band == MaxBand)
            {
                if (_zone == 31 || _zone == 37)
                {
                    Width += 3.0;
                    if (_zone == 37)
                        _llCoordinates.Longitude -= 3.0;
                }
                else if (_zone == 33 || _zone == 35)
                {
                    Width += 6.0;
                    _llCoordinates.Longitude -= 3.0;
                }
            }
        }

        /// <summary>
        /// Sets the zone and band in constructor.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <param name="band">The band.</param>
        /// <param name="noGridException">if set to <c>true</c> [no grid exception].</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void SetZoneAndBandInConstructor(int zone, int band, bool noGridException = false)
        {
            if (!noGridException && band == MaxBand && (zone == 32 || zone == 34 || zone == 36))
                throw new ArgumentOutOfRangeException(Properties.Resources.GRID_EXCEPTION);

            if (zone < MinZone || zone > MaxZone)
                throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_ZONE);

            _zone = zone;

            if (band < MinBand || band > MaxBand)
                throw new ArgumentOutOfRangeException(Properties.Resources.INVALID_BAND);

            _band = band;
            ComputeSizes();
        }

        /// <summary>
        /// Sets the zone and band in constructor.
        /// </summary>
        /// <param name="zone">The zone.</param>
        /// <param name="band">The band.</param>
        private void SetZoneAndBandInConstructor(int zone, char band)
        {
            SetZoneAndBandInConstructor(zone, BandChars.IndexOf(band), true);
        }

        /// <summary>
        /// Check wether a point is in the grid
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <returns>True if the point is inside</returns>
        public bool IsInside(GlobalCoordinates point)
        {
            if (point.Longitude >= LowerLeftCorner.Longitude && point.Longitude <= LowerRightCorner.Longitude)
            {
                if (point.Latitude >= LowerLeftCorner.Latitude && point.Latitude <= UpperLeftCorner.Latitude)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Compare the grid to another grid for equality.
        /// </summary>
        /// <param name="other">The other grid</param>
        /// <returns>True if they are equal</returns>
        public bool Equals(UtmGrid other)
        {
            return other.Projection.Equals(Projection) &&
                    _band == other._band && _zone == other._zone;
        }

        /// <summary>
        /// Compare these coordinates to another object for equality.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is UtmGrid))
                return false;
            var other = (UtmGrid)obj;
            return ((IEquatable<UtmGrid>)this).Equals(other);
        }

        /// <summary>
        /// Test two Grids for equality
        /// </summary>
        /// <param name="lhs">The first grid</param>
        /// <param name="rhs">The second grid</param>
        /// <returns>True if the first equals the second grid</returns>
        public static bool operator ==(UtmGrid lhs, UtmGrid rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Test two Grids for inequality
        /// </summary>
        /// <param name="lhs">The first grid</param>
        /// <param name="rhs">The second grid</param>
        /// <returns>True if the first is not equal to the second grid</returns>
        public static bool operator !=(UtmGrid lhs, UtmGrid rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// The Hashcode
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Ordinal;
        }

        /// <summary>
        /// The culture invariant string representation of the Grid
        /// </summary>
        /// <returns>ZThe name of the Grid</returns>
        public override string ToString()
        {
            return _zone.ToString(NumberFormatInfo.InvariantInfo) + Band;
        }

        #region Corners
        /// <summary>
        /// The latitude/longitude of the lower left corner of this grid
        /// </summary>
        /// <value>The lower left corner.</value>
        public GlobalCoordinates LowerLeftCorner
        {
            get { return _llCoordinates; }
        }

        /// <summary>
        /// The latitude/longitude of the upper right corner of this grid
        /// </summary>
        /// <value>The upper right corner.</value>
        public GlobalCoordinates UpperRightCorner
        {
            get
            {
                return new GlobalCoordinates(
                    _llCoordinates.Latitude + Height - Delta,
                    _llCoordinates.Longitude + Width - Delta);
            }
        }

        /// <summary>
        /// The latitude/longitude of the upper left corner of this grid
        /// </summary>
        /// <value>The upper left corner.</value>
        public GlobalCoordinates UpperLeftCorner
        {
            get
            {
                return new GlobalCoordinates(
                    _llCoordinates.Latitude + Height - Delta,
                    _llCoordinates.Longitude);
            }
        }

        /// <summary>
        /// The latitude/longitude of the lower right corner of this grid
        /// </summary>
        /// <value>The lower right corner.</value>
        public GlobalCoordinates LowerRightCorner
        {
            get
            {
                return new GlobalCoordinates(
                    _llCoordinates.Latitude,
                    _llCoordinates.Longitude + Width - Delta);
            }
        }

        /// <summary>
        /// The longitude of the center of this Grid
        /// </summary>
        /// <value>The center meridian.</value>
        public Angle CenterMeridian
        {
            get { return _llCoordinates.Longitude + Width * 0.5; }
        }
        #endregion

        #region Neighbors
        /// <summary>
        /// The western neighbor of the grid
        /// </summary>
        /// <value>The west.</value>
        public UtmGrid West
        {
            get
            {
                var newZone = _zone - 1;
                if (newZone < MinZone)
                    newZone = MaxZone;
                if (_band == MaxBand && (newZone == 32 || newZone == 34 || newZone == 36))
                    newZone--;
                return new UtmGrid(Projection, newZone, _band);
            }
        }

        /// <summary>
        /// The eastern neighbor of the grid
        /// </summary>
        /// <value>The east.</value>
        public UtmGrid East
        {
            get
            {
                var newZone = _zone + 1;
                if (newZone > MaxZone)
                    newZone = MinZone;
                if (_band == MaxBand && (newZone == 32 || newZone == 34 || newZone == 36))
                    newZone++;
                return new UtmGrid(Projection, newZone, _band);
            }
        }

        /// <summary>
        /// The northern neighbor of the grid
        /// </summary>
        /// <value>The north.</value>
        /// <exception cref="Exception">If there is no northern neighbor</exception>
        public UtmGrid North
        {
            get
            {
                if (Band == 'U' && _zone == 31 ||
                    Band == 'W' && (_zone == 32 || _zone == 34 || _zone == 36))
                    throw new Exception(Properties.Resources.NO_UNIQUE_NORTH_NEIGHBOR);

                var newBand = _band + 1;

                if (newBand > MaxBand)
                    throw new Exception(Properties.Resources.NO_NORTH_NEIGHBOR);

                return new UtmGrid(Projection, _zone, newBand);
            }
        }

        /// <summary>
        /// The southern neighbor of the grid
        /// </summary>
        /// <value>The south.</value>
        /// <exception cref="Exception">If there is no southern neighbor</exception>
        public UtmGrid South
        {
            get
            {
                if (Band == 'W' && _zone == 31 || Band == 'X' && _zone >= 31 && _zone <= 37)
                    throw new Exception(Properties.Resources.NO_UNIQUE_SOUTH_NEIGHBOR);

                var newBand = _band - 1;

                if (newBand < MinBand)
                    throw new Exception(Properties.Resources.NO_SOUTH_NEIGHBOR);

                return new UtmGrid(Projection, _zone, newBand);
            }
        }
        #endregion
    }
}
