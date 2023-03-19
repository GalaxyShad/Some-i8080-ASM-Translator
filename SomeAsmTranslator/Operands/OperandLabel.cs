using MyProject;

namespace SomeAsmTranslator.Operands;

class OperandLabel : IOperand
{
    private Label _label;

    public OperandLabel(Label label)
    {
        _label = label;
    }

    public ushort To16bitAdress() => NumericDataParser.SwapBytes(_label.Value);

    public byte ToImmediateData() =>
        throw new InvalidCastException("Label cannot be converted to 8bit data");

    public Register ToRegister() =>
        throw new InvalidCastException("Label cannot be converted to Register");

    public RegisterPair ToRegisterPair() =>
        throw new InvalidCastException("Label cannot be converted to Register Pair");

    public override string ToString()
    {
        return _label.Name;
    }
}
