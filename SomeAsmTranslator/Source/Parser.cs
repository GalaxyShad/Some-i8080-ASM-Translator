using DocumentFormat.OpenXml.Vml;
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
        if (_previousToken.TokenType is not TokenType.Instruction and TokenType.EOF)
            throw new Exception($"Unknown instruction {TokenAt().Value}");

        var operandList = new OperandList();

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

        while (TokenAt().TokenType is TokenType.Symbol
                                   or TokenType.Number
                                   or TokenType.ProgramCounterData
                                   or TokenType.ExpressionOperator
        )
        {
            expression.Add(TokenEat());
        }

        if (expression.Count == 0)
            return null;

        return (expression.Count != 1) ? expression : expression.ConvertToSingleOperand();
    }

    private Label? ParseLabel() =>
        TokenAt().TokenType is TokenType.LabelAddress or TokenType.Symbol
            ? _labelTable.AddOrUpdateLabel(new Label { Token = TokenAt(), Name = TokenEat().Value })
            : null;

    private string? ParseInstruction() =>
        TokenAt().TokenType is TokenType.Instruction ? TokenEat().Value : null;

    private string? ParseComment() =>
        TokenAt().TokenType is TokenType.Comment ? TokenEat().Value : null;
}
