using System;
using System.Linq;

//TODO AutoNullCheck

namespace Lab11Space {
  class Complex : ICloneable, IEquatable<Complex>, IComparable<Complex>, IComparable {

    // --------------SECTION: Properties ---------- //
    public double Real { get; set; }
    public double Imaginary { get; set; }
    public double Magnitude { get { return Abs(this); } }
    public double Phase { get { return Math.Atan2(Imaginary, Real); } }


    // --------------SECTION: Attributes -------------- //
    public static readonly Complex Zero = new Complex(0.0, 0.0);
    public static readonly Complex One = new Complex(1.0, 0.0);
    public static readonly Complex ImaginaryOne = new Complex(0.0, 1.0);


    // --------------SECTION: Constructors and factory methods------------ //
    public Complex() { }
    public Complex(double number) => Real = number;
    public Complex(double real, double imaginary) : this(real) => Imaginary = imaginary;
    public Complex(Complex complex) : this(complex.Real, complex.Imaginary) { }

    public static Complex FromPolarCoordinates(double magnitude, double phase) =>
      new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));


    // --------------SECTION: Cast operators-------------- //
    public static implicit operator Complex(double value) => new Complex(value, 0.0);

    public static implicit operator double(Complex value) => value.Real;


    // --------------SECTION: Events------------ //
    public static event EventHandler<DivisionByZeroEventArgs> DivisionByZero;

    protected static void OnDivisionByZero(Complex value, DivisionByZeroEventArgs e) {
      EventHandler<DivisionByZeroEventArgs> handler = DivisionByZero;
      handler?.Invoke(value, e);
    }


    // --------------SECTION: Formatting-------------- //
    public override string ToString() => $"({Real.ToString("F")}, {Imaginary.ToString("F")})";
    public string ToString(string format) => $"({Real.ToString(format)}, {Imaginary.ToString(format)})";


    // --------------SECTION: Implementations of interfaces-------------- //
    /// ICloneable
    public object Clone() => new Complex(this);


    /// IEquatable
    public override bool Equals(object obj) {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is null)
        return false;
      return Equals(obj as Complex);
    }

    public bool Equals(Complex value) {
      if (value! is Complex)
        return false;
      return Math.Abs(Math.Abs(Real) - Math.Abs(value.Real)) < Constants.Eps &&
        Math.Abs(Math.Abs(Imaginary) - Math.Abs(value.Imaginary)) < Constants.Eps;
    }


    /// IComparable
    public int CompareTo(object obj) {
      if (obj is null)
        throw new ArgumentNullException("Object is null");
      return CompareTo(obj as Complex);
    }

    public int CompareTo(Complex other) {
      if (!(other is Complex))
        throw new ArgumentException("Object is not a Complex");

      double difference = this.Magnitude - other.Magnitude;

      if (Math.Abs(difference) < Constants.Eps)
        return 0;
      else if (difference > 0)
        return 1;
      else
        return -1;
    }


    // --------------SECTION: Comparison Operator Overloading -------------- //
    public static bool operator ==(Complex lhs, Complex rhs) => lhs.CompareTo(rhs) == 0;
    public static bool operator !=(Complex lhs, Complex rhs) => !(lhs == rhs);
    public static bool operator <(Complex lhs, Complex rhs) => lhs.CompareTo(rhs) < 0;
    public static bool operator <=(Complex lhs, Complex rhs) => lhs.CompareTo(rhs) <= 0;
    public static bool operator >(Complex lhs, Complex rhs) => lhs.CompareTo(rhs) > 0;
    public static bool operator >=(Complex lhs, Complex rhs) => lhs.CompareTo(rhs) >= 0;


    // --------------SECTION: Alternates for ariphmetic operators------------ //
    public static Complex Negate(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(-value.Real, -value.Imaginary);
    }

    public static Complex Sum(Complex lhs, Complex rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));
      return new Complex(lhs.Real + rhs.Real, lhs.Imaginary + rhs.Imaginary);
    }

    public static Complex Subtract(Complex lhs, Complex rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));
      return new Complex(lhs.Real - rhs.Real, lhs.Imaginary - rhs.Imaginary);
    }

    public static Complex Multiply(Complex lhs, Complex rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));
      // Multiplication:  (a + bi)(c + di) = (ac - bd) + (bc + ad)i
      double result_Realpart = (lhs.Real * rhs.Real) - (lhs.Imaginary * rhs.Imaginary);
      double result_Imaginarypart = (lhs.Imaginary * rhs.Real) + (lhs.Real * rhs.Imaginary);
      return new Complex(result_Realpart, result_Imaginarypart);
    }

    public static Complex Divide(Complex lhs, Complex rhs) {
      if (lhs is null)
        throw new ArgumentNullException(nameof(lhs));
      else if (rhs is null)
        throw new ArgumentNullException(nameof(rhs));

      //
      if (rhs.FuzzyNull()) {
        DivisionByZeroEventArgs args = new DivisionByZeroEventArgs {
          Divisor= nameof(rhs)
        };
        OnDivisionByZero(rhs, args);
      }

      // Division : Smith's formula.
      double a = lhs.Real;
      double b = lhs.Imaginary;
      double c = rhs.Real;
      double d = rhs.Imaginary;

      if (Math.Abs(d) < Math.Abs(c)) {
        double doc = d / c;
        return new Complex((a + b * doc) / (c + d * doc), (b - a * doc) / (c + d * doc));
      }
      else {
        double cod = c / d;
        return new Complex((b + a * cod) / (d + c * cod), (-a + b * cod) / (d + c * cod));
      }
    }


    // --------------SECTION: Arithmetic Operator(unary) Overloading -------------- //
    public static Complex operator -(Complex value) => Negate(value);


    // --------------SECTION: Arithmetic Operator(binary) Overloading -------------- //       
    public static Complex operator +(Complex lhs, Complex rhs) => Sum(lhs, rhs);
    public static Complex operator -(Complex lhs, Complex rhs) => Subtract(lhs, rhs);
    public static Complex operator *(Complex lhs, Complex rhs) => Multiply(lhs, rhs);
    public static Complex operator /(Complex lhs, Complex rhs) => Divide(lhs, rhs);


    // --------------SECTION: Other arithmetic operations  -------------- //
    public static double Abs(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (double.IsInfinity(value.Real) || double.IsInfinity(value.Imaginary)) 
        return double.PositiveInfinity;

      // |value| == sqrt(a^2 + b^2)
      // sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2)
      // Using the above we can factor out the square of the larger component to dodge overflow.

      double c = Math.Abs(value.Real);
      double d = Math.Abs(value.Imaginary);

      if (c > d) {
        double r = d / c;
        return c * Math.Sqrt(1.0 + r * r);
      }
      else if (d == 0.0) {
        return c;  // c is either 0.0 or NaN
      }
      else {
        double r = c / d;
        return d * Math.Sqrt(1.0 + r * r);
      }
    }


    // Conjugate of a Complex number: the conjugate of x+i*y is x-i*y 
    public static Complex Conjugate(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(value.Real, -value.Imaginary);
    }


    // The reciprocal of x+i*y is 1/(x+i*y)
    public static Complex Reciprocal(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return value.FuzzyNull() ? Zero : One / value;
    }


    // --------------SECTION: Trigonometric operations (methods implementing ITrigonometric)  -------------- //
    public static Complex Sin(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(Math.Sin(value.Real) * Math.Cosh(value.Imaginary), Math.Cos(value.Real) * Math.Sinh(value.Imaginary));
    }

    public static Complex Sinh(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(Math.Sinh(value.Real) * Math.Cos(value.Imaginary), Math.Cosh(value.Real) * Math.Sin(value.Imaginary));
    }

    public static Complex[] Asin(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      Complex[] result = new Complex[2];
      var sqrts = Sqrt(One - (value * value));
      for (int i = 0; i < sqrts.Length; i++) {
        result[i] = -ImaginaryOne * Ln((ImaginaryOne * value) + sqrts[i]);
      }
      return result;
    }

    public static Complex Cos(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(Math.Cos(value.Real) * Math.Cosh(value.Imaginary), -Math.Sin(value.Real) * Math.Sinh(value.Imaginary));
    }

    public static Complex Cosh(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(Math.Cosh(value.Real) * Math.Cos(value.Imaginary), Math.Sinh(value.Real) * Math.Sin(value.Imaginary));
    }

    public static Complex[] Acos(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      Complex[] result = new Complex[2];
      var sqrts = Sqrt(One - (value * value));
      for (int i = 0; i < sqrts.Length; i++) {
        result[i] = -ImaginaryOne * Ln(value + (ImaginaryOne * sqrts[i]));
      }
      return result;
    }

    public static Complex Tan(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return Sin(value) / Cos(value);
    }

    public static Complex Tanh(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return Sinh(value) / Cosh(value);
    }

    public static Complex Atan(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      Complex two = new Complex(2.0, 0.0);
      return (ImaginaryOne / two) * (Ln(One - ImaginaryOne * value) - Ln(One + ImaginaryOne * value));
    }

    // --------------SECTION: Other numerical functions  -------------- //        
    public static Complex Ln(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(Math.Log(Abs(value)), Math.Atan2(value.Imaginary, value.Real));
    }

    public static Complex Log(Complex value, double baseValue) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return Ln(value) / Ln(baseValue);
    }

    public static Complex Lg(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return Scale(Ln(value), 1 / Math.Log(10));
    }

    public static Complex Exp(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      double temp_factor = Math.Exp(value.Real);
      double result_re = temp_factor * Math.Cos(value.Imaginary);
      double result_im = temp_factor * Math.Sin(value.Imaginary);
      return (new Complex(result_re, result_im));
    }

    public static Complex[] Sqrt(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      //return FromPolarCoordinates(Math.Sqrt(value.Magnitude), value.Phase / 2.0);
      return NthRoots(value, 2);
    }

    /// <summary>Raise complex number to a natural complex degree.</summary>
    /// <returns>The first root.</returns>
    public static Complex CPow(Complex value, Complex power) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      else if (power is null)
        throw new ArgumentNullException(nameof(power));

      if (power.FuzzyEquals(Zero)) return One;
      if (value.FuzzyEquals(Zero)) return Zero;

      double a = value.Real;
      double b = value.Imaginary;
      double c = power.Real;
      double d = power.Imaginary;

      double rho = Abs(value);
      double theta = Math.Atan2(b, a);
      double newRho = c * theta + d * Math.Log(rho);

      double t = Math.Pow(rho, c) * Math.Pow(Math.E, -d * theta);

      return new Complex(t * Math.Cos(newRho), t * Math.Sin(newRho));
    }

    public static Complex Pow(Complex value, int power) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return CPow(value, power);
    }

    public static Complex[] NthRoots(Complex value, int n) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      int absN = Math.Abs(n);
      double nthRootOfMagnitude = Math.Pow(value.Magnitude, 1.0 / n);
      return Enumerable.Range(0, absN)
               .Select(k => FromPolarCoordinates(nthRootOfMagnitude, value.Phase / absN + k * 2 * Math.PI / absN))
               .ToArray(); 
    }


    //--------------- SECTION: Private member functions for internal use -----------------------------------//
    private static Complex Scale(Complex value, double factor) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return new Complex(factor * value.Real, factor * value.Imaginary);
    }

    private bool FuzzyEquals(Complex value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));
      return Math.Abs(Magnitude - value.Magnitude) < Constants.Eps;
    }

    private bool FuzzyNull() => FuzzyEquals(0.0);

  }
}
