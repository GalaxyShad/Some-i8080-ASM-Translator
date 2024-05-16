namespace SomeAsmTranslator.Source.Listing;

public class ListingLine
{
    public string Address { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string AsmCode { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;

    public string Csv => $"{HexFormat(Address)};{HexFormat(MachineCode)};{Label};{AsmCode};{Comment};";
    public string ToStringFormatted(ListingLineFormatParameters parameters)
    {
        return $"{Address.PadLeft(parameters.AddressColumnWidth)} | " +
               $"{MachineCode.PadLeft(parameters.MachineCodeColumnWidth)} | " +
               $"{Label.PadRight(parameters.LabelColumnWidth)} | " +
               $"{AsmCode.PadRight(parameters.AsmCodeColumnWidth)} {parameters.CommentDividerChar} " +
               $"{Comment}";
    }

    private string HexFormat(string hex) => hex + (string.IsNullOrEmpty(hex) ? string.Empty : "h");

}



