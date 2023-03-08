using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace MyProject;
class Program
{
    static void Main()
    {
        StreamReader streamReader = new(Console.ReadLine());

        var assembler = new Assembler(streamReader.ReadToEnd());
        streamReader.Close();

        var asseblyStatementList = new List<KeyValuePair<int, AssemblyStatement>>();

        AssemblyStatement statement = assembler.Next();
        while (!statement.IsEmpty())
        {
            /*Console.WriteLine($"{statement.ToString()} [{statement.Instruction.GetCode(statement.OperandList):X}]");*/
            int pg = assembler.ProgramCounter;
            assembler.AssemleStatement(statement);
            asseblyStatementList.Add(new KeyValuePair<int, AssemblyStatement>(pg, statement));
            statement = assembler.Next();
        }

        foreach (var st in asseblyStatementList)
        {
            assembler.AssemleStatement(st.Value);
            Console.WriteLine($"{st.Key:X4} {st.Value}");
        }

        
    }
}

class Assembler
{

    public int ProgramCounter { get; private set; } = 0x0800;

    /*private readonly List<AssemblyStatement> _statements = new ();*/

    private readonly Dictionary<string, Label> _labelList = new ();

    private readonly Dictionary<string, Label> _setList = new ()
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

    private Queue<Token> _tokenQueue = new ();

    private Token TokenAt() =>
        (_tokenQueue.Count != 0) ? _tokenQueue.Peek() : new Token { TokenType = TokenType.EOF, Value = "EOF" };

    private Token TokenEat() => 
        (_tokenQueue.Count != 0) ? _tokenQueue.Dequeue() : new Token { TokenType = TokenType.EOF, Value = "EOF" };

