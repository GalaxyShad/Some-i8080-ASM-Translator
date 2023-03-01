namespace MyProject;

public enum TokenType 
{ 
    UNKNOWN,
    LABEL,
    SYMBOL,             // OP, Pseudo OP, Symbol for SET or EQU, Label
    STRING,
    PG_DATA,
    NUMBER,
    COMMENT,
    COMMA,
    EOF,
    OPCODE,
}
