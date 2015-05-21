;//An example on how to use PESpin License markers 

includelib  C:\WinDDK\6000\lib\wnet\amd64\kernel32.lib
includelib  C:\WinDDK\6000\lib\wnet\amd64\user32.lib

extrn MessageBoxA : PROC
extrn ExitProcess : PROC


.data
caption db "x64",0
text    db "Hello World!",0

.code
start proc
  
  ;[i] Please turn off "Remove OEP" option when testing this example
     
  mov     rdx,0C0DE064022222222h
  mov     rcx,0C0DE064011111111h
  call    Licence_code
  
  sub   rsp, 28h    ; the area to protect starts here 
  xor   r9d, r9d
  lea   r8, caption
  lea   rdx, text
  xor   rcx, rcx
  call  MessageBoxA ; end of area to protect 

  mov   rdx,0C0DE064088888888h
  mov   rcx,0C0DE064044444444h
  call  Licence_code

  xor   ecx, ecx
  call  ExitProcess

start endp

Licence_code proc
  ret
Licence_code endp

end

