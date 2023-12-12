using I8080Translator;
using SomeAsmTranslator.Source;

namespace SomeAsmTranslator.Operands;

class OperandNumeric : IOperand
{
    private int _value;

    private readonly string _valueString;

    private void Parse(string value)
    {
        var dataParser = new NumericDataParser();

        char last = value.ToUpper().Last();

        _value = last switch
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
        Parse(value);
    }

    public OperandNumeric(int value)
    {
        _value = value;
        _valueString = _value.ToString();
    }

    public ushort To16bitAdress()
    {
        if (_value > 0xFFFF)
            throw new InvalidCastException("Cannot convert value  to 16 bit adr. Value greater than 16 bit");

        return NumericDataParser.SwapBytes((ushort)_value);
    }

    public byte ToImmediateData()
    {
        if (_value > 0xFF)
            throw new InvalidCastException("Cannot convert value to 8 bit data. Value greater than 8 bit");

        return (byte)_value;
    }

    public Register ToRegister()
    {
        if (_value > 7 || _value < 0)
            throw new InvalidCastException("Cannot convert value to register. Unexisting register");

        return (Register)_value;
    }

    public RegisterPair ToRegisterPair()
    {
        throw new InvalidCastException("Numeric value cannot be specified as Register Pair");
    }

    public override string ToString() => _valueString;
}
