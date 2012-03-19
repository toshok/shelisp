using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shelisp {
	public class Hash : Object {

		public enum Weakness {
			None,
			Key,
			Value,
			KeyOrValue,
			KeyAndValue
		}

		public Hash (L l, Shelisp.Object test, Shelisp.Object weakness, Shelisp.Object size, Shelisp.Object rehash_size, Shelisp.Object rehash_threshold)
		{
			this.l = l;
			this.test = test;
			this.weakness = weakness;
			this.size = size;
			this.rehash_size = size;
			this.rehash_threshold = rehash_threshold;

			this.count = 0;
			// map weakness to our enum
			if (L.NILP (weakness))
				weakness_ = Weakness.None;
			else if (weakness.LispEq (L.Qt))
				weakness_ = Weakness.KeyAndValue;
			else if (weakness.LispEq (L.Qkey))
				weakness_ = Weakness.Key;
			else if (weakness.LispEq (L.Qvalue))
				weakness_ = Weakness.Value;
			else if (weakness.LispEq (L.Qkey_or_value))
				weakness_ = Weakness.KeyOrValue;
			else if (weakness.LispEq (L.Qkey_and_value))
				weakness_ = Weakness.KeyAndValue;
			else
				throw new Exception (string.Format ("invalid weakness {0}", weakness));

			table = new Tuple<Shelisp.Object,Shelisp.Object>[(int)((Number)size).boxed];
		}

		public override string ToString ()
		{
			return string.Format ("#<hash-table '{0} {1} {2}/{3} {4}>", test, weakness, count, size, -1/*XXX*/);
		}

		private L l;
		private int count;
		private Tuple<Shelisp.Object,Shelisp.Object>[] table;
		private Shelisp.Object test;
		private Shelisp.Object weakness;
		private Weakness weakness_;
		private Shelisp.Object size;
		private Shelisp.Object rehash_size;
		private Shelisp.Object rehash_threshold;

		/*
		  — Function: make-hash-table &rest keyword-args

		  :test test
		  This specifies the method of key lookup for this
		  hash table. The default is eql; eq and equal are
		  other alternatives:

		  eql
		  Keys which are numbers are “the same” if they are
		  equal, that is, if they are equal in value and
		  either both are integers or both are floating point
		  numbers; otherwise, two distinct objects are never
		  “the same.”
		  eq
		  Any two distinct Lisp objects are “different” as
		  keys.
		  equal
		  Two Lisp objects are “the same,” as keys, if they
		  are equal according to equal.

		  You can use define-hash-table-test (see Defining
		  Hash) to define additional possibilities for test.

		  :weakness weak
		  The weakness of a hash table specifies whether the
		  presence of a key or value in the hash table
		  preserves it from garbage collection.  The value,
		  weak, must be one of nil, key, value, key-or-value,
		  key-and-value, or t which is an alias for
		  key-and-value. If weak is key then the hash table
		  does not prevent its keys from being collected as
		  garbage (if they are not referenced anywhere else);
		  if a particular key does get collected, the
		  corresponding association is removed from the hash
		  table.

		  If weak is value, then the hash table does not
		  prevent values from being collected as garbage (if
		  they are not referenced anywhere else); if a
		  particular value does get collected, the
		  corresponding association is removed from the hash
		  table.

		  If weak is key-and-value or t, both the key and the
		  value must be live in order to preserve the
		  association. Thus, the hash table does not protect
		  either keys or values from garbage collection; if
		  either one is collected as garbage, that removes the
		  association.

		  If weak is key-or-value, either the key or the value
		  can preserve the association. Thus, associations are
		  removed from the hash table when both their key and
		  value would be collected as garbage (if not for
		  references from weak hash tables).

		  The default for weak is nil, so that all keys and
		  values referenced in the hash table are preserved
		  from garbage collection.

		  :size size
		  This specifies a hint for how many associations you
		  plan to store in the hash table. If you know the
		  approximate number, you can make things a little
		  more efficient by specifying it this way. If you
		  specify too small a size, the hash table will grow
		  automatically when necessary, but doing that takes
		  some extra time.  The default size is 65.

		  :rehash-size rehash-size
		  When you add an association to a hash table and the
		  table is “full,” it grows automatically. This value
		  specifies how to make the hash table larger, at that
		  time.  If rehash-size is an integer, it should be
		  positive, and the hash table grows by adding that
		  much to the nominal size. If rehash-size is a
		  floating point number, it had better be greater than
		  1, and the hash table grows by multiplying the old
		  size by that number.

		  The default value is 1.5. 

		  :rehash-threshold threshold
		  This specifies the criterion for when the hash table
		  is “full” (so it should be made larger). The value,
		  threshold, should be a positive floating point
		  number, no greater than 1. The hash table is “full”
		  whenever the actual number of entries exceeds this
		  fraction of the nominal size. The default for
		  threshold is 0.8.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fmake_hash_table (L l, params Shelisp.Object[] keyword_args)
		{
			Shelisp.Object test = L.intern ("eql"); // this is wrong, it's not just numbers, right?
			Shelisp.Object weakness = L.Qnil;
			Shelisp.Object size = new Number (65);
			Shelisp.Object rehash_size = new Number (1.5);
			Shelisp.Object rehash_threshold = new Number (0.8);

			for (int i = 0; i < keyword_args.Length; i += 2) {
				var keyword = keyword_args[i];

				Console.WriteLine (keyword);

				if (i == keyword_args.Length - 1)
					throw new Exception (string.Format ("keyword {0} has no value", keyword));

				var value = keyword_args[i+1];

				if (keyword.LispEq (L.Qtest))
					test = value;
				else if (keyword.LispEq (L.Qweakness))
					weakness = value;
				else if (keyword.LispEq (L.Qsize))
					size = value;
				else if (keyword.LispEq (L.Qrehash_size))
					rehash_size = value;
				else if (keyword.LispEq (L.Qrehash_threshold))
					rehash_threshold = value;
			}

			return new Hash (l, test, weakness, size, rehash_size, rehash_threshold);
		}

		public static int sxhash (Shelisp.Object obj)
		{
			return obj.GetHashCode();
		}

		/*
		  — Function: gethash key table &optional default
		  This function looks up key in table, and returns its associated value—or default, if key has no association in table.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fgethash (L l, Shelisp.Object key, Shelisp.Object table, [LispOptional] Shelisp.Object @default)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			return ((Hash)table).Get (key, @default ?? L.Qnil);
		}
		
		private int GetIndex (Shelisp.Object key)
		{
			long hash = (long)(uint)sxhash (key);
			int start = (int)(hash % table.Length);

			int i = start;

			do {
				if (table[i] == null)
					return i;
				var list = new List (new Shelisp.Object[] { test, table[i].Item1, key });
				Debug.Print ("invoking test with {0}", list);
				var test_rv = list.Eval(l);
				Debug.Print ("test result = {0}", test_rv);
				if (!L.NILP (test_rv))
					return i;
				i = (i + 1) % table.Length;
			} while (i != start);

			throw new Exception ("hash table is full and wasn't rehashed?");
		}

		public Shelisp.Object Get (Shelisp.Object key, Shelisp.Object @default)
		{
			int index = GetIndex (key);
			if (table[index] == null)
				return @default;
			return table[index].Item2;
		}

		/*
		  — Function: puthash key value table
		  This function enters an association for key in table, with value value. If key already has an association in table, value replaces the old associated value.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fputhash (L l, Shelisp.Object key, Shelisp.Object value, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			((Hash)table).Put (key, value);

			return value;
		}

		public void Put (Shelisp.Object key, Shelisp.Object value)
		{
			int index = GetIndex (key);
			bool adding = false;

			if (table[index] == null)
				adding = true;

			table[index] = Tuple.Create<Shelisp.Object,Shelisp.Object>(key, value);

			if (adding) {
				// up the count and maybe rehash
				count ++;
			}
		}

		/*
		  — Function: remhash key table

		  This function removes the association for key from
		  table, if there is one. If key has no association,
		  remhash does nothing.

		  Common Lisp note: In Common Lisp, remhash returns
		  non-nil if it actually removed an association and
		  nil otherwise. In Emacs Lisp, remhash always returns
		  nil.

		*/
		[LispBuiltin]
		public static Shelisp.Object Fremhash (L l, Shelisp.Object key, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			((Hash)table).Remove (key);

			return L.Qnil;
		}

		public void Remove (Shelisp.Object key)
		{
			int index = GetIndex (key);

			if (table[index] == null)
				return;

			table[index] = null;
			count --;
		}

		/*
		  — Function: clrhash table

		  This function removes all the associations from hash
		  table table, so that it becomes empty. This is also
		  called clearing the hash table.

		  Common Lisp note: In Common Lisp, clrhash returns
		  the empty table. In Emacs Lisp, it returns nil.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fclrhash (L l, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			((Hash)table).Clear ();

			return L.Qnil;
		}

		public void Clear ()
		{
			table = new Tuple<Shelisp.Object,Shelisp.Object>[(int)((Number)size).boxed];
			count = 0;
		}

		/*
		  — Function: maphash function table

		  This function calls function once for each of the
		  associations in table. The function function should
		  accept two arguments—a key listed in table, and its
		  associated value. maphash returns nil.
		*/

		/*
		  — Function: define-hash-table-test name test-fn hash-fn

		  This function defines a new hash table test, named
		  name.

		  After defining name in this way, you can use it as
		  the test argument in make-hash-table. When you do
		  that, the hash table will use test-fn to compare key
		  values, and hash-fn to compute a “hash code” from a
		  key value.

		  The function test-fn should accept two arguments,
		  two keys, and return non-nil if they are considered
		  “the same.”

		  The function hash-fn should accept one argument, a
		  key, and return an integer that is the “hash code”
		  of that key. For good results, the function should
		  use the whole range of integer values for hash
		  codes, including negative integers.

		  The specified functions are stored in the property
		  list of name under the property hash-table-test; the
		  property value's form is (test-fn hash-fn).
		*/

		/*
		  — Function: sxhash obj

		  This function returns a hash code for Lisp object
		  obj. This is an integer which reflects the contents
		  of obj and the other Lisp objects it points to.

		  If two objects obj1 and obj2 are equal, then (sxhash
		  obj1) and (sxhash obj2) are the same integer.

		  If the two objects are not equal, the values
		  returned by sxhash are usually different, but not
		  always; once in a rare while, by luck, you will
		  encounter two distinct-looking objects that give the
		  same result from sxhash.

		  This example creates a hash table whose keys are
		  strings that are compared case-insensitively.

		  (defun case-fold-string= (a b)
		  (compare-strings a nil nil b nil nil t))
		  (defun case-fold-string-hash (a)
		  (sxhash (upcase a)))
     
		  (define-hash-table-test 'case-fold
		  'case-fold-string= 'case-fold-string-hash)
     
		  (make-hash-table :test 'case-fold)

		  Here is how you could define a hash table test
		  equivalent to the predefined test value equal. The
		  keys can be any Lisp object, and equal-looking
		  objects are considered the same key.

		  (define-hash-table-test 'contents-hash 'equal 'sxhash)
     
		  (make-hash-table :test 'contents-hash)
		*/
		[LispBuiltin]
		public static Shelisp.Object Fsxhash (L l, Shelisp.Object obj)
		{
			return new Number (sxhash (obj));
		}		

		// Here are some other functions for working with hash tables.

		/*
		  — Function: hash-table-p table
		  This returns non-nil if table is a hash table object.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fhash_table_p (L l, Shelisp.Object table)
		{
			return (table is Hash) ? L.Qt : L.Qnil;
		}		

		/*
		  — Function: copy-hash-table table
		  This function creates and returns a copy of table. Only the table itself is copied—the keys and values are shared.
		*/

		/*
		  — Function: hash-table-count table
		  This function returns the actual number of entries in table.
		*/

		/*
		  — Function: hash-table-test table

		  This returns the test value that was given when
		  table was created, to specify how to hash and
		  compare keys. See make-hash-table (see Creating
		  Hash).
		*/
		[LispBuiltin]
		public static Shelisp.Object Fhash_table_test (L l, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);
			
			return ((Hash)table).test;
		}		

		/*
		  — Function: hash-table-weakness table
		  This function returns the weak value that was specified for hash table table.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fhash_table_weakness (L l, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			return ((Hash)table).weakness;
		}		

		/*
		  — Function: hash-table-rehash-size table
		  This returns the rehash size of table.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fhash_table_rehash_size (L l, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			return ((Hash)table).rehash_size;
		}

		/*
		  — Function: hash-table-rehash-threshold table
		  This returns the rehash threshold of table.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fhash_table_rehash_threshold (L l, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			return ((Hash)table).rehash_threshold;
		}

		/*
		  — Function: hash-table-size table
		  This returns the current nominal size of table.
		*/
		[LispBuiltin]
		public static Shelisp.Object Fhash_table_size (L l, Shelisp.Object table)
		{
			if (!(table is Hash))
				throw new WrongTypeArgumentException ("hashp", table);

			return ((Hash)table).size;
		}
	}
}
