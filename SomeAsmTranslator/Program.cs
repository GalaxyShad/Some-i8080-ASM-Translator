using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata.Ecma335;

namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        StreamReader streamReader = new StreamReader(Console.ReadLine());

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
            Console.WriteLine($"{st.Key.ToString("X4")} {st.Value}");
        }

        
    }
}

class Assembler
{

    public int ProgramCounter { get; private set; } = 0x0800;

    private List<AssemblyStatement> _statements = new ();

    private Dictionary<string, Label> _labelList = new ();

    private Dictionary<string, Label> _setList = new ()
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
        var tokenizer = new Tokenizer(source);

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

    private IAssemblyInstruction? ParseInstruction() =>
        (TokenAt().TokenType is TokenType.OPCODE) ? _instructionTable[TokenEat().Value] : null;

    private string? ParseComment() =>
        (TokenAt().TokenType is TokenType.COMMENT) ? TokenEat().Value : null;

    public void AssemleStatement(AssemblyStatement statement)
    {
        if (statement.Label != null)
        {
            _labelList[statement.Label.Name].Value = (ushort)ProgramCounter;
            //statement.Label.Value = (ushort)ProgramCounter;
        }
        //statement.Label.Value = (ushort)ProgramCounter;

        if (statement.Instruction == null)
            return;

        statement.ExecuteInstruction();
        ProgramCounter += statement.Instruction.ByteCount;
    }


