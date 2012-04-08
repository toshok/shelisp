
(if (not (boundp 'verbose-tests))
  (progn
    (setq running-on-emacs t)
    ;; we need to do these here since emacs doesn't have the TestRunner hooks
    (setq verbose-tests t)
    (setq number-of-tests 0)
    (setq number-of-ignored-tests 0)
    (setq failed-tests nil)
    (setq load-path (cons default-directory load-path))
))

(require 'eq-tests)
(require 'symbol-tests)
(require 'list-tests)
(require 'string-tests)
(require 'hash-tests)
(require 'obarray-tests)

(if (boundp 'running-on-emacs)
  (message "ran %d tests, %d failures, %d ignored tests" number-of-tests (if (null failed-tests) 0 (length failed-tests)) number-of-ignored-tests))
