namespace SomeAsmTranslator.Source;

public class TranslatorLexerException : TranslatorExceptionBase
{
    public TranslatorLexerException(long errorLine) : base(errorLine) { }

    public TranslatorLexerException(string message, long errorLine) : base(message, errorLine) { }

    public TranslatorLexerException(string message, Exception inner, long errorLine) : base(message, inner, errorLine) { }
}


