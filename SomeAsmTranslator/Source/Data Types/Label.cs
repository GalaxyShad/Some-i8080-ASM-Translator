namespace I8080Translator;

public class Label
{
    public required string Name { get; set; }
    public ushort? Data { get; set; }
    public LabelType Type { get; set; } = LabelType.Unknown;
    public override string ToString() => 
        $"{Name} = {(Data != null ? $"{Data:X4}" : "NULL")}, {Type}";

}
