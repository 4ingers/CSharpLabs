using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using ExceptionSpace;
using MatrixSpace;
using System.Collections;

namespace PolynomialSpace {
  public class Polynomial<T> : ICloneable, IComparable, IEnumerable<T> {
    private readonly SortedDictionary<int, T> monoms;

    public Polynomial() => monoms = new SortedDictionary<int, T>();

    public Polynomial(SortedDictionary<int, T> dict) {
      if (dict is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      var negatives = dict.Keys.Where(power => power < 0).Count();
      if (negatives != 0)
        throw new ArgumentException("At least one negative degree was found");

      monoms = new SortedDictionary<int, T>(dict);
    }

    public Polynomial(Polynomial<T> polynomial) : this(polynomial.monoms) {
      if (polynomial is null || polynomial.monoms is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      var negatives = polynomial.monoms.Keys.Where(power => power < 0).Count();
      if (negatives != 0)
        throw new ArgumentException("At least one negative degree was found");

      monoms = new SortedDictionary<int, T>(polynomial.monoms);
    }

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

      //? Is it correct?
      var rightLast = right.monoms.Last();
      
      while (true) {
        if (!dividend.monoms.Any())
          return result;

        var dividendLast = dividend.monoms.Last();

        if (dividendLast.Key < rightLast.Key)
          return result;
        else {
          //? Simplify?
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

      //? Is it correct?
      var rightLast = right.monoms.Last();

      while (true) {
        if (!result.monoms.Any())
          return result;

        var resultLast = result.monoms.Last();

        if (resultLast.Key < rightLast.Key)
          return result;
        else {
          //? Simplify?
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

    public T ValueMatrix<T>(T x) where T : Matrix {
      if (this is null || x is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      dynamic result;
      result = new Matrix((x as Matrix).Size, 0);

      if (monoms.Count == 0)
        return result;

      dynamic product;

      foreach (var monom in monoms) {
        if (0 != monom.Key)
          product = x;
        else
          product = new Matrix((monom.Value as Matrix).Size, 1);

        for (int i = monom.Key - 1; i > 0; i--) 
          product = product * monom.Value;
        result = result + product * monom.Value;
      }
      return result;
    }

    public T ValueStruct<T>(T x) where T : struct {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      dynamic result;
      result = 0;

      if (monoms.Count == 0)
        return result;

      dynamic product;

      foreach (var monom in monoms) {
        if (0 != monom.Key)
          product = x;
        else 
          product = 1;

        for (int i = monom.Key - 1; i > 0; i--)
          product = product * x;
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

    public int CompareTo(object obj) {
      if (this is null || obj is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      // Cast checking
      Polynomial<T> other = obj as Polynomial<T>;
      if (other is null) throw new ArgumentException("Right hand side argument is not instance of Polynomial");
      // At least one is empty
      if (!monoms.Any() && !other.monoms.Any())
        return 0;
      else if (!monoms.Any())
        return -1;
      else if (!other.monoms.Any())
        return 1;
      
      // Degrees comparation
      if (monoms.Keys.Last() > other.monoms.Keys.Last())
        return 1;
      else if (monoms.Keys.Last() < other.monoms.Keys.Last())
        return -1;
      else
        return 0;
    }

    public override bool Equals(object obj) { return ReferenceEquals(this, obj) ? true : this.CompareTo(obj) == 0; }

    public override int GetHashCode() { throw new NotImplementedException(); }

    public override string ToString() {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (!monoms.Any())
        return "0";
      return string.Join(", ", monoms.Select(x => $"{x.Value}x{x.Key}").ToArray());
    }

    public IEnumerator<T> GetEnumerator() {
      foreach (var monom in monoms) {
        yield return monom.Value;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
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

    public static bool operator ==(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return left.CompareTo(right) == 0;
    }

    public static bool operator !=(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return left.CompareTo(right) != 0;
    }

    public static bool operator >(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return left.CompareTo(right) == 1;
    }

    public static bool operator >=(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return left.CompareTo(right) >= 0;
    }

    public static bool operator <(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return left.CompareTo(right) == -1;
    }

    public static bool operator <=(Polynomial<T> left, Polynomial<T> right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return left.CompareTo(right) <= 0;
    }
  }
}
