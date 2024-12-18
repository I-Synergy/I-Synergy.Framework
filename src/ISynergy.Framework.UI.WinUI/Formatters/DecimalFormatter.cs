﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using Windows.Globalization.NumberFormatting;

namespace ISynergy.Framework.UI.Formatters;

public class DecimalFormatter : INumberFormatter2, INumberParser
{
    private readonly IContext _context;

    public int Decimals { get; set; }

    public DecimalFormatter()
    {
        var scopedContextService = ServiceLocator.Default.GetService<IScopedContextService>();
        _context = scopedContextService.GetService<IContext>();
        Decimals = _context?.NumberFormat?.CurrencyDecimalDigits ?? 2;
    }

    public DecimalFormatter(int decimals)
        : this()
    {
        Decimals = decimals;
    }

    public string FormatDouble(double value) =>
        value.ToString($"N{Decimals}", _context.NumberFormat);

    public string FormatInt(long value) =>
        value.ToString($"N{Decimals}", _context.NumberFormat);

    public string FormatUInt(ulong value) =>
        value.ToString($"N{Decimals}", _context.NumberFormat);

    public double? ParseDouble(string text)
    {
        var decimalSeparator = _context.NumberFormat.NumberDecimalSeparator;
        var groupSeparator = _context.NumberFormat.NumberGroupSeparator;

        if (text.Contains(groupSeparator))
            text = text.Replace(groupSeparator, decimalSeparator);

        var charposition = text.LastIndexOf(decimalSeparator);

        if (charposition != -1)
        {
            var originalPosition = text.Length - charposition;
            text = text.Replace(decimalSeparator, "");
            text = text.Insert(text.Length - (originalPosition - 1), decimalSeparator);

            if (text.StartsWith(decimalSeparator))
                text = text.Insert(0, "0");
        }

        if (double.TryParse(text, _context.NumberFormat, out double result))
            return result;

        return 0d;
    }

    public long? ParseInt(string text)
    {
        if (long.TryParse(text, _context.NumberFormat, out long result))
            return result;

        return 0L;
    }

    public ulong? ParseUInt(string text)
    {
        if (ulong.TryParse(text, _context.NumberFormat, out ulong result))
            return result;

        return 0;
    }
}
