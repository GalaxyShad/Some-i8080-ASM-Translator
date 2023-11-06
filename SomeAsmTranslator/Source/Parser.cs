using I8080Translator;
using SomeAsmTranslator.Operands;
using System.Reflection;

namespace SomeAsmTranslator.Source;

class Parser
{
    private readonly Dictionary<string, Label> _labelList = new();

    private readonly Dictionary<string, Label> _setList = new()
    {
        { "B", new Label("B", 0) },
        { "C", new Label("C", 1) },
        { "D", new Label("D", 2) },
        { "E", new Label("E", 3) },
        { "H", new Label("H", 4) },
        { "L", new Label("L", 5) },
        { "M", new Label("M", 6) },
        { "A", new Label("A", 7) },
    };

    private readonly Queue<Token> _tokenQueue = new();

    public Parser(string source)
    {
        PreScan(source);
    }

    private Token TokenAt() => 
        _tokenQueue.Count != 0 ? _tokenQueue.Peek() : Token.EOF;

    private Token TokenEat() => 
        _tokenQueue.Count != 0 ? _tokenQueue.Dequeue() : Token.EOF;

    private void PreScan(string source)
    {
        var tokenizer = new Lexer(source);

        var token = tokenizer.Next();
        while (token.TokenType != TokenType.EOF)
        {
            if (token.TokenType == TokenType.NewLine)
            {
                token = tokenizer.Next();
                continue;
            }

            if (token.TokenType == TokenType.Label)
            {
                if (_labelList.ContainsKey(token.Value))
                    throw new InvalidDataException($"Double Label {token.Value}");

                _labelList.Add(token.Value, new Label(token.Value));
            }

            _tokenQueue.Enqueue(token);
            token = tokenizer.Next();
        }
    }

    public AssemblyStatement Next()
    {
        return new AssemblyStatement(
            ParseLabel(),
            ParseInstruction(),
            ParseOperands(),
            ParseComment()
        );
    }

    private OperandList ParseOperands()
    {
        var operandList = new OperandList();

        var left = ParseOperand();
        if (left != null)
            operandList.Add(left);

        while (TokenAt().TokenType is TokenType.Comma)
        {
            TokenEat();
            left = ParseOperand();

            if (left != null)
                operandList.Add(left);
        }

        return operandList;
    }

    private IOperand? ParseOperand()
    {
        IOperand operand;

        switch (TokenAt().TokenType)
        {
            case TokenType.ProgramCounterData:
                operand = new OperandProgramCounter();
                break;
            case TokenType.Number:
                operand = new OperandNumeric(TokenAt().Value);
                break;
            case TokenType.String:
                throw new NotImplementedException("Strings are not implemented yet");
            case TokenType.Symbol:
                if (_labelList.ContainsKey(TokenAt().Value))
                    operand = new OperandLabel(_labelList[TokenAt().Value]);

                else if (_setList.ContainsKey(TokenAt().Value))
                    operand = new OperandLabelAssignedValue(_setList[TokenAt().Value]);

                else if (TokenAt().Value is "SP" or "PSW")
                    operand = new OperandLabelAssignedValue(new Label(TokenAt().Value));

                else
                    throw new InvalidDataException($"Unexisting label {TokenAt().Value}");

                break;
            default:
                return null;
        }

        TokenEat();

        return operand;
    }

    private Label? ParseLabel() =>
        TokenAt().TokenType is TokenType.Label ? _labelList[TokenEat().Value] : null;

    private string? ParseInstruction() =>
        TokenAt().TokenType is TokenType.Instruction ? TokenEat().Value : null;

    private string? ParseComment() =>
        TokenAt().TokenType is TokenType.Comment ? TokenEat().Value : null;

}
