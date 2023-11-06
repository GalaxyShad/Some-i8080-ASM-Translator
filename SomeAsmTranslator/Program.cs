﻿using CommandLine;
using SomeAsmTranslator.Source;
using SomeAsmTranslator.Source.Listing;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;
using SomeAsmTranslator.Source.Listing.ListingWriters.Writers;
using Path = System.IO.Path;

namespace MyProject;

partial class Program
{
    static private readonly Assembler _assembler = new ();
    static private readonly ListingGenerator _listingGenerator = new ();

    static void Main(string[] args)
    {
        var parser = new CommandLine.Parser();

        CommandLine.Parser.Default.ParseArguments<ArgumentsOptions>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);

        Console.WriteLine("\nSuccessfull");
    }

    static void RunOptions(ArgumentsOptions opts)
    {
        string sourceCode = ReadSourceCodeFromFile(opts.InputFilePath);

        var assemblerLines = Assemble(sourceCode);
        var listing = _listingGenerator.Generate(assemblerLines);

        var listingWriterFactory = new AssemblerListingWriterFactory(listing, assemblerLines);
        var listingWritersList = listingWriterFactory.Get(opts);

        var inputFileWihoutExt = GetDirectoryPathWithoutExtension(opts.InputFilePath);
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
    }
    static void HandleParseError(IEnumerable<Error> errs)
    {
        //handle errors
    }

    static string GetDirectoryPathWithoutExtension(string path) => 
        $"{Path.GetDirectoryName(path)}" +
        $"{Path.DirectorySeparatorChar}" +
        $"{Path.GetFileNameWithoutExtension(path)}";

    static private IEnumerable<AssemblyLine> Assemble(string sourceCode)
    {
        try
        {
            return _assembler.AssembleAll(sourceCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR] [ASM] [{ex.GetType()}] {ex.Message}");
            throw;
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
            Console.WriteLine($"[ERR] [IO] {err.Message}");
            throw;
        }
    }
}



