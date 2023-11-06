using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using I8080Translator;
using SomeAsmTranslator.Source.Listing;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;

namespace SomeAsmTranslator.Source.Listing.ListingWriters.Writers;

public class AssemblerListingWriterWord : IAssemblerFileWriter
{
    private readonly IEnumerable<ListingLine> _listing;

    public AssemblerListingWriterWord(IEnumerable<ListingLine> listing)
    {
        _listing = listing;
    }

    public void WriteToFile(string outFilePathWithoutExtension)
    {
        using (var document = WordprocessingDocument.Create(
            $"{outFilePathWithoutExtension}.docx",
            DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
        {
            var body = ConfigureDocumentStructure(document);

            var listingTable = GenerateTable();

            body.AppendChild(listingTable);
        }
    }

    private Body ConfigureDocumentStructure(WordprocessingDocument document)
    {
        var mainPart = document.AddMainDocumentPart();

        mainPart.Document = new Document();

        var styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();

        var wordDocumentGenerator = new WordDocumentGenerator();
        wordDocumentGenerator.CreateStyleDefinitionsPart(styleDefinitionsPart);

        return mainPart.Document.AppendChild(new Body());
    }

    private Table GenerateTable()
    {
        // Create a table.
        var table = new Table();

        var tblProp = new TableProperties();

        // Make the table width 100% of the page width.
        var tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };

        // Apply
        var tableStyle = new TableStyle { Val = "a3" };

        tblProp.TableStyle = tableStyle;
        tblProp.Append(tableWidth);
        table.AppendChild(tblProp);

        // Add columns to the table.
        var tableGrid = new TableGrid(
            new GridColumn(), new GridColumn(), new GridColumn(), new GridColumn(), new GridColumn());
        table.AppendChild(tableGrid);

        foreach (var line in _listing)
        {
            var tableRow = new TableRow();

            var GetRunProps = () => new RunProperties(
                new RunFonts() { Ascii = "Consolas", HighAnsi = "Consolas" },
                new FontSize() { Val = "24" }
            );

            var runs = new Run[]
            {
                new Run(new Text(line.Address)),
                new Run(new Text(line.Label)),
                new Run(new Text(line.MachineCode)),
                new Run(new Text(line.AsmCode)),
                new Run(new Text(line.Comment))
            };

            foreach (var run in runs)
            {
                run.PrependChild(GetRunProps());
                tableRow.Append(new TableCell(new Paragraph(run)));
            }


            table.AppendChild(tableRow);
        }

        return table;
    }
}



