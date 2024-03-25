namespace SomeAsmTranslator.Source;

public class TranslatorAssemblerException : TranslatorExceptionBase
{
    public TranslatorAssemblerException(long errorLine) : base(errorLine) { }
    public TranslatorAssemblerException(string message, long errorLine) : base(message, errorLine) { }
    public TranslatorAssemblerException(string message, Exception inner, long errorLine) : base(message, inner, errorLine) { }
}


