ORG 0800h
INIT:
    MVI A, 11000011b    ; Reset (Сброс ККД)
    OUT 0A5h
    DS 3
    MVI A, 00001101b    ; Mode set
    OUT 0A5h

KSCAN:
    MVI A, 01010000b    ; Установка режима чтения сенсоров
    OUT 0A5h            ; Отправка в РУС
    MVI H, 0Dh 
KSCA1:
    IN 0A4h
    MOV B, A 
    MVI A, 10000000b
KSCA2:
    MOV C, A
    ANA B
    MOV A, C
    JZ KSCA3

    CALL DECR           ; Преобразование информации о колонках в 8ричное значение

    MVI A, 11010000b    ; Очистка дисплея 
    OUT 0A5h            ; Отправка в РУС

    MOV A, H            ; Копируем H, чтобы узнать на какой строке сенсор
    CMA                 ; Инверсия
    ANI 00010000b       ; Выделение 4 разряда из H (D = 110_1, C = 110_0)
    RRC                 ; Сдвиг на 1 разряд вправо
    ORI 10000000b       ; Команда записи в память ККД
    ORA L               ; Выбор ячейки ОЗУ
    OUT 0A5h            ; Отправка в РУС

    MOV A, M            ; Чтение буквы из HL
    OUT 0A4h            ; Отпрвка буквы в ККД

    JMP KSCAN           ; Прыгаем в начало
KSCA3:
    RAR                 ; Сдвиг аккумулятора
    JNZ KSCA2           

    DCR H               ; После декремента у 0Ch паритет четный, а у 0Bh нечетный
    JPE KSCA1           ; Прыгаем если 0Ch

    JMP KSCAN           ; Повтор программы


DECR:
    MVI L, 10o          ; Перевод битовых значений в восьмиричные (1000_0000 -> 7, 0100_0000 -> 6)
DECR1:
    DCR L
    RAL  
    RC
    JMP DECR1



ORG 0D00h   
    DB 77h  ; A
    DB 7Ch  ; b
    DB 39h  ; C 
    DB 5Eh  ; d
    DB 79h  ; E
    DB 71h  ; F
    DB 76h  ; H
    DB 30h  ; I

ORG 0C00h  
    DB 0DCh ; q or 67h
    DB 1Eh  ; J
    DB 38h  ; L
    DB 3Fh  ; O or 5Ch
    DB 73h  ; P
    DB 6Dh  ; S
    DB 3Eh  ; U
    DB 6Eh  ; Y