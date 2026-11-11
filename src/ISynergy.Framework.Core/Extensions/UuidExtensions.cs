using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace ISynergy.Framework.Core.Extensions;

public static class Uuidv7
{
    // Time values and sequence counter from the last call
    private static long _x = 0;
    private static long _y = 0;
    private static long _z = 0;
    private static int _seq = 0;

    // Time values and sequence counter from the last asOfNs call.
    // This ensures real-time operations will stay monotonic.
    private static long _x_asOf = 0;
    private static long _y_asOf = 0;
    private static long _z_asOf = 0;
    private static int _seq_asOf = 0;

    /// <summary>
    /// The current time in integer nanoseconds, 
    /// measured from the Unix epoch (midnight on 1 January 1970).
    /// </summary>
    /// <returns>Integer number of nanoseconds.</returns>
    public static long CurrentTime() => 100 * (DateTime.UtcNow.Ticks - DateTime.UnixEpoch.Ticks);

    /// <summary>
    /// Return a zero UUID.
    /// </summary>
    /// <returns></returns>
    public static Guid Empty() => NewGuid(0);

    /// <summary>
    /// A new UUIDv7 Guid, which is time-ordered, with a nominal
    /// time resolution of 100ns and 32 bits of randomness.
    /// The current time is used, unless overridden.
    /// Consecutive calls using the same Uuidv7 instance employ a 14-bit sequence
    /// counter so their uuids/Id25s stay time-ordered. The lowest 48 bits are random.
    /// 
    /// The special value of 0 gives an all zero uuid.
    /// </summary>
    /// <param name="asOfCurrentTime">Optional time to use, in integer nanoseconds since the Unix epoch.</param>
    /// <returns>
    /// Guid that follows UUID v7 format whose string and integer representations are time-sortable.
    /// </returns>
    public static Guid NewGuid(long? asOfCurrentTime = null) // NOSONAR
    {
        /* The time resolution stored here is 24 fractional bits,
         * corresponding to 50ns. This is sufficient for the underlying
         * 100ns tick size. The actual clock precision may be several
         * times less than this.

          0                   1                   2                   3
          0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
         +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
         |                            unixts                             |
         +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
         |unixts |   msec (12 bits)      |  ver  |     usec (12 bits)    |
         +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
         |var|       seq (14 bits)       |          rand (16 bits)       |
         +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
         |                          rand (32 bits)                       |
         +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ 
         */
        int uuidVersion = 7;
        // The UUID variant is the top-most bits of byte #9.
        // The number of bits increases because as new variants were added, 
        // the previous maximum variant value with "all 1 bits" got
        // extended with a new 0/1 bit to the right.
        // Thus variant 1 for RFC4122 UUIDs is represented by bits 10.
        // (Variant 2 for Microsoft Guids is represented by three bits 110).
        int uuidVariant = 0b10;
        int maxSeqValue = 0x3FFF;

        if (!TryResolveTimestamp(asOfCurrentTime, out var ns))
            return Guid.Empty;

        // Get timestamp components of length 32, 16, 12 bits,
        // with the first 36 bits being whole seconds and
        // the remaining 24 bits being fractional seconds.
        long x = Math.DivRem(ns, 16_000_000_000L, out long rest1);
        long y = Math.DivRem(rest1 << 16, 16_000_000_000L, out long rest2);
        long z = Math.DivRem(rest2 << 12, 16_000_000_000L, out long _);

        // Timestamp tick and 14-bit sequence counter: consecutive calls that land on the same tick
        // increment the counter, timestamp changes reset it.
        var seq = asOfCurrentTime is not null
            ? NextSequence(x, y, z, maxSeqValue, ref _x, ref _y, ref _z, ref _seq)
            : NextSequence(x, y, z, maxSeqValue, ref _x_asOf, ref _y_asOf, ref _z_asOf, ref _seq_asOf);

        // Last 8 bytes of uuid have variant and sequence in first two bytes,
        // then six bytes of randomness.
        var last8Bytes = new byte[8];
        RandomNumberGenerator.Fill(last8Bytes);
        last8Bytes[0] = (byte)(uuidVariant << 6 | seq >> 8);
        last8Bytes[1] = (byte)(seq & 0xFF);

        // Don't use Guid(bytes[]), which internally uses a mix of
        // big and little endian byte orderings for historical reasons
        // (see https://en.wikipedia.org/wiki/Universally_unique_identifier#Variants).
        // Instead use Guid(int, short, short, bytes[]), which doesn't mix endianness.
        return new Guid(
            (int)x,
            (short)y,
            (short)((uuidVersion << 12) + z & 0xFFFF),
            last8Bytes
        );
    }

    /// <summary>
    /// Resolves the timestamp (in integer nanoseconds) to encode in the UUID.
    /// </summary>
    /// <param name="asOfCurrentTime">Optional time to use, in integer nanoseconds since the Unix epoch.</param>
    /// <param name="nanoseconds">The resolved timestamp.</param>
    /// <returns><c>false</c> when the all-zero UUID was requested (a timestamp of 0).</returns>
    private static bool TryResolveTimestamp(long? asOfCurrentTime, out long nanoseconds)
    {
        if (asOfCurrentTime is null)
        {
            nanoseconds = CurrentTime();
            return true;
        }

        // No randomness. In case one is needed for "earlier than everything else"
        nanoseconds = asOfCurrentTime.Value;
        return nanoseconds != 0;
    }

