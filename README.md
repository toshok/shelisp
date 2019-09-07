# shelisp
An implementation of Emacs Lisp (elisp) in C#.  The idea was to port the entirety of emacs over to C# (shemacs!) but I figured I'd start with the lisp engine first.

## Building/Running it
Currently there isn't anything resembling an editor, but there's a repl to play around with.

```
# to build
$ make

# to run the repl
$ make run
shelisp[0]> 1
 => 1
shelisp[0]> (+ 1 2)
 => 3
shelisp[0]> (defun add(a b) (+ a b))
 => add
shelisp[0]> (add 1 2)
 => 3
shelisp[0]>
```

## err, why "she lisp"?
I got sick of the convention of putting "sharp" at the end of a project name when it was a port to C#.  sharp = sh, which makes a nice prefix.

