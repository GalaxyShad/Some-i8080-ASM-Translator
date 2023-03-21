using System.Text.RegularExpressions;

namespace MyProject;

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

    private string NormalizeString(string str) => str.Trim().ToUpper();

    public int ParseHexadecimal(string source)
    {
        source = NormalizeString(source);

        if (!IsValidHex(source))
            throw new InvalidDataException("Invalid HEX value");

        return Convert.ToInt32(source[..^1], 16);
    }

    public int ParseDecimal(string source)
    {
        source = NormalizeString(source);

        if (!IsValidDecimal(source))
            throw new InvalidDataException("Invalid Decimal value");

        return Convert.ToInt32((source.Last() == 'D') ? source[..^1] : source, 10);
    }

    public int ParseOctal(string source)
    {
        source = NormalizeString(source);

        if (!IsValidOctal(source))
            throw new InvalidDataException("Invalid Octal value");

        return Convert.ToInt32(source[..^1], 8);
    }

    public int ParseBinary(string source)
    {
        source = NormalizeString(source);

        if (!IsValidBinary(source))
            throw new InvalidDataException("Invalid Binary value");

        return Convert.ToInt32(source[..^1], 2);
    }

    public byte ParseAscii(char ascii) => (byte)ascii;

    public static ushort SwapBytes(ushort x) => (ushort)((x >> 8) | (x << 8));
}
