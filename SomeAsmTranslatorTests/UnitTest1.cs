

using FluentAssertions;
using SomeAsmTranslator.Source;

namespace SomeAsmTranslatorTests
{
    [TestClass]
    public class LabelTests
    {
        [TestMethod]
        public void TestEqu()
        {
            var asm = new Assembler(@"TEST EQU 20h
                                      CALL TEST     ");

            var listing = asm.AssembleAll().ToList();
            listing[1].AssembledAssemblyStatement
                      .MachineCode
                      .Should().Equal(new byte[] { 0xCD, 0x00, 0x20 });
        }

        [TestMethod]
        public void WhenSourceCodeStartsWithNewLines()
        {
            var asm = new Assembler(@"



                ORG 0800h
                DB 20h
            ");

            var listing = asm.AssembleAll().ToList();
            listing[1].AssembledAssemblyStatement
                      .MachineCode
                      .Should()
                      .Equal(new byte[] { 0x20 });
        }
    }

    [TestClass]
    public class InstructionsMachineCodeTests
    {
        private void TestInstruction(string sourceCode, byte[] expectedMachineCode)
        {
            var asm = new Assembler(sourceCode);

            asm.AssembleAll()
               .ToList()[0]
               .AssembledAssemblyStatement
               .MachineCode
               .Should()
               .Equal(expectedMachineCode);
        }

        [TestMethod]
        public void ADD_A() => TestInstruction("ADD A", new byte[] { 0x87 });

        [TestMethod]
        public void ADD_B() => TestInstruction("ADD B", new byte[] { 0x80 });

        [TestMethod]
        public void ADI_D8() => TestInstruction("ADI 32h", new byte[] { 0xC6, 0x32 });
    }
}