using I8080Translator;

namespace SomeAsmTranslator.Source;

class Preproccesor
{
    public IEnumerable<AssemblyLine> Procces(string source)
    {
        var lineList = new List<AssemblyLine>();

        var parser = new Parser(source);

        AssemblyStatement statement = parser.Next();
        while (!statement.IsEmpty())
        {
            if (statement.Label != null)
                statement.Label.Value = (ushort)_pgCounter;

            var asmLine = new AssemblyLine(_pgCounter, statement);

            if (statement.Instruction != null)
            {
                if (statement.Instruction == "ORG")
                {
                    asmLine.Bytes = null;
                    asmLine.Address = null;
                    _pgCounter = NumericDataParser.SwapBytes(statement.OperandList.First.To16bitAdress());
                }

                else if (statement.Instruction == "EQU")
                {
                    if (statement.Label == null)
                        throw new InvalidDataException("Name of symbol is missing for EQU");

                    statement.Label.Value = statement.OperandList.First.ToImmediateData();
                }

                else
                {
                    if (statement.Instruction is "DB" or "DS" or "DW")
                    {
                        var args = new List<object>();

                        if (statement.Instruction is "DB")
                            args.Add(statement.OperandList.Operands.Select(x => x.ToImmediateData()).ToArray());
                        else if (statement.Instruction is "DS")
                            args.Add(statement.OperandList.First.ToImmediateData());
                        else if (statement.Instruction is "DW")
                            args.Add(statement.OperandList.First.To16bitAdress());

                        _pgCounter += InstructionTranslator
                            .GetDataDefinitionInstructionByteCount(statement.Instruction, args.ToArray());
                    }
                    else
                        _pgCounter += InstructionTranslator.GetInstructionByteCount(statement.Instruction);
                }
            }

            lineList.Add(asmLine);
            statement = parser.Next();
        }

        return lineList;
    }

    private int _pgCounter = 0x800;

    public static IEnumerable<string> GetPseudoInstructrions() =>
        new string[] { "ORG", "EQU", "SET", "END", "IF", "ENDIF", "MACRO", "ENDM" };
}
