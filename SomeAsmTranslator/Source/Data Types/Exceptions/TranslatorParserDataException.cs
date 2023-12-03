namespace SomeAsmTranslator.Source;

public class TranslatorParserDataException : TranslatorParserException
{
    public TranslatorParserDataException(long errorLine) : base(errorLine) { }
    public TranslatorParserDataException(string message, long errorLine) : base(message, errorLine) { }
    public TranslatorParserDataException(string message, Exception inner, long errorLine) : base(message, inner, errorLine) { }
}


