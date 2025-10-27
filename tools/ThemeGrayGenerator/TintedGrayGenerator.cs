using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ThemeGrayGenerator;

/// <summary>
/// Utility to generate tinted grays for all theme files
/// </summary>
public class TintedGrayGenerator
{
    private static readonly Dictionary<string, int[]> BaseGrays = new()
    {
      ["000"] = [0xF0, 0xF0, 0xF0],
        ["100"] = [0xE1, 0xE1, 0xE1],
        ["200"] = [0xC8, 0xC8, 0xC8],
        ["300"] = [0xAC, 0xAC, 0xAC],
        ["400"] = [0x91, 0x91, 0x91],
        ["500"] = [0x6E, 0x6E, 0x6E],
        ["600"] = [0x40, 0x40, 0x40],
        ["900"] = [0x21, 0x21, 0x21],
        ["950"] = [0x14, 0x14, 0x14]
    };

    /// <summary>
    /// Blends two colors together
    /// </summary>
    private static string BlendColors(int r1, int g1, int b1, int r2, int g2, int b2, double tintAmount = 0.15)
    {
var r = (int)Math.Round(r2 * (1 - tintAmount) + r1 * tintAmount);
        var g = (int)Math.Round(g2 * (1 - tintAmount) + g1 * tintAmount);
      var b = (int)Math.Round(b2 * (1 - tintAmount) + b1 * tintAmount);

        return $"{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Generates tinted gray XAML for a given primary color
    /// </summary>
public static string GenerateTintedGrays(string primaryColorHex)
    {
  // Remove # if present
        primaryColorHex = primaryColorHex.TrimStart('#');

        // Parse primary color RGB
        var r1 = Convert.ToInt32(primaryColorHex.Substring(0, 2), 16);
        var g1 = Convert.ToInt32(primaryColorHex.Substring(2, 2), 16);
        var b1 = Convert.ToInt32(primaryColorHex.Substring(4, 2), 16);

        var sb = new StringBuilder();
      sb.AppendLine($"    <!-- Tinted Grays (15% blend with Primary #{primaryColorHex.ToUpper()}) -->");

        foreach (var (key, gray) in BaseGrays)
        {
var tintedColor = BlendColors(r1, g1, b1, gray[0], gray[1], gray[2]);
          sb.AppendLine($" <Color x:Key=\"TintedGray{key}\">#{tintedColor}</Color>");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates XAML output for all theme colors
    /// </summary>
    public static void Main(string[] args)
    {
    // All 48 theme colors
        var themeColors = new Dictionary<string, string>
  {
            ["f7630c"] = "F7630C",
            ["ca5010"] = "CA5010",
 ["da3b01"] = "DA3B01",
["ef6950"] = "EF6950",
            ["d13438"] = "D13438",
            ["ff4343"] = "FF4343",
        ["e74856"] = "E74856",
  ["e81123"] = "E81123",
       ["ea005e"] = "EA005E",
          ["c30052"] = "C30052",
   ["e3008c"] = "E3008C",
     ["bf0077"] = "BF0077",
     ["c239b3"] = "C239B3",
          ["9a0089"] = "9A0089",
     ["0063b1"] = "0063B1",
   ["8e8cd8"] = "8E8CD8",
         ["6b69d6"] = "6B69D6",
            ["8764b8"] = "8764B8",
          ["744da9"] = "744DA9",
            ["b146c2"] = "B146C2",
 ["881798"] = "881798",
            ["0099bc"] = "0099BC",
["2d7d9a"] = "2D7D9A",
   ["00b7c3"] = "00B7C3",
            ["038387"] = "038387",
     ["00b294"] = "00B294",
 ["018574"] = "018574",
            ["00cc6a"] = "00CC6A",
            ["10893e"] = "10893E",
            ["7a7574"] = "7A7574",
       ["5d5a58"] = "5D5A58",
            ["68768a"] = "68768A",
          ["515c6b"] = "515C6B",
            ["567c73"] = "567C73",
         ["486860"] = "486860",
     ["498205"] = "498205",
 ["107c10"] = "107C10",
  ["767676"] = "767676",
          ["4c4a48"] = "4C4A48",
      ["69797e"] = "69797E",
            ["4a5459"] = "4A5459",
            ["647c64"] = "647C64",
     ["525e54"] = "525E54",
            ["847545"] = "847545",
            ["7e735f"] = "7E735F"
};

        Console.WriteLine("=== Tinted Gray Generator for MAUI Themes ===\n");
        Console.WriteLine("Copy and paste the output below into each theme file:\n");
        Console.WriteLine("="+ new string('=', 60) + "\n");

        foreach (var (key, hex) in themeColors)
        {
      Console.WriteLine($"Theme: {key}");
       Console.WriteLine(GenerateTintedGrays(hex));
     Console.WriteLine();
        }
    }
}
