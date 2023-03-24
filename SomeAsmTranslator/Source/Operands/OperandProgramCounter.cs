using MyProject;
using SomeAsmTranslator.Source;

namespace SomeAsmTranslator.Operands;

class OperandProgramCounter : IOperand
{
    private ushort? _pgCounter;

    public ushort Value
    {
        get => _pgCounter != null ?
            (ushort)_pgCounter :
            throw new ArgumentNullException($"Program Counter is not assigned");
    }

    public OperandProgramCounter(ushort? pgCounter = null)
    {
        _pgCounter = pgCounter;
    }

    public ushort To16bitAdress() => NumericDataParser.SwapBytes(Value);

    public byte ToImmediateData() =>
        throw new InvalidCastException("Program Counter cannot be specified as 8bit data");

    public Register ToRegister() =>
        throw new InvalidCastException("Program Counter cannot be specified as Register");

    public RegisterPair ToRegisterPair() =>
        throw new InvalidCastException("Program Counter cannot be specified as Register Pair");

    public override string ToString()
    {
        return Value.ToString();
    }
}
