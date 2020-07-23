﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISynergy.Framework.Windows.Controls.Charts;
using ISynergy.Framework.Windows.Controls.Charts.Enumerations;
using SkiaSharp;

namespace ISynergy.Framework.Windows.Samples.Data
{
    public class ChartData
    {
        public readonly SKColor TextColor = SKColors.Gray;

        public readonly SKColor[] Colors =
        {
                        SKColor.Parse("#266489"),
                        SKColor.Parse("#68B9C0"),
                        SKColor.Parse("#90D585"),
                        SKColor.Parse("#F3C151"),
                        SKColor.Parse("#F37F64"),
                        SKColor.Parse("#424856"),
                        SKColor.Parse("#8F97A4"),
                        SKColor.Parse("#DAC096"),
                        SKColor.Parse("#76846E"),
                        SKColor.Parse("#DABFAF"),
                        SKColor.Parse("#A65B69"),
                        SKColor.Parse("#97A69D"),
                };

        private int ColorIndex = 0;

        public SKColor NextColor()
        {
            var result = Colors[ColorIndex];
            ColorIndex = (ColorIndex + 1) % Colors.Length;
            return result;
        }

        public (string label, int value)[] PositiveData =
        {
            ("January",     400),
            ("February",    600),
            ("March",       900),
            ("April",       100),
            ("May",         200),
            ("June",        500),
            ("July",        300),
            ("August",      200),
            ("September",   200),
            ("October",     800),
            ("November",    950),
            ("December",    700),

        };

        public (string label, int value)[] MixedData =
        {
            ("January",    -400),
            ("February",    600),
            ("March",       900),
            ("April",       100),
            ("May",        -200),
            ("June",        500),
            ("July",        300),
            ("August",     -200),
            ("September",   200),
            ("October",     800),
            ("November",    950),
            ("December",   -700),

        };

        public (string label, int value)[] NegativeData =
        {
            ("January",     -400),
            ("February",    -600),
            ("March",       -900),
            ("April",       -100),
            ("May",         -200),
            ("June",        -500),
            ("July",        -300),
            ("August",      -200),
            ("September",   -200),
            ("October",     -800),
            ("November",    -950),
            ("December",    -700),

        };

        public Chart[] CreateXamarinSample()
        {
            var entries = new[]
            {
                new ChartEntry(212)
                {
                    Label = "UWP",
                    ValueLabel = "212",
                    Color = SKColor.Parse("#2c3e50")
                },
                new ChartEntry(248)
                {
                    Label = "Android",
                    ValueLabel = "248",
                    Color = SKColor.Parse("#77d065")
                },
                new ChartEntry(128)
                {
                    Label = "iOS",
                    ValueLabel = "128",
                    Color = SKColor.Parse("#b455b6")
                },
                new ChartEntry(514)
                {
                    Label = "Shared",
                    ValueLabel = "514",
                    Color = SKColor.Parse("#3498db")
                }
            };

            return new Chart[]
            {
                new BarChart() { Entries = entries },
                new PointChart() { Entries = entries },
                new LineChart()
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    LineSize = 4,
                    PointMode = PointMode.Square,
                    PointSize = 10,
                },
                new DonutChart() { Entries = entries },
                new RadialGaugeChart() { Entries = entries },
                new RadarChart() { Entries = entries },
            };
        }

        public Chart[] CreateQuickstart()
        {
            var entries = new[]
            {
                new ChartEntry(200)
                {
                        Label = "January",
                        ValueLabel = "200",
                        Color = SKColor.Parse("#266489"),
                },
                new ChartEntry(400)
                {
                        Label = "February",
                        ValueLabel = "400",
                        Color = SKColor.Parse("#68B9C0"),
                },
                new ChartEntry(100)
                {
                        Label = "March",
                        ValueLabel = "100",
                        Color = SKColor.Parse("#90D585"),
                },
            };

            return new Chart[]
            {
                new BarChart() { Entries = entries, LabelTextSize = 12, LabelOrientation = Orientation.Horizontal },
                new PointChart() { Entries = entries, LabelTextSize = 12, LabelOrientation = Orientation.Horizontal },
                new LineChart() { Entries = entries, LabelTextSize = 12, LabelOrientation = Orientation.Horizontal },
                new DonutChart() { Entries = entries, LabelTextSize = 12 },
                new RadialGaugeChart() { Entries = entries, LabelTextSize = 12 },
                new RadarChart() { Entries = entries, LabelTextSize = 12 },
            };
        }

        public ChartEntry[] CreateEntries(int values, bool hasPositiveValues, bool hasNegativeValues, bool hasLabels, bool hasValueLabel, bool isSingleColor)
        {
            ColorIndex = 0;

            (string label, int value)[] data;

            if (hasPositiveValues && hasNegativeValues)
            {
                data = MixedData;
            }
            else if (hasPositiveValues)
            {
                data = PositiveData;
            }
            else if (hasNegativeValues)
            {
                data = NegativeData;
            }
            else
            {
                data = new (string label, int value)[0];
            }

            data = data.Take(values).ToArray();

            return data.Select(d => new ChartEntry(d.value)
            {
                Label = hasLabels ? d.label : null,
                ValueLabel = hasValueLabel ? d.value.ToString() : null,
                TextColor = TextColor,
                Color = isSingleColor ? Colors[2] : NextColor(),
            }).ToArray();
        }
    }
}
