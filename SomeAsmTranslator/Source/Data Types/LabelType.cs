using System.ComponentModel;

namespace I8080Translator;

public enum LabelType
{
    [Description("Label data is not evaluated yet")]
    Unknown,
    [Description("Ordinary label")]
    Address,
    [Description("Value cannot be redefined")]
    Equ,
    [Description("Value can be redefined")]
    Set
}
