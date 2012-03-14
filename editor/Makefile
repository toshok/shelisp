TOP=..

PROGRAM=shemacs.exe
PROGRAM_SOURCES=$(wildcard *.cs)
REFERENCES=$(TOP)/lisp/shelisp.dll

# this should live in $(TOP)/build/config.mk
#EXTRA_FLAGS=-define:DEBUG

include $(TOP)/build/build.mk


run: $(PROGRAM)
	MONO_INLINELIMIT=0 MONO_PATH=$(shell pwd)/$(TOP)/lisp $(MONO) --debug $(PROGRAM)