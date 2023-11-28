using CommandLine;
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

    static void RunOptions(ArgumentsOptions opts)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    static void HandleParseError(IEnumerable<Error> errs)
    {
        //handle errors
    }

    static string GetDirectoryPathWithoutExtension(string path) =>
        $"{Path.GetDirectoryName(path)}" +
        $"{Path.DirectorySeparatorChar}" +
        $"{Path.GetFileNameWithoutExtension(path)}";

    static private IEnumerable<AssemblyLine> Assemble(string filepath)
    {
        try
        {
            using var streamReader = new StreamReader(filepath);
            var asm = new Assembler(streamReader);
            return asm.AssembleAll().ToList();
        }
        catch (TranslatorLexerException ex)
        {
            throw new Exception(
                $"[ERR] [LEXER] {ex.GetType()}\n" +
                $"Error at line {ex.ErrorLine}\n" +
                $"      {ex.Message}"
            );
        }
        catch (TranslatorParserException ex)
        {
            throw new Exception(
                $"[ERR] [PARSER] {ex.GetType()}\n" +
                $"Error at line {ex.ErrorLine}\n" +
                $"      {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"[ERR] [Unexpected error] [{ex.GetType()}] Send a bug report to the developer.\n" +
                $"{ex.Message}\n"
            );
        }
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



