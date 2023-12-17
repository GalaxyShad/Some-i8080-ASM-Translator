using I8080Translator;
using SomeAsmTranslator.Operands;

namespace SomeAsmTranslator.Source.Listing;

public class ListingGenerator
{
    public bool IsMachineCodeLineSeperation { get; set; } = true;

    public IEnumerable<ListingLine> Generate(IEnumerable<AssemblyLine> assemblyLines)
    {
        var listing = new List<ListingLine>();

        foreach (var asmLine in assemblyLines)
        {
            var asmStatement = asmLine.AssembledAssemblyStatement.AssemblyStatement;
            var bytes = asmLine.AssembledAssemblyStatement.MachineCode;

            listing.Add(new ListingLine()
            {
                Address = $"{asmLine.Address:X4}",
                Label = FormatLabel(asmStatement.Label),
                MachineCode = IsMachineCodeLineSeperation
                    ? bytes.Length != 0 ? $"{bytes[0]:X2}" : ""
                    : FormatMachineCode(bytes),
                AsmCode = FormatInstruction(asmStatement.Instruction?.Name, asmStatement.OperandList),
                Comment = asmStatement.Comment ?? string.Empty,
            });

            if (!IsMachineCodeLineSeperation)
                continue;

            for (int i = 1; i < bytes?.Length; i++)
            {
                listing.Add(new ListingLine()
                {
                    Address = $"{asmLine.Address + i:X4}",
                    MachineCode = $"{bytes?[i]:X2}"
                });
            }
        }

        return listing;
    }

    private string FormatLabel(Label? label) =>
        label?.Type switch
        {
            LabelType.Address => $"{label.Name}:",
            null => string.Empty,
            _ => $"{label.Name}"
        };

    private string FormatInstruction(string? instruction, IOperandMultiple opList) =>
        $"{instruction ?? string.Empty} {string.Join(",", opList.Operands)}";

    private string FormatMachineCode(byte[]? code) =>
        code != null ? BitConverter.ToString(code).Replace("-", "") : string.Empty;
}



