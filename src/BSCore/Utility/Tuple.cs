﻿using System;
using System.Collections;
using System.Collections.Generic;
//
// IStructuralComparable.cs
//
// Authors:
//  Zoltan Varga (vargaz@gmail.com)
//
// Copyright (C) 2009 Novell
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !MOONLIGHT && !NET_4_0 && !MOBILE

namespace System.Collections
{
	public interface IStructuralComparable {
		int CompareTo (object other, IComparer comparer);
	}
}

//
// IStructuralEquatable.cs
//
// Authors:
//  Zoltan Varga (vargaz@gmail.com)
//
// Copyright (C) 2009 Novell
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Collections
{
	public interface IStructuralEquatable {
		bool Equals (object other, IEqualityComparer comparer);

		int GetHashCode (IEqualityComparer comparer);
	}
}

//
// Tuple.cs
//
// Authors:
//  Zoltan Varga (vargaz@gmail.com)
//
// Copyright (C) 2009 Novell
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System
{
	public static class Tuple
	{
		public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>
			(
			 T1 item1,
			 T2 item2,
			 T3 item3,
			 T4 item4,
			 T5 item5,
			 T6 item6,
			 T7 item7,
			 T8 item8) {
			return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> (item1, item2, item3, item4, item5, item6, item7, new Tuple<T8> (item8));
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>
			(
			 T1 item1,
			 T2 item2,
			 T3 item3,
			 T4 item4,
			 T5 item5,
			 T6 item6,
			 T7 item7) {
			return new Tuple<T1, T2, T3, T4, T5, T6, T7> (item1, item2, item3, item4, item5, item6, item7);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>
			(
			 T1 item1,
			 T2 item2,
			 T3 item3,
			 T4 item4,
			 T5 item5,
			 T6 item6) {
			return new Tuple<T1, T2, T3, T4, T5, T6> (item1, item2, item3, item4, item5, item6);
		}

		public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>
			(
			 T1 item1,
			 T2 item2,
			 T3 item3,
			 T4 item4,
			 T5 item5) {
			return new Tuple<T1, T2, T3, T4, T5> (item1, item2, item3, item4, item5);
		}

		public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>
			(
			 T1 item1,
			 T2 item2,
			 T3 item3,
			 T4 item4) {
			return new Tuple<T1, T2, T3, T4> (item1, item2, item3, item4);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3>
			(
			 T1 item1,
			 T2 item2,
			 T3 item3) {
			return new Tuple<T1, T2, T3> (item1, item2, item3);
		}

		public static Tuple<T1, T2> Create<T1, T2>
			(
			 T1 item1,
			 T2 item2) {
			return new Tuple<T1, T2> (item1, item2);
		}

		public static Tuple<T1> Create<T1>
			(
			 T1 item1) {
			return new Tuple<T1> (item1);
		}
	}		
}

//
// Tuples.cs
//
// Authors:
//  Zoltan Varga (vargaz@gmail.com)
//  Marek Safar (marek.safar@gmail.com)
//
// Copyright (C) 2009 Novell
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System
{
	public partial class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>
	{
		public Tuple (T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
		{
			this.item1 = item1;
			this.item2 = item2;
			this.item3 = item3;
			this.item4 = item4;
			this.item5 = item5;
			this.item6 = item6;
			this.item7 = item7;
			this.rest = rest;

			bool ok = true;
			if (!typeof (TRest).IsGenericType)
				ok = false;
			if (ok) {
				Type t = typeof (TRest).GetGenericTypeDefinition ();
				if (!(t == typeof (Tuple<>) || t == typeof (Tuple<,>) || t == typeof (Tuple<,,>) || t == typeof (Tuple<,,,>) || t == typeof (Tuple<,,,,>) || t == typeof (Tuple <,,,,,>) || t == typeof (Tuple<,,,,,,>) || t == typeof (Tuple<,,,,,,,>)))
					ok = false;
			}
			if (!ok)
				throw new ArgumentException ("The last element of an eight element tuple must be a Tuple.");
		}
	}

	/* The rest is generated by the script at the bottom */

	[Serializable]
	public class Tuple<T1> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;

		public Tuple (T1 item1)
		{
			 this.item1 = item1;
		}

		public T1 Item1 {
			get { return item1; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			return comparer.Compare (item1, t.item1);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			return comparer.GetHashCode (item1);
		}

		public override string ToString ()
		{
			return String.Format ("({0})", item1);
		}
	}

	[Serializable]
	public class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;
		T2 item2;

		public Tuple (T1 item1, T2 item2)
		{
			 this.item1 = item1;
			 this.item2 = item2;
		}

		public T1 Item1 {
			get { return item1; }
		}

		public T2 Item2 {
			get { return item2; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1, T2>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			int res = comparer.Compare (item1, t.item1);
			if (res != 0) return res;
			return comparer.Compare (item2, t.item2);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1, T2>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1) &&
				comparer.Equals (item2, t.item2);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			int h = comparer.GetHashCode (item1);
			h = (h << 5) - h + comparer.GetHashCode (item2);
			return h;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1})", item1, item2);
		}
	}

	[Serializable]
	public class Tuple<T1, T2, T3> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;
		T2 item2;
		T3 item3;

		public Tuple (T1 item1, T2 item2, T3 item3)
		{
			 this.item1 = item1;
			 this.item2 = item2;
			 this.item3 = item3;
		}

		public T1 Item1 {
			get { return item1; }
		}

		public T2 Item2 {
			get { return item2; }
		}

		public T3 Item3 {
			get { return item3; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			int res = comparer.Compare (item1, t.item1);
			if (res != 0) return res;
			res = comparer.Compare (item2, t.item2);
			if (res != 0) return res;
			return comparer.Compare (item3, t.item3);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1) &&
				comparer.Equals (item2, t.item2) &&
				comparer.Equals (item3, t.item3);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			int h = comparer.GetHashCode (item1);
			h = (h << 5) - h + comparer.GetHashCode (item2);
			h = (h << 5) - h + comparer.GetHashCode (item3);
			return h;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2})", item1, item2, item3);
		}
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;
		T2 item2;
		T3 item3;
		T4 item4;

		public Tuple (T1 item1, T2 item2, T3 item3, T4 item4)
		{
			 this.item1 = item1;
			 this.item2 = item2;
			 this.item3 = item3;
			 this.item4 = item4;
		}

		public T1 Item1 {
			get { return item1; }
		}

		public T2 Item2 {
			get { return item2; }
		}

		public T3 Item3 {
			get { return item3; }
		}

		public T4 Item4 {
			get { return item4; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			int res = comparer.Compare (item1, t.item1);
			if (res != 0) return res;
			res = comparer.Compare (item2, t.item2);
			if (res != 0) return res;
			res = comparer.Compare (item3, t.item3);
			if (res != 0) return res;
			return comparer.Compare (item4, t.item4);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1) &&
				comparer.Equals (item2, t.item2) &&
				comparer.Equals (item3, t.item3) &&
				comparer.Equals (item4, t.item4);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			int h = comparer.GetHashCode (item1);
			h = (h << 5) - h + comparer.GetHashCode (item2);
			h = (h << 5) - h + comparer.GetHashCode (item3);
			h = (h << 5) - h + comparer.GetHashCode (item4);
			return h;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2}, {3})", item1, item2, item3, item4);
		}
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;
		T2 item2;
		T3 item3;
		T4 item4;
		T5 item5;

		public Tuple (T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			 this.item1 = item1;
			 this.item2 = item2;
			 this.item3 = item3;
			 this.item4 = item4;
			 this.item5 = item5;
		}

		public T1 Item1 {
			get { return item1; }
		}

		public T2 Item2 {
			get { return item2; }
		}

		public T3 Item3 {
			get { return item3; }
		}

		public T4 Item4 {
			get { return item4; }
		}

		public T5 Item5 {
			get { return item5; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			int res = comparer.Compare (item1, t.item1);
			if (res != 0) return res;
			res = comparer.Compare (item2, t.item2);
			if (res != 0) return res;
			res = comparer.Compare (item3, t.item3);
			if (res != 0) return res;
			res = comparer.Compare (item4, t.item4);
			if (res != 0) return res;
			return comparer.Compare (item5, t.item5);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1) &&
				comparer.Equals (item2, t.item2) &&
				comparer.Equals (item3, t.item3) &&
				comparer.Equals (item4, t.item4) &&
				comparer.Equals (item5, t.item5);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			int h = comparer.GetHashCode (item1);
			h = (h << 5) - h + comparer.GetHashCode (item2);
			h = (h << 5) - h + comparer.GetHashCode (item3);
			h = (h << 5) - h + comparer.GetHashCode (item4);
			h = (h << 5) - h + comparer.GetHashCode (item5);
			return h;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2}, {3}, {4})", item1, item2, item3, item4, item5);
		}
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;
		T2 item2;
		T3 item3;
		T4 item4;
		T5 item5;
		T6 item6;

		public Tuple (T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			 this.item1 = item1;
			 this.item2 = item2;
			 this.item3 = item3;
			 this.item4 = item4;
			 this.item5 = item5;
			 this.item6 = item6;
		}

		public T1 Item1 {
			get { return item1; }
		}

		public T2 Item2 {
			get { return item2; }
		}

		public T3 Item3 {
			get { return item3; }
		}

		public T4 Item4 {
			get { return item4; }
		}

		public T5 Item5 {
			get { return item5; }
		}

		public T6 Item6 {
			get { return item6; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5, T6>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			int res = comparer.Compare (item1, t.item1);
			if (res != 0) return res;
			res = comparer.Compare (item2, t.item2);
			if (res != 0) return res;
			res = comparer.Compare (item3, t.item3);
			if (res != 0) return res;
			res = comparer.Compare (item4, t.item4);
			if (res != 0) return res;
			res = comparer.Compare (item5, t.item5);
			if (res != 0) return res;
			return comparer.Compare (item6, t.item6);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5, T6>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1) &&
				comparer.Equals (item2, t.item2) &&
				comparer.Equals (item3, t.item3) &&
				comparer.Equals (item4, t.item4) &&
				comparer.Equals (item5, t.item5) &&
				comparer.Equals (item6, t.item6);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			int h = comparer.GetHashCode (item1);
			h = (h << 5) - h + comparer.GetHashCode (item2);
			h = (h << 5) - h + comparer.GetHashCode (item3);
			h = (h << 5) - h + comparer.GetHashCode (item4);
			h = (h << 5) - h + comparer.GetHashCode (item5);
			h = (h << 5) - h + comparer.GetHashCode (item6);
			return h;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2}, {3}, {4}, {5})", item1, item2, item3, item4, item5, item6);
		}
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;
		T2 item2;
		T3 item3;
		T4 item4;
		T5 item5;
		T6 item6;
		T7 item7;

		public Tuple (T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			 this.item1 = item1;
			 this.item2 = item2;
			 this.item3 = item3;
			 this.item4 = item4;
			 this.item5 = item5;
			 this.item6 = item6;
			 this.item7 = item7;
		}

		public T1 Item1 {
			get { return item1; }
		}

		public T2 Item2 {
			get { return item2; }
		}

		public T3 Item3 {
			get { return item3; }
		}

		public T4 Item4 {
			get { return item4; }
		}

		public T5 Item5 {
			get { return item5; }
		}

		public T6 Item6 {
			get { return item6; }
		}

		public T7 Item7 {
			get { return item7; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			int res = comparer.Compare (item1, t.item1);
			if (res != 0) return res;
			res = comparer.Compare (item2, t.item2);
			if (res != 0) return res;
			res = comparer.Compare (item3, t.item3);
			if (res != 0) return res;
			res = comparer.Compare (item4, t.item4);
			if (res != 0) return res;
			res = comparer.Compare (item5, t.item5);
			if (res != 0) return res;
			res = comparer.Compare (item6, t.item6);
			if (res != 0) return res;
			return comparer.Compare (item7, t.item7);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1) &&
				comparer.Equals (item2, t.item2) &&
				comparer.Equals (item3, t.item3) &&
				comparer.Equals (item4, t.item4) &&
				comparer.Equals (item5, t.item5) &&
				comparer.Equals (item6, t.item6) &&
				comparer.Equals (item7, t.item7);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			int h = comparer.GetHashCode (item1);
			h = (h << 5) - h + comparer.GetHashCode (item2);
			h = (h << 5) - h + comparer.GetHashCode (item3);
			h = (h << 5) - h + comparer.GetHashCode (item4);
			h = (h << 5) - h + comparer.GetHashCode (item5);
			h = (h << 5) - h + comparer.GetHashCode (item6);
			h = (h << 5) - h + comparer.GetHashCode (item7);
			return h;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2}, {3}, {4}, {5}, {6})", item1, item2, item3, item4, item5, item6, item7);
		}
	}

	[Serializable]
	public partial class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IStructuralEquatable, IStructuralComparable, IComparable
	{
		T1 item1;
		T2 item2;
		T3 item3;
		T4 item4;
		T5 item5;
		T6 item6;
		T7 item7;
		TRest rest;

		public T1 Item1 {
			get { return item1; }
		}

		public T2 Item2 {
			get { return item2; }
		}

		public T3 Item3 {
			get { return item3; }
		}

		public T4 Item4 {
			get { return item4; }
		}

		public T5 Item5 {
			get { return item5; }
		}

		public T6 Item6 {
			get { return item6; }
		}

		public T7 Item7 {
			get { return item7; }
		}

		public TRest Rest {
			get { return rest; }
		}

		int IComparable.CompareTo (object obj)
		{
			return ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo (object other, IComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;
			if (t == null) {
				if (other == null) return 1;
				throw new ArgumentException ();
			}

			int res = comparer.Compare (item1, t.item1);
			if (res != 0) return res;
			res = comparer.Compare (item2, t.item2);
			if (res != 0) return res;
			res = comparer.Compare (item3, t.item3);
			if (res != 0) return res;
			res = comparer.Compare (item4, t.item4);
			if (res != 0) return res;
			res = comparer.Compare (item5, t.item5);
			if (res != 0) return res;
			res = comparer.Compare (item6, t.item6);
			if (res != 0) return res;
			res = comparer.Compare (item7, t.item7);
			if (res != 0) return res;
			return comparer.Compare (rest, t.rest);
		}

		public override bool Equals (object obj)
		{
			return ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);
		}

		bool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)
		{
			var t = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;
			if (t == null) {
				if (other == null) return false;
				throw new ArgumentException ();
			}

			return comparer.Equals (item1, t.item1) &&
				comparer.Equals (item2, t.item2) &&
				comparer.Equals (item3, t.item3) &&
				comparer.Equals (item4, t.item4) &&
				comparer.Equals (item5, t.item5) &&
				comparer.Equals (item6, t.item6) &&
				comparer.Equals (item7, t.item7) &&
				comparer.Equals (rest, t.rest);
		}

		public override int GetHashCode ()
		{
			return ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);
		}

		int IStructuralEquatable.GetHashCode (IEqualityComparer comparer)
		{
			int h = comparer.GetHashCode (item1);
			h = (h << 5) - h + comparer.GetHashCode (item2);
			h = (h << 5) - h + comparer.GetHashCode (item3);
			h = (h << 5) - h + comparer.GetHashCode (item4);
			h = (h << 5) - h + comparer.GetHashCode (item5);
			h = (h << 5) - h + comparer.GetHashCode (item6);
			h = (h << 5) - h + comparer.GetHashCode (item7);
			h = (h << 5) - h + comparer.GetHashCode (rest);
			return h;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", item1, item2, item3, item4, item5, item6, item7, rest);
		}
	}

}
	
