TOP=..

PROGRAM=test.exe
PROGRAM_SOURCES=Main.cs Shunit.cs
REFERENCES=$(TOP)/lisp/shelisp.dll
EMACS ?= env emacs

include $(TOP)/build/build.mk

check: $(PROGRAM)
	MONO_PATH=$(shell pwd)/$(TOP)/lisp $(MONO) --debug $(PROGRAM)

check-emacs:
	$(EMACS) --batch -l run-tests.el