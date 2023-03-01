namespace MyProject;

class AssemblyStatement
{
    public Label? Label { get; private set; }
    public IAssemblyInstruction? Instruction { get; private set; }
    public IOperandMultiple OperandList { get; private set; }
    public string? Comment { get; private set; }

    public uint MachineCode { get; private set; } = 0;

    public AssemblyStatement(Label? label, IAssemblyInstruction? code, IOperandMultiple operand, string? comment)
    {
        Label = label;
        Instruction = code;
        OperandList = operand;
        Comment = comment;
    }

    public void ExecuteInstruction()
    {
        if (Instruction == null) return;

        MachineCode = Instruction.GetCode(OperandList);
    }

    public bool IsEmpty() => 
        Label == null && Instruction == null && OperandList.Count == 0 && Comment == null;

    public override string ToString() =>
        $"{(Label == null ? string.Empty : Label.Name).PadRight(15)}: {(Instruction == null ? string.Empty : Instruction.OpCode).PadRight(5)} " +
        $"{string.Join(",", OperandList.Operands).PadRight(15)} {MachineCode.ToString((MachineCode <= 0xFF) ? "X2" : (MachineCode <= 0xFFFF) ? "X4" : "X6").PadRight(6)} ; {Comment:020}";
}
