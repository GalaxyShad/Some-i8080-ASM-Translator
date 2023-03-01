using System.Net.Security;

namespace MyProject;

public enum InstructionArgs
{
    None,
    Register,
    TwoRegisters,
    RegisterPair,
    RegisterPairAndWord,
    RegisterAndByte,
    Byte
}

static class InstructionTranslator
{

    // Carry Bit Instructions
    public static uint CMC() => 0b0011_1111;
    public static uint STC() => 0b0011_0111;


    // Single Register Instructions
    public static uint INR(Register reg) => 0b00_000_100 | ((uint)reg << 3);
    public static uint DCR(Register reg) => 0b00_000_101 | ((uint)reg << 3);
    public static uint CMA() => 0b0010_1111;
    public static uint DAA() => 0b0010_0111;

    // NOP
    public static uint NOP() => 0;

    // Data Transfer Instructions
    public static uint MOV(Register dstReg, Register srcReg)
    {
        if (dstReg == Register.M && srcReg == Register.M)
        {
            throw new InvalidDataException("In MOV Dst and Src cannot both = M (110b)");
        }

        return 0b01_000_000 | ((uint)dstReg << 3) | ((uint)srcReg);
    }

    public static uint STAX(RegisterPair regPair)
    {
        if (regPair != RegisterPair.BC && regPair != RegisterPair.DE)
        {
            throw new InvalidDataException("In STAX only BC and DE can be specified as register pairs");
        }

        return 0b000_0_0010 | ((uint)regPair << 4);
    }

    public static uint LDAX(RegisterPair regPair)
    {
        if (regPair != RegisterPair.BC && regPair != RegisterPair.DE)
        {
            throw new InvalidDataException("In LDAX only BC and DE can be specified as register pairs");
        }

        return 0b000_0_1010 | ((uint)regPair << 4);
    }

    // Register or memory to accumulator instructions
    public static uint ADD(Register reg) => 0b10_000_000 | (uint)reg;
    public static uint ADC(Register reg) => 0b10_001_000 | (uint)reg;
    public static uint SUB(Register reg) => 0b10_010_000 | (uint)reg;
    public static uint SBB(Register reg) => 0b10_011_000 | (uint)reg;
    public static uint ANA(Register reg) => 0b10_100_000 | (uint)reg;
    public static uint XRA(Register reg) => 0b10_101_000 | (uint)reg;
    public static uint ORA(Register reg) => 0b10_110_000 | (uint)reg;
    public static uint CMP(Register reg) => 0b10_111_000 | (uint)reg;

    // Rotate Accumulator Instructions
    public static uint RLC() => 0b000_00_111;
    public static uint RRC() => 0b000_01_111;
    public static uint RAL() => 0b000_10_111;
    public static uint RAR() => 0b000_11_111;

    // Register Pair Instructions
    public static uint PUSH(RegisterPair regPair)
    {
        if (regPair == RegisterPair.SP)
            throw new ArgumentException("SP cannot be specified in PUSH instruction");

        return 0b11_00_0101 | (uint)regPair << 4;
    }

    public static uint POP(RegisterPair regPair)
    {
        if (regPair == RegisterPair.SP)
            throw new ArgumentException("SP cannot be specified in POP instruction");

        return 0b11_00_0001 | (uint)regPair << 4;
    }

    public static uint DAD(RegisterPair regPair)
    {
        if (regPair == RegisterPair.PSW)
            throw new ArgumentException("PSW cannot be specified in DAD instruction");

        return 0b00_00_1001 | (((regPair == RegisterPair.SP) ? 0b11 : (uint)regPair) << 4);
    }

    public static uint INX(RegisterPair regPair)
    {
        if (regPair == RegisterPair.PSW)
            throw new ArgumentException("PSW cannot be specified in INX instruction");

        return 0b00_00_0011 | (((regPair == RegisterPair.SP) ? 0b11 : (uint)regPair) << 4);
    }

    public static uint DCX(RegisterPair regPair)
    {
        if (regPair == RegisterPair.PSW)
            throw new ArgumentException("PSW cannot be specified in DCX instruction");

        return 0b00_00_1011 | (((regPair == RegisterPair.SP) ? 0b11 : (uint)regPair) << 4);
    }

    public static uint XCHG() => 0b1110_1011;
    public static uint XTHL() => 0b1110_0011;
    public static uint SPHL() => 0b1111_1001;

    // Immediate Instructions

