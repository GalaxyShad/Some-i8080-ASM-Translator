using MyProject;

namespace SomeAsmTranslator.Source;

class Assembler
{
    public IEnumerable<AssemblyLine> AssembleAll(string source)
    {
        var list = new List<AssemblyLine>();

        var prep = new Preproccesor();

        foreach (var line in prep.Procces(source))
        {
            line.Bytes = AssembleStatement(line.AssemblyStatement);
            list.Add(line);
        }

        return list;
    }

    public byte[] AssembleStatement(AssemblyStatement statement)
    {
        if (statement.Instruction == null)
            return new byte[] { 0 };

        if (Preproccesor.GetPseudoInstructrions().Contains(statement.Instruction))
            return new byte[] { 0 };

        var compiler = new InstructionTranslator();

        var instruction = compiler.GetType().GetMethod(statement.Instruction);
        if (instruction == null)
            throw new InvalidDataException($"Unknown instruction {statement.Instruction}");

        var instructionParamInfo = instruction.GetParameters();
        if (statement.OperandList.Count != instructionParamInfo.Length)
            throw new ArgumentException(
                $"{instruction.Name} takes {instructionParamInfo.Length} arguments, " +
                $"but {statement.OperandList.Count} were given");

        if (instruction.Name is "DS")
            return compiler.DS(statement.OperandList.First.ToImmediateData());
        else if (instruction.Name is "DW")
            return compiler.DW(statement.OperandList.First.To16bitAdress());
        else if (instruction.Name is "DB")
            return compiler.DB(statement.OperandList.Operands.Select(x => x.ToImmediateData()).ToArray());

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

        uint code = (uint)instruction.Invoke(compiler, instructionArgs.ToArray())!;
        byte[] bytes = BitConverter.GetBytes(code);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        // NOP Fix
        if (bytes.Last() == 0)
            return new byte[] { 0 };

        return bytes.SkipWhile(x => x == 0).ToArray();
    }
}