namespace I8080Translator;

public class Instruction
{
    public required string Name { get; set; }
    public long Line { get; set; }

    public override string ToString() => Name;
}
