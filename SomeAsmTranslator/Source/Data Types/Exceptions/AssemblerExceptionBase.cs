namespace SomeAsmTranslator.Source;

public class TranslatorExceptionBase : Exception
{
    public long ErrorLine { get; private set; }

    public TranslatorExceptionBase(long errorLine)
    {
        ErrorLine = errorLine;
    }

    public TranslatorExceptionBase(string message, long errorLine) : base(message)
    {
        ErrorLine = errorLine;
    }

    public TranslatorExceptionBase(string message, Exception inner, long errorLine) : base(message, inner)
    {
        ErrorLine = errorLine;
    }
}
