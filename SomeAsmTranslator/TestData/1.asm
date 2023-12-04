; 
; Адреса УВВ модуля
; 
OTR     EQU     4       ; Регистр вывода на магнитафон, звук
ITR     EQU     4       ; Регистр ввода с магнитафона
LOUT    EQU     5       ; Выходной регистр
KEY     EQU     6       ; Регистр чтения клавиатуры
DSP     EQU     6       ; Регистр сегментов дисплея
SCAN    EQU     7       ; Регистр сканирования
CTL     EQU     8       ; Регистр управления (Базовый адрес)

;
; Назначение констант
;
PC      EQU     0800h   ; Начальное значение PC пользователя
TIME    EQU     67h     ; Константа цикла задержки в 1 МС
FREQ    EQU     20h     ; Константа частоты BEEP
DUR     EQU     40h     ; Константа длительности BEEP

RS4C    EQU     0BF6h   ; Точка выхода по RST 4
RS5C    EQU     0BF9h   ; Точка выхода по RST 5
RS6C    EQU     0BFCh   ; Точка выхода по RST 6

TRP     EQU     0BFFh   ; Верх защищенного ОЗУ
UR      EQU     0BEFh   ; Верх ОЗУ (Без точек входа по RST)
ERAM    EQU     10h     ; Конец ОЗУ

;
; Области стека монитора и пользователя в ОЗУ
;
ORG 0FB0h               
USP:    DS 0            ; Начальное значение SP пользователя
        DS 1Eh
        
MSP:    DS 0            ; Начальное значение SP монитора

;
; Область переменных монитора в ОЗУ
;
ORG 0FD1h
TSAVS:  DS 2
TSAVH:  DS 2
RAML1:  DS 1
RS:     DS 1
RAML2:  DS 4

SAVRG:  DS 1
SAVPC:  DS 2
SAVSL:  DS 1
SAVSH:  DS 1
SAVL:   DS 2
SAVE:   DS 2
SAVC:   DS 2
SAVPW:  DS 1
SAVA:   DS 1
UDKY:   DS 8
UDSP:   DS 6

UDSP6:  DS 2
RMP:    DS 2
DDSP:   DS 6

; 
; RST 0 - точка входа в монитор по сбросу
; ( Клавиша "R" или включение питания )
;
RESET:
    MVI H, 008h     ; Адрес начала ОЗУ
    XRA A
    CMA
    MOV M, A        ; Запись FF в одну из ячеек ОЗУ
    ;JMP STRT

; 
; RST 1 - точка входа в монитор по прерыванию 
; ( Клавиша "СТ" или точка останова )
;
B:
RS1:
    SHLD TSAVH
    OUT 008h
    JMP TRP

;
; RST 2 - свисток с фиксированной длительносью и тоном
;
BEEP:
    MVI B, FREQ     ; Частота сигнала
    MVI D, DUR      ; Длительность сигнала 
    ;JMP BEEP2
    NOP

; 
; RST 3 - перемещение сообщения по адресу DE в область UDSP
; 
STDM:
    PUSH B
    PUSH PSW
    LXI H, UDSP
    ;JMP SDM 
    NOP 

RS4:
    ;JMP RS4C 
    DB 0, 0, 0
    DB 0, 0

RS5:
    ;JMP RS5C
    DB 0, 0, 0
    DB 0, 0

RS6:
    ;JMP RS6C
    DB 0, 0, 0
    DB 0, 0

