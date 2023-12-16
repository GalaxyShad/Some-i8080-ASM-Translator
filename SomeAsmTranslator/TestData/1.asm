XRI 05h+(5))

ORG 0800h
  
M1:    
    MOV B, M            ; Reading an array element
    INR H               ; Increment of the pointer's high register
    MOV A, L            ; Low pointer register >= 08h?
    ANI 08h             
    MOV C, L            ; Temporary saving of the low pointer register
    JZ (M1)             ; Jump if the zero flag is active
    MOV A, L            ; Inverting the first 3 bits of the low pointer register if it >= 08h
    MOV L, A
M2:
    MOV M, B            ; Writing an element by pointer
    MOV L, C            ; Return the pointer value to its previous state
    DCR H
    INX H               ; Increment of the entire pointer
    MOV A, L        
    ANI 0F0h            ; Checking for array overruns
    JZ M1               ; Return to loop if not out of bounds
    RST 1               ; Stop
