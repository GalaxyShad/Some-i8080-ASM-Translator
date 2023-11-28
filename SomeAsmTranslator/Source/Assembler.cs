using I8080Translator;

namespace SomeAsmTranslator.Source;

class Assembler
{
    private readonly Lexer _lexer;
    private readonly Parser _parser;
    private readonly LabelTable _labelTable;
    private readonly InstructionTranslator _instructionTranslator;

    private readonly LinkedList<AssemblyLine> _assembledLines = new();
    private readonly LinkedList<AssembledAssemblyStatement> _assembledLinesWithLabels = new();

    private int _pgCounter = 0x0800;

    public Assembler(TextReader sourceCodeReader)
    {
        _labelTable = new LabelTable();
        _lexer = new Lexer(sourceCodeReader);
        _parser = new Parser(_lexer, _labelTable);
        _instructionTranslator = new InstructionTranslator();
    }

    public Assembler(string sourceCodeString) : this(new StringReader(sourceCodeString)) { }

    public IEnumerable<AssemblyLine> AssembleAll()
    {
        AssembleWithoutLabelsData();
        ReassembleInstructionsWithLabels();
        return _assembledLines;
    }

    private void ReassembleInstructionsWithLabels()
    {
        foreach (var line in _assembledLinesWithLabels)
        {
            line.MachineCode = CompileInstruction(line.AssemblyStatement).MachineCode;
        }
    }

    private void AssembleWithoutLabelsData()
    {
        AssemblyStatement statement = _parser.Next();

        while (!statement.IsEmpty())
        {
            if (statement.Label != null)
            {
                statement.Label.Type = LabelType.Address;
                statement.Label.Data = (ushort)_pgCounter;
            }

            if (statement.Instruction != null)
            {
                if (statement.Instruction == "ORG")
                    _assembledLines.AddLast(AssembleOrg(statement));
                else if (statement.Instruction == "EQU")
                    _assembledLines.AddLast(AssembleEqu(statement));
                else
                    _assembledLines.AddLast(AssembleInstruction(statement));
            }

            statement = _parser.Next();
        }
    }

    private static AssemblyLine MakeAssemblyLineForPseudoInsturction(AssemblyStatement statement)
    {
        return new AssemblyLine
        {
            Address = null,
            AssembledAssemblyStatement = new AssembledAssemblyStatement
            {
                MachineCode = null,
                AssemblyStatement = statement
            }
        };
    }

    private AssemblyLine AssembleOrg(AssemblyStatement statement)
    {
        _pgCounter = NumericDataParser.SwapBytes(statement.OperandList.First.To16bitAdress());

        return MakeAssemblyLineForPseudoInsturction(statement);
    }

    private AssemblyLine AssembleEqu(AssemblyStatement statement)
    {
        if (statement.Label == null)
            throw new InvalidDataException("Name of symbol is missing for EQU");

        statement.Label.Type = LabelType.Equ;
        statement.Label.Data = statement.OperandList.First.ToImmediateData();

        return MakeAssemblyLineForPseudoInsturction(statement);
    }

    private AssemblyLine AssembleInstruction(AssemblyStatement statement)
    {
        var assembledStatement = CompileInstruction(statement);

        var result = new AssemblyLine
        {
            Address = _pgCounter,
            AssembledAssemblyStatement = assembledStatement
        };

        _pgCounter += assembledStatement.MachineCode.Length;

        return result;
    }

    private AssembledAssemblyStatement CompileDataInstruction(
        string instructionName,
        AssemblyStatement statement)
    {
        return new AssembledAssemblyStatement
        {
            AssemblyStatement = statement,
            MachineCode = instructionName switch
            {
                "DS" => _instructionTranslator.DS(statement.OperandList.First.ToImmediateData()),
                "DW" => _instructionTranslator.DW(statement.OperandList.First.To16bitAdress()),
                "DB" => _instructionTranslator.DB(statement.OperandList.Operands.Select(x => x.ToImmediateData()).ToArray()),
                _ => throw new NotImplementedException()
            }
        };
    }

    private AssembledAssemblyStatement CompileInstruction(AssemblyStatement statement)
    {
        var assembled = new AssembledAssemblyStatement
        {
            AssemblyStatement = statement,
            MachineCode = Array.Empty<byte>()
        };

        if (statement.Instruction == null)
            throw new Exception($"AssemblyStatement ${statement} has no instruction");

        var instruction =
            _instructionTranslator.GetType()
                                  .GetMethod(statement.Instruction)
            ?? throw new InvalidDataException($"Unknown instruction {statement.Instruction}");

        var instructionParamInfo = instruction.GetParameters();

        if (statement.OperandList.Count != instructionParamInfo.Length)
        {
            throw new ArgumentException(
                $"{instruction.Name} takes {instructionParamInfo.Length} arguments, " +
                $"but {statement.OperandList.Count} were given");
        }

        if (instruction.Name is "DS" or "DW" or "DB")
        {
            return CompileDataInstruction(instruction.Name, statement);
        }

        var instructionArgs = new List<object>();
        foreach (var (First, Second) in instructionParamInfo.Zip(statement.OperandList.Operands))
        {
            if (Second is OperandLabel label && label.LabelType == LabelType.Unknown)
                _assembledLinesWithLabels.AddLast(assembled);

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

        uint code = (uint)instruction.Invoke(_instructionTranslator, instructionArgs.ToArray())!;

        byte[] bytes = BitConverter.GetBytes(code);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // NOP Fix
        //if (statement.Instruction == "NOP")
        //    return new byte[] { 0 };

        assembled.MachineCode = bytes.SkipWhile(x => x == 0).ToArray();

        return assembled;
    }

    public static IEnumerable<string> GetPseudoInstructrions() =>
        new string[] { "ORG", "EQU", "SET", "END", "IF", "ENDIF", "MACRO", "ENDM" };

}