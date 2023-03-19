namespace MyProject;

class Lexer
{
    private readonly string[] _instructionTable = 
        InstructionTranslator.GetInstructionNames()
        .Concat(Preproccesor.GetPseudoInstructrions())
        .ToArray();

    private string _sourceCode;

    private int _pos = 0;

    private char CurrentChar { get => (_pos < _sourceCode.Length) ? _sourceCode[_pos] : '\0'; }

    private void NextChar() => _pos++;

    public Lexer(string sourceCode)
    {
        _sourceCode = sourceCode.ToUpper();
    }

    private void PassWhiteSpaces()
    {
        while (char.IsWhiteSpace(CurrentChar) && CurrentChar is not '\n') 
            NextChar();
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
                throw new Exception($"Label cannot be named by existing instruction name \"{value}\"");

            return new Token { TokenType = TokenType.Instruction, Value = value };
        }

        if (CurrentChar == ':')
        {
            NextChar();
            return new Token { TokenType = TokenType.Label, Value = value };
        }

        return new Token { TokenType = TokenType.Symbol, Value = value };
    }

    private Token ProcNumbers()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        while (!char.IsWhiteSpace(CurrentChar) && CurrentChar != '\0' && CurrentChar != ',')
        {
            value += CurrentChar;
            NextChar();
        }

        return new Token { TokenType = TokenType.Number, Value = value };
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

        return new Token { TokenType = TokenType.Comment, Value = value.Trim() };
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
                throw new Exception("No end quote found");
        }

        NextChar();

        return new Token { TokenType = TokenType.String, Value = value.Trim() };
    }

    private Token ProcProgramCounterData()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        return new Token { TokenType = TokenType.ProgramCounterData, Value = value };
    }

    private Token ProcComma()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        return new Token { TokenType = TokenType.Comma, Value = value };
    }

    public Token Next()
    {
        string value = string.Empty;

        PassWhiteSpaces();

        if (CurrentChar == '\0')
        {
            return new Token { TokenType = TokenType.EOF };
        }
        else if (CurrentChar == '\n')
        {
            NextChar();
            return new Token { TokenType = TokenType.NewLine };
        }
        else if (IsCharLetterOrSpecialSym(CurrentChar))
        {
            return ProcLabelAndSymbols();
        }
        else if (char.IsNumber(CurrentChar))
        {
            return ProcNumbers();
        }
        else if (CurrentChar == ';')
        {
            return ProcComment();
        }
        else if (CurrentChar == '\'')
        {
            return ProcString();
        }
        else if (CurrentChar == '$')
        {
            return ProcProgramCounterData();
        }
        else if (CurrentChar == ',')
        {
            return ProcComma();
        }

        NextChar();

        return new Token { TokenType = TokenType.Unknown, Value = value };
    }

    private bool IsCharLetterOrSpecialSym(char c) => (c >= 'A' && c <= 'Z') || c == '?' || c == '@' || c == '_';
}
