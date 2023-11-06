using I8080Translator;
using SomeAsmTranslator.Source;

namespace SomeAsmTranslator.Operands;

class OperandNumeric : IOperand
{
    private int _value;

    private string _valueString;

    private void Parse(string value)
    {
        var dataParser = new NumericDataParser();

        char last = value.ToUpper().Last();

        if (last == 'H')
            _value = dataParser.ParseHexadecimal(value);
        else if (last == 'O' || last == 'Q')
            _value = dataParser.ParseOctal(value);
        else if (last == 'B')
            _value = dataParser.ParseBinary(value);
        else if (last == 'D' || char.IsDigit(last))
            _value = dataParser.ParseDecimal(value);
        else
            throw new InvalidDataException($"{value} is invalid numeric value");
    }

    public OperandNumeric(string value)
    {
        _valueString = value;
        Parse(value);
    }

    public ushort To16bitAdress()
    {
        if (_value > 0xFFFF)
            throw new InvalidCastException("Cannot convert value to 16 bit adr. Value greater than 16 bit");

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

    public override string ToString()
    {
        return _valueString;
    }
}
