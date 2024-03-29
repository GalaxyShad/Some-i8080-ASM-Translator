﻿namespace I8080Translator;

public enum TokenType
{
    Unknown,
    LabelAddress,
    Symbol,             // OP, Pseudo OP, Symbol for SET or EQU, Label
    String,
    ProgramCounterData,
    Number,
    Comment,
    Comma,
    EOF,
    Instruction,
    NewLine,
    ExpressionOperator
}
