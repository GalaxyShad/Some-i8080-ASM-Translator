using SomeAsmTranslator.Operands;

namespace MyProject;

public class AssemblyStatement
{
    public Label? Label { get; private set; }
    public string? Instruction { get; private set; }
    public IOperandMultiple OperandList { get; private set; }
    public string? Comment { get; private set; }

    public uint MachineCode { get; set; } = 0;

    public AssemblyStatement(Label? label, string? code, IOperandMultiple operand, string? comment)
    {
        Label = label;
        Instruction = code;
        OperandList = operand;
        Comment = comment;
    }

    public bool IsEmpty() => 
        Label == null && Instruction == null && OperandList.Count == 0 && Comment == null;

    public override string ToString() =>
        $"{(Label == null ? string.Empty : Label.Name),-15}: {(Instruction == null ? string.Empty : Instruction),-5} " +
        $"{string.Join(",", OperandList.Operands),-15} {MachineCode.ToString((MachineCode <= 0xFF) ? "X2" : (MachineCode <= 0xFFFF) ? "X4" : "X6"),-6} ; {Comment:020}";
}
