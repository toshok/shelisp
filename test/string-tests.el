(require 'lisp-tests)

(if verbose-tests
  (message ";;;; STRING TESTS"))

(assert-equal (concat "1" "2") "12" "simple string concat of 2 strings")

(assert-equal (concat '(?a ?b ?c) '(?d ?e ?f)) "abcdef" "concat 2 lists")
(assert-equal (concat '(?a ?b ?c) nil) "abc" "concat list + nil doesn't error")
(assert-equal (concat '(?a ?b ?c) nil '(?d ?e ?f)) "abcdef" "nil is just ignored")

;; capitalization

(assert-equal (capitalize "hello") "Hello" "single word capitalization")
(assert-equal (capitalize "hello there") "Hello There" "two word capitalization")
(assert-equal (capitalize "hello-there") "Hello-There" "hyphenated two word capitalization")
(assert-equal (capitalize "hello.there") "Hello.There" "random character between two word capitalization")
(ignore-tests "shelisp's version of the character capitalize is broken"
  (assert-equal (capitalize ?a) ?A "capitalize also works on characters")
)

;; substrings

(assert-equal (substring "abcdefg" 0 3) "abc" "substring 0 3")
(assert-equal (substring "abcdefg" -3 -1) "ef" "substring -3 -1")
(assert-equal (substring "abcdefg" -3 nil) "efg" "substring -3 nil")
(assert-equal (substring "abcdefg" -3) "efg" "substring -3")

(assert-equal (substring "abcdefg" -5 6) "cdef" "substring -5 6")

;; matches

(assert-equal (string-match "f" "hi therefbyethere") 8 "string-match of a single character in the middle of a string")
(assert-equal (string-match "there" "hi therefbyethere") 3 "string-match of a larger string appearing multiple times in a string")

(assert-null (string-match "x" "hi therefbyethere") "string-match returns nil for patterns that don't match")

(assert-equal (progn (string-match "there" "hi therefbyethere")
                     (match-data)) '(3 8)
   "match-data returns the extents of the matched string")

(ignore-tests "shelisp doesn't have match-string yet"
  (assert-equal (progn (string-match "there" "hi therefbyethere")
                       (match-string 0 "hi therefbyethere")) "there"
   "match-string returns the actual text matched from the given string")
)

(assert-equal (string-match "-mode\\'" "foo-mode-in-mode") 11 "make sure the \' matches against the end")
(assert-equal (string-match "-mode\\'" "-mode") 0 "make sure it still matches the entire string")

;; replacing

(assert-equal (progn (string-match "there" "hi therefbyethere")
                     (replace-match "foo" nil t "hi therefbyethere")) "hi foofbyethere"
 "replacing a match in a string (fixedcase = nil, literal = t)")

(provide 'string-tests)