    public static uint LXI(RegisterPair regPair, ushort data)
    {
        if (regPair == RegisterPair.PSW)
            throw new ArgumentException("PSW cannot be specified in LXI instruction");

        return ((0b00_00_0001 | (((regPair == RegisterPair.SP) ? 0b11 : (uint)regPair) << 4)) << 16) | data;
    }
    public static uint MVI(Register reg, byte data) => ((0b00_000_110 | ((uint)reg << 3)) << 8) | (uint)data;
    public static uint ADI(byte data) => (0b11_000_110 << 8) | (uint)data;
    public static uint ACI(byte data) => (0b11_001_110 << 8) | (uint)data;
    public static uint SUI(byte data) => (0b11_010_110 << 8) | (uint)data;
    public static uint SBI(byte data) => (0b11_011_110 << 8) | (uint)data;
    public static uint ANI(byte data) => (0b11_100_110 << 8) | (uint)data;
    public static uint XRI(byte data) => (0b11_101_110 << 8) | (uint)data;
    public static uint ORI(byte data) => (0b11_110_110 << 8) | (uint)data;
    public static uint CPI(byte data) => (0b11_111_110 << 8) | (uint)data;

    // Direct Addresing Instructions
    public static uint SHLD(ushort adr) => (0b001_00_010 << 16) | (uint)adr;
    public static uint LHLD(ushort adr) => (0b001_01_010 << 16) | (uint)adr;
    public static uint STA(ushort adr) => (0b001_10_010 << 16) | (uint)adr;
    public static uint LDA(ushort adr) => (0b001_11_010 << 16) | (uint)adr;

    // Jump Instructions
    public static uint PCHL() => 0b1110_1001;
    public static uint JMP(ushort adr) => (0b11_000_011 << 16) | (uint)adr;

    public static uint JNZ(ushort adr) => (0b11_000_010 << 16) | (uint)adr;
    public static uint JZ(ushort adr) => (0b11_001_010 << 16) | (uint)adr;

    public static uint JNC(ushort adr) => (0b11_010_010 << 16) | (uint)adr;
    public static uint JC(ushort adr) => (0b11_011_010 << 16) | (uint)adr;

    public static uint JPO(ushort adr) => (0b11_100_010 << 16) | (uint)adr;
    public static uint JPE(ushort adr) => (0b11_101_010 << 16) | (uint)adr;

    public static uint JP(ushort adr) => (0b11_110_010 << 16) | (uint)adr;
    public static uint JM(ushort adr) => (0b11_111_010 << 16) | (uint)adr;


    // Call subroutine instructions
    public static uint CALL(ushort adr) => (0b11_001_101 << 16) | (uint)adr;

    public static uint CNZ(ushort adr) => (0b11_000_100 << 16) | (uint)adr;
    public static uint CZ(ushort adr) => (0b11_001_100 << 16) | (uint)adr;

    public static uint CNC(ushort adr) => (0b11_010_100 << 16) | (uint)adr;
    public static uint CC(ushort adr) => (0b11_011_100 << 16) | (uint)adr;

    public static uint CPO(ushort adr) => (0b11_100_100 << 16) | (uint)adr;
    public static uint CPE(ushort adr) => (0b11_101_100 << 16) | (uint)adr;

    public static uint CP(ushort adr) => (0b11_110_100 << 16) | (uint)adr;
    public static uint CM(ushort adr) => (0b11_111_100 << 16) | (uint)adr;


    // Return from subroutine instructions
    public static uint RET() => 0b11_001_001;

    public static uint RNZ() => 0b11_000_000;
    public static uint RZ() => 0b11_001_000;

    public static uint RNC() => 0b11_010_000;
    public static uint RC() => 0b11_011_000;

    public static uint RPO() => 0b11_100_000;
    public static uint RPE() => 0b11_101_000;

    public static uint RP() => 0b11_110_000;
    public static uint RM() => 0b11_111_000;

    // Rst instruction
    public static uint RST(byte exp)
    {
        if (exp > 7)
            throw new ArgumentException("RST arg must evaluate to a number in range 0 to 7");

        return 0b11_000_111 | ((uint)exp << 3);
    }

    // Interrupt flip flop instructions
    public static uint EI() => 0b1111_1011;
    public static uint DI() => 0b1111_0011;

    // Input/Output instructions
    public static uint IN(byte exp) => (0b1101_1011 << 8) | (uint)exp;
    public static uint OUT(byte exp) => (0b1101_0011 << 8) | (uint)exp;

    // Halt instruction
    public static uint HLT() => 0b0111_0110;
}
