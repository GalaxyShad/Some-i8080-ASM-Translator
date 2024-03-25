namespace I8080Translator;

class OperandLabel : IOperand
{
    private readonly Label _label;

    public LabelType LabelType => _label.Type;
    public bool IsRegisterPair => _label.Name is "B" or "D" or "H" or "SP" or "PSW";

    public OperandLabel(Label label)
    {
        _label = label;
    }

    public ushort To16bitAdress()
    {
        if (_label.Type is LabelType.Unknown)
            return 0x0000;

        if (_label.Data == null)
            throw new ArgumentNullException(
                $"Cannot convert label {_label.Name} to 16bit data. Label data is null");

        return _label.Data.To16bitAdress();
    }

    public byte ToImmediateData()
    {
        if (_label.Type is LabelType.Unknown)
            return 0x00;

        if (_label.Type is LabelType.Address)
            throw new ArgumentNullException(
                $"Address label {_label.Name} cannot be used as immediate data.");

        if (_label.Data == null)
            throw new ArgumentNullException(
                $"Cannot convert label {_label.Name} to immediate data. Label data is null");

        return _label.Data.ToImmediateData();

        //if (_label.Data > 0xFF)
        //    throw new InvalidCastException(
        //        $"Cannot convert label {_label.Name} to 8 bit data. " +
        //        $"Value {_label.Data} is greater than 0xFF");

        //return (byte)_label.Data;
    }

    public Register ToRegister()
    {
        if (_label.Type is LabelType.Unknown)
            return Register.B;

        if (_label.Type is LabelType.Address)
            throw new ArgumentNullException(
                $"Address label {_label.Name} cannot be used as register");

        if (_label.Data == null)
            throw new ArgumentNullException(
                $"Cannot convert label {_label.Name} to Register. Label data is null");

        //if (_label.Data > 7 || _label.Data < 0)
        //    throw new InvalidCastException(
        //        $"Cannot convert label {_label.Name} to register. " +
        //        $"Value {_label.Data} is out of range");

        return _label.Data.ToRegister();
    }

    public RegisterPair ToRegisterPair()
    {
        if (_label.Type is LabelType.Address)
            throw new ArgumentNullException(
                $"Address label {_label.Name} cannot be used as register pair");

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

    public ushort ToRawData() => _label.Data?.ToRawData() ?? 0;
}
