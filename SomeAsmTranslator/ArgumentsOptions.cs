using CommandLine;

namespace I8080Translator;

public class ArgumentsOptions
{
    [Value(0, Required = true, MetaName = "Source code file", HelpText = "Input file-name including path")]
    public string InputFilePath { get; set; } = string.Empty;

    [Option('c', "csv", Required = false, HelpText = "Create listing file in .csv table format")]
    public bool IsGenerateCsv { get; set; }

    [Option('w', "word", Required = false, HelpText = "Create listing file in .docx word table format")]
    public bool IsGenerateWord { get; set; }

    [Option('s', "samelinebyte", Required = false, HelpText = "Keep all instruction bytes on the same line")]
    public bool IsKeepAllInstructionBytesOnSameLine { get; set; }

    [Option('m', "md", Required = false, HelpText = "Create listing file in .md Markdown table format")]
    public bool IsGenerateMarkdown { get; set; }

    [Option('b', "bin", Required = false, HelpText = "Generate binary file")]
    public bool IsGenerateBinary { get; set; }
}





