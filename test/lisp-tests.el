;; unit tests in elisp

;; ideally we'd be able to use macros here, but in emacs backquote and friends are defined in .el files
;; that are loaded as part of the emacs loadup/dump.  so we need to turn these into functions:

;; (defmacro assert-signals (expr signal-type)
;;   `(condition-case sig
;;n      (progn
;;         ,expr
;;         (message "FAIL")
;;         nil)
;;     (,signal-type (message "PASS") t)))

;; (defmacro assert-is (expr predicate)
;;   `(condition-case sig
;;       (if (,predicate ,expr)
;;         (progn (message "PASS") t)
;;         (progn (message "FAIL") nil))
;;     (t (message "failex") nil)))

;; (defmacro assert-is-not (expr predicate)
;;   `(condition-case sig
;;       (if (,predicate ,expr)
;;         (progn (message "FAIL") nil)
;;         (progn (message "PASS") t))
;;     (t (message "failex") nil)))

;; (defmacro assert-null (expr)
;;   `(condition-case sig
;;       (if (null ,expr)
;;         (progn (message "PASS") t)
;;         (progn (message "FAIL") nil))
;;     (t (message "failex") nil)))

(defun test-failure (messsage)
  (if verbose-tests
      (message "%d) FAILED %s" number-of-tests message)
     (princ "F"))
  (setq failed-tests (cons '(number-of-tests message) failed-tests))
  (setq number-of-tests (1+ number-of-tests))
  nil)

(defun test-success (messsage)
  (if verbose-tests
     (message "%d) PASSED %s" number-of-tests message)
    (princ "."))
  (setq number-of-tests (1+ number-of-tests))
  t)

(defun assert-signals (expr signal-type)
   (condition-case sig
     (progn
       expr
       (message "FAIL")
       nil)
    (signal-type (message "PASS") t)))

(defmacro assert-is (expr predicate &optional desc)
  (condition-case sig
      (if (predicate expr)
        (test-success desc)
       (test-failure desc))
   (t (message "failex") nil)))

(defun assert-null (expr &optional desc)
  (condition-case sig
      (if (null expr)
        (test-success desc)
       (test-failure desc))
    (t (message "failex") nil)))

(defun assert-not-null (expr &optional desc)
  (condition-case sig
      (if (null expr)
        (test-failure desc)
       (test-success desc)) 
    (t (message "failex") nil)))


(defun assert-eq (expr val &optional desc)
  (condition-case sig
      (if (eq expr val)
        (test-success desc)
       (test-failure desc))
   (t (message "failex") nil)))

(defun assert-not-eq (expr val &optional desc)
  (condition-case sig
      (if (eq expr val)
        (test-failure desc)
       (test-success desc))
   (t (message "failex") nil)))

(defun assert-eql (expr val &optional desc)
  (condition-case sig
      (if (eql expr val)
        (test-success desc)
       (test-failure desc))
   (t (message "failex") nil)))

(defun assert-not-eql (expr val &optional desc)
  (condition-case sig
      (if (eql expr val)
        (test-failure desc)
       (test-success desc))
   (t (message "failex") nil)))

(defun assert-equal (expr val &optional desc)
  (condition-case sig
      (if (equal expr val)
        (test-success desc)
       (test-failure desc))
   (t (message "failex") nil)))

(defun assert-not-equal (expr val &optional desc)
  (condition-case sig
      (if (equal expr val)
        (test-failure desc)
       (test-success desc))
   (t (message "failex") nil)))

(provide 'lisp-tests)