    private readonly Dictionary<string, IAssemblyInstruction> _instructionTable = new()
    {
        { "CMC", new NoArgsAsmInstruction("CMC", InstructionTranslator.CMC) },
        { "STC", new NoArgsAsmInstruction("STC", InstructionTranslator.STC) },

        { "INR", new RegisterArgAsmInstruction("INR", InstructionTranslator.INR) },
        { "DCR", new RegisterArgAsmInstruction("DCR", InstructionTranslator.DCR) },
        { "CMA", new NoArgsAsmInstruction("CMA", InstructionTranslator.CMA) },
        { "DAA", new NoArgsAsmInstruction("DAA", InstructionTranslator.DAA) },

        { "NOP", new NoArgsAsmInstruction("NOP", InstructionTranslator.NOP) },

        { "MOV",  new TwoRegistersArgAsmInstruction("MOV", InstructionTranslator.MOV) },
        { "STAX", new RegisterPairArgAsmInstruction("STAX", InstructionTranslator.STAX) },
        { "LDAX", new RegisterPairArgAsmInstruction("LDAX", InstructionTranslator.LDAX) },
        
        { "ADD",  new RegisterArgAsmInstruction("ADD", InstructionTranslator.ADD) },
        { "ADC",  new RegisterArgAsmInstruction("ADC", InstructionTranslator.ADC) },
        { "SUB",  new RegisterArgAsmInstruction("SUB", InstructionTranslator.SUB) },
        { "SBB",  new RegisterArgAsmInstruction("SBB", InstructionTranslator.SBB) },
        { "ANA",  new RegisterArgAsmInstruction("ANA", InstructionTranslator.ANA) },
        { "XRA",  new RegisterArgAsmInstruction("XRA", InstructionTranslator.XRA) },
        { "ORA",  new RegisterArgAsmInstruction("ORA", InstructionTranslator.ORA) },
        { "CMP",  new RegisterArgAsmInstruction("CMP", InstructionTranslator.CMP) },
        
        { "RLC", new NoArgsAsmInstruction("RLC", InstructionTranslator.RLC) },
        { "RRC", new NoArgsAsmInstruction("RRC", InstructionTranslator.RRC) },
        { "RAL", new NoArgsAsmInstruction("RAL", InstructionTranslator.RAL) },
        { "RAR", new NoArgsAsmInstruction("RAR", InstructionTranslator.RAR) },

        { "PUSH", new RegisterPairArgAsmInstruction("PUSH", InstructionTranslator.PUSH) },
        { "POP",  new RegisterPairArgAsmInstruction("POP", InstructionTranslator.POP) },
        { "DAD",  new RegisterPairArgAsmInstruction("DAD", InstructionTranslator.DAD) },
        { "INX",  new RegisterPairArgAsmInstruction("INX", InstructionTranslator.INX) },
        { "DCX",  new RegisterPairArgAsmInstruction("DCX", InstructionTranslator.DCX) },

        { "XCHG", new NoArgsAsmInstruction("XCHG", InstructionTranslator.XCHG) },
        { "XTHL", new NoArgsAsmInstruction("XTHL", InstructionTranslator.XTHL) },
        { "SPHL", new NoArgsAsmInstruction("SPHL", InstructionTranslator.SPHL) },

        { "LXI", new RegisterPairAndWordArgAsmInstruction("LXI", InstructionTranslator.LXI) },
        { "MVI", new RegisterAndByteArgAsmInstruction("MVI", InstructionTranslator.MVI) },
        { "ADI", new ByteArgAsmInstruction("ADI", InstructionTranslator.ADI) },
        { "ACI", new ByteArgAsmInstruction("ACI", InstructionTranslator.ACI) },
        { "SUI", new ByteArgAsmInstruction("SUI", InstructionTranslator.SUI) },
        { "SBI", new ByteArgAsmInstruction("SBI", InstructionTranslator.SBI) },
        { "ANI", new ByteArgAsmInstruction("ANI", InstructionTranslator.ANI) },
        { "XRI", new ByteArgAsmInstruction("XRI", InstructionTranslator.XRI) },
        { "ORI", new ByteArgAsmInstruction("ORI", InstructionTranslator.ORI) },
        { "CPI", new ByteArgAsmInstruction("CPI", InstructionTranslator.CPI) },

        { "SHLD", new WordArgAsmInstruction("SHLD", InstructionTranslator.SHLD) },
        { "LHLD", new WordArgAsmInstruction("LHLD", InstructionTranslator.LHLD) },
        { "STA", new WordArgAsmInstruction("STA", InstructionTranslator.STA) },
        { "LDA", new WordArgAsmInstruction("LDA", InstructionTranslator.LDA) },

        { "PCHL", new NoArgsAsmInstruction("PCHL", InstructionTranslator.PCHL) },
        { "JMP", new WordArgAsmInstruction("JMP", InstructionTranslator.JMP) },
        { "JNZ", new WordArgAsmInstruction("JNZ", InstructionTranslator.JNZ) },
        { "JZ", new WordArgAsmInstruction("JZ", InstructionTranslator.JZ) },
        { "JNC", new WordArgAsmInstruction("JNC", InstructionTranslator.JNC) },
        { "JC", new WordArgAsmInstruction("JC", InstructionTranslator.JC) },
        { "JPO", new WordArgAsmInstruction("JPO", InstructionTranslator.JPO) },
        { "JPE", new WordArgAsmInstruction("JPE", InstructionTranslator.JPE) },
        { "JP", new WordArgAsmInstruction("JP", InstructionTranslator.JP) },
        { "JM", new WordArgAsmInstruction("JM", InstructionTranslator.JM) },

        { "CALL", new WordArgAsmInstruction("CALL", InstructionTranslator.CALL) },
        { "CNZ", new WordArgAsmInstruction("CNZ", InstructionTranslator.CNZ) },
        { "CZ", new WordArgAsmInstruction("CZ", InstructionTranslator.CZ) },
        { "CNC", new WordArgAsmInstruction("CNC", InstructionTranslator.CNC) },
        { "CC", new WordArgAsmInstruction("CC", InstructionTranslator.CC) },
        { "CPO", new WordArgAsmInstruction("CPO", InstructionTranslator.CPO) },
        { "CPE", new WordArgAsmInstruction("CPE", InstructionTranslator.CPE) },
        { "CP", new WordArgAsmInstruction("CP", InstructionTranslator.CP) },
        { "CM", new WordArgAsmInstruction("CM", InstructionTranslator.CM) },

        { "RET", new NoArgsAsmInstruction("RET", InstructionTranslator.RET) },
        { "RNZ", new NoArgsAsmInstruction("RNZ", InstructionTranslator.RNZ) },
        { "RZ", new NoArgsAsmInstruction("RZ", InstructionTranslator.RZ) },
        { "RNC", new NoArgsAsmInstruction("RNC", InstructionTranslator.RNC) },
        { "RC", new NoArgsAsmInstruction("RC", InstructionTranslator.RC) },
        { "RPO", new NoArgsAsmInstruction("RPO", InstructionTranslator.RPO) },
        { "RPE", new NoArgsAsmInstruction("RPE", InstructionTranslator.RPE) },
        { "RP", new NoArgsAsmInstruction("RP", InstructionTranslator.RP) },
        { "RM", new NoArgsAsmInstruction("RM", InstructionTranslator.RM) },

        { "RST", new ByteArgAsmInstruction("RST", InstructionTranslator.RST) },

        { "EI", new NoArgsAsmInstruction("EI", InstructionTranslator.EI) },
        { "DI", new NoArgsAsmInstruction("DI", InstructionTranslator.DI) },

        { "IN", new ByteArgAsmInstruction("IN", InstructionTranslator.IN) },
        { "OUT", new ByteArgAsmInstruction("OUT", InstructionTranslator.OUT) },

        { "HLT", new NoArgsAsmInstruction("HLT", InstructionTranslator.HLT) },
    };

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



interface IAssemblyInstruction
{
    string OpCode { get; }

