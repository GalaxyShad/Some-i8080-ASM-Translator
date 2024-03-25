namespace I8080Translator;

public interface IOperand
{
    Register ToRegister();
    RegisterPair ToRegisterPair();
    byte ToImmediateData();
    ushort To16bitAdress();
    ushort ToRawData();

    string ToString();
}
