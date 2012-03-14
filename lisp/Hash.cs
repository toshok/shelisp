using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class Hash : Object {

		/*
— Function: make-hash-table &rest keyword-args
:test test
This specifies the method of key lookup for this hash table. The default is eql; eq and equal are other alternatives:
eql
Keys which are numbers are “the same” if they are equal, that is, if they are equal in value and either both are integers or both are floating point numbers; otherwise, two distinct objects are never “the same.” 
eq
Any two distinct Lisp objects are “different” as keys. 
equal
Two Lisp objects are “the same,” as keys, if they are equal according to equal.

You can use define-hash-table-test (see Defining Hash) to define additional possibilities for test. 

:weakness weak
The weakness of a hash table specifies whether the presence of a key or value in the hash table preserves it from garbage collection.
The value, weak, must be one of nil, key, value, key-or-value, key-and-value, or t which is an alias for key-and-value. If weak is key then the hash table does not prevent its keys from being collected as garbage (if they are not referenced anywhere else); if a particular key does get collected, the corresponding association is removed from the hash table.

If weak is value, then the hash table does not prevent values from being collected as garbage (if they are not referenced anywhere else); if a particular value does get collected, the corresponding association is removed from the hash table.

If weak is key-and-value or t, both the key and the value must be live in order to preserve the association. Thus, the hash table does not protect either keys or values from garbage collection; if either one is collected as garbage, that removes the association.

If weak is key-or-value, either the key or the value can preserve the association. Thus, associations are removed from the hash table when both their key and value would be collected as garbage (if not for references from weak hash tables).

The default for weak is nil, so that all keys and values referenced in the hash table are preserved from garbage collection. 

:size size
This specifies a hint for how many associations you plan to store in the hash table. If you know the approximate number, you can make things a little more efficient by specifying it this way. If you specify too small a size, the hash table will grow automatically when necessary, but doing that takes some extra time.
The default size is 65. 

:rehash-size rehash-size
When you add an association to a hash table and the table is “full,” it grows automatically. This value specifies how to make the hash table larger, at that time.
If rehash-size is an integer, it should be positive, and the hash table grows by adding that much to the nominal size. If rehash-size is a floating point number, it had better be greater than 1, and the hash table grows by multiplying the old size by that number.

The default value is 1.5. 

:rehash-threshold threshold
This specifies the criterion for when the hash table is “full” (so it should be made larger). The value, threshold, should be a positive floating point number, no greater than 1. The hash table is “full” whenever the actual number of entries exceeds this fraction of the nominal size. The default for threshold is 0.8.
		 */
		[LispBuiltin ("make-hash-table", MinArgs = 0)]
		public static Shelisp.Object Fmake_hash_table (L l, params Shelisp.Object[] keyword_args)
		{
			throw new NotImplementedException ();
		}

		/*
— Function: gethash key table &optional default
This function looks up key in table, and returns its associated value—or default, if key has no association in table.
		*/
		[LispBuiltin ("gethash", MinArgs = 2)]
		public static Shelisp.Object Fgethash (L l, Shelisp.Object key, Shelisp.Object table, Shelisp.Object @default)
		{
			throw new NotImplementedException ();
		}
		
		/*
— Function: puthash key value table
This function enters an association for key in table, with value value. If key already has an association in table, value replaces the old associated value.
		*/
		[LispBuiltin ("puthash", MinArgs = 3)]
		public static Shelisp.Object Fputhash (L l, Shelisp.Object key, Shelisp.Object value, Shelisp.Object table)
		{
			Console.WriteLine ("puthash not implemented");
			return L.Qnil;
		}

		/*
— Function: remhash key table
This function removes the association for key from table, if there is one. If key has no association, remhash does nothing.

Common Lisp note: In Common Lisp, remhash returns non-nil if it actually removed an association and nil otherwise. In Emacs Lisp, remhash always returns nil.

— Function: clrhash table
This function removes all the associations from hash table table, so that it becomes empty. This is also called clearing the hash table.

Common Lisp note: In Common Lisp, clrhash returns the empty table. In Emacs Lisp, it returns nil.

— Function: maphash function table
This function calls function once for each of the associations in table. The function function should accept two arguments—a key listed in table, and its associated value. maphash returns nil.
		 */

		/*
— Function: define-hash-table-test name test-fn hash-fn
This function defines a new hash table test, named name.

After defining name in this way, you can use it as the test argument in make-hash-table. When you do that, the hash table will use test-fn to compare key values, and hash-fn to compute a “hash code” from a key value.

The function test-fn should accept two arguments, two keys, and return non-nil if they are considered “the same.”

The function hash-fn should accept one argument, a key, and return an integer that is the “hash code” of that key. For good results, the function should use the whole range of integer values for hash codes, including negative integers.

The specified functions are stored in the property list of name under the property hash-table-test; the property value's form is (test-fn hash-fn).

— Function: sxhash obj
This function returns a hash code for Lisp object obj. This is an integer which reflects the contents of obj and the other Lisp objects it points to.

If two objects obj1 and obj2 are equal, then (sxhash obj1) and (sxhash obj2) are the same integer.

If the two objects are not equal, the values returned by sxhash are usually different, but not always; once in a rare while, by luck, you will encounter two distinct-looking objects that give the same result from sxhash.

This example creates a hash table whose keys are strings that are compared case-insensitively.

     (defun case-fold-string= (a b)
       (compare-strings a nil nil b nil nil t))
     (defun case-fold-string-hash (a)
       (sxhash (upcase a)))
     
     (define-hash-table-test 'case-fold
       'case-fold-string= 'case-fold-string-hash)
     
     (make-hash-table :test 'case-fold)
Here is how you could define a hash table test equivalent to the predefined test value equal. The keys can be any Lisp object, and equal-looking objects are considered the same key.

     (define-hash-table-test 'contents-hash 'equal 'sxhash)
     
     (make-hash-table :test 'contents-hash)
		*/

		/*
Here are some other functions for working with hash tables.

— Function: hash-table-p table
This returns non-nil if table is a hash table object.

— Function: copy-hash-table table
This function creates and returns a copy of table. Only the table itself is copied—the keys and values are shared.

— Function: hash-table-count table
This function returns the actual number of entries in table.

— Function: hash-table-test table
This returns the test value that was given when table was created, to specify how to hash and compare keys. See make-hash-table (see Creating Hash).

— Function: hash-table-weakness table
This function returns the weak value that was specified for hash table table.

— Function: hash-table-rehash-size table
This returns the rehash size of table.

— Function: hash-table-rehash-threshold table
This returns the rehash threshold of table.

— Function: hash-table-size table
This returns the current nominal size of table.
		 */
	}
}
