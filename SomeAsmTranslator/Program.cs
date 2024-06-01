using CommandLine;
using SomeAsmTranslator.Source;
using SomeAsmTranslator.Source.Listing;
using SomeAsmTranslator.Source.Listing.ListingWriters.Writers;
using Path = System.IO.Path;

namespace I8080Translator;

partial class Program
{
    private static readonly ListingGenerator ListingGenerator = new();

    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<ArgumentsOptions>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
    }

    private static void ProccessProgram(ArgumentsOptions opts)
    {
        ListingGenerator.IsMachineCodeLineSeperation = !opts.IsKeepAllInstructionBytesOnSameLine;

        var assemblerLines = Assemble(opts.InputFilePath);
        var listing = ListingGenerator.Generate(assemblerLines);

        var listingWriterFactory = new AssemblerListingWriterFactory(listing, assemblerLines);
        var listingWritersList = listingWriterFactory.Get(opts);

        var inFilePath = Path.GetFullPath(opts.InputFilePath);
        var inputFileWithoutExt = GetDirectoryPathWithoutExtension(inFilePath);
        var outFilePath = $"{inputFileWithoutExt}.i8080asm";

        foreach (var listingWriter in listingWritersList)
        {
            if (listingWriter is AssemblerListingWriterText { FileExtension: AssemblerListingWriterText.Extension.Txt } consoleWriter) 
                consoleWriter.WriteToConsole();

            listingWriter.WriteToFile(outFilePath);
        }

        Console.WriteLine("\nSuccessful");
    }

    private static void RunOptions(ArgumentsOptions opts)
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
            Console.Error.WriteLine(ex.Message);
        }
#endif
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        //handle errors
    }

    private static string GetDirectoryPathWithoutExtension(string path) =>
        $"{Path.GetDirectoryName(path)}" +
        $"{Path.DirectorySeparatorChar}" +
        $"{Path.GetFileNameWithoutExtension(path)}";

    private static IList<AssemblyLine> AssembleFile(string filepath)
    {
        using var streamReader = new StreamReader(filepath);
        var asm = new Assembler(streamReader);
        return asm.AssembleAll().ToList();
    }

    private static IEnumerable<AssemblyLine> Assemble(string filepath)
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

    private static string ReadSourceCodeFromFile(string filepath)
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



