INIT: 
  MVI E,080h 		  ;инициализация числа для правого дисплея
  MVI D,02h  		  ;инициализация числа для левого дисплея

STOP:
  CALL GET_KEY  	;вызов функции получения нажатой клавиши
  CPI 0FEh       	;если нажата клавиша 1, то идет в send_display
  JZ SEND_DISPLAY   ;прыжок
  JMP STOP      	;иначе зацикливание функции если ничего не нажато

GET_KEY:
  MVI я A,0F7h		  ;записываем число в аккум чтобы проверить затем какая клавиша нажата
  OUT 07h		      ;запись числа в порт 7
  IN 06h      	 	;чтение нажатой клавиши возвращение
  RET			        ;возврат из подпрограммы

SEND_DISPLAY:	
  CALL RIGHT   		;вызов подпрограммы для проверки сегментов дисплея
  MOV A,E      		;чтение данных из регистра Е в аккумулятор 
  RLC          		;сдвиг числа которое отвечает за сегмент влево
  STA 0BFAh   		;запись в ячейку, которая отвечает за правый дисплей
  MOV E,A      		;обмен данными между аккумулятором и регистром Е
  MOV A,D      		;чтение данных из регистра D в аккумулятор
  RRC          		;сдвиг числа вправо которое отвечает за сегмент влево
  STA 0BFBh   		;запись в ячейку, которая отвечает за левый дисплей
  MOV D,A      		;сохранение данных из аккумулятора в регистр D
  CALL 01C8h  		;вызов подпрограммы декодирования, чтобы информация отобразилась на дисплее
  CALL DELAY   		;вызов подпрограммы задержки
  CALL GET_KEY    ;вызов подпрограммы проверки нажатия клавиш
  CPI 0FDh		      ;если нажата клавиша 2, то делаем прыжок на STOP
  JZ STOP		      ;ПРЫЖОК	     
  JMP SEND_DISPLAY 	;иначе нажата клавиша 1
	
RIGHT:
  MOV A,E		  ;считывание числа из регистра Е в аккум
  CPI 20h		  ;проверка дошли ли мы до f
  JNZ LEFT		;если нет, то проверяем левый дисплей
  MVI E,80h		;иначе устанавливаем начальное состояние
 
LEFT:
  MOV A,D		  ;считывание числа из регистра D в аккум
  CPI 01h		  ;проверка дошли ли до а
  RNZ			    ;если нет то про
  MVI D,40h		;записываем число g,т.к на след итерации будет сдвиг и в аккумуляторе будет число f
  RET 			  ;возврат из подпрограммы
   
DELAY:
    MVI B,3Ch		  ;записываем кол-во секунд
DELAY_LOOP:
    CALL 0429h		;вызов подпрограммы задержки
    CALL 01C8h		;обновление данных на экране
    DCR B		      ;декремент счетчика
    JNZ DELAY_LOOP	;зацикливание
    RET			        ;возвращение из подпрограмы