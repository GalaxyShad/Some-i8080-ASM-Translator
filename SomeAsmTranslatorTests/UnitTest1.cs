

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
                      .Should().Equal(new byte[] { 0xCD, 0x20, 0x00 });
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
        public void ADD_C() => TestInstruction("ADD C", new byte[] { 0x81 });

        [TestMethod]
        public void ADD_D() => TestInstruction("ADD D", new byte[] { 0x82 });

        [TestMethod]
        public void ADD_E() => TestInstruction("ADD E", new byte[] { 0x83 });

        [TestMethod]
        public void ADD_H() => TestInstruction("ADD H", new byte[] { 0x84 });

        [TestMethod]
        public void ADD_L() => TestInstruction("ADD L", new byte[] { 0x85 });

        [TestMethod]
        public void ADD_M() => TestInstruction("ADD M", new byte[] { 0x86 });

        [TestMethod]
        public void ADI_D8() => TestInstruction("ADI 32h", new byte[] { 0xC6, 0x32 });

        [TestMethod]
        public void ADC_A() => TestInstruction("ADC A", new byte[] { 0x8F });

        [TestMethod]
        public void ADC_B() => TestInstruction("ADC B", new byte[] { 0x88 });

        [TestMethod]
        public void ADC_C() => TestInstruction("ADC C", new byte[] { 0x89 });

        [TestMethod]
        public void ADC_D() => TestInstruction("ADC D", new byte[] { 0x8A });

        [TestMethod]
        public void ADC_E() => TestInstruction("ADC E", new byte[] { 0x8B });

        [TestMethod]
        public void ADC_H() => TestInstruction("ADC H", new byte[] { 0x8C });

        [TestMethod]
        public void ADC_L() => TestInstruction("ADC L", new byte[] { 0x8D });

        [TestMethod]
        public void ADC_M() => TestInstruction("ADC M", new byte[] { 0x8E });

        [TestMethod]
        public void ACI_D8() => TestInstruction("ACI 32h", new byte[] { 0xCE, 0x32 });

        [TestMethod]
        public void ANA_A() => TestInstruction("ANA A", new byte[] { 0xA7 });

        [TestMethod]
        public void ANA_B() => TestInstruction("ANA B", new byte[] { 0xA0 });

        [TestMethod]
        public void ANA_C() => TestInstruction("ANA C", new byte[] { 0xA1 });

        [TestMethod]
        public void ANA_D() => TestInstruction("ANA D", new byte[] { 0xA2 });

        [TestMethod]
        public void ANA_E() => TestInstruction("ANA E", new byte[] { 0xA3 });

        [TestMethod]
        public void ANA_H() => TestInstruction("ANA H", new byte[] { 0xA4 });

        [TestMethod]
        public void ANA_L() => TestInstruction("ANA L", new byte[] { 0xA5 });

        [TestMethod]
        public void ANA_M() => TestInstruction("ANA M", new byte[] { 0xA6 });

        [TestMethod]
        public void ANI_D8() => TestInstruction("ANI 32h", new byte[] { 0xE6, 0x32 });

        [TestMethod]
        public void CALL() => TestInstruction("CALL 6432h", new byte[] { 0xCD, 0x32, 0x64 });

        [TestMethod]
        public void CZ() => TestInstruction("CZ 6432h", new byte[] { 0xCC, 0x32, 0x64 });

        [TestMethod]
        public void CNZ() => TestInstruction("CNZ 6432h", new byte[] { 0xC4, 0x32, 0x64 });

        [TestMethod]
        public void CP() => TestInstruction("CP 6432h", new byte[] { 0xF4, 0x32, 0x64 });

        [TestMethod]
        public void CM() => TestInstruction("CM 6432h", new byte[] { 0xFC, 0x32, 0x64 });

        [TestMethod]
        public void CC() => TestInstruction("CC 6432h", new byte[] { 0xDC, 0x32, 0x64 });

        [TestMethod]
        public void CNC() => TestInstruction("CNC 6432h", new byte[] { 0xD4, 0x32, 0x64 });

        [TestMethod]
        public void CPE() => TestInstruction("CPE 6432h", new byte[] { 0xEC, 0x32, 0x64 });

        [TestMethod]
        public void CPO() => TestInstruction("CPO 6432h", new byte[] { 0xE4, 0x32, 0x64 });

        [TestMethod]
        public void CMA() => TestInstruction("CMA", new byte[] { 0x2F });

        [TestMethod]
        public void CMC() => TestInstruction("CMC", new byte[] { 0x3F });

        [TestMethod]
        public void CMP_A() => TestInstruction("CMP A", new byte[] { 0xBF });

        [TestMethod]
        public void CMP_B() => TestInstruction("CMP B", new byte[] { 0xB8 });

        [TestMethod]
        public void CMP_C() => TestInstruction("CMP C", new byte[] { 0xB9 });

        [TestMethod]
        public void CMP_D() => TestInstruction("CMP D", new byte[] { 0xBA });

        [TestMethod]
        public void CMP_E() => TestInstruction("CMP E", new byte[] { 0xBB });

        [TestMethod]
        public void CMP_H() => TestInstruction("CMP H", new byte[] { 0xBC });

        [TestMethod]
        public void CMP_L() => TestInstruction("CMP L", new byte[] { 0xBD });

        [TestMethod]
        public void CMP_M() => TestInstruction("CMP M", new byte[] { 0xBE });

        [TestMethod]
        public void CPI() => TestInstruction("CPI 32h", new byte[] { 0xFE, 0x32 });

        [TestMethod]
        public void DAA() => TestInstruction("DAA", new byte[] { 0x27 });

        [TestMethod]
        public void DAD_B() => TestInstruction("DAD B", new byte[] { 0x09 });

        [TestMethod]
        public void DAD_D() => TestInstruction("DAD D", new byte[] { 0x19 });

        [TestMethod]
        public void DAD_H() => TestInstruction("DAD H", new byte[] { 0x29 });

        [TestMethod]
        public void DAD_SP() => TestInstruction("DAD SP", new byte[] { 0x39 });

        [TestMethod]
        public void DCR_A() => TestInstruction("DCR A", new byte[] { 0x3D });

        [TestMethod]
        public void DCR_B() => TestInstruction("DCR B", new byte[] { 0x05 });

        [TestMethod]
        public void DCR_C() => TestInstruction("DCR C", new byte[] { 0x0D });

        [TestMethod]
        public void DCR_D() => TestInstruction("DCR D", new byte[] { 0x15 });

        [TestMethod]
        public void DCR_E() => TestInstruction("DCR E", new byte[] { 0x1D });

        [TestMethod]
        public void DCR_H() => TestInstruction("DCR H", new byte[] { 0x25 });

        [TestMethod]
        public void DCR_L() => TestInstruction("DCR L", new byte[] { 0x2D });

        [TestMethod]
        public void DCR_M() => TestInstruction("DCR M", new byte[] { 0x35 });

        [TestMethod]
        public void DCX_B() => TestInstruction("DCX B", new byte[] { 0x0B });

        [TestMethod]
        public void DCX_D() => TestInstruction("DCX D", new byte[] { 0x1B });

        [TestMethod]
        public void DCX_H() => TestInstruction("DCX H", new byte[] { 0x2B });

        [TestMethod]
        public void DCX_SP() => TestInstruction("DCX SP", new byte[] { 0x3B });

        [TestMethod]
        public void DI() => TestInstruction("DI", new byte[] { 0xF3 });

        [TestMethod]
        public void EI() => TestInstruction("EI", new byte[] { 0xFB });

        [TestMethod]
        public void HLT() => TestInstruction("HLT", new byte[] { 0x76 });

        [TestMethod]
        public void IN() => TestInstruction("IN 32h", new byte[] { 0xDB, 0x32 });

        [TestMethod]
        public void INR_A() => TestInstruction("INR A", new byte[] { 0x3C });

        [TestMethod]
        public void INR_B() => TestInstruction("INR B", new byte[] { 0x04 });

        [TestMethod]
        public void INR_C() => TestInstruction("INR C", new byte[] { 0x0C });

        [TestMethod]
        public void INR_D() => TestInstruction("INR D", new byte[] { 0x14 });

        [TestMethod]
        public void INR_E() => TestInstruction("INR E", new byte[] { 0x1C });

        [TestMethod]
        public void INR_H() => TestInstruction("INR H", new byte[] { 0x24 });

        [TestMethod]
        public void INR_L() => TestInstruction("INR L", new byte[] { 0x2C });

        [TestMethod]
        public void INR_M() => TestInstruction("INR M", new byte[] { 0x34 });

        [TestMethod]
        public void INX_B() => TestInstruction("INX B", new byte[] { 0x03 });

        [TestMethod]
        public void INX_D() => TestInstruction("INX D", new byte[] { 0x13 });

        [TestMethod]
        public void INX_H() => TestInstruction("INX H", new byte[] { 0x23 });

        [TestMethod]
        public void INX_SP() => TestInstruction("INX SP", new byte[] { 0x33 });

        [TestMethod]
        public void JMP() => TestInstruction("JMP 6432h", new byte[] { 0xC3, 0x32, 0x64 });

        [TestMethod]
        public void JZ() => TestInstruction("JZ 6432h", new byte[] { 0xCA, 0x32, 0x64 });

        [TestMethod]
        public void JNZ() => TestInstruction("JNZ 6432h", new byte[] { 0xC2, 0x32, 0x64 });

        [TestMethod]
        public void JP() => TestInstruction("JP 6432h", new byte[] { 0xF2, 0x32, 0x64 });

        [TestMethod]
        public void JM() => TestInstruction("JM 6432h", new byte[] { 0xFA, 0x32, 0x64 });

        [TestMethod]
        public void JC() => TestInstruction("JC 6432h", new byte[] { 0xDA, 0x32, 0x64 });

        [TestMethod]
        public void JNC() => TestInstruction("JNC 6432h", new byte[] { 0xD2, 0x32, 0x64 });

        [TestMethod]
        public void JPE() => TestInstruction("JPE 6432h", new byte[] { 0xEA, 0x32, 0x64 });

        [TestMethod]
        public void JPO() => TestInstruction("JPO 6432h", new byte[] { 0xE2, 0x32, 0x64 });

        [TestMethod]
        public void LDA() => TestInstruction("LDA 6432h", new byte[] { 0x3A, 0x32, 0x64 });

        [TestMethod]
        public void LDAX_B() => TestInstruction("LDAX B", new byte[] { 0x0A });

        [TestMethod]
        public void LDAX_D() => TestInstruction("LDAX D", new byte[] { 0x1A });

        [TestMethod]
        public void LHLD() => TestInstruction("LHLD 6432h", new byte[] { 0x2A, 0x32, 0x64 });

        [TestMethod]
        public void LXI_B() => TestInstruction("LXI B, 6432h", new byte[] { 0x01, 0x32, 0x64 });

        [TestMethod]
        public void LXI_D() => TestInstruction("LXI D, 6432h", new byte[] { 0x11, 0x32, 0x64 });

        [TestMethod]
        public void LXI_H() => TestInstruction("LXI H, 6432h", new byte[] { 0x21, 0x32, 0x64 });

        [TestMethod]
        public void LXI_SP() => TestInstruction("LXI SP, 6432h", new byte[] { 0x31, 0x32, 0x64 });

        [TestMethod]
        public void MOV_A_A() => TestInstruction("MOV A, A", new byte[] { 0x7F });

        [TestMethod]
        public void MOV_A_B() => TestInstruction("MOV A, B", new byte[] { 0x78 });

        [TestMethod]
        public void MOV_A_C() => TestInstruction("MOV A, C", new byte[] { 0x79 });

        [TestMethod]
        public void MOV_A_D() => TestInstruction("MOV A, D", new byte[] { 0x7A });

        [TestMethod]
        public void MOV_A_E() => TestInstruction("MOV A, E", new byte[] { 0x7B });

        [TestMethod]
        public void MOV_A_H() => TestInstruction("MOV A, H", new byte[] { 0x7C });

        [TestMethod]
        public void MOV_A_L() => TestInstruction("MOV A, L", new byte[] { 0x7D });

        [TestMethod]
        public void MOV_A_M() => TestInstruction("MOV A, M", new byte[] { 0x7E });

        [TestMethod]
        public void MOV_B_A() => TestInstruction("MOV B, A", new byte[] { 0x47 });

        [TestMethod]
        public void MOV_B_B() => TestInstruction("MOV B, B", new byte[] { 0x40 });

        [TestMethod]
        public void MOV_B_C() => TestInstruction("MOV B, C", new byte[] { 0x41 });

        [TestMethod]
        public void MOV_B_D() => TestInstruction("MOV B, D", new byte[] { 0x42 });

        [TestMethod]
        public void MOV_B_E() => TestInstruction("MOV B, E", new byte[] { 0x43 });

        [TestMethod]
        public void MOV_B_H() => TestInstruction("MOV B, H", new byte[] { 0x44 });

        [TestMethod]
        public void MOV_B_L() => TestInstruction("MOV B, L", new byte[] { 0x45 });

        [TestMethod]
        public void MOV_B_M() => TestInstruction("MOV B, M", new byte[] { 0x46 });

        [TestMethod]
        public void MOV_C_A() => TestInstruction("MOV C, A", new byte[] { 0x4F });

        [TestMethod]
        public void MOV_C_B() => TestInstruction("MOV C, B", new byte[] { 0x48 });

        [TestMethod]
        public void MOV_C_C() => TestInstruction("MOV C, C", new byte[] { 0x49 });

        [TestMethod]
        public void MOV_C_D() => TestInstruction("MOV C, D", new byte[] { 0x4A });

        [TestMethod]
        public void MOV_C_E() => TestInstruction("MOV C, E", new byte[] { 0x4B });

        [TestMethod]
        public void MOV_C_H() => TestInstruction("MOV C, H", new byte[] { 0x4C });

        [TestMethod]
        public void MOV_C_L() => TestInstruction("MOV C, L", new byte[] { 0x4D });

        [TestMethod]
        public void MOV_C_M() => TestInstruction("MOV C, M", new byte[] { 0x4E });

        [TestMethod]
        public void MOV_D_A() => TestInstruction("MOV D, A", new byte[] { 0x57 });

        [TestMethod]
        public void MOV_D_B() => TestInstruction("MOV D, B", new byte[] { 0x50 });

        [TestMethod]
        public void MOV_D_C() => TestInstruction("MOV D, C", new byte[] { 0x51 });

        [TestMethod]
        public void MOV_D_D() => TestInstruction("MOV D, D", new byte[] { 0x52 });

        [TestMethod]
        public void MOV_D_E() => TestInstruction("MOV D, E", new byte[] { 0x53 });

        [TestMethod]
        public void MOV_D_H() => TestInstruction("MOV D, H", new byte[] { 0x54 });

        [TestMethod]
        public void MOV_D_L() => TestInstruction("MOV D, L", new byte[] { 0x55 });

        [TestMethod]
        public void MOV_D_M() => TestInstruction("MOV D, M", new byte[] { 0x56 });

        [TestMethod]
        public void MOV_E_A() => TestInstruction("MOV E, A", new byte[] { 0x5F });

        [TestMethod]
        public void MOV_E_B() => TestInstruction("MOV E, B", new byte[] { 0x58 });

        [TestMethod]
        public void MOV_E_C() => TestInstruction("MOV E, C", new byte[] { 0x59 });

        [TestMethod]
        public void MOV_E_D() => TestInstruction("MOV E, D", new byte[] { 0x5A });

        [TestMethod]
        public void MOV_E_E() => TestInstruction("MOV E, E", new byte[] { 0x5B });

        [TestMethod]
        public void MOV_E_H() => TestInstruction("MOV E, H", new byte[] { 0x5C });

        [TestMethod]
        public void MOV_E_L() => TestInstruction("MOV E, L", new byte[] { 0x5D });

        [TestMethod]
        public void MOV_E_M() => TestInstruction("MOV E, M", new byte[] { 0x5E });

        [TestMethod]
        public void MOV_H_A() => TestInstruction("MOV H, A", new byte[] { 0x67 });

        [TestMethod]
        public void MOV_H_B() => TestInstruction("MOV H, B", new byte[] { 0x60 });

        [TestMethod]
        public void MOV_H_C() => TestInstruction("MOV H, C", new byte[] { 0x61 });

        [TestMethod]
        public void MOV_H_D() => TestInstruction("MOV H, D", new byte[] { 0x62 });

        [TestMethod]
        public void MOV_H_E() => TestInstruction("MOV H, E", new byte[] { 0x63 });

        [TestMethod]
        public void MOV_H_H() => TestInstruction("MOV H, H", new byte[] { 0x64 });

        [TestMethod]
        public void MOV_H_L() => TestInstruction("MOV H, L", new byte[] { 0x65 });

        [TestMethod]
        public void MOV_H_M() => TestInstruction("MOV H, M", new byte[] { 0x66 });

        [TestMethod]
        public void MOV_L_A() => TestInstruction("MOV L, A", new byte[] { 0x6F });

        [TestMethod]
        public void MOV_L_B() => TestInstruction("MOV L, B", new byte[] { 0x68 });

        [TestMethod]
        public void MOV_L_C() => TestInstruction("MOV L, C", new byte[] { 0x69 });

        [TestMethod]
        public void MOV_L_D() => TestInstruction("MOV L, D", new byte[] { 0x6A });

        [TestMethod]
        public void MOV_L_E() => TestInstruction("MOV L, E", new byte[] { 0x6B });

        [TestMethod]
        public void MOV_L_H() => TestInstruction("MOV L, H", new byte[] { 0x6C });

        [TestMethod]
        public void MOV_L_L() => TestInstruction("MOV L, L", new byte[] { 0x6D });

        [TestMethod]
        public void MOV_L_M() => TestInstruction("MOV L, M", new byte[] { 0x6E });

        [TestMethod]
        public void MOV_M_A() => TestInstruction("MOV M, A", new byte[] { 0x77 });

        [TestMethod]
        public void MOV_M_B() => TestInstruction("MOV M, B", new byte[] { 0x70 });

        [TestMethod]
        public void MOV_M_C() => TestInstruction("MOV M, C", new byte[] { 0x71 });

        [TestMethod]
        public void MOV_M_D() => TestInstruction("MOV M, D", new byte[] { 0x72 });

        [TestMethod]
        public void MOV_M_E() => TestInstruction("MOV M, E", new byte[] { 0x73 });

        [TestMethod]
        public void MOV_M_H() => TestInstruction("MOV M, H", new byte[] { 0x74 });

        [TestMethod]
        public void MOV_M_L() => TestInstruction("MOV M, L", new byte[] { 0x75 });

        [TestMethod]
        public void MVI_A_D8() => TestInstruction("MVI A, 32h", new byte[] { 0x3E, 0x32 });

        [TestMethod]
        public void MVI_B_D8() => TestInstruction("MVI B, 32h", new byte[] { 0x06, 0x32 });

        [TestMethod]
        public void MVI_C_D8() => TestInstruction("MVI C, 32h", new byte[] { 0x0E, 0x32 });

        [TestMethod]
        public void MVI_D_D8() => TestInstruction("MVI D, 32h", new byte[] { 0x16, 0x32 });

        [TestMethod]
        public void MVI_E_D8() => TestInstruction("MVI E, 32h", new byte[] { 0x1E, 0x32 });

        [TestMethod]
        public void MVI_H_D8() => TestInstruction("MVI H, 32h", new byte[] { 0x26, 0x32 });

        [TestMethod]
        public void MVI_L_D8() => TestInstruction("MVI L, 32h", new byte[] { 0x2E, 0x32 });

        [TestMethod]
        public void MVI_M_D8() => TestInstruction("MVI M, 32h", new byte[] { 0x36, 0x32 });

        [TestMethod]
        public void NOP() => TestInstruction("NOP", new byte[] { 0x00 });

        [TestMethod]
        public void ORA_A() => TestInstruction("ORA A", new byte[] { 0xB7 });

        [TestMethod]
        public void ORA_B() => TestInstruction("ORA B", new byte[] { 0xB0 });

        [TestMethod]
        public void ORA_C() => TestInstruction("ORA C", new byte[] { 0xB1 });

        [TestMethod]
        public void ORA_D() => TestInstruction("ORA D", new byte[] { 0xB2 });

        [TestMethod]
        public void ORA_E() => TestInstruction("ORA E", new byte[] { 0xB3 });

        [TestMethod]
        public void ORA_H() => TestInstruction("ORA H", new byte[] { 0xB4 });

        [TestMethod]
        public void ORA_L() => TestInstruction("ORA L", new byte[] { 0xB5 });

        [TestMethod]
        public void ORA_M() => TestInstruction("ORA M", new byte[] { 0xB6 });

        [TestMethod]
        public void ORI_D8() => TestInstruction("ORI 32h", new byte[] { 0xF6, 0x32 });

        [TestMethod]
        public void OUT() => TestInstruction("OUT 32h", new byte[] { 0xD3, 0x32 });

        [TestMethod]
        public void PCHL() => TestInstruction("PCHL", new byte[] { 0xE9 });

        [TestMethod]
        public void POP_B() => TestInstruction("POP B", new byte[] { 0xC1 });

        [TestMethod]
        public void POP_D() => TestInstruction("POP D", new byte[] { 0xD1 });

        [TestMethod]
        public void POP_H() => TestInstruction("POP H", new byte[] { 0xE1 });

        [TestMethod]
        public void POP_PSW() => TestInstruction("POP PSW", new byte[] { 0xF1 });

        [TestMethod]
        public void PUSH_B() => TestInstruction("PUSH B", new byte[] { 0xC5 });

        [TestMethod]
        public void PUSH_D() => TestInstruction("PUSH D", new byte[] { 0xD5 });

        [TestMethod]
        public void PUSH_H() => TestInstruction("PUSH H", new byte[] { 0xE5 });

        [TestMethod]
        public void PUSH_PSW() => TestInstruction("PUSH PSW", new byte[] { 0xF5 });

        [TestMethod]
        public void RAL() => TestInstruction("RAL", new byte[] { 0x17 });

        [TestMethod]
        public void RAR() => TestInstruction("RAR", new byte[] { 0x1F });

        [TestMethod]
        public void RLC() => TestInstruction("RLC", new byte[] { 0x07 });

        [TestMethod]
        public void RRC() => TestInstruction("RRC", new byte[] { 0x0F });

        [TestMethod]
        public void RET() => TestInstruction("RET", new byte[] { 0xC9 });

        [TestMethod]
        public void RZ() => TestInstruction("RZ", new byte[] { 0xC8 });

        [TestMethod]
        public void RNZ() => TestInstruction("RNZ", new byte[] { 0xC0 });

        [TestMethod]
        public void RP() => TestInstruction("RP", new byte[] { 0xF0 });

        [TestMethod]
        public void RM() => TestInstruction("RM", new byte[] { 0xF8 });

        [TestMethod]
        public void RC() => TestInstruction("RC", new byte[] { 0xD8 });

        [TestMethod]
        public void RNC() => TestInstruction("RNC", new byte[] { 0xD0 });

        [TestMethod]
        public void RPE() => TestInstruction("RPE", new byte[] { 0xE8 });

        [TestMethod]
        public void RPO() => TestInstruction("RPO", new byte[] { 0xE0 });

        [TestMethod]
        public void RST_0() => TestInstruction("RST 0", new byte[] { 0xC7 });

        [TestMethod]
        public void RST_1() => TestInstruction("RST 1", new byte[] { 0xCF });

        [TestMethod]
        public void RST_2() => TestInstruction("RST 2", new byte[] { 0xD7 });

        [TestMethod]
        public void RST_3() => TestInstruction("RST 3", new byte[] { 0xDF });

        [TestMethod]
        public void RST_4() => TestInstruction("RST 4", new byte[] { 0xE7 });

        [TestMethod]
        public void RST_5() => TestInstruction("RST 5", new byte[] { 0xEF });

        [TestMethod]
        public void RST_6() => TestInstruction("RST 6", new byte[] { 0xF7 });

        [TestMethod]
        public void RST_7() => TestInstruction("RST 7", new byte[] { 0xFF });

        [TestMethod]
        public void SPHL() => TestInstruction("SPHL", new byte[] { 0xF9 });

        [TestMethod]
        public void SHLD() => TestInstruction("SHLD, 6432h", new byte[] { 0x22, 0x32, 0x64 });

        [TestMethod]
        public void STA() => TestInstruction("STA 6432h", new byte[] { 0x32, 0x32, 0x64 });

        [TestMethod]
        public void STAX_B() => TestInstruction("STAX B", new byte[] { 0x02 });

        [TestMethod]
        public void STAX_D() => TestInstruction("STAX D", new byte[] { 0x12 });

        [TestMethod]
        public void STC() => TestInstruction("STC", new byte[] { 0x37 });

        [TestMethod]
        public void SUB_A() => TestInstruction("SUB A", new byte[] { 0x97 });

        [TestMethod]
        public void SUB_B() => TestInstruction("SUB B", new byte[] { 0x90 });

        [TestMethod]
        public void SUB_C() => TestInstruction("SUB C", new byte[] { 0x91 });

        [TestMethod]
        public void SUB_D() => TestInstruction("SUB D", new byte[] { 0x92 });

        [TestMethod]
        public void SUB_E() => TestInstruction("SUB E", new byte[] { 0x93 });

        [TestMethod]
        public void SUB_H() => TestInstruction("SUB H", new byte[] { 0x94 });

        [TestMethod]
        public void SUB_L() => TestInstruction("SUB L", new byte[] { 0x95 });

        [TestMethod]
        public void SUB_M() => TestInstruction("SUB M", new byte[] { 0x96 });

        [TestMethod]
        public void SUI_D8() => TestInstruction("SUI 32h", new byte[] { 0xD6, 0x32 });

        [TestMethod]
        public void SBB_A() => TestInstruction("SBB A", new byte[] { 0x9F });

        [TestMethod]
        public void SBB_B() => TestInstruction("SBB B", new byte[] { 0x98 });

        [TestMethod]
        public void SBB_C() => TestInstruction("SBB C", new byte[] { 0x99 });

        [TestMethod]
        public void SBB_D() => TestInstruction("SBB D", new byte[] { 0x9A });

        [TestMethod]
        public void SBB_E() => TestInstruction("SBB E", new byte[] { 0x9B });

        [TestMethod]
        public void SBB_H() => TestInstruction("SBB H", new byte[] { 0x9C });

        [TestMethod]
        public void SBB_L() => TestInstruction("SBB L", new byte[] { 0x9D });

        [TestMethod]
        public void SBB_M() => TestInstruction("SBB M", new byte[] { 0x9E });

        [TestMethod]
        public void SBI_D8() => TestInstruction("SBI 32h", new byte[] { 0xDE, 0x32 });

        [TestMethod]
        public void XCHG() => TestInstruction("XCHG", new byte[] { 0xEB });

        [TestMethod]
        public void XTHL() => TestInstruction("XTHL", new byte[] { 0xE3 });

        [TestMethod]
        public void XRA_A() => TestInstruction("XRA A", new byte[] { 0xAF });

        [TestMethod]
        public void XRA_B() => TestInstruction("XRA B", new byte[] { 0xA8 });

        [TestMethod]
        public void XRA_C() => TestInstruction("XRA C", new byte[] { 0xA9 });

        [TestMethod]
        public void XRA_D() => TestInstruction("XRA D", new byte[] { 0xAA });

        [TestMethod]
        public void XRA_E() => TestInstruction("XRA E", new byte[] { 0xAB });

        [TestMethod]
        public void XRA_H() => TestInstruction("XRA H", new byte[] { 0xAC });

        [TestMethod]
        public void XRA_L() => TestInstruction("XRA L", new byte[] { 0xAD });

        [TestMethod]
        public void XRA_M() => TestInstruction("XRA M", new byte[] { 0xAE });

        [TestMethod]
        public void XRI_D8() => TestInstruction("XRI 32h", new byte[] { 0xEE, 0x32 });
    }
}
