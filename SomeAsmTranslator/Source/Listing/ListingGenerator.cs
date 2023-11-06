using MyProject;
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
            var asmStatement = asmLine.AssemblyStatement;

            listing.Add(new ListingLine()
            {
                Address = $"{asmLine.Address:X4}",
                Label = FormatLabel(asmStatement.Label),
                MachineCode = IsMachineCodeLineSeperation ? $"{asmLine.Bytes?[0]:X2}" : FormatMachineCode(asmLine.Bytes),
                AsmCode = FormatInstruction(asmStatement.Instruction, asmStatement.OperandList),
                Comment = asmStatement.Comment ?? string.Empty,
            });

            if (!IsMachineCodeLineSeperation)
                continue;

            for (int i = 1; i < asmLine.Bytes?.Length; i++)
            {
                listing.Add(new ListingLine()
                {
                    Address = $"{asmLine.Address + i:X4}",
                    MachineCode = $"{asmLine.Bytes?[i]:X2}"
                });
            }
        }

        return listing;
    }

    private string FormatLabel(Label? label) =>
        label is not null ? $"{label.Name}:" : string.Empty;

    private string FormatInstruction(string? instruction, IOperandMultiple opList) =>
        $"{instruction ?? string.Empty} {string.Join(",", opList.Operands)}";

    private string FormatMachineCode(byte[]? code) =>
        code != null ? BitConverter.ToString(code).Replace("-", "") : string.Empty;
}



