(defun factorial (integer)
  (if (= 1 integer) 1
    (* integer (factorial (- integer 1)))))
