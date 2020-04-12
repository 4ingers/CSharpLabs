using ExceptionSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PolynomialSpace {
  public class Polynomial<T> : ICloneable, IComparable<Polynomial<T>>, IEnumerable<T>, IEquatable<Polynomial<T>> {

    private readonly SortedDictionary<int, T> monoms;


    public Polynomial() => monoms = new SortedDictionary<int, T>();

    public Polynomial(SortedDictionary<int, T> dict) {
      if (dict is null)
        throw new ArgumentNullException(nameof(dict));

      // Check negative powers
      var negatives = dict.Keys.Where(power => power < 0).Count();
      if (negatives != 0)
        throw new ArgumentException("At least one negative degree was found");

      monoms = new SortedDictionary<int, T>(dict);
    }

    public Polynomial(Polynomial<T> polynomial) : this(polynomial.monoms) {}

    public void ActionOverData(Action<int, T> action) {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (monoms.Any())
        foreach (var monom in monoms)
          action(monom.Key, monom.Value);
    }

    public void AddMonom(int degree, T coefficient) {
      if (this is null || coefficient is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (degree < 0)
        throw new ArgumentException("Degree is less then 0");

      if (monoms.ContainsKey(degree)) {
        try {
          monoms[degree] = (dynamic)monoms[degree] + coefficient;
        }
        catch (OperationException) {
          throw new RankException("Different sizes");
        }
      }
      else {
        monoms.Add(degree, coefficient);
      }

      dynamic monomInPolynomial = (dynamic)monoms[degree];
      if (monomInPolynomial == 0)
        monoms.Remove(degree);
    }

    public static Polynomial<T> Add(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Polynomial<T> result = new Polynomial<T>(left);
      right.ActionOverData((power, coefficient) => result.AddMonom(power, coefficient));
      return result;
    }

    public static Polynomial<T> Subtract(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Polynomial<T> result = new Polynomial<T>();
      right.ActionOverData((power, coefficient) => result.AddMonom(power, (dynamic)coefficient * -1.0));
      return left + result;
    }

    public static Polynomial<T> Multiply(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Polynomial<T> result = new Polynomial<T>();
      left.ActionOverData((leftPower, leftCoefficient) => {
        right.ActionOverData((rightPower, rightCoefficient) => {
          result.AddMonom(leftPower + rightPower, (dynamic)leftCoefficient * rightCoefficient);
        });
      });
      return result;
    }

    public static Polynomial<T> Multiply(Polynomial<T> left, double right) {
      if (left is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Polynomial<T> result = new Polynomial<T>();
      left.ActionOverData((leftPower, leftCoefficient) => result.AddMonom(leftPower, (dynamic)leftCoefficient * right));
      return result;
    }

    public static Polynomial<T> Divide(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Polynomial<T> dividend = new Polynomial<T>(left);
      Polynomial<T> result = new Polynomial<T>();

      var rightLast = right.monoms.Last();
      
      while (true) {
        if (!dividend.monoms.Any())
          return result;

        var dividendLast = dividend.monoms.Last();

        if (dividendLast.Key < rightLast.Key)
          return result;
        else {
          foreach (var rightMonom in right.monoms) {
            if (rightMonom.Key == rightLast.Key)
              break;
            dividend.AddMonom(rightMonom.Key + dividendLast.Key - rightLast.Key,
                              (dynamic)rightMonom.Value * dividendLast.Value / rightLast.Value * (-1));
          }
          result.AddMonom(dividendLast.Key - rightLast.Key, (dynamic)dividendLast.Value / rightLast.Value);
          dividend.monoms.Remove(dividendLast.Key);
        }
      }
    }

    public static Polynomial<T> Modulo(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Polynomial<T> result = new Polynomial<T>(left);

      var rightLast = right.monoms.Last();

      while (true) {
        if (!result.monoms.Any())
          return result;

        var resultLast = result.monoms.Last();

        if (resultLast.Key < rightLast.Key)
          return result;
        else {
          foreach (var rightMonom in right.monoms) {
            if (rightMonom.Key == rightLast.Key)
              break;
            result.AddMonom(rightMonom.Key + resultLast.Key - rightLast.Key,
                            (dynamic)rightMonom.Value * resultLast.Value / rightLast.Value * (-1));
          }
          result.monoms.Remove(resultLast.Key);
        }
      }
    }

    public T Value(T x) {
      if (x is null)
        throw new ArgumentNullException(nameof(x));

      dynamic result = (dynamic)x - x;

      if (monoms.Count == 0)
        return result;

      dynamic product;

      foreach (var monom in monoms) {
        if (0 != monom.Key)
          product = x;
        else
          product = (dynamic)x - x + 1;

        for (int i = monom.Key - 1; i > 0; i--) 
          product = product * monom.Value;
        result = result + product * monom.Value;
      }
      return result;
    }

    public static Polynomial<T> Composition(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Polynomial<T> result = new Polynomial<T>();

      foreach (var leftMonom in left.monoms) {
        if (leftMonom.Key > 0) {
          Polynomial<T> powersPolynomial = new Polynomial<T>(right);
          Polynomial<T> coefficientsPolynomial = new Polynomial<T>();

          for (int i = leftMonom.Key - 1; i > 0; i--)
            powersPolynomial = powersPolynomial * right;
          coefficientsPolynomial.AddMonom(0, leftMonom.Value);
          result = result + powersPolynomial * coefficientsPolynomial;
        }
        else
          result.AddMonom(0, leftMonom.Value);
      }
      return result;
    }


    public object Clone() {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return new Polynomial<T>(this); 
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is null)
        return false;

      return Equals(obj as Polynomial<T>);
    }

    public bool Equals(Polynomial<T> other) {
      if (other! is Polynomial<T>)
        return false;

      return CompareTo(other) == 0;
    }

    public int CompareTo(object obj) {
      if (obj is null)
        throw new ArgumentNullException("Object is null");

      return CompareTo(obj as Polynomial<T>);
    }

    public int CompareTo(Polynomial<T> other) {
      if (!(other is Polynomial<T>))
        throw new ArgumentException("Object is not a Polynomial");

      if (!monoms.Any() && !other.monoms.Any())
        return 0;
      else if (!monoms.Any())
        return -1;
      else if (!other.monoms.Any())
        return 1;

      var e1 = monoms.Reverse().GetEnumerator();
      var e2 = other.monoms.Reverse().GetEnumerator();

      while (e1.MoveNext() && e2.MoveNext()) {
        var leftMonom = e1.Current;
        var rightMonom = e2.Current;

        if (leftMonom.Key > rightMonom.Key)
          return 1;
        else if (leftMonom.Key < rightMonom.Key)
          return -1;

        if (Math.Abs((dynamic)leftMonom.Value - rightMonom.Value) > ConstantsSpace.Constants.Eps) {
          if ((dynamic)leftMonom.Value > rightMonom.Value)
            return 1;
          else
            return -1;
        }
      }
      return monoms.Count().CompareTo(other.monoms.Count());
    }

    public IEnumerator<T> GetEnumerator() {
      foreach (var monom in monoms) {
        yield return monom.Value;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() {
      return !monoms.Any() ? "0x0" : string.Join(", ", monoms.Select(x => $"{x.Value}x{x.Key}").ToArray());
    }


    //  --- Operators ---
    public static Polynomial<T> operator +(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Add(left, right);
    }

    public static Polynomial<T> operator -(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Subtract(left, right);
    }

    public static Polynomial<T> operator *(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Multiply(left, right);
    }

    public static Polynomial<T> operator *(Polynomial<T> left, double right) {
      if (left is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Multiply(left, right);
    }

    public static Polynomial<T> operator /(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Divide(left, right);
    }

    public static Polynomial<T> operator %(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Modulo(left, right);
    }

    public static bool operator ==(Polynomial<T> left, Polynomial<T> right) => left.CompareTo(right) == 0;

    public static bool operator !=(Polynomial<T> left, Polynomial<T> right) => left.CompareTo(right) != 0;

    public static bool operator >(Polynomial<T> left, Polynomial<T> right) => left.CompareTo(right) == 1;

    public static bool operator >=(Polynomial<T> left, Polynomial<T> right) => left.CompareTo(right) >= 0;

    public static bool operator <(Polynomial<T> left, Polynomial<T> right) => left.CompareTo(right) == -1;

    public static bool operator <=(Polynomial<T> left, Polynomial<T> right) => left.CompareTo(right) <= 0;
  }
}
