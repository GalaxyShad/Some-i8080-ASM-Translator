

using FluentAssertions;
using SomeAsmTranslator.Source;

namespace SomeAsmTranslatorTests
{
    [TestClass]
    public class LabelTests
    {
        [TestMethod]
        public void TestMethod1()
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
                      .Should().Equal(new byte[] { 0x20 });
        }
    }
}