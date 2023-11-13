# Some-i8080-ASM-Translator
<p align="center">
  <img src="https://github.com/GalaxyShad/Some-i8080-ASM-Translator/assets/52833080/2b692322-90f3-4f74-b8f6-969fd142fac9" /><br/>
</p>
Транслятор ассемблера КР580ВМ80А / Intel 8080A в машинный код, с возможностью генерации листинга в разных форматах.

## Как пользоваться
Для вывода справки вызовите:
```
i8080 --help
```
Общий формат 
```
i8080 <имя файла> -<флаг 1> -<флаг2>  
```
Не забудьте добавить исполняемый файл i8080 в переменные окружения PATH

## Доступные опции
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

## Пример использования
Предположим, что имеется директория MyProject со следующей структурой:
```
MyProject
└── example.asm         // Файл исходного кода на ассемблере 
```
Содержимое файла "example.asm"
```asm
ORG 0800h
INIT:
  MVI A, 0Fh          ; put value 0F to reg A
  OUT 5               ; send value from A to port 5

  IN 5                ; get value from port 5 to reg A
  XRI 0FFh

  CALL SOME_FOO

SOME_FOO:
  CALL 04FCh
  RET

ORG 0900h             ; raw data definition
  DB 5
  DB 18
  DB 18h
  DB 12o
  DB 0100_0101b
```
Тогда, после выполнения команды:
```
i8080 example.asm -b -w
```
Будет сгенерировано 2 файла рядом с example.asm:
```
MyProject
├── example.asm                     // Файл исходного кода на ассемблере
├── example.i8080asm.bin            // Бинарный файл с машинным кодом 
├── example.i8080asm.txt            // Файл листинга 
└── example.i8080asm.docx           // Файл Word с таблицей листинга
```
Содержимое файла "example.i8080asm.txt"
```
 ADR | MC | LABEL     | ASM           ; COMMENT
     |    |           | ORG 0800H     ; 
0800 | 3E | INIT:     | MVI A,0FH     ; PUT VALUE 0F TO REG A
0801 | 0F |           |               ; 
0802 | D3 |           | OUT 5         ; SEND VALUE FROM A TO PORT 5
0803 | 05 |           |               ; 
0804 | DB |           | IN 5          ; GET VALUE FROM PORT 5 TO REG A
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

