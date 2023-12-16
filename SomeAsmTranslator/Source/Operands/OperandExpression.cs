using I8080Translator;
using SomeAsmTranslator.Source;
using System.Data;

namespace SomeAsmTranslator.Operands;

public class OperandExpression : IOperand
{
    // === Grammar ===
    // F ::= F "AND" E | F "OR" E | F "XOR" E | E
    // E ::= E + T | E - T | T        
    // T ::= T * M | T / M | T "MOD" M | T "SHL" M | T "SHR" M | M
    // M ::= (F) | -M | +M | "NOT" M | C   

    private readonly List<Token> _tokenList = new();
    private readonly Queue<Token> _tokenQueue = new();
    private readonly LabelTable _labelTable;

    private bool _haveLabel = false;
    public int Count => _tokenList.Count;

    public OperandExpression(LabelTable labelTable)
    {
        _labelTable = labelTable;
    }

    public void Add(Token token)
    {
        if (token.TokenType == TokenType.Symbol)
            _haveLabel = true;

        _tokenList.Add(token);
    }

    public bool HaveLabel => _haveLabel;

    public IOperand ConvertToSingleOperand() => ParseOperand(_tokenList[0]);

    // F ::= F "AND" E | F "OR" E | F "XOR" E | E
    private short ParseF()
    {
        var result = ParseE();

        while (_tokenQueue.Peek().Value is "AND" or "OR" or "XOR")
        {
            var token = _tokenQueue.Dequeue();

            result = (short)(token.Value switch
            {
                "AND" => result & ParseE(),
                "OR"  => result | ParseE(),
                "XOR" => result ^ ParseE(),
                _ => throw new NotImplementedException(),
            });
        }

        return result;
    }

    // E ::= E + T | E - T | T        
    private short ParseE()
    {
        var result = ParseT();

        while (_tokenQueue.Peek().Value is "+" or "-")
        {
            var token = _tokenQueue.Dequeue();

            result = (short)(token.Value switch
            {
                "+" => result + ParseT(),
                "-" => result - ParseT(),
                _ => throw new NotImplementedException(),
            });
        }

        return result;
    }

    // T::= T * M | T / M | T "MOD" M | T "SHL" M | T "SHR" M | M
    private short ParseT()
    {
        var result = ParseM();

        while (_tokenQueue.Peek().Value is "*" or "/" or "MOD" or "SHL" or "SHR")
        {
            var token = _tokenQueue.Dequeue();

            result = (short)(token.Value switch
            {
                "*"   => result  * ParseM(),
                "/"   => result  / ParseM(),
                "MOD" => result  % ParseM(),
                "SHL" => result << ParseM(),
                "SHR" => result >> ParseM(),
                _ => throw new NotImplementedException(),
            });
        }

        return result;
    }

    // M ::= (F) | -M | +M | "NOT" M | C
    private short ParseM()
    {
        short result;

        switch (_tokenQueue.Peek().Value)
        {
            case "(":
                _tokenQueue.Dequeue();

                result = ParseF();

                if (_tokenQueue.Peek().Value is not ")")
                    throw new InvalidExpressionException(") is missing.");

                _tokenQueue.Dequeue();

                break;
            case "-":
                _tokenQueue.Dequeue();
                result = (short)-ParseM();
                break;
            case "+":
                _tokenQueue.Dequeue();
                result = (short)+ParseM();
                break;
            case "NOT":
                _tokenQueue.Dequeue();
                result = (short)~ParseM();
                break;
            default:
                result = (short)ParseC().ToRawData();
                break;
        }

        return result;
    }

    private IOperand ParseC() => ParseOperand(_tokenQueue.Dequeue());

    private short Parse()
    {
        _tokenQueue.Clear();
        _tokenList.ForEach(o => _tokenQueue.Enqueue(o));
        _tokenQueue.Enqueue(Token.EOF);

        var result = ParseF();

        if (_tokenQueue.Peek().TokenType != TokenType.EOF)
            throw new InvalidExpressionException($"Invalid expression {ToString()}");

        return result;
    }

    private IOperand ParseOperand(Token token) => 
        token.TokenType switch
        {
            TokenType.Number =>             new OperandNumeric(token.Value),
            TokenType.ProgramCounterData => new OperandProgramCounter(),
            TokenType.String =>             throw new NotImplementedException($"Strings are not implemented yet -> {token.Value}"),
            TokenType.Symbol =>
                (token.Value is "PSW" or "SP") 
                    ? new OperandLabel(new Label { Name = token.Value })
                    : new OperandLabel(_labelTable.AddOrUpdateLabel(new Label { Name = token.Value, Token = token })),
            
            _ => throw new ArgumentException($"Unexpected token {token.TokenType} with value {token.Value}")
        };

    public ushort To16bitAdress()
    {
        return NumericDataParser.SwapBytes((ushort)Parse());
    }

    public byte ToImmediateData()
    {
        var result = (ushort)Parse();

        if (result > 0xFF)
            throw new Exception($"Cannot convert expression to immediate data. Evaluated value <{result}> is greater than 0xFF");

        return (byte)result;
    }

    public Register ToRegister()
    {
        var result = (ushort)Parse();

        if (result > 7)
            throw new InvalidCastException($"Cannot expression to register. Evaluated value <{result}> is greater than 7");

        return (Register)result;
    }

    public RegisterPair ToRegisterPair()
    {
        throw new Exception($"Expression cannot be used as Register Pair");
    }

    public override string ToString()
    {
        return string.Join(" ", _tokenList.Select(x => x.Value));
    }

    public ushort ToRawData() => (ushort)Parse();
}