#endif

#if FALSE

//
// generator script
//

using System;
using System.Text;

public class TupleGen
{
	public static void Main () {
		for (int arity = 1; arity < 9; ++arity) {
			string type_name = GetTypeName (arity);

			Console.WriteLine ("\t[Serializable]");
			Console.Write ("\tpublic {0}class ", arity < 8 ? null : "partial ");
			Console.Write (type_name);
			Console.WriteLine (" : IStructuralEquatable, IStructuralComparable, IComparable");
			Console.WriteLine ("\t{");
			for (int i = 1; i <= arity; ++i)
				Console.WriteLine ("\t\t{0} {1};", GetItemTypeName (i), GetItemName (i));
				
			if (arity < 8) {
				Console.WriteLine ();
				Console.Write ("\t\tpublic Tuple (");
				for (int i = 1; i <= arity; ++i) {
					Console.Write ("{0} {1}", GetItemTypeName (i), GetItemName (i));
					if (i < arity)
						Console.Write (", ");
				}
				Console.WriteLine (")");
				Console.WriteLine ("\t\t{");
				for (int i = 1; i <= arity; ++i)
					Console.WriteLine ("\t\t\t this.{0} = {0};", GetItemName (i));
				Console.WriteLine ("\t\t}");
			}

			for (int i = 1; i <= arity; ++i) {
				Console.WriteLine ();
				Console.WriteLine ("\t\tpublic {0} {1} {{", GetItemTypeName (i),
					System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase (GetItemName (i)));
				Console.Write ("\t\t\tget { ");
				Console.WriteLine ("return {0}; }}", GetItemName (i));
				Console.WriteLine ("\t\t}");
			}

			Console.WriteLine ();
			Console.WriteLine ("\t\tint IComparable.CompareTo (object obj)");
			Console.WriteLine ("\t\t{");
			Console.WriteLine ("\t\t\treturn ((IStructuralComparable) this).CompareTo (obj, Comparer<object>.Default);");
			Console.WriteLine ("\t\t}");
			
			Console.WriteLine ();
			Console.WriteLine ("\t\tint IStructuralComparable.CompareTo (object other, IComparer comparer)");
			Console.WriteLine ("\t\t{");
			Console.WriteLine ("\t\t\tvar t = other as {0};", type_name);
			Console.WriteLine ("\t\t\tif (t == null) {");
			Console.WriteLine ("\t\t\t\tif (other == null) return 1;");
			Console.WriteLine ("\t\t\t\tthrow new ArgumentException ();");
			Console.WriteLine ("\t\t\t}");
			Console.WriteLine ();
			
			for (int i = 1; i < arity; ++i) {
				Console.Write ("\t\t\t");
				if (i == 1)
					Console.Write ("int ");

				Console.WriteLine ("res = comparer.Compare ({0}, t.{0});", GetItemName (i));
				Console.WriteLine ("\t\t\tif (res != 0) return res;");
			}
			Console.WriteLine ("\t\t\treturn comparer.Compare ({0}, t.{0});", GetItemName (arity));
			Console.WriteLine ("\t\t}");			
			
			Console.WriteLine ();
			Console.WriteLine ("\t\tpublic override bool Equals (object obj)");
			Console.WriteLine ("\t\t{");
			Console.WriteLine ("\t\t\treturn ((IStructuralEquatable) this).Equals (obj, EqualityComparer<object>.Default);");
			Console.WriteLine ("\t\t}");
			
			Console.WriteLine ();
			Console.WriteLine ("\t\tbool IStructuralEquatable.Equals (object other, IEqualityComparer comparer)");
			Console.WriteLine ("\t\t{");
			Console.WriteLine ("\t\t\tvar t = other as {0};", type_name);
			Console.WriteLine ("\t\t\tif (t == null) {");
			Console.WriteLine ("\t\t\t\tif (other == null) return false;");
			Console.WriteLine ("\t\t\t\tthrow new ArgumentException ();");
			Console.WriteLine ("\t\t\t}");
			Console.WriteLine ();
			Console.Write ("\t\t\treturn");
			
			for (int i = 1; i <= arity; ++i) {
				if (i == 1)
					Console.Write (" ");
				else
					Console.Write ("\t\t\t\t");

				Console.Write ("comparer.Equals ({0}, t.{0})", GetItemName (i));
				if (i != arity)
					Console.WriteLine (" &&");
				else
					Console.WriteLine (";");
			}
			Console.WriteLine ("\t\t}");
			
			Console.WriteLine ();
			Console.WriteLine ("\t\tpublic override int GetHashCode ()");
			Console.WriteLine ("\t\t{");
			Console.WriteLine ("\t\t\treturn ((IStructuralEquatable) this).GetHashCode (EqualityComparer<object>.Default);");
			Console.WriteLine ("\t\t}");
			
			Console.WriteLine ();
			Console.WriteLine ("\t\tint IStructuralEquatable.GetHashCode (IEqualityComparer comparer)");
			Console.WriteLine ("\t\t{");
			if (arity == 1) {
				Console.WriteLine ("\t\t\treturn comparer.GetHashCode ({0});", GetItemName (arity));
			} else {
				Console.WriteLine ("\t\t\tint h = comparer.GetHashCode ({0});", GetItemName (1));
				for (int i = 2; i <= arity; ++i)
					Console.WriteLine ("\t\t\th = (h << 5) - h + comparer.GetHashCode ({0});", GetItemName (i));
				Console.WriteLine ("\t\t\treturn h;");
			}

			Console.WriteLine ("\t\t}");

			Console.WriteLine ();
			Console.WriteLine ("\t\tpublic override string ToString ()");
			Console.WriteLine ("\t\t{");
			Console.Write ("\t\t\treturn String.Format (\"(");
			for (int i = 1; i <= arity; ++i) {
				Console.Write ("{" + (i - 1) + "}");
				if (i < arity)
					Console.Write (", ");
			}
			Console.Write (")\", ");
			for (int i = 1; i <= arity; ++i) {
				Console.Write (GetItemName (i));
				if (i < arity)
					Console.Write (", ");
			}
			Console.WriteLine (");");
			Console.WriteLine ("\t\t}");

			Console.WriteLine ("\t}\n");
		}
	}

	static string GetTypeName (int arity)
	{
		StringBuilder sb = new StringBuilder ();
		sb.Append ("Tuple<");
		for (int i = 1; i <= arity; ++i) {
			sb.Append (GetItemTypeName (i));
			if (i < arity)
				sb.Append (", ");
		}
		sb.Append (">");
		
		return sb.ToString ();
	}
	
	static string GetItemName (int arity)
	{
		return arity < 8 ? "item" + arity.ToString () : "rest";
	}
	
	static string GetItemTypeName (int arity)
	{
		return arity < 8 ? "T" + arity.ToString () : "TRest";
	}
}

#endif

