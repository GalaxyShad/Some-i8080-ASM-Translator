using SomeAsmTranslator.Operands;

namespace I8080Translator;

public class AssemblyStatement
{
    public Label? Label { get; private set; }
    public string? Instruction { get; private set; }
    public IOperandMultiple OperandList { get; private set; }
    public string? Comment { get; private set; }

    public AssemblyStatement(Label? label, string? instruction, IOperandMultiple operands, string? comment)
    {
        Label = label;
        Instruction = instruction;
        OperandList = operands;
        Comment = comment;
    }

    public bool IsEmpty() =>
        Label == null && Instruction == null && OperandList.Count == 0 && Comment == null;



    public override string ToString() =>
        $"{Label?.Name ?? "_"} | {Instruction} {string.Join(",", OperandList.Operands)} | {Comment}";
}
