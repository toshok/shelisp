
(require 'lisp-tests)

(if verbose-tests
  (message ";;;; SYMBOL TESTS"))

(assert-null (boundp 'unused-symbol) "symbol has no value")
(assert-equal (setq unused-symbol 5) 5 "setq returns the value")
(assert-equal unused-symbol 5 "evaluating the symbol returns the value")
(assert-equal (symbol-value 'unused-symbol) 5 "symbol-value")
(assert-not-null (boundp 'unused-symbol) "symbol has a value")

(defun myfun (x) x)

(assert-null (fboundp 'unused-symbol) "symbol has no function")

(ignore-tests "#' reader syntax is presently broken"
 (assert-equal (fset 'unused-symbol #'myfun) 'myfun "fset returns the function")
 (assert-equal (symbol-function 'unused-symbol) 'myfun "symbol-function")
 (assert-not-null (fboundp 'unused-symbol) "symbol has a function")
)

(provide 'symbol-tests)
