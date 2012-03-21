
(require 'lisp-tests)

(if verbose-tests
  (message ";;;; OBARRAY TESTS"))

(assert-null (intern-soft "frazzle") "an uninterned symbol is not in the obarray")
(assert-not-null (intern "frazzle")  "interning the symbol")
(assert-not-null (intern-soft "frazzle") "after interning, it's in the obarray")
(assert-not-null (unintern "frazzle") "uninterning returns non-nil if it's in the obarray")
(assert-null (intern-soft "frazzle") "after uninterning, the symbol is no longer in the obarray")

(assert-null (unintern "frazzle") "uninterning returns nil if it's not in the obarray")

(provide 'obarray-tests)