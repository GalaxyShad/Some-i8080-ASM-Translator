using SomeAsmTranslator.Operands;

namespace I8080Translator;

class OperandList : IOperandMultiple
{
    public int Count => _operands.Count;

    public IEnumerable<IOperand> Operands => _operands;

    private List<IOperand> _operands = new ();

    public IOperand First => _operands.First();

    public IOperand Second => _operands.ElementAt(1);

    public void Add(IOperand operand) => _operands.Add(operand);
}
