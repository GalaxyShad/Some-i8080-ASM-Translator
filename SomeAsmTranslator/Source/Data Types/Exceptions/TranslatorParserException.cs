namespace SomeAsmTranslator.Source;

public class TranslatorParserException : TranslatorExceptionBase
{
    public TranslatorParserException(long errorLine) : base(errorLine) { }
    public TranslatorParserException(string message, long errorLine) : base(message, errorLine) { }
    public TranslatorParserException(string message, Exception inner, long errorLine) : base(message, inner, errorLine) { }
}


