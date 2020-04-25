using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

//TODO Null-checks

namespace Lab11Space {
  class Vector<T> : ICloneable, IEquatable<Vector<T>>, IComparable<Vector<T>>, IComparable where T : new() {

    // --------------SECTION: Fields ---------- //
    private T[] coordinates;


    // --------------SECTION: Properties ---------- //
    public int Dimension { get => coordinates.Length; }


    // --------------SECTION: Constructors---------- //
    private Vector() { }
    public Vector(int dimension) {
      if (dimension < 1)
        throw new RankException();
      coordinates = new T[dimension];
    }

    public Vector(T[] array) => coordinates = array;


    // --------------SECTION: Indexer---------- //
    public T this[int i] {
      get {
        if (i < 0 || i > Dimension) {
          throw new IndexOutOfRangeException("Requested vector index is out of range!");
        }
        return coordinates[i];
      }
      set {
        coordinates[i] = value;
      }
    }

    // --------------SECTION: Cast operators---------- //
    public static implicit operator Vector<T>(T[] array) => new Vector<T>(array);
    public static implicit operator T[](Vector<T> vector) => vector.coordinates.Clone() as T[];

    // --------------SECTION: Formatting-------------- //
    public override string ToString() {
      //StringBuilder sb = new StringBuilder("<");
      //foreach (var value in coordinates) 
      //  sb.Append($"{value.ToString()}, ");
      //sb.Append($"{coordinates.Last().ToString()}>");
      //return sb.ToString();
      return $"<{string.Join(", ", coordinates)}>";
    }


    // --------------SECTION: Implementations of interfaces-------------- //
    /// ICloneable
    public object Clone() => new Vector<T>(coordinates.Clone() as T[]);


    /// IEquatable
    public override bool Equals(object obj) {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is null)
        return false;
      return Equals(obj as Vector<T>);
    }

    public bool Equals(Vector<T> value) {
      if (value! is Vector<T>)
        return false;

      if (Dimension != value.Dimension)
        return false;

      for (int i = 0; i < Dimension; i++) {
        if (!coordinates[i].Equals(value.coordinates[i]))
          return false;
      }
      return true;
    }


    /// IComparable
    public int CompareTo(object obj) {
      if (obj is null)
        throw new ArgumentNullException("Object is null");
      return CompareTo(obj as Vector<T>);
    }

    public int CompareTo(Vector<T> other) {
      if (!(other is Complex))
        throw new ArgumentException("Object is not a Complex");

      T difference = (dynamic)this.GetNormSquare() - other.GetNormSquare();
      if ((dynamic)difference < 0)
        difference = -(dynamic)difference;

      if ((dynamic)difference < Constants.Eps)
        return 0;
      else if ((dynamic)difference > 0)
        return 1;
      else
        return -1;
    }


    // --------------SECTION: Comparison Operator Overloading -------------- //
    public static bool operator ==(Vector<T> lhs, Vector<T> rhs) => lhs.CompareTo(rhs) == 0;
    public static bool operator !=(Vector<T> lhs, Vector<T> rhs) => !(lhs == rhs);
    public static bool operator <(Vector<T> lhs, Vector<T> rhs) => lhs.CompareTo(rhs) < 0;
    public static bool operator <=(Vector<T> lhs, Vector<T> rhs) => lhs.CompareTo(rhs) <= 0;
    public static bool operator >(Vector<T> lhs, Vector<T> rhs) => lhs.CompareTo(rhs) > 0;
    public static bool operator >=(Vector<T> lhs, Vector<T> rhs) => lhs.CompareTo(rhs) >= 0;


    // --------------SECTION: Alternates for ariphmetic operators------------ //
    public static Vector<T> Negate(Vector<T> value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      Vector<T> result = new Vector<T>(value.Dimension);
      for (int i = 0; i < value.Dimension; i++)
        result[i] = -(dynamic)value[i];
      return result;
    }

