using System;

namespace SomeAsmTranslator.Source;

public class TranslatorExceptionBase : Exception 
{
    public long ErrorLine { get; private set; }

    public TranslatorExceptionBase(long errorLine)
    {
        ErrorLine = errorLine;
    }

    public TranslatorExceptionBase(string message, long errorLine): base(message) 
    {
        ErrorLine = errorLine;
    }

    public TranslatorExceptionBase(string message, Exception inner, long errorLine): base(message, inner) 
    {
        ErrorLine = errorLine;
    }
}

public class TranslatorLexerException : TranslatorExceptionBase
{
    public TranslatorLexerException(long errorLine): base(errorLine) { }

    public TranslatorLexerException(string message, long errorLine) : base(message, errorLine) { }

    public TranslatorLexerException(string message, Exception inner, long errorLine) : base(message, inner, errorLine) { }
}

public class TranslatorParserException : TranslatorExceptionBase
{
    public TranslatorParserException(long errorLine) : base(errorLine) { }
    public TranslatorParserException(string message, long errorLine) : base(message, errorLine) { }
    public TranslatorParserException(string message, Exception inner, long errorLine) : base(message, inner, errorLine) { }
}

public class TranslatorParserDataException : TranslatorParserException
{
    public TranslatorParserDataException(long errorLine) : base(errorLine) { }
    public TranslatorParserDataException(string message, long errorLine) : base(message, errorLine) { }
    public TranslatorParserDataException(string message, Exception inner, long errorLine) : base(message, inner, errorLine) { }
}


