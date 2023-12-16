using I8080Translator;
using SomeAsmTranslator.Source;

namespace SomeAsmTranslator.Operands;

class OperandNumeric : IOperand
{
    private readonly string _valueString;

    private int Parse(string value)
    {
        var dataParser = new NumericDataParser();

        char last = value.ToUpper().Last();

        return last switch
        {
            'H'        => dataParser.ParseHexadecimal(value),
            'O' or 'Q' => dataParser.ParseOctal(value),
            'B'        => dataParser.ParseBinary(value),
            'D'        => dataParser.ParseDecimal(value),

            var x when char.IsDigit(x)  => dataParser.ParseDecimal(value),

            _ => throw new InvalidDataException($"{value} is invalid numeric value")
        };
    }

    public OperandNumeric(string value)
    {
        _valueString = value;
    }

    public OperandNumeric(int value) : this(value.ToString()) { }

    public ushort To16bitAdress()
    {
        var res = Parse(_valueString);

        if (res > 0xFFFF)
            throw new InvalidCastException($"Cannot convert value '{res:X}' to 16 bit adr. Value greater than 16 bit");

        return NumericDataParser.SwapBytes((ushort)res);
    }

    public byte ToImmediateData()
    {
        var res = Parse(_valueString);

        if (res > 0xFF)
            throw new InvalidCastException($"Cannot convert value '{res:X}' to 8 bit data. Value greater than 8 bit");

        return (byte)res;
    }

    public Register ToRegister()
    {
        var res = Parse(_valueString);

        if (res > 7 || res < 0)
            throw new InvalidCastException($"Cannot convert value '{res:X}' to register. Unexisting register");

        return (Register)res;
    }

    public RegisterPair ToRegisterPair()
    {
        throw new InvalidCastException($"Numeric value cannot be specified as Register Pair");
    }

    public override string ToString() => _valueString;

    public ushort ToRawData() => (ushort)Parse(_valueString);
}
