using I8080Translator;
using SomeAsmTranslator.Operands;

namespace SomeAsmTranslator.Source;

public class LabelTable
{
    private readonly Dictionary<string, Label> _labelList = new()
    {
        { "B", new Label { Name = "B", Data = new OperandNumeric((int)Register.B), Type = LabelType.Set } },
        { "C", new Label { Name = "C", Data = new OperandNumeric((int)Register.C), Type = LabelType.Set } },
        { "D", new Label { Name = "D", Data = new OperandNumeric((int)Register.D), Type = LabelType.Set } },
        { "E", new Label { Name = "E", Data = new OperandNumeric((int)Register.E), Type = LabelType.Set } },
        { "H", new Label { Name = "H", Data = new OperandNumeric((int)Register.H), Type = LabelType.Set } },
        { "L", new Label { Name = "L", Data = new OperandNumeric((int)Register.L), Type = LabelType.Set } },
        { "M", new Label { Name = "M", Data = new OperandNumeric((int)Register.M), Type = LabelType.Set } },
        { "A", new Label { Name = "A", Data = new OperandNumeric((int)Register.A), Type = LabelType.Set } },
    };

    public Label AddOrUpdateLabel(Label label)
    {
        if (_labelList.ContainsKey(label.Name))
        {
            _labelList[label.Name].Token = label.Token;
            return _labelList[label.Name];
        }

        return _labelList[label.Name] = label;
    }

    public bool Has(Label label) => _labelList.ContainsKey(label.Name);
    public IEnumerable<Label> GetValues() => _labelList.Values;
}
