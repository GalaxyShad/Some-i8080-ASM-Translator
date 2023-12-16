using CommandLine;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using SomeAsmTranslator.Source;
using SomeAsmTranslator.Source.Listing;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;
using SomeAsmTranslator.Source.Listing.ListingWriters.Writers;
using Path = System.IO.Path;

namespace I8080Translator;

partial class Program
{
    static private readonly ListingGenerator _listingGenerator = new();

    static void Main(string[] args)
    {
        var parser = new CommandLine.Parser();

        CommandLine.Parser.Default.ParseArguments<ArgumentsOptions>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
    }

    static void ProccessProgram(ArgumentsOptions opts)
    {
        _listingGenerator.IsMachineCodeLineSeperation = !opts.IsKeepAllInstructionBytesOnSameLine;

        var assemblerLines = Assemble(opts.InputFilePath);
        var listing = _listingGenerator.Generate(assemblerLines);

        var listingWriterFactory = new AssemblerListingWriterFactory(listing, assemblerLines);
        var listingWritersList = listingWriterFactory.Get(opts);

        var inFilePath = Path.GetFullPath(opts.InputFilePath);
        var inputFileWihoutExt = GetDirectoryPathWithoutExtension(inFilePath);
        var outFilePath = $"{inputFileWihoutExt}.i8080asm";

        foreach (IAssemblerFileWriter listingWriter in listingWritersList)
        {
            if (listingWriter is AssemblerListingWriterText consoleWriter)
            {
                if (consoleWriter.FileExtension == AssemblerListingWriterText.Extension.Txt)
                    consoleWriter.WriteToConsole();
            }

            listingWriter.WriteToFile(outFilePath);
        }

        Console.WriteLine($"\nSuccessfull");
    }

    static void RunOptions(ArgumentsOptions opts)
    {
#if DEBUG
        ProccessProgram(opts);
#else
        try
        {
            ProccessProgram(opts);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
#endif
    }
    static void HandleParseError(IEnumerable<Error> errs)
    {
        //handle errors
    }

    static string GetDirectoryPathWithoutExtension(string path) =>
        $"{Path.GetDirectoryName(path)}" +
        $"{Path.DirectorySeparatorChar}" +
        $"{Path.GetFileNameWithoutExtension(path)}";

    static private IList<AssemblyLine> AssembleFile(string filepath)
    {
        using var streamReader = new StreamReader(filepath);
        var asm = new Assembler(streamReader);
        return asm.AssembleAll().ToList();
    }

    static private IEnumerable<AssemblyLine> Assemble(string filepath)
    {
#if DEBUG
        return AssembleFile(filepath);
#else
        try
        {
            return AssembleFile(filepath);
        }
        catch (TranslatorLexerException ex)
        {
            throw new Exception(
                $"[ERR] [LEXER] {filepath}\n" +
                $"[Line {ex.ErrorLine}] {ex.Message}\n"
            );
        }
        catch (TranslatorParserException ex)
        {
            throw new Exception(
                $"[ERR] [PARSER] {filepath}\n" +
                $"[Line {ex.ErrorLine}] {ex.Message}\n"
            );
        }
        catch (TranslatorAssemblerException ex)
        {
            throw new Exception(
                $"[ERR] [ASSEMBLER] {filepath}\n" +
                $"[Line {ex.ErrorLine}] {ex.Message}\n"
            );
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"[ERR] [Unexpected error] {filepath}\n" +
                $"[{ex.GetType()}] Send a bug report to the developer.\n" +
                $"{ex.Message}\n"
            );
        }
#endif
    }

    static private string ReadSourceCodeFromFile(string filepath)
    {
        try
        {
            using var streamReader = new StreamReader(filepath);
            return streamReader.ReadToEnd();
        }
        catch (Exception err)
        {
            throw new Exception($"[ERR] [IO] {err.Message}");
        }
    }
}



