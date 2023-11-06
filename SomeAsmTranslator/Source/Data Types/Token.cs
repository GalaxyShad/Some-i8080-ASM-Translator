namespace I8080Translator;

class Token
{
    public TokenType TokenType { get; set; }
    public string Value { get; set; } = string.Empty;

    public static Token EOF => new() { TokenType = TokenType.EOF, Value = "EOF" };
    public static Token NewLine => new() { TokenType = TokenType.NewLine, Value = "\n" };
}
