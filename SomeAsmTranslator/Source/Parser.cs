using I8080Translator;
using SomeAsmTranslator.Operands;

namespace SomeAsmTranslator.Source;

class Parser
{
    private readonly LabelTable _labelTable;
    private readonly Lexer _lexer;

    private Token _currentToken = Token.EOF;
    private Token _previousToken = Token.EOF;

    public Parser(Lexer lexer, LabelTable labelTable)
    {
        _labelTable = labelTable;
        _lexer = lexer;
        _currentToken = TokenNext();
    }

    public AssemblyStatement Next() =>
        new(ParseLabel(), ParseInstruction(), ParseOperands(), ParseComment());

    private Token TokenNext()
    {
        _currentToken = _lexer.Next();

        while (_currentToken.TokenType is TokenType.NewLine)
            _currentToken = _lexer.Next();

        return _currentToken;
    }

    private Token TokenAt() => _currentToken;

    private Token TokenEat()
    {
        _previousToken = new Token
        {
            Line = _currentToken.Line,
            TokenType = _currentToken.TokenType,
            Value = _currentToken.Value
        };

        TokenNext();

        return _previousToken;
    }

    private OperandList ParseOperands()
    {
        var operandList = new OperandList();

        if (_previousToken.TokenType is not TokenType.Instruction)
            return operandList;

        var left = ParseExpression();
        if (left != null)
            operandList.Add(left);

        while (TokenAt().TokenType is TokenType.Comma)
        {
            TokenEat();
            left = ParseExpression();

            if (left != null)
                operandList.Add(left);
        }

        return operandList;
    }

    private IOperand? ParseExpression()
    {
        var expression = new OperandExpression(_labelTable);

        if (TokenAt().TokenType is TokenType.Comma)
        {
            throw new TranslatorParserException(
                $"Expected symbol, number, pg counter or expression operator, but ',' was found",
                TokenAt().Line);
        }

        while (TokenAt().TokenType
            is TokenType.Symbol
            or TokenType.Number
            or TokenType.ProgramCounterData
            or TokenType.ExpressionOperator)
        {
            var token = TokenEat();

            expression.Add(token);

            if (token.TokenType is TokenType.Symbol or TokenType.Number &&
                TokenAt().TokenType is TokenType.Symbol or TokenType.Number)
            {
                break;
            }
        }

        if (TokenAt().TokenType is TokenType.Number)
        {
            throw new TranslatorParserException(
                $"Expected instruction, label or column, but number '{TokenAt().Value}' was found",
                TokenAt().Line);
        }

        if (expression.Count == 0)
            return null;

        return (expression.Count != 1)
            ? expression
            : expression.ConvertToSingleOperand();
    }

    private Label? ParseLabel()
    {
        return TokenAt().TokenType switch
        {
            TokenType.LabelAddress or TokenType.Symbol =>
                _labelTable.AddOrUpdateLabel(new Label
                {
                    Token = TokenAt(),
                    Name = TokenEat().Value
                }),

            TokenType.Instruction or TokenType.Comment or TokenType.EOF => null,

            _ => throw new TranslatorParserException(
                    $"Expected label, instruction or comment, but ${TokenAt().TokenType} was found",
                    TokenAt().Line
                 )
        };
    }

    private Instruction? ParseInstruction()
    {
        return TokenAt().TokenType switch
        {
            TokenType.Instruction => new Instruction { Line = TokenAt().Line, Name = TokenEat().Value },

            TokenType.Comment or TokenType.LabelAddress or TokenType.Symbol or TokenType.EOF => null,

            _ => throw new TranslatorParserException(
                    $"Expected label, instruction or comment, but ${TokenAt().TokenType} was found",
                    TokenAt().Line
                 )
        };
    }


    private string? ParseComment()
    {
        return TokenAt().TokenType switch
        {
            TokenType.Comment => TokenEat().Value,

            TokenType.Instruction or TokenType.LabelAddress or TokenType.Symbol or TokenType.EOF => null,

            _ => throw new TranslatorParserException(
                    $"Expected label, instruction or comment, but ${TokenAt().TokenType} was found",
                    TokenAt().Line
                 )
        };
    }
}
