using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;

namespace SomeAsmTranslator.Source.Listing.ListingWriters.Writers;

public class AssemblerListingWriterText : IAssemblerFileWriter, IAssemblerConsoleWriter
{
    private readonly IEnumerable<ListingLine> _listing;
    private readonly ListingLineFormatParameters _formatParameters;
    private readonly Extension _fileExtension;

    public enum Extension
    {
        MarkDown,
        Txt
    }

    public AssemblerListingWriterText(
        IEnumerable<ListingLine> listing,
        ListingLineFormatParameters formatParameters,
        Extension fileExtension = Extension.Txt)
    {
        _listing = listing;
        _fileExtension = fileExtension;
        _formatParameters = formatParameters;
    }

    public void WriteToFile(string outFilePathWithoutExtension)
    {
        var extension = _fileExtension == Extension.Txt ? "txt" : "md";

        WriteToStream(new StreamWriter($"{outFilePathWithoutExtension}.{extension}"));
    }

    public void WriteToConsole()
    {
        WriteToStream(Console.Out);
    }

    public Extension FileExtension => _fileExtension;

    private void WriteToStream(TextWriter textWriter)
    {
        using (textWriter)
        {
            textWriter.WriteLine(Header);

            foreach (var line in _listing)
            {
                if (FileExtension == Extension.MarkDown)
                    textWriter.WriteLine($"| {line.ToStringFormatted(_formatParameters)} |");
                else
                    textWriter.WriteLine(line.ToStringFormatted(_formatParameters));
            }
        }
    }

    private string HeaderMarkdown =>
        //"| ADR | MC | LABEL | ASM | COMMENT |\n" +
        //"|-----|----|-------|-----|---------|";
        $"| {"ADR".PadLeft(_formatParameters.AddressColumnWidth)} | " +
        $"{"MC".PadLeft(_formatParameters.MachineCodeColumnWidth)} | " +
        $"{"LABEL".PadRight(_formatParameters.LabelColumnWidth)} | " +
        $"{"ASM".PadRight(_formatParameters.AsmCodeColumnWidth)} | " +
        $"{"COMMENT".PadLeft(_formatParameters.CommentColumnWidth)} |\n" +

        $"|-{new string('-', _formatParameters.AddressColumnWidth)}-|" +
        $"-{new string('-', _formatParameters.MachineCodeColumnWidth)}-|" +
        $"-{new string('-', _formatParameters.LabelColumnWidth)}-|" +
        $"-{new string('-', _formatParameters.AsmCodeColumnWidth)}-|" +
        $"-{new string('-', _formatParameters.CommentColumnWidth)}-|";

    private string HeaderText =>
        $"{"ADR".PadLeft(_formatParameters.AddressColumnWidth)} | " +
        $"{"MC".PadLeft(_formatParameters.MachineCodeColumnWidth)} | " +
        $"{"LABEL".PadRight(_formatParameters.LabelColumnWidth)} | " +
        $"{"ASM".PadRight(_formatParameters.AsmCodeColumnWidth)} {_formatParameters.CommentDividerChar} " +
        $"{"COMMENT"}";

    private string Header => _fileExtension == Extension.Txt ? HeaderText : HeaderMarkdown;
}



