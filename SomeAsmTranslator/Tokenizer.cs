﻿namespace MyProject;

class Tokenizer
{
    private readonly string[] _instructionTable =
    {
        "STC", "CMC",               // Carry bit instructions
        "INR", "DCR", "CMA", "DAA", // Single register instructions
        "NOP",                      // Nop
        "MOV", "STAX", "LDAX",      // Data Transfer Instruction
        "ADD", "ADC", "SUB", "SBB", "ANA", "XRA", "ORA", "CMP", // Register or memory to accumulator instructions
        "RLC", "RRC", "RAL", "RAR",
        "PUSH", "POP", "DAD", "INX", "DCX", "XCHG", "XTHL", "SPHL",
        "LXI", "MVI", "ADI", "ACI", "SUI", "SBI", "ANI", "XRI", "ORI", "CPI",
        "STA", "LDA", "SHLD", "LHLD",
        "PCHL", "JMP", "JC", "JNC", "JZ", "JNZ", "JP", "JM", "JPE", "JPO",
        "CALL", "CC", "CNC", "CZ", "CNZ", "CP", "CM", "CPE", "CPO",
        "RET", "RC", "RNC", "RZ", "RNZ", "RM", "RP", "RPE", "RPO",
        "RST",
        "EI", "DI",
        "IN", "OUT",
        "HLT",
        "ORG",
        "EQU",
        "SET",
        "END",
        "IF", "ENDIF",
        "MACRO", "ENDM"
    };

    private string _sourceCode;

    private int _pos = 0;

    private char CurrentChar { get => (_pos < _sourceCode.Length) ? _sourceCode[_pos] : '\0'; }

    private void NextChar()
    {
        _pos++;
    }

    public Tokenizer(string sourceCode)
    {
        _sourceCode = sourceCode.ToUpper();
    }

    private void PassWhiteSpaces()
    {
        while (char.IsWhiteSpace(CurrentChar))
            NextChar();
    }

    private Token ProcLabelAndSymbols()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        while (IsCharLetterOrSpecialSym(CurrentChar) || char.IsDigit(CurrentChar))
        {
            value += CurrentChar;
            NextChar();
        }

        if (_instructionTable.Contains(value))
        {
            if (CurrentChar == ':')
            {
                throw new Exception("Label cannot be named by existing instruction name");
            }

            return new Token { TokenType = TokenType.OPCODE, Value = value };
        }

        if (CurrentChar == ':')
        {
            NextChar();
            return new Token { TokenType = TokenType.LABEL, Value = value };
        }

        return new Token { TokenType = TokenType.SYMBOL, Value = value };
    }

    private Token ProcNumbers()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        while (!char.IsWhiteSpace(CurrentChar) && CurrentChar != '\0' && CurrentChar != ',')
        {
            value += CurrentChar;
            NextChar();
        }

        return new Token { TokenType = TokenType.NUMBER, Value = value };
    }

    private Token ProcComment()
    {
        string value = string.Empty;

        NextChar();

        while (CurrentChar != '\n' && CurrentChar != '\r' && CurrentChar != '\0')
        {
            value += CurrentChar;
            NextChar();
        }

        return new Token { TokenType = TokenType.COMMENT, Value = value.Trim() };
    }

    private Token ProcString()
    {
        string value = string.Empty;

        NextChar();

        while (CurrentChar != '\'')
        {
            value += CurrentChar;
            NextChar();

            if (CurrentChar == '\0')
                throw new Exception("No end quote found");
        }

        NextChar();

        return new Token { TokenType = TokenType.STRING, Value = value.Trim() };
    }

    private Token ProcProgramCounterData()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        return new Token { TokenType = TokenType.PG_DATA, Value = value };
    }

    private Token ProcComma()
    {
        string value = string.Empty;

        value += CurrentChar;
        NextChar();

        return new Token { TokenType = TokenType.COMMA, Value = value };
    }

    public Token Next()
    {
        string value = string.Empty;

        PassWhiteSpaces();

        if (CurrentChar == '\0')
        {
            return new Token { TokenType = TokenType.EOF };
        }
        else if (IsCharLetterOrSpecialSym(CurrentChar))
        {
            return ProcLabelAndSymbols();
        }
        else if (char.IsNumber(CurrentChar))
        {
            return ProcNumbers();
        }
        else if (CurrentChar == ';')
        {
            return ProcComment();
        }
        else if (CurrentChar == '\'')
        {
            return ProcString();
        }
        else if (CurrentChar == '$')
        {
            return ProcProgramCounterData();
        }
        else if (CurrentChar == ',')
        {
            return ProcComma();
        }

        NextChar();

        return new Token { TokenType = TokenType.UNKNOWN, Value = value };
    }

    private bool IsCharLetterOrSpecialSym(char c) => (c >= 'A' && c <= 'Z') || c == '?' || c == '@' || c == '_';

    private bool IsHexNumber(char c) => char.IsDigit(c) || (c >= 'A' && c <= 'F');

}
