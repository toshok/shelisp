
(require 'lisp-tests)

(if verbose-tests
  (message ";;;; EQUALITY TESTS"))

;; eq

(assert-eq 5 5 "numbers are eq")
(assert-not-eq "hi" "hi" "strings are not eq")
(assert-eq 'hi 'hi "symbols are eq")
(assert-not-eq '(1) '(1) "lists are not eq")

(assert-not-eq 5.0 5 "floats are not eq to ints")

;; eql

(assert-eql 5 5 "numbers are eql")
(assert-not-eql "hi" "hi" "strings are not eql")
(assert-eq 'hi 'hi "symbols are eql")
(assert-not-eq '(1) '(1) "lists are not eql")

;; more number eql tests

(assert-not-eql 5.0 5 "floats are not eql to ints")

;; equal

(assert-equal 5 5 "numbers are equal")
(assert-equal "hi" "hi" "strings are equal")
(assert-equal 'hi 'hi "symbols are equal")
(assert-equal '(1) '(1) "lists are equal")

;; more number equal tests

(assert-not-equal 5.0 5 "floats are not equal to ints")
