@echo off 
Echo.
Echo Demo 1
Echo.
Echo This demo creates compressed assembly from a single windows forms executable.
Echo --------------------------------------------------------------------------------  

"Rpx.exe" "Demo1\RpxDemo.exe" /o "Demo1\Compressed\RpxDemo1_Comp1.exe" /v Normal

Echo.
Echo Test the outputted assembly
Echo --------------------------------------------------------------------------------  
Echo.

"Demo1\Compressed\RpxDemo1_Comp1.exe"
Echo.
pause 

Echo.
Echo Demo 2
Echo.
Echo This demo creates compressed assembly from a single console executable and 
Echo a single supporting .dll assembly.
Echo --------------------------------------------------------------------------------  

"Rpx.exe" "Demo2\RpxDemo2.exe" "Demo2\DemoLib1.dll" /o "Demo2\Compressed\RpxDemo2_Comp1.exe" /v Normal

Echo Test the outputted assembly passing in the arguments "Dog" "Cat" "Pig" "Cow"
Echo --------------------------------------------------------------------------------  
Echo.

"Demo2\Compressed\RpxDemo2_Comp1.exe" "Dog" "Cat" "Pig" "Cow"
Echo.
pause 


Echo.
Echo Tool Demo
Echo This demo creates a tool assembly from multiple assemblys. and using an external
Echo source file for the assembly info ("ToolDemo\AssemblyInfo.cs")
Echo --------------------------------------------------------------------------------  

"Rpx.exe" "ToolDemo\ToolDemo.exe" /a "ToolDemo\AssemblyInfo.cs" /t "Tool1:ToolDemo\ToolDemo1.exe,Tool2:ToolDemo\ToolDemo2.exe,Tool3:ToolDemo\ToolDemo3.exe"

Echo.

pause 

Echo.
Echo Test the outputted assembly 
Echo --------------------------------------------------------------------------------
Echo.

Echo Test Tool 1
Echo.
"ToolDemo\ToolDemo.exe" Tool1
Echo.
Echo.

Echo Test Tool 2
Echo.
"ToolDemo\ToolDemo.exe" Tool2
Echo.
Echo.

Echo Test Tool 3
Echo.
"ToolDemo\ToolDemo.exe" Tool3
Echo.
Echo.

pause 


Echo.
Echo Demo 3 A 
Echo This demo tests the capabilities of compressed assemblys when using AppDomains 
Echo With the additional assembly compressed within the final assembly and not 
Echo availible in the file system
Echo --------------------------------------------------------------------------------  

"Rpx.exe" "Demo3\RpxDemo3.exe" "Demo3\DemoLib2.dll" /o "Demo3\Compressed1\RpxDemo3_Comp1.exe"

Echo.
Echo Test the outputted assembly (this should fail on both Test 2 and Test 3)
Echo --------------------------------------------------------------------------------  
Echo.

"Demo3\Compressed1\RpxDemo3_Comp1.exe"
Echo.

pause

Echo.
Echo Demo 3 B
Echo This demo tests the capabilities of compressed assemblys when using AppDomains 
Echo With the additional assembly availible in the file system and not compressed 
Echo within the final assembly
Echo --------------------------------------------------------------------------------  


"Rpx.exe" "Demo3\RpxDemo3.exe" /o "Demo3\Compressed2\RpxDemo3_Comp1.exe"

Echo.
echo Copy the additonal assembly to the output folder

copy /y "Demo3\DemoLib2.dll" "Demo3\Compressed2\DemoLib2.dll"

Echo.
Echo Test the outputted assembly (this should work on Test 2 but still fail on Test 3)
Echo --------------------------------------------------------------------------------  
Echo.

"Demo3\Compressed2\RpxDemo3_Comp1.exe"

Echo.

pause

:EOF

