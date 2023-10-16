using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.VisualBasic;
using SomeAsmTranslator.Operands;
using SomeAsmTranslator.Source;
using System.Collections;

namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        string inFilePath = string.Empty;
        var paramList = new List<string>();

        var assembler = new Assembler();
        var listingGenerator = new ListingGenerator();

        if (args.Length == 0)
        {
            Console.WriteLine("[ERR] No args specified, type -help for Help");
            return;
        }

        bool isGenerateWord = false;
        bool isGenerateCsv = false;
        bool isGenerateBinary = false;
        bool isGenerateMarkdown = false;

        foreach (var arg in args)
        {
            if (arg.First() is '-')
            {
                switch (arg)
                {
                    case "-word":
                        isGenerateWord = true;
                        break;
                    case "-csv":
                        isGenerateCsv = true;
                        break;
                    case "-samelinebyte":
                        listingGenerator.IsMachineCodeLineSeperation = false;
                        break;
                    case "-bin":
                        isGenerateBinary = true;
                        break;
                    case "-md":
                        isGenerateMarkdown = true;
                        break;
                    case "-help":
                        Console.WriteLine(
                            "-word          - create listing in .docx word table\n" +
                            "-csv           - create .csv table listing file\n" +
                            "-samelinebyte  - keep all instruction bytes on the same line\n" +
                            "-bin           - generate binary file\n" +
                            "-md            - generate markdown"           
                        );
                        break;
                    default:
                        Console.WriteLine($"[ERR] Unknown flag {arg}");
                        return;
                }
            }
            else
                inFilePath = Path.GetFullPath(arg);
        }

        if (string.IsNullOrEmpty(inFilePath))
        {
            Console.WriteLine("[ERR] No input files specified, type -help for Help");
            return;
        }

        if (!File.Exists(inFilePath))
        {
            Console.WriteLine($"[ERR] {inFilePath} File does no exist");
            return;
        }

        string sourceCode = string.Empty;

        try
        {
            var streamReader = new StreamReader(inFilePath);
            sourceCode = streamReader.ReadToEnd();
            streamReader.Close();
        }
        catch (Exception err)
        {
            Console.WriteLine($"[ERR] [IO] {err.Message}");
            return;
        }

        IEnumerable<AssemblyLine>? list = null;

        try
        {
            list = assembler.AssembleAll(sourceCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR] [ASM] [{ex.GetType()}] {ex.Message}");
            return;
        }

        var listing = listingGenerator.Generate(list);

        var maxMachineCodeWidth = listing.Max(x => x.MachineCode.Length);
        var maxLabelWidth = listing.Max(x => x.Label.Length);
        var maxAsmCodeWidth = listing.Max(x => x.AsmCode.Length);

        

        var outFilePath =
            $"{Path.GetDirectoryName(inFilePath)}" +
            $"{Path.DirectorySeparatorChar}" +
            $"{Path.GetFileNameWithoutExtension(inFilePath)}";

        if (isGenerateWord)
            SaveToWord(listing, $"{outFilePath}.docx");

        if (isGenerateCsv)
        {
            var sw = new StreamWriter($"{outFilePath}.csv");
            foreach (var line in listing)
            {
                sw.WriteLine($"{line.Address};" +
                    $"{line.MachineCode};" +
                    $"{line.Label};" +
                    $"{line.AsmCode};" +
                    $"{line.Comment};");
            }
            sw.Close();
        }

        if (isGenerateBinary)
        {
            SaveToBinary(list, $"{outFilePath}.i8080asm.bin");
        }

        if (isGenerateMarkdown)
        {
            var sw = new StreamWriter($"{outFilePath}.i8080asm.md");

            sw.WriteLine("| ADR | MC | LABEL | ASM | COMMENT |");
            sw.WriteLine("|------|----|-------|-------------|-|");
            foreach (var line in listing)
            {
                var stringLine =
                    $"| " +
                    $"{line.Address.PadLeft(4)} | " +
                    $"{line.MachineCode.PadLeft(maxMachineCodeWidth)} | " +
                    $"{line.Label.PadRight(maxLabelWidth)} | " +
                    $"{line.AsmCode.PadRight(maxAsmCodeWidth)} | " +
                    $"{line.Comment} |";

                sw.WriteLine(stringLine);
            }
            sw.Close();
        }

        var sww = new StreamWriter($"{outFilePath}.i8080asm.txt");
        foreach (var line in listing)
        {
            var stringLine =
                $"{line.Address.PadLeft(4)} | " +
                $"{line.MachineCode.PadLeft(maxMachineCodeWidth)} | " +
                $"{line.Label.PadRight(maxLabelWidth)} | " +
                $"{line.AsmCode.PadRight(maxAsmCodeWidth)} ; " +
                $"{line.Comment}";

            Console.WriteLine(stringLine);

            sww.WriteLine(stringLine);
        }
        sww.Close();



        Console.WriteLine("\nSuccessfull");
    }

    static void SaveToBinary(IEnumerable<AssemblyLine> asmLineList, string filepath)
    {
        var orderedAsmLine = asmLineList.OrderBy(x => x.Address).ToList();
        AssemblyLine? previousLine = null;

        using (var fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
        {
            foreach (var line in orderedAsmLine)
            {
                if (line.Bytes == null) { continue; };

                if (previousLine == null)
                {
                    fs.Write(line.Bytes);
                    previousLine = line;
                    continue;
                }

                int dif = (int)((line.Address - (previousLine.Address + previousLine.Bytes.Length)));
                for (int i = 0; i < dif; i++)
                    fs.WriteByte(0);

                fs.Write(line.Bytes);
                previousLine = line;
            }
        }
    }

    static void SaveToWord(IEnumerable<ListingLine> listing, string filepath)
    {
        using (WordprocessingDocument doc = WordprocessingDocument.Create(
            filepath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
        {
            // Add a main document part. 
            MainDocumentPart mainPart = doc.AddMainDocumentPart();

            // Create the document structure and add some text.
            mainPart.Document = new Document();

            StyleDefinitionsPart styleDefinitionsPart = mainPart.AddNewPart<StyleDefinitionsPart>();

            WordDocumentGenerator wordDocumentGenerator = new WordDocumentGenerator();
            wordDocumentGenerator.CreateStyleDefinitionsPart(styleDefinitionsPart);

            Body body = mainPart.Document.AppendChild(new Body());

            // Create a table.
            Table tbl = new Table();

            TableProperties tblProp = new TableProperties();

            // Make the table width 100% of the page width.
            TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };

            // Apply
            TableStyle ts = new TableStyle { Val = "a3" };

            tblProp.TableStyle = ts;
            tblProp.Append(tableWidth);
            tbl.AppendChild(tblProp);

            // Add 3 columns to the table.
            TableGrid tg = new TableGrid(
                new GridColumn(), new GridColumn(), new GridColumn(), new GridColumn(), new GridColumn());
            tbl.AppendChild(tg);

            foreach (var line in listing)
            {
                TableRow tr = new TableRow();

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
                    tr.Append(new TableCell(new Paragraph(run)));
                }
                   

                tbl.AppendChild(tr);
            }

            // Add the table to the document
            body.AppendChild(tbl);
        }
    }
}

class ListingLine
{
    public string Address { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string AsmCode { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}


class ListingGenerator
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
                MachineCode = (IsMachineCodeLineSeperation) ? $"{asmLine.Bytes?[0]:X2}" : FormatMachineCode(asmLine.Bytes),
                AsmCode = FormatInstruction(asmStatement.Instruction, asmStatement.OperandList),
                Comment = asmStatement.Comment ?? string.Empty,
            });

            if (!IsMachineCodeLineSeperation)
                continue;

            for (int i = 1; i < asmLine.Bytes?.Length; i++)
            {
                listing.Add(new ListingLine()
                {
                    Address = $"{(asmLine.Address+i):X4}",
                    MachineCode = $"{asmLine.Bytes?[i]:X2}"
                });
            }
        }

        return listing;
    }

    private string FormatLabel(Label? label) => 
        (label is not null) ? $"{label.Name}:" : string.Empty;

    private string FormatInstruction(string? instruction, IOperandMultiple opList) =>
        $"{instruction ?? string.Empty} {string.Join(",", opList.Operands)}";

    private string FormatMachineCode(byte[]? code) =>
        (code != null) ? BitConverter.ToString(code).Replace("-", "") : string.Empty;
}
