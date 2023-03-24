namespace MyProject;

interface IOperand
{
    Register ToRegister();
    RegisterPair ToRegisterPair();
    byte ToImmediateData();
    ushort To16bitAdress();

    string ToString();
}
