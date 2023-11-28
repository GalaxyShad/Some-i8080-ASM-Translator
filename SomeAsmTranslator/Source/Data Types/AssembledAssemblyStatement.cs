namespace I8080Translator;

public class AssembledAssemblyStatement
{
    public required AssemblyStatement AssemblyStatement { get; set; }
    public required byte[] MachineCode { get; set; }

    public override string ToString() =>
        $" {(MachineCode != null
            ? string.Join("", MachineCode.Select(x => x.ToString("X2")))
            : string.Empty)} " +
        $"| {AssemblyStatement}";
}
