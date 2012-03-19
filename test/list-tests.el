(require 'lisp-tests)

(if verbose-tests
  (message ";;;; LIST TESTS"))


(assert-equal (cdr '(1 2)) '(2) "the cdr of a normal list is a list")
(assert-equal (cdr '(1 . 2)) 2  "the cdr of a pair is the second element")
