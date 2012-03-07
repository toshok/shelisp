TOP=..

PROGRAM=test.exe
PROGRAM_SOURCES=Main.cs
REFERENCES=$(TOP)/lisp/shelisp.dll

include $(TOP)/build/build.mk

check: $(PROGRAM)
	MONO_PATH=$(shell pwd)/$(TOP)/lisp $(MONO) --debug $(PROGRAM)