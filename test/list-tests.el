(require 'lisp-tests)

(if verbose-tests
  (message ";;;; LIST TESTS"))


;; dotted lists
(assert-equal (cdr '(1 2)) '(2) "the cdr of a normal list is a list")
(assert-equal (cdr '(1 . 2)) 2  "the cdr of a pair is the second element")
(assert-equal (cdr '(1 2 . 3)) '(2 . 3) "cdr = dotted list")

(assert-null (list) "(list) == nil")
(assert-equal (list 1 2 3) '(1 2 3) "(list 1 2 3) == (1 2 3)")

(assert-null (nconc) "(conc) == nil")
(assert-equal (nconc '(1 2) '(3)) '(1 2 3) "conc of 2 lists joins them")
(assert-equal (nconc '(1 2) 3) '(1 2 . 3) "(conc (1 2) 3) == (1 2 . 3)")
(assert-equal (nconc '(1) '(2) '3) '(1 2 . 3) "(conc (1) (2) 3)  == (1 2 . 3)")

(assert-equal (member '1 '(1 2 3)) '(1 2 3) "(member 1 (1 2 3)) == (1 2 3)")
(assert-null (member '1 nil) "(member 1 nil) == nil")

(assert-equal (reverse '(1 2 3)) '(3 2 1))
(assert-equal (reverse '(1 2)) '(2 1))
(assert-equal (reverse '(1)) '(1))
(assert-equal (reverse nil) nil)

(assert-equal (nreverse '(1 2 3)) '(3 2 1))
(assert-equal (nreverse '(1 2)) '(2 1))
(assert-equal (nreverse '(1)) '(1))
(assert-equal (nreverse nil) nil)

(assert-equal (memq '1 '(1 2 3)) '(1 2 3) "(memq 1 (1 2 3)) == (1 2 3)")
(assert-null (memq '1 nil) "(memq 1 nil) == nil")

(assert-equal (delq '1 '(1 2 3)) '(2 3) "(delq 1 (1 2 3)) == (2 3)")
(assert-null (delq '1 nil) "(delq 1 nil) == nil")


(assert-null (nthcdr 1 '(1)) "(nthcdr 1 '(1)) == nil")

(assert-equal (nthcdr 0 '(1)) '(1) "(nthcdr 0 '(1)) == '(1)")
(assert-equal (nthcdr -1 '(1)) '(1) "(nthcdr -1 '(1)) == '(1)")
(assert-null (nthcdr 2 '(1)) "(nthcdr 2 '(1)) == nil")
(assert-equal (nthcdr 1 '(1 2 3 4)) '(2 3 4) "(nthcdr 1 '(1 2 3 4)) == (2 3 4)")
(assert-equal (nthcdr 1 '(1 . 2)) 2 "(nthcdr 1 '(1 . 2)) == 2")

(assert-equal
    (let ((mylist '(1 2))) (setcar mylist 3) mylist) '(3 2) "setcar modifies in-place")

(assert-equal
    (let ((mylist '(1 2))) (setcdr mylist '(3)) mylist) '(1 3) "setcdr modifies in-place")

(assert-equal
    (let ((mylist '(1 2))) (setcdr mylist 2) mylist) '(1 . 2) "setcdr can create dotted lists")

(provide 'list-tests)