TOP=..

PROGRAM=shrepl.exe
PROGRAM_SOURCES=$(wildcard *.cs)
REFERENCES=$(TOP)/lisp/shelisp.dll

include $(TOP)/build/build.mk

run: $(PROGRAM)
	MONO_PATH=$(shell pwd)/$(TOP)/lisp $(MONO) --debug $(PROGRAM)