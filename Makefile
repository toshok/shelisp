TOP=.

SUBDIRS=lisp repl test

run:
	$(MAKE) -C repl run

include $(TOP)/build/build.mk
