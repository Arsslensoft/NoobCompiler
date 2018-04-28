bits 16
PROGRAM_ORG:
;section .bss


;section .rodata

;section .data
	xa db 0, 0
	t db 0, 0


;section .text



		test_2:
			; Method: Name = test
			; create stackframe
			push  BP
			mov  BP, SP
			; Parameters Definitions
			; Parameter s @BP4
			; Parameter i @BP6
			; Block
			; Push Parameter @BP 4
			push word [BP + 4]
			; Push Parameter @BP 6
			; 1 * i
			mov word CX, [BP + 6]
			mov word AX, 1
			mul  CX
			; s + 1 * i
			pop word AX
			add  AX, CX
			; v=s + 1 * i
			; Pop Var @BP-2
			mov word [BP - 2], AX
			; Push Field @t 0
			push word [t]
			; s=t
			; Pop Parameter @BP 4
			pop word [BP + 4]
			; return label

		test_2_ret:
			; destroy stackframe
			leave
			ret 4
			; Entry Point
			; xa=6
			; Pop Field @xa 0
			mov word [xa], 6
PROGRAM_END:
