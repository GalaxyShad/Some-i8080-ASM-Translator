using I8080Translator;

namespace SomeAsmTranslator.Source;

public class LabelTable
{
    private readonly Dictionary<string, Label> _labelList = new()
    {
        { "B", new Label { Name = "B", Data = (ushort)Register.B, Type = LabelType.Set } },
        { "C", new Label { Name = "C", Data = (ushort)Register.C, Type = LabelType.Set } },
        { "D", new Label { Name = "D", Data = (ushort)Register.D, Type = LabelType.Set } },
        { "E", new Label { Name = "E", Data = (ushort)Register.E, Type = LabelType.Set } },
        { "H", new Label { Name = "H", Data = (ushort)Register.H, Type = LabelType.Set } },
        { "L", new Label { Name = "L", Data = (ushort)Register.L, Type = LabelType.Set } },
        { "M", new Label { Name = "M", Data = (ushort)Register.M, Type = LabelType.Set } },
        { "A", new Label { Name = "A", Data = (ushort)Register.A, Type = LabelType.Set } },
    };
    public Label AddOrUpdateLabel(Label label) =>
        _labelList.ContainsKey(label.Name) ? _labelList[label.Name] : _labelList[label.Name] = label;

    public bool Has(Label label) => _labelList.ContainsKey(label.Name);
}
