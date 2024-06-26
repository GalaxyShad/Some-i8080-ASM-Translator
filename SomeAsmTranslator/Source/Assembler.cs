﻿using I8080Translator;
using SomeAsmTranslator.Operands;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SomeAsmTranslator.Source;

public class Assembler
{
    private readonly Lexer _lexer;
    private readonly Parser _parser;
    private readonly LabelTable _labelTable;
    private readonly InstructionTranslator _instructionTranslator;

    private readonly List<AssemblyLine> _assembledLines = new();
    private readonly List<AssembledAssemblyStatement> _assembledLinesWithLabels = new();

    private bool _isFirstAssembly = true;

    private int _pgCounter = 0x0000;

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
        ValidateLabels();
        ReassembleInstructionsWithLabels();

        return _assembledLines;
    }

    private void ValidateLabels()
    {
        foreach (var label in _labelTable.GetValues())
        {
            if (label.Type != LabelType.Unknown)
                continue;

            var errString = $"Undefined label \"{label.Name}\".";
            if (Regex.IsMatch(label.Name, "^([\\dA-F])*$"))
            {
                var correctedNumber = "0" + label.Name;

                if (!correctedNumber.EndsWith('H'))
                    correctedNumber += "h";

                errString +=
                    $"\n\nDid you mean \"{correctedNumber}\"?\n" +
                    $"Remember, each hexadecimal number must be followed by a letter 'H' and must begin with a numeric digit (0-9)";
            }

            throw new TranslatorAssemblerException(
                $"Label [{label.Name}]. {errString}",
                label.Token?.Line ?? 0
            );
        }
    }

    private void ReassembleInstructionsWithLabels()
    {
        foreach (var line in _assembledLinesWithLabels)
        {
            line.MachineCode = CompileInstruction(line.AssemblyStatement).MachineCode;
        }
    }

    private void AssignAdressLabel(AssemblyStatement statement)
    {
        if (statement.Label == null)
            return;

        if (statement.Label.Type is LabelType.Address)
        {
            throw new InvalidDataException(
                $"Double label \"{statement.Label.Name}\"");
        }
        else if (statement.Label.Type is LabelType.Equ or LabelType.Set)
        {
            throw new InvalidDataException(
                $"EQU or SET label cannot be redefined as Address label \"{statement.Label.Name}\"");
        }

        if (statement.Label.Token?.TokenType != TokenType.LabelAddress)
        {
            throw new InvalidDataException(
               $"\"{statement.Label.Name}\" is not an address label. Did you mean \"{statement.Label.Name}:\"?");
        }
           
        statement.Label.Type = LabelType.Address;
        statement.Label.Data = new OperandProgramCounter((ushort)_pgCounter);
    }

    private void AssembleWithoutLabelsData()
    {
        AssemblyStatement statement = _parser.Next();

        while (!statement.IsEmpty())
        {
            if (statement.Instruction?.Name is not "EQU" and not "SET")
            {
                try
                {
                    AssignAdressLabel(statement);
                }
                catch (Exception ex)
                {
                    throw new TranslatorAssemblerException(
                        $"Label '{statement.Label?.Name}': {ex.Message}",
                        statement.Label?.Token?.Line ?? 0
                    );
                }
            }

            if (statement.Instruction?.Name is "END")
                break;

            try
            {
                _assembledLines.Add(statement.Instruction?.Name switch
                {
                    "ORG" => AssembleOrg(statement),
                    "EQU" => AssembleEqu(statement),
                    "SET" => AssembleSet(statement),
                    null => MakeAssemblyLineWithEmptyMachineCode(statement),
                    _ => AssembleInstruction(statement),
                });
            }
            catch (Exception ex)
            {
                throw new TranslatorAssemblerException(
                    $"Instruction '{statement.Instruction?.Name}': {ex.Message}",
                    statement.Instruction?.Line ?? 0
                );
            }

            statement = _parser.Next();
        }

        _isFirstAssembly = false;
    }

    private static AssemblyLine MakeAssemblyLineWithEmptyMachineCode(AssemblyStatement statement)
    {
        return new AssemblyLine
        {
            Address = null,
            AssembledAssemblyStatement = new AssembledAssemblyStatement
            {
                MachineCode = Array.Empty<byte>(),
                AssemblyStatement = statement
            }
        };
    }

    private AssemblyLine AssembleOrg(AssemblyStatement statement)
    {
        if (statement.OperandList.Count != 1)
            throw new ArgumentException($"ORG takes 1 argument, but {statement.OperandList.Count} were given");

        _pgCounter = NumericDataParser.SwapBytes(statement.OperandList.First.To16bitAdress());

        return MakeAssemblyLineWithEmptyMachineCode(statement);
    }

    private AssemblyLine AssembleEqu(AssemblyStatement statement)
    {
        if (statement.Label == null)
            throw new InvalidDataException("Name of symbol is missing for EQU");

        if (statement.Label.Token?.TokenType is TokenType.LabelAddress)
            throw new InvalidDataException($"Extra colon in EQU definition \"{statement.Label.Name}:\". Did you mean \"{statement.Label.Name}\"?");

        if (_labelTable.Has(statement.Label))
        {
            if (statement.Label.Type == LabelType.Equ)
                throw new InvalidDataException($"EQU '{statement.Label.Name}' cannot be redefined");
            else if (statement.Label.Type == LabelType.Set)
                throw new InvalidDataException($"SET '{statement.Label.Name}' cannot be redefined with EQU");
        }

        if (statement.OperandList.Count != 1)
            throw new ArgumentException($"EQU takes 1 argument, but {statement.OperandList.Count} were given");

        statement.Label.Type = LabelType.Equ;
        statement.Label.Data = statement.OperandList.First;

        return MakeAssemblyLineWithEmptyMachineCode(statement);
    }

    private AssemblyLine AssembleSet(AssemblyStatement statement)
    {
        if (statement.Label == null)
            throw new InvalidDataException("Name of symbol is missing for SET");

        if (statement.Label.Token?.TokenType is TokenType.LabelAddress)
            throw new InvalidDataException($"Extra colon in SET definition \"{statement.Label.Name}:\". Did you mean \"{statement.Label.Name}\"?");

        if (_labelTable.Has(statement.Label) && statement.Label.Type == LabelType.Equ)
            throw new InvalidDataException($"EQU '{statement.Label.Name}' cannot be redefined with SET");

        if (statement.OperandList.Count != 1)
            throw new ArgumentException($"SET takes 1 argument, but {statement.OperandList.Count} were given");

        statement.Label.Type = LabelType.Set;
        statement.Label.Data = statement.OperandList.First;

        return MakeAssemblyLineWithEmptyMachineCode(statement);
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
        string instructionName, AssemblyStatement statement)
    {
        return new AssembledAssemblyStatement
        {
            AssemblyStatement = statement,
            MachineCode = instructionName switch
            {
                "DS" => _instructionTranslator.DS(statement.OperandList
                                                           .First
                                                           .ToImmediateData()),

                "DW" => _instructionTranslator.DW(statement.OperandList
                                                           .Operands
                                                           .Select(x => x.To16bitAdress())
                                                           .ToArray()),

                "DB" => _instructionTranslator.DB(statement.OperandList
                                                           .Operands
                                                           .Select(x => x.ToImmediateData())
                                                           .ToArray()),

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
                                  .GetMethod(statement.Instruction.Name)
            ?? throw new InvalidDataException($"Unknown instruction \"{statement.Instruction}\"");

        var instructionParamInfo = instruction.GetParameters();

        if (statement.OperandList.Count != instructionParamInfo.Length
            && instructionParamInfo[0].ParameterType != typeof(byte[])
            && instructionParamInfo[0].ParameterType != typeof(ushort[]))
        {
            throw new ArgumentException(
                $"{instruction.Name} takes {instructionParamInfo.Length} arguments, " +
                $"but {statement.OperandList.Count} were given");
        }

        if (instruction.Name is "DS" or "DW" or "DB")
        {
            assembled = CompileDataInstruction(instruction.Name, statement);

            if (_isFirstAssembly)
            {
                _assembledLinesWithLabels.Add(assembled);
            }

            return assembled;
        }

        var instructionArgs = new List<object>();
        foreach (var (compilerFunction, operand) in instructionParamInfo.Zip(statement.OperandList.Operands))
        {
            ConvertOperandsToInstructionData(assembled, instructionArgs, compilerFunction, operand);
        }

        uint code = (uint)instruction.Invoke(_instructionTranslator, instructionArgs.ToArray())!;

        byte[] bytes = BitConverter.GetBytes(code);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        assembled.MachineCode = bytes.SkipWhile(x => x == 0).ToArray();

        // NOP Fix
        if (statement.Instruction.Name == "NOP")
            assembled.MachineCode = new byte[] { 0 };

        return assembled;
    }

    private void ConvertOperandsToInstructionData(AssembledAssemblyStatement assembled, List<object> instructionArgs, ParameterInfo CompilerFunction, IOperand Operand)
    {
        if (_isFirstAssembly)
        {
            if (Operand is OperandLabel label && label.LabelType == LabelType.Unknown && !label.IsRegisterPair)
                _assembledLinesWithLabels.Add(assembled);

            if (Operand is OperandExpression exp && exp.HaveLabel)
                _assembledLinesWithLabels.Add(assembled);
        }

        if (CompilerFunction.ParameterType == typeof(byte))
            instructionArgs.Add(Operand.ToImmediateData());

        else if (CompilerFunction.ParameterType == typeof(ushort))
            instructionArgs.Add(Operand.To16bitAdress());

        else if (CompilerFunction.ParameterType == typeof(Register))
            instructionArgs.Add(Operand.ToRegister());

        else if (CompilerFunction.ParameterType == typeof(RegisterPair))
            instructionArgs.Add(Operand.ToRegisterPair());

        else throw new InvalidDataException("Unhandled type");
    }

    public static IEnumerable<string> GetPseudoInstructrions() =>
        new string[] { "ORG", "EQU", "SET", "END" };

}