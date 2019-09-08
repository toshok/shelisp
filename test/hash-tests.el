(require 'lisp-tests)

(if verbose-tests
  (message ";;;; HASH TESTS"))

(assert-eql (sxhash "hi") (sxhash "hi") "sxhash returns the same thing for different string objects that are equal")
(assert-eql (sxhash 10) (sxhash 10) "sxhash returns the same thing for different number objects that are equal")
(assert-eql (sxhash '(1)) (sxhash 1) "(sxhash '(1)) == (sxhash 1)")
(assert-eql (sxhash '(1 2 3)) (sxhash '(1 2 3)) "sxhash returns the same thing for different list objects that are equal")
(assert-eql (sxhash '[1 2 3]) (sxhash '[1 2 3]) "sxhash returns the same thing for different vector objects that are equal")
(ignore-tests "our sxhash uses .net's GetHashCode"
  (assert-eql (sxhash '[1]) (+ 20 (sxhash 1)) "(sxhash '[1]) == (sxhash 1) + 20"))
(assert-eql (sxhash 'hi) (sxhash (symbol-name 'hi)) "sxhash returns the same thing for SYM and (symbol-name SYM)")

(defun test-puthash-returns-same-value (value type)
  (let ((test-hash (make-hash-table))
        (test-hash-value value))
    (assert-eq (puthash 'test-hash-value test-hash-value test-hash) test-hash-value (format "puthash returns the same object you pass in (%s)" type))))

(test-puthash-returns-same-value "hi" "value is string")
(test-puthash-returns-same-value 5 "value is number")
(test-puthash-returns-same-value '(1 2 3 4) "value is list")
(test-puthash-returns-same-value (make-vector 65 0) "value is vector")


(defun test-retrieve-with-different-key (putkey getkey value expected-pass type)
  (let ((test-hash (make-hash-table))
        (test-hash-value value))
    (puthash putkey test-hash-value test-hash)
    (if expected-pass
        (assert-eq test-hash-value (gethash getkey test-hash) (format "puthash and gethash return the same value using different (but equal) keys (keys = %s)" type))
        (assert-not-eq test-hash-value (gethash getkey test-hash) (format "puthash and gethash do not return the same value using different (but equal) keys (keys = %s)" type)))))

(test-retrieve-with-different-key "hi" "hi" '(v a l u e) nil "string")
(test-retrieve-with-different-key 5 5 '(v a l u e) t "int")

(provide 'hash-tests)
