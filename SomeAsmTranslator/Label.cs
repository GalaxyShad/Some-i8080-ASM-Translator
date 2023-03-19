namespace MyProject;

class Label
{
    public string Name { get; }
    public ushort Value { 
        get => _value != null ? (ushort)_value : throw new ArgumentNullException($"Label {Name} value is not assigned"); 
        set => _value = value; 
    }

    private ushort? _value = null;

    public Label(string name, ushort? value = null)
    {
        Name = name;
        _value = value;
    }


}