    /// <summary>
    /// Returns the 14-bit sequence counter for this timestamp tick, keeping consecutive UUIDs ordered.
    /// </summary>
    /// <param name="x">Whole-seconds part of the timestamp.</param>
    /// <param name="y">First fractional part of the timestamp.</param>
    /// <param name="z">Second fractional part of the timestamp.</param>
    /// <param name="maxSeqValue">The maximum value the sequence counter may reach.</param>
    /// <param name="lastX">The whole-seconds part of the previous call.</param>
    /// <param name="lastY">The first fractional part of the previous call.</param>
    /// <param name="lastZ">The second fractional part of the previous call.</param>
    /// <param name="sequence">The sequence counter of the previous call.</param>
    /// <returns>The sequence value to encode.</returns>
    private static int NextSequence(
        long x,
        long y,
        long z,
        int maxSeqValue,
        ref long lastX,
        ref long lastY,
        ref long lastZ,
        ref int sequence)
    {
        if (x == lastX && y == lastY && z == lastZ)
        {
            // Shouldn't be possible to call often enough that seq overflows before the next time tick.
            // If that does happen, subsequent uuids with that time tick will be unique (because of the
            // random bytes) but no longer ordered.
            if (sequence < maxSeqValue)
                sequence += 1;
        }
        else
        {
            sequence = 0;
            lastX = x;
            lastY = y;
            lastZ = z;
        }

        return sequence;
    }

    /// <summary>
    /// A UUIDv7 Guid transformed into a 25-character lower-case string which 
    /// preserves the time-ordered property. Using this distinct representation can
    /// reduce the chance that some v4 UUIDs end up in a collection of v7 UUIDs.
    /// 
    /// As for the Uuidv7.Guid(), the current time is used, unless overridden.
    /// The special value of 0 gives an all zero uuid.
    /// </summary>
    /// <param name="guid">Guid to use.</param>
    /// <returns>
    /// 25-character string like "0q974fmmvghw8qfathid7qekc" that is time-sortable.
    /// </returns>
    public static string ToId25(this Guid guid)
    {
        const string alphabet = "0123456789abcdefghijkmnopqrstuvwxyz"; // 35 chars - no "l"
        char[] id25_chars = new char[25];

        byte[] arr = guid.ToByteArray();
        // C# GUIDs use a mix of big endian and little ending ordering.
        // e.g. Guid  00010203-0405-0607-0809-0A0B0C0D0E0F becomes
        // byte array 030201000504070608090A0B0C0D0E0F.
        // So do endian conversion for first 8 bytes as long-short-short.
        byte b;
        b = arr[3];
        arr[3] = arr[0];
        arr[0] = b;
        b = arr[2];
        arr[2] = arr[1];
        arr[1] = b;
        b = arr[4];
        arr[4] = arr[5];
        arr[5] = b;
        b = arr[6];
        arr[6] = arr[7];
        arr[7] = b;

        var isZero = Array.TrueForAll(arr, b => b == 0);
        int uuidVersion = arr[6] >> 4;
        int uuidVariant = arr[8] >> 6;
        if ((!isZero) && (uuidVersion != 7 || uuidVariant != 2))
        {
            throw new ArgumentException("Not v7 UUID");
        }

        var rest = new BigInteger(arr, true, true);
        BigInteger rem;
        BigInteger divisor = 35;

        for (var pos = 24; pos >= 0; pos--)
        {
            rem = rest % divisor;
            rest /= divisor;
            char c = alphabet[(int)rem];
            id25_chars[pos] = c;
        }
        return new string(id25_chars);
    }


    /// <summary>
    /// Check whether the tick values on this system are being returned
    /// with ~100ns precision. We should not see 15ms!
    /// Typical values on Win11 seem to be 132ns.
    /// </summary>
    /// <returns>String with description of timing analysis.</returns>
    public static string CheckTimingPrecision()
    {
        var distinctValues = new HashSet<long>();
        var sw = Stopwatch.StartNew();
        long numLoops = 0;
        while (sw.Elapsed.TotalSeconds < 0.5 && numLoops < 1000)
        {
            distinctValues.Add(CurrentTime());
            numLoops += 1;
        }
        sw.Stop();

        var numSamples = distinctValues.Count;
        var actualPrecisionNs = 1_000_000 * sw.Elapsed.TotalMilliseconds / numSamples;
        var maxPrecisionNs = 1_000_000 * sw.Elapsed.TotalMilliseconds / numLoops;

        if (numSamples == numLoops)
            return $"Precision is {actualPrecisionNs:0}ns with no repeats in {numLoops:N0} loops taking {sw.Elapsed.TotalMilliseconds}ms";
        else
            return $"Precision is {actualPrecisionNs:0}ns rather than {maxPrecisionNs:0}ns ({numSamples:N0} unique timestamps from {numLoops:N0} loops taking {sw.Elapsed.TotalMilliseconds}ms)";
    }
}