using I8080Translator;

namespace SomeAsmTranslator.Operands;

public interface IOperandMultiple
{
    public int Count { get; }

    public IEnumerable<IOperand> Operands { get; }

    public IOperand First { get; }

    public IOperand Second { get; }
}
