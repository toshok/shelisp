(require 'lisp-tests)

(if verbose-tests
  (message ";;;; SEQUENCE TESTS"))


(assert-equal (mapcar 'car '((a b) (c d) (e f)))  '(a c e) "mapcar works on list of lists")
(assert-equal (mapcar '1+ [1 2 3]) '(2 3 4) "mapcar works on vectors")
(assert-equal (mapcar 'string "abc") '("a" "b" "c") "mapcar works on strings")

(provide 'sequence-tests)
