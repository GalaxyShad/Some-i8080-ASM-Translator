namespace I8080Translator;

class Token
{
    public TokenType TokenType { get; set; }
    public string Value { get; set; } = string.Empty;
    public required long Line { get; set; } = 0;

    public static Token EOF => new() { TokenType = TokenType.EOF, Value = "EOF", Line = 0 };
    public static Token NewLine => new() { TokenType = TokenType.NewLine, Value = "\n", Line = 0 };
}
