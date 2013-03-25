# makefile for nmake
# Builds EasyOpt library and three sample programs using EasyOpt

basepath=src^\

all:	EasyOpt.dll UseCaseLs.exe UseCaseTouch.exe UseCaseTime.exe

EasyOpt.dll: 
    csc /target:library /out:$@ $(basepath)EasyOpt\*.cs

UseCaseTime.exe: EasyOpt.dll
    csc /target:exe /reference:EasyOpt.dll /out:$@ $(basepath)UseCaseTime\Program.cs

UseCaseTouch.exe: EasyOpt.dll
    csc /target:exe /reference:EasyOpt.dll /out:$@ $(basepath)UseCaseTouch\Program.cs

UseCaseLs.exe: EasyOpt.dll
    csc /target:exe /reference:EasyOpt.dll /out:$@ $(basepath)UseCaseLs\Program.cs
