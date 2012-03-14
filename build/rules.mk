all: all-local all-recurse all-hook

clean: clean-local clean-recurse clean-hook

dist: dist-local dist-recurse dist-hook

all-local::
clean-local::
dist-local::

all-recurse::
clean-recurse::
dist-recurse::

all-hook::
clean-hook::
dist-hook::

RECURSE_INTO_SUBDIRS= \
	@target=`echo $@ | sed -e s/-recurse//`; \
	for i in $(SUBDIRS); do \
	  echo Making $$target in $$i; \
	  $(MAKE) -C $$i $$target || exit 1; \
	done

ifneq ($(SUBDIRS),)
all-recurse::
	$(RECURSE_INTO_SUBDIRS)

clean-recurse::
	$(RECURSE_INTO_SUBDIRS)

dist-recurse::
	$(RECURSE_INTO_SUBDIRS)
endif

.PHONY: all all-recurse all-hook
.PHONY: clean clean-recurse clean-hook
.PHONY: dist dist-recurse dist-hook

# special handling if you define either PROGRAM+PROGRAM_SOURCES or LIBRARY+LIBRARY_SOURCES

ifneq ($(PROGRAM),)
$(PROGRAM): $(PROGRAM_SOURCES) $(REFERENCES)
	$(GMCS) -debug $(EXTRA_FLAGS) -out:$@ $(PROGRAM_SOURCES) $(REFERENCES:%=-r:%)

all-local:: $(PROGRAM)

clean-local::
	rm -f $(PROGRAM) $(PROGRAM).mdb
endif

ifneq ($(LIBRARY),)
$(LIBRARY): $(LIBRARY_SOURCES) $(REFERENCES)
	$(GMCS) -debug $(EXTRA_FLAGS) -out:$@ -target:library $(LIBRARY_SOURCES) $(REFERENCES:%=-r:%)

all-local:: $(LIBRARY)

clean-local::
	rm -f $(LIBRARY) $(LIBRARY).mdb
endif