namespace MyProject;

public class AssemblyLine
{
    public int? Address { get; set; } = null;
    public AssemblyStatement AssemblyStatement { get; set; }
    public byte[]? Bytes { get; set; } = null;

    public AssemblyLine(int? address, AssemblyStatement assemblyStatement, byte[]? bytes = null)
    {
        Address = address;
        AssemblyStatement = assemblyStatement;
        Bytes = bytes;
    }
}
