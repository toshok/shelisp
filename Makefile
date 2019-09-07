TOP=.

SUBDIRS=lisp repl test

run: all
	$(MAKE) -C repl run

check: all
	$(MAKE) -C test check

include $(TOP)/build/build.mk