    public Assembler(string source)
    {
        var tokenizer = new Lexer(source);

        var token = tokenizer.Next();
        while (token.TokenType != TokenType.EOF)
        {
            if (token.TokenType == TokenType.LABEL)
            {
                if (_labelList.ContainsKey(token.Value))
                    throw new InvalidDataException($"Double Label {token.Value}");

                _labelList.Add(token.Value, new Label(token.Value, 0));
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

        while (TokenAt().TokenType is TokenType.COMMA)
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
            case TokenType.PG_DATA:
                operand = new OperandProgramCounter((ushort)ProgramCounter);
                break;
            case TokenType.NUMBER:
                operand = new OperandNumeric(TokenAt().Value);
                break;
            case TokenType.STRING:
                throw new NotImplementedException("Strings are not implemented yet");
            case TokenType.SYMBOL:
                if (_labelList.ContainsKey(TokenAt().Value))
                    operand = new OperandLabel(_labelList[TokenAt().Value]);

                else if (_setList.ContainsKey(TokenAt().Value))
                    operand = new OperandLabelAssignedValue(_setList[TokenAt().Value]);

                else if (TokenAt().Value is "SP" or "PSW")
                    operand = new OperandLabelAssignedValue(new Label(TokenAt().Value, 0));

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
        (TokenAt().TokenType is TokenType.LABEL) ? new Label(TokenEat().Value, (ushort)ProgramCounter) : null;

    private string? ParseInstruction() =>
        (TokenAt().TokenType is TokenType.OPCODE) ? TokenEat().Value : null;

    private string? ParseComment() =>
        (TokenAt().TokenType is TokenType.COMMENT) ? TokenEat().Value : null;

    public void AssemleStatement(AssemblyStatement statement)
    {
        if (statement.Label != null)
            _labelList[statement.Label.Name].Value = (ushort)ProgramCounter;

        if (statement.Instruction == null)
            return;

        var compiler = new InstructionTranslator();

        var instruction = compiler.GetType().GetMethod(statement.Instruction);
        if (instruction == null)
            throw new InvalidDataException($"Unknown instruction {statement.Instruction}");

        var instructionParamInfo = instruction.GetParameters();
        if (statement.OperandList.Count != instructionParamInfo.Length)
            throw new ArgumentException(
                $"{instruction.Name} takes {instructionParamInfo.Length} arguments, " +
                $"but {statement.OperandList.Count} were given");

        var instructionArgs = new List<object>();
        foreach (var (First, Second) in instructionParamInfo.Zip(statement.OperandList.Operands))
        {
            if (First.ParameterType == typeof(byte))
                instructionArgs.Add(Second.ToImmediateData());

            else if (First.ParameterType == typeof(ushort))
                instructionArgs.Add(Second.To16bitAdress());

            else if (First.ParameterType == typeof(Register))
                instructionArgs.Add(Second.ToRegister());

            else if (First.ParameterType == typeof(RegisterPair))
                instructionArgs.Add(Second.ToRegisterPair());

            else throw new InvalidDataException("Unhandled type");
        }

        if (instruction.ReturnType == typeof(byte[]))
        {
            var bytes = (byte[])instruction.Invoke(compiler, instructionArgs.ToArray())!;
        }

        uint code = (uint)instruction.Invoke(compiler, instructionArgs.ToArray())!;

        ProgramCounter += (code <= 0xFF) ? 1 : (code <= 0xFFFF) ? 2 : 3;
    }

}



class Label
{
    public string Name { get; }
    public ushort Value { get; set; }

    public Label(string name, ushort value)
    {
        Name = name;
        Value = value;
    }
}


interface IOperand
{
    Register ToRegister();
    RegisterPair ToRegisterPair();
    byte ToImmediateData();
    ushort To16bitAdress();

    string ToString();
}

interface IOperandMultiple
{
    public int Count { get; }

    public IEnumerable<IOperand> Operands { get; }

    public IOperand First { get; }

    public IOperand Second { get; }
}

class OperandList : IOperandMultiple
{
    public int Count => _operands.Count;

    public IEnumerable<IOperand> Operands => _operands;

    private List<IOperand> _operands = new ();

    public IOperand First => _operands.First();

    public IOperand Second => _operands.ElementAt(1);

    public void Add(IOperand operand) => _operands.Add(operand);
}

class OperandNumeric : IOperand
{
    private int _value;

    private string _valueString;

    private void Parse(string value)
    {
        var dataParser = new IntelDataParser();

        char last = value.ToUpper().Last();

        if (last == 'H')
            _value = dataParser.ParseHexadecimal(value);
        else if (last == 'O' || last == 'Q')
            _value = dataParser.ParseOctal(value);
        else if (last == 'B')
            _value = dataParser.ParseBinary(value);
        else if (last == 'D' || char.IsDigit(last))
            _value = dataParser.ParseDecimal(value);
        else 
            throw new InvalidDataException($"{value} is invalid numeric value");
    }

    public OperandNumeric(string value)
    {
        _valueString = value;
        Parse(value);
    }

    public ushort To16bitAdress()
    {
        if (_value > 0xFFFF)
            throw new InvalidCastException("Cannot convert value to 16 bit adr. Value greater than 16 bit");

        return IntelDataParser.SwapBytes((ushort)_value);
    }

    public byte ToImmediateData()
    {
        if (_value > 0xFF)
            throw new InvalidCastException("Cannot convert value to 8 bit data. Value greater than 8 bit");

        return (byte)_value;
    }

    public Register ToRegister()
    {
        if (_value > 7 || _value < 0)
            throw new InvalidCastException("Cannot convert value to register. Unexisting register");

        return (Register)_value;
    }

    public RegisterPair ToRegisterPair()
    {
        throw new InvalidCastException("Numeric value cannot be specified as Register Pair");
    }

    public override string ToString()
    {
        return _valueString;
    }
}

class OperandProgramCounter: IOperand
{
    private ushort _pgCounter;

    public string Value { get; private set; }
    public OperandProgramCounter(ushort pgCounter)
    {
        _pgCounter = pgCounter;
    }

    public ushort To16bitAdress() => IntelDataParser.SwapBytes((ushort)_pgCounter);

    public byte ToImmediateData() =>
        throw new InvalidCastException("Program Counter cannot be specified as 8bit data");

    public Register ToRegister() =>
        throw new InvalidCastException("Program Counter cannot be specified as Register");

    public RegisterPair ToRegisterPair() =>
        throw new InvalidCastException("Program Counter cannot be specified as Register Pair");

    public override string ToString()
    {
        return _pgCounter.ToString();
    }
}

class OperandLabelAssignedValue : IOperand
{
    private Label _label;

    public OperandLabelAssignedValue(Label label)
    {
        _label = label;
    }

    public ushort To16bitAdress()
    {
        if (_label.Value > 0xFFFF)
            throw new InvalidCastException(
                $"Cannot convert label {_label.Name} to 16 bit adress. " +
                $"Value {_label.Value} is greater than 0xFFFF");

        return IntelDataParser.SwapBytes(_label.Value);
    }

    public byte ToImmediateData()
    {
        if (_label.Value > 0xFF)
            throw new InvalidCastException(
                $"Cannot convert label {_label.Name} to 8 bit data. " +
                $"Value {_label.Value} is greater than 0xFF");

        return (byte)_label.Value;
    }

    public Register ToRegister()
    {
        if (_label.Value > 7 || _label.Value < 0)
            throw new InvalidCastException(
                $"Cannot convert label {_label.Name} to register. " +
                $"Value {_label.Value} is out of range");

        return (Register)_label.Value;
    }

    public RegisterPair ToRegisterPair()
    {
        return _label.Name switch
        {
            "B" => RegisterPair.BC,
            "D" => RegisterPair.DE,
            "H" => RegisterPair.HL,
            "SP" => RegisterPair.SP,
            "PSW" => RegisterPair.PSW,
            _ => throw new InvalidCastException($"{_label.Name} is invalid Register Pair name"),
        };
    }

    public override string ToString()
    {
        return _label.Name;
    }
}
