(progn
  (print-format "this comes from a lisp file, result = {0}" (car '(1 2 3 4 "hi")))
  (print (car '(1 2 3 4 "hi"))))