    public static Vector<T> Sum(Vector<T> lhs, Vector<T> rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));

      if (lhs.Dimension != rhs.Dimension)
        throw new RankException();

      Vector<T> result = new Vector<T>(lhs.Dimension);
      for (int i = 0; i < lhs.Dimension; i++) 
        result[i] = (dynamic)lhs[i] + rhs[i];
      return result;
    }

    public static Vector<T> Sum(Vector<T> lhs, T rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));

      Vector<T> result = new Vector<T>(lhs.Dimension);
      for (int i = 0; i < lhs.Dimension; i++)
        result[i] = (dynamic)lhs[i] + rhs;
      return result;
    }

    public static Vector<T> Subtract(Vector<T> lhs, Vector<T> rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));

      if (lhs.Dimension != rhs.Dimension)
        throw new RankException();

      Vector<T> result = new Vector<T>(lhs.Dimension);
      for (int i = 0; i < lhs.Dimension; i++) 
        result[i] = (dynamic)lhs[i] - rhs[i];
      return result;
    }

    public static Vector<T> Subtract(Vector<T> lhs, T rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));

      Vector<T> result = new Vector<T>(lhs.Dimension);
      for (int i = 0; i < lhs.Dimension; i++) 
        result[i] = (dynamic)lhs[i] - rhs;
      return result;
    }

    public static Vector<T> Multiply(Vector<T> lhs, T rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));

      Vector<T> result = new Vector<T>(lhs.Dimension);
      for (int i = 0; i < lhs.Dimension; i++)
        result[i] = (dynamic)lhs[i] * rhs;
      return result;
    }

    public static Vector<T> Divide(Vector<T> lhs, T rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));

      Vector<T> result = new Vector<T>(lhs.Dimension);
      for (int i = 0; i < lhs.Dimension; i++) 
        result[i] = (dynamic)lhs[i] / rhs;
      return result;
    }


    // --------------SECTION: Arithmetic Operator(unary) Overloading -------------- //
    public static Vector<T> operator -(Vector<T> value) => Negate(value);


    // --------------SECTION: Arithmetic Operator(binary) Overloading -------------- //       
    public static Vector<T> operator +(Vector<T> lhs, Vector<T> rhs) => Sum(lhs, rhs);
    public static Vector<T> operator -(Vector<T> lhs, Vector<T> rhs) => Subtract(lhs, rhs);
    public static Vector<T> operator *(Vector<T> lhs, T rhs) => Multiply(lhs, rhs);
    public static Vector<T> operator *(T lhs, Vector<T> rhs) => Multiply(rhs, lhs);
    public static Vector<T> operator /(Vector<T> lhs, T rhs) => Divide(lhs, rhs);


    // --------------SECTION: Other arithmetic operations  -------------- //
    public static T DotProduct(Vector<T> lhs, Vector<T> rhs) {
      T result = (dynamic)0;
      for (int i = 0; i < lhs.Dimension; i++)
        result = (dynamic)result + ((dynamic)lhs[i] * Complex.Conjugate(rhs[i] as Complex));
      return result;
    }

    public double GetNorm() {
      Complex[] norm = Complex.Sqrt(GetNormSquare());
      return Math.Sqrt(norm[0].Magnitude * norm[0].Magnitude + norm[1].Magnitude * norm[1].Magnitude);
    }

    public Complex GetNormSquare() {
      Complex result = 0;
      for (int i = 0; i < Dimension; i++) {
        result = result + (dynamic)coordinates[i] * coordinates[i];
      }
      return result;
    }

    public static Vector<T> Normalized(Vector<T> vector) {
      double norm = vector.GetNorm();
      var result = vector.Clone() as Vector<T>;
      if (Complex.Abs(norm) < Constants.Eps)
        throw new Exception("Tried to normalize a vector with norm of zero");

      for (int i = 0; i < vector.Dimension; i++) {
        result.coordinates[i] = (dynamic)result.coordinates[i] / norm;
      }
      return result;
    }

    public static Vector<T> CrossProduct(Vector<T> lhs, Vector<T> rhs) {
      if (lhs.Dimension != 3 || rhs.Dimension != 3)
        throw new ArgumentException("Vector must be 3-dimensional");

      Vector<T> result = new Vector<T>(3);
      result[0] = (dynamic)lhs[1] * rhs[2] - (dynamic)lhs[2] * rhs[1];
      result[1] = (dynamic)lhs[2] * rhs[0] - (dynamic)lhs[0] * rhs[2];
      result[2] = (dynamic)lhs[0] * rhs[1] - (dynamic)lhs[1] * rhs[0];

      return result;
    }

    public static IEnumerable<Vector<T>> Orthogonalize(IEnumerable<Vector<T>> system) {
      var tmpVector = new Vector<T>();
      var sumVector = new Vector<T>();
      var b_j = new List<Vector<T>>();

      for (int i = 0; i < system.Count(); i++) {
        var system_i = system.ElementAt(i);
        for (int j = 0; j < i; j++) {
          T k = (dynamic)DotProduct(system_i, b_j[j]) / DotProduct(b_j[j], b_j[j]);
          sumVector = Sum(sumVector, Multiply(b_j[j], k));
        }
        b_j[i] = Subtract(system_i, sumVector);
        sumVector = tmpVector;
      }
      return b_j;
    }

  }
}



























//public static double Angle(Vector v1, Vector v2) {
//  double result = 0.0, norm1 = 0.0, norm2 = 0.0;
//  for (int i = 0; i < v1.dim; i++) {
//    result += v1[i] * v2[i];
//    norm1 += v1[i] * v1[i];
//    norm2 += v2[i] * v2[i];
//  }
//  result = result / Math.Sqrt(norm1) / Math.Sqrt(norm2);
//  return Math.Acos(result);
//}