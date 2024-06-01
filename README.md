<div align="center">
  <p align="center">
      <h1 align="center" style="color:red;">Some-i8080-ASM-Translator</h1>
      <img src="https://github.com/GalaxyShad/Some-i8080-ASM-Translator/assets/52833080/2b692322-90f3-4f74-b8f6-969fd142fac9" /><br/>
      Some-i8080-ASM-Translator is a powerful, lightweight, and easy-to-use assembler translator for the KR580VM80A / Intel 8080A, developed according to the specifications in the INTEL8080 Assembly Language Programming Manual. It generates machine code and offers the ability to generate listings in various formats.
  </p>
</div>

## Download Link

You can download the latest version of the translator in the [Releases Section](https://github.com/GalaxyShad/Some-i8080-ASM-Translator/releases).

## Features

- **Underscores in Numeric Literals**: You can define binary operands as ```1010_1100b``` for improved readability.
- **Any Length Labels**: Use labels of any length to make your code more descriptive.
- **Cross-platform**: Some-i8080-ASM-Translator works on all major platforms, thanks to .NET 8.0.
- **ORG, SET, EQU, END Pseudo Instructions**: Organize your code and data with these useful pseudo instructions.
- **DB, DS, DW**: Define and allocate bytes, strings, and words in your program.
- **Arithmetic and Logical Expressions**: Perform complex calculations and operations with ease.
- **Generation of Listings**: Some-i8080-ASM-Translator can generate listings in ```.DOCX```, ```.TXT```, ```.CSV```, and ```.MD``` formats for your convenience.
- **Error Reporting**: The translator provides detailed descriptions of errors to help you quickly identify and fix issues in your code.

## Limitations

The translator implements all the features specified in the [INTEL8080 Assembly Language Programming Manual](https://altairclone.com/downloads/manuals/8080%20Programmers%20Manual.pdf) with the following exceptions:

- No Pseudo instructions: IF AND ENDIF, MACRO AND ENDM
- No ASCII Constant operands

## How to Use

To display the help, run:
```
i8080 --help
```
The general format is:
```
i8080 <filename> -<flag1> -<flag2>
```
Don't forget to add the executable file i8080 to your PATH environment variables.

## Available Options

```
  -c, --csv                    Create listing file in .csv table format
  -w, --word                   Create listing file in .docx word table format
  -s, --samelinebyte           Keep all instruction bytes on the same line
  -m, --md                     Create listing file in .md Markdown table format
  -b, --bin                    Generate binary file

  --help                       Display help screen.
  --version                    Display version information.

  Source code file (pos. 0)    Required. Input file-name including path
```

## Usage Example

Suppose you have a directory called MyProject with the following structure:
```
MyProject
└── example.asm         // Assembly source code file
```
The contents of the "example.asm" file are as follows:
```asm
ORG 0800h
INIT:
  MVI A, 0Fh          ; Load 0F into register A
  OUT 5               ; Output the value in A to port 5

  IN 5                ; Input from port 5 into register A
  XRI 0FFh

  CALL SOME_FOO

SOME_FOO:
  CALL 04FCh
  RET

ORG 0900h             ; Raw data definition
  DB 5
  DB 18
  DB 18h
  DB 12o
  DB 0100_0101b
```
After running the command:
```
i8080 example.asm -b -w
```
Two files will be generated next to "example.asm":
```
MyProject
├── example.asm                     // Assembly source code file
├── example.i8080asm.bin            // Binary file with machine code
├── example.i8080asm.txt            // Listing file
└── example.i8080asm.docx           // Word file with listing table
```
The contents of the "example.i8080asm.txt" file are as follows:
```
 ADR | MC | LABEL     | ASM           ; COMMENT
     |    |           | ORG 0800H     ;
0800 | 3E | INIT:     | MVI A,0FH     ; LOAD 0F INTO REGISTER A
0801 | 0F |           |               ;
0802 | D3 |           | OUT 5         ; OUTPUT THE VALUE IN A TO PORT 5
0803 | 05 |           |               ;
0804 | DB |           | IN 5          ; INPUT FROM PORT 5 INTO REGISTER A
0805 | 05 |           |               ;
0806 | EE |           | XRI 0FFH      ;
0807 | FF |           |               ;
0808 | CD |           | CALL SOME_FOO ;
0809 | 0B |           |               ;
080A | 08 |           |               ;
080B | CD | SOME_FOO: | CALL 04FCH    ;
080C | FC |           |               ;
080D | 04 |           |               ;
080E | C9 |           | RET           ;
     |    |           | ORG 0900H     ; RAW DATA DEFINITION
0900 | 05 |           | DB 5          ;
0901 | 12 |           | DB 18         ;
0902 | 18 |           | DB 18H        ;
0903 | 0A |           | DB 12O        ;
0904 | 45 |           | DB 0100_0101B ;
```

## Building the Project for All Platforms

To build the project, you need to have .NET 8.0 SDK installed. You can download it from the official [.NET website](https://dotnet.microsoft.com/download/dotnet/8.0). After installing the SDK, you can build the project for all platforms using the following command:
```
dotnet publish -c Release -r linux-x64 --self-contained true
```
This will generate the executable file "i8080" in the "bin/Release/net8.0/linux-x64/publish" directory. You can use this file on any Linux-based platform. For other platforms, simply replace "linux-x64" with the appropriate runtime identifier.
