using I8080Translator;

namespace SomeAsmTranslator.Source;

class Lexer
{
    private readonly string[] _instructionTable =
        InstructionTranslator.GetInstructionNames()
        .Concat(Assembler.GetPseudoInstructrions())
        .ToArray();

    private readonly string[] _expressionOperators = new[]
    {
        "+", "-", "*", "/", "MOD", "NOT", "AND", "OR", "XOR", "SHR", "SHL", "(", ")"
    };

    private readonly TextReader _sourceCodeReader;

    private int _currentChar;
    private long _lineCounter = 1;

    public Lexer(string sourceCode) : this(new StringReader(sourceCode)) { }

    public Lexer(TextReader sourceCodeReader)
    {
        _sourceCodeReader = sourceCodeReader;
        _currentChar = NextChar();
    }

    public Token Next()
    {
        PassWhiteSpaces();

        return CurrentChar switch
        {
            '\0' => Token.EOF,
            '\n' => ProcNewLine(),
            '\'' => ProcString(),
            ';' => ProcComment(),
            ',' => ProcComma(),
            '$' => ProcProgramCounterData(),

            var c when isCharExpressionOperand(c) => ProcMath(),

            var c when IsCharLetterOrSpecialSym(c) => ProcLabelAndSymbols(),
            var c when char.IsNumber(c) => ProcNumbers(),

            _ => throw new TranslatorLexerException(
                    $"Unknown token {(int)CurrentChar} {CurrentChar}",
                    _lineCounter
                 ),
        };
    }

    private char CurrentChar => _currentChar != -1 ? char.ToUpper((char)_currentChar) : '\0';

    private int NextChar() => _currentChar = _sourceCodeReader.Read();

    private void PassWhiteSpaces()
    {
        while (char.IsWhiteSpace(CurrentChar) && CurrentChar is not '\n')
            NextChar();
    }

    private Token ProcMath()
    {
        var current = CurrentChar;

        NextChar();

        return new Token { Line = _lineCounter, TokenType = TokenType.ExpressionOperator, Value = current.ToString() };
    }

    private Token ProcLabelAndSymbols()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        while (IsCharLetterOrSpecialSym(CurrentChar) || char.IsDigit(CurrentChar))
        {
            value += CurrentChar;
            NextChar();
        }

        if (_instructionTable.Contains(value))
        {
            if (CurrentChar == ':')
                throw new TranslatorLexerException(
                    $"Label cannot be named by existing instruction name \"{value}\"",
                    _lineCounter
                );

            return new Token { TokenType = TokenType.Instruction, Value = value, Line = _lineCounter };
        }

        if (_expressionOperators.Contains(value))
        {
            return new Token { TokenType = TokenType.ExpressionOperator, Value = value, Line = _lineCounter };
        }

        if (CurrentChar == ':')
        {
            NextChar();
            return new Token { TokenType = TokenType.LabelAddress, Value = value, Line = _lineCounter };
        }

        return new Token { TokenType = TokenType.Symbol, Value = value, Line = _lineCounter };
    }

    private Token ProcNumbers()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        while (!char.IsWhiteSpace(CurrentChar) && CurrentChar != '\0' && CurrentChar != ',' &&
            !isCharExpressionOperand(CurrentChar))
        {
            value += CurrentChar;
            NextChar();
        }

        return new Token { TokenType = TokenType.Number, Value = value, Line = _lineCounter };
    }

    private Token ProcComment()
    {
        string value = string.Empty;

        NextChar();

        while (CurrentChar != '\n' && CurrentChar != '\r' && CurrentChar != '\0')
        {
            value += CurrentChar;
            NextChar();
        }

        return new Token { TokenType = TokenType.Comment, Value = value.Trim(), Line = _lineCounter };
    }

    private Token ProcString()
    {
        string value = string.Empty;

        NextChar();

        while (CurrentChar != '\'')
        {
            value += CurrentChar;
            NextChar();

            if (CurrentChar == '\0')
                throw new TranslatorLexerException("No end quote found", _lineCounter);
        }

        NextChar();

        return new Token { TokenType = TokenType.String, Value = value.Trim(), Line = _lineCounter };
    }

    private Token ProcProgramCounterData()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        return new Token { TokenType = TokenType.ProgramCounterData, Value = value, Line = _lineCounter };
    }

    private Token ProcComma()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        return new Token { TokenType = TokenType.Comma, Value = value, Line = _lineCounter };
    }

    private Token ProcNewLine()
    {
        _lineCounter++;

        NextChar();

        return Token.NewLine;
    }

    private static bool isCharExpressionOperand(char c) =>
        c is '+' or '-' or '*' or '/' or '(' or ')';

    private static bool IsCharLetterOrSpecialSym(char c) =>
        c >= 'A' && c <= 'Z' || c == '?' || c == '@' || c == '_';
}
