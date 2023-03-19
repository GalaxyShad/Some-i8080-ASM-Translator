namespace MyProject;

class OperandLabelAssignedValue : IOperand
{
    private Label _label;

    public OperandLabelAssignedValue(Label label)
    {
        _label = label;
    }

    public ushort To16bitAdress()
    {
        if (_label.Value > 0xFFFF)
            throw new InvalidCastException(
                $"Cannot convert label {_label.Name} to 16 bit adress. " +
                $"Value {_label.Value} is greater than 0xFFFF");

        return NumericDataParser.SwapBytes(_label.Value);
    }

    public byte ToImmediateData()
    {
        if (_label.Value > 0xFF)
            throw new InvalidCastException(
                $"Cannot convert label {_label.Name} to 8 bit data. " +
                $"Value {_label.Value} is greater than 0xFF");

        return (byte)_label.Value;
    }

    public Register ToRegister()
    {
        if (_label.Value > 7 || _label.Value < 0)
            throw new InvalidCastException(
                $"Cannot convert label {_label.Name} to register. " +
                $"Value {_label.Value} is out of range");

        return (Register)_label.Value;
    }

    public RegisterPair ToRegisterPair()
    {
        return _label.Name switch
        {
            "B" => RegisterPair.BC,
            "D" => RegisterPair.DE,
            "H" => RegisterPair.HL,
            "SP" => RegisterPair.SP,
            "PSW" => RegisterPair.PSW,
            _ => throw new InvalidCastException($"{_label.Name} is invalid Register Pair name"),
        };
    }

    public override string ToString()
    {
        return _label.Name;
    }
}
