# shelisp
An implementation of Emacs Lisp (elisp) in C#.  The idea was to port the entirety of emacs over to C# (shemacs!) but I figured I'd start with the lisp engine first.  The code here is derived from emacs source, and therefore subject to GPLv3.

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

## what version of emacs is this code based on?
Short answer: emacs 24.1

Longer answer: not entirely sure.  I resurrected this project recently (as of 7/9/2019) and am going by git commit timestamps. The correct answer is probably "some master commit between the release of 23.4 and 24.1."  24.1 seems like a safe bet if you want to help out (and now it's written down so I won't forget again.)

## err, why "she lisp"?
I got sick of the convention of putting "sharp" at the end of a project name when it was a port to C#.  sharp = sh, which makes a nice prefix here.

