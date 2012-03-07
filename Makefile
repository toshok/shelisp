#EXTRAFLAGS=-define:DEBUG

SOURCES = Main.cs $(wildcard lisp/*.cs)
test.exe: $(SOURCES)
	gmcs -debug $(EXTRAFLAGS) -out:$@ $(SOURCES)

clean:
	rm *.exe *.mdb