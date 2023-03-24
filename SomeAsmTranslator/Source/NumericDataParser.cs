using System.Text.RegularExpressions;

namespace SomeAsmTranslator.Source;

class NumericDataParser
{
    private bool IsValidHex(string hex) =>
        Regex.IsMatch(hex, "^\\d+([\\dA-F])*H$");
    private bool IsValidDecimal(string dec) =>
        Regex.IsMatch(dec, "^\\d+D?$");
    private bool IsValidOctal(string oct) =>
        Regex.IsMatch(oct, "^[0-7]+[OQ]$");
    private bool IsValidBinary(string bin) =>
        Regex.IsMatch(bin, "^[01]+B$");

    private string RemoveUnderscores(string source)
    {
        char last = source.Last();

        if (last is '_')
            throw new FormatException($"Cannot put underscores at the end of a literal -> {source}");

        if (last is 'Q' or 'O' or 'D' or 'H' or 'B' && source.LastIndexOf('_') == source.Length - 2)
            throw new FormatException($"Cannot put underscores prior to an {last} suffix -> {source}");

        return source.Replace("_", string.Empty);
    }


    private string NormalizeString(string str) => RemoveUnderscores(str.Trim().ToUpper());

    public int ParseHexadecimal(string source)
    {
        source = NormalizeString(source);

        if (!IsValidHex(source))
            throw new InvalidDataException($"Invalid HEX value -> {source}");

        return Convert.ToInt32(source[..^1], 16);
    }

    public int ParseDecimal(string source)
    {
        source = NormalizeString(source);

        if (!IsValidDecimal(source))
            throw new InvalidDataException($"Invalid Decimal value -> {source}");

        return Convert.ToInt32(source.Last() == 'D' ? source[..^1] : source, 10);
    }

    public int ParseOctal(string source)
    {
        source = NormalizeString(source);

        if (!IsValidOctal(source))
            throw new InvalidDataException($"Invalid Octal value -> {source}");

        return Convert.ToInt32(source[..^1], 8);
    }

    public int ParseBinary(string source)
    {
        source = NormalizeString(source);

        if (!IsValidBinary(source))
            throw new InvalidDataException($"Invalid Binary value -> {source}");

        return Convert.ToInt32(source[..^1], 2);
    }

    public byte ParseAscii(char ascii) => (byte)ascii;


    public static ushort SwapBytes(ushort x) => (ushort)(x >> 8 | x << 8);
}
