namespace I8080Translator;

public class AssemblyLine
{
    public int? Address { get; set; } = null;
    public required AssembledAssemblyStatement AssembledAssemblyStatement { get; set; }

    public override string ToString() => $"{Address:X4} | {AssembledAssemblyStatement}";
}