    uint GetCode(IOperandMultiple args);

    int ByteCount { get; }
}

class NoArgsAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 0;

    public int ByteCount => 1;

    private Func<uint> _translateFunc;

    public NoArgsAsmInstruction(string opCode, Func<uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                $"{nameof(OpCode)} takes no arguments, but {args.Count} were given");

        return _translateFunc();
    }
}

class RegisterArgAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 1;

    public int ByteCount => 1;

    private Func<Register, uint> _translateFunc;

    public RegisterArgAsmInstruction(string opCode, Func<Register, uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                $"{nameof(OpCode)} takes {ArgCount} arguments, but {args.Count} were given");

        return _translateFunc(args.First.ToRegister());
    }
}

class TwoRegistersArgAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 2;

    public int ByteCount => 1;

    private Func<Register, Register, uint> _translateFunc;

    public TwoRegistersArgAsmInstruction(string opCode, Func<Register, Register, uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                 $"{nameof(OpCode)} takes {ArgCount} arguments, but {args.Count} were given");

        return _translateFunc(args.First.ToRegister(), args.Second.ToRegister());
    }
}

class RegisterPairArgAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 1;

    public int ByteCount => 1;

    private Func<RegisterPair, uint> _translateFunc;

    public RegisterPairArgAsmInstruction(string opCode, Func<RegisterPair, uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                 $"{nameof(OpCode)} takes {ArgCount} arguments, but {args.Count} were given");

        return _translateFunc(args.First.ToRegisterPair());
    }
}

class RegisterPairAndWordArgAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 2;

    public int ByteCount => 3;

    private Func<RegisterPair, ushort, uint> _translateFunc;

    public RegisterPairAndWordArgAsmInstruction(string opCode, Func<RegisterPair, ushort, uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                 $"{nameof(OpCode)} takes {ArgCount} arguments, but {args.Count} were given");

        return _translateFunc(args.First.ToRegisterPair(), args.Second.To16bitAdress());
    }
}

class RegisterAndByteArgAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 2;

    public int ByteCount => 2;

    private Func<Register, byte, uint> _translateFunc;

    public RegisterAndByteArgAsmInstruction(string opCode, Func<Register, byte, uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                 $"{nameof(OpCode)} takes {ArgCount} arguments, but {args.Count} were given");

        return _translateFunc(args.First.ToRegister(), args.Second.ToImmediateData());
    }
}

class ByteArgAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 1;

    public int ByteCount => 2;

    private Func<byte, uint> _translateFunc;

    public ByteArgAsmInstruction(string opCode, Func<byte, uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                 $"{nameof(OpCode)} takes {ArgCount} arguments, but {args.Count} were given");

        return _translateFunc(args.First.ToImmediateData());
    }
}

class WordArgAsmInstruction : IAssemblyInstruction
{
    public string OpCode { get; private set; }

    public int ArgCount => 1;

    public int ByteCount => 3;

    private Func<ushort, uint> _translateFunc;

    public WordArgAsmInstruction(string opCode, Func<ushort, uint> translateFunc)
    {
        OpCode = opCode;
        _translateFunc = translateFunc;
    }

    public uint GetCode(IOperandMultiple args)
    {
        if (args.Count != ArgCount)
            throw new ArgumentException(
                 $"{nameof(OpCode)} takes {ArgCount} arguments, but {args.Count} were given");

        return _translateFunc(args.First.To16bitAdress());
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
    public int Count => _operands.Count();

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
