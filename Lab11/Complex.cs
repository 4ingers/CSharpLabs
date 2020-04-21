using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Lab11Space {
  class Complex {

    // --------------SECTION: Auto-properties ---------- //
    public double Real { get; set; }
    public double Imaginary { get; set; }
    public double Magnitude { get { return Abs(this); } }
    public double Phase { get { return Math.Atan2(Imaginary, Real); } }


    // --------------SECTION: Attributes -------------- //
    public static readonly Complex Zero = new Complex(0.0, 0.0);
    public static readonly Complex One = new Complex(1.0, 0.0);
    public static readonly Complex ImaginaryOne = new Complex(0.0, 1.0);


    // --------------SECTION: Constructors and factory methods ------------ //
    public Complex(double number) => Real = number;
    public Complex(double real, double imaginary) : this(real) => Imaginary = imaginary;
    public Complex(Complex complex) : this(complex.Real, complex.Imaginary) { }


    public static Complex FromPolarCoordinates(double magnitude, double phase) =>
      new Complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));


    public static Complex Negate(Complex value) => new Complex(-value.Real, -value.Imaginary);

    public static Complex Add(Complex left, Complex right) => new Complex(left.Real + right.Real, left.Imaginary + right.Imaginary);

    public static Complex Subtract(Complex left, Complex right) => new Complex(left.Real - right.Real, left.Imaginary - right.Imaginary);

    public static Complex Multiply(Complex left, Complex right) {
      // Multiplication:  (a + bi)(c + di) = (ac - bd) + (bc + ad)i
      double result_Realpart = (left.Real * right.Real) - (left.Imaginary * right.Imaginary);
      double result_Imaginarypart = (left.Imaginary * right.Real) + (left.Real * right.Imaginary);
      return new Complex(result_Realpart, result_Imaginarypart);
    }

    public static Complex Divide(Complex left, Complex right) {
      // Division : Smith's formula.
      double a = left.Real;
      double b = left.Imaginary;
      double c = right.Real;
      double d = right.Imaginary;

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
    public static Complex operator +(Complex left, Complex right) => Add(left, right);
    public static Complex operator -(Complex left, Complex right) => Subtract(left, right);
    public static Complex operator *(Complex left, Complex right) => Multiply(left, right);
    public static Complex operator /(Complex left, Complex right) => Divide(left, right);


    // --------------SECTION: Other arithmetic operations  -------------- //
    public static double Abs(Complex value) {
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
    public static Complex Conjugate(Complex value) => new Complex(value.Real, -value.Imaginary);


    // The reciprocal of x+i*y is 1/(x+i*y)
    public static Complex Reciprocal(Complex value) {
      if (Math.Abs(value.Real) < Constants.Eps && Math.Abs(value.Imaginary) < Constants.Eps)
        return Zero;
      return One / value;
    }


    // --------------SECTION: Comparison Operator Overloading -------------- //
    public static bool operator ==(Complex left, Complex right) => (left.Real == right.Real) && (left.Imaginary == right.Imaginary);
    public static bool operator !=(Complex left, Complex right) => !(left == right);


    // --------------SECTION: Comparison operations (methods implementing IEquatable<ComplexNumber>,IComparable<ComplexNumber>) -------------- //
    public override bool Equals(object obj) {
      if (!(obj is Complex))
        return false;
      return Equals(obj as Complex);
    }


    public bool Equals(Complex value) {
      return ((this.Real.Equals(value.Real)) && (this.Imaginary.Equals(value.Imaginary)));

    }


    // --------------SECTION: Type-casting basic numeric data-types to ComplexNumber  -------------- //
    public static implicit operator Complex(double value) {
      return (new Complex(value, 0.0));
    }


    // --------------SECTION: Formattig/Parsing options  -------------- //
    public override string ToString() => string.Format(CultureInfo.CurrentCulture, $"({Real}, {Imaginary})");

    public String ToString(string format) {
      CultureInfo culture = CultureInfo.CurrentCulture;
      return string.Format(culture, $"({Real.ToString(format, culture)}, {Imaginary.ToString(format, culture)})");
    }

    public string ToString(IFormatProvider provider) => string.Format(provider, $"({Real}, {Imaginary})");

    public string ToString(string format, IFormatProvider provider) => string.Format(provider, $"({Real.ToString(format, provider)}, {Imaginary.ToString(format, provider)})");


    // --------------SECTION: Trigonometric operations (methods implementing ITrigonometric)  -------------- //
    public static Complex Sin(Complex value) => new Complex(Math.Sin(value.Real) * Math.Cosh(value.Imaginary), Math.Cos(value.Real) * Math.Sinh(value.Imaginary));
    public static Complex Sinh(Complex value) => new Complex(Math.Sinh(value.Real) * Math.Cos(value.Imaginary), Math.Cosh(value.Real) * Math.Sin(value.Imaginary));
    public static Complex Asin(Complex value) => -ImaginaryOne * Ln(ImaginaryOne * value + Sqrt(One - value * value));

    public static Complex Cos(Complex value) => new Complex(Math.Cos(value.Real) * Math.Cosh(value.Imaginary), -Math.Sin(value.Real) * Math.Sinh(value.Imaginary));
    public static Complex Cosh(Complex value) => new Complex(Math.Cosh(value.Real) * Math.Cos(value.Imaginary), Math.Sinh(value.Real) * Math.Sin(value.Imaginary));
    public static Complex Acos(Complex value) => -ImaginaryOne * Ln(value + (ImaginaryOne * Sqrt(One - value * value)));

    public static Complex Tan(Complex value) => Sin(value) / Cos(value);
    public static Complex Tanh(Complex value) => Sinh(value) / Cosh(value);
    public static Complex Atan(Complex value)
    {
      Complex two = new Complex(2.0, 0.0);
      return (ImaginaryOne / two) * (Ln(One - ImaginaryOne * value) - Ln(One + ImaginaryOne * value));
    }

    // --------------SECTION: Other numerical functions  -------------- //        
    public static Complex Ln(Complex value) => (new Complex((Math.Log(Abs(value))), (Math.Atan2(value.Imaginary, value.Real))));
    public static Complex Log(Complex value, double baseValue) => Ln(value) / Ln(baseValue);
    public static Complex Lg(Complex value) => Scale(Ln(value), 1 / Math.Log(10));
    public static Complex Exp(Complex value) /* The complex number raised to e */
    {
      double temp_factor = Math.Exp(value.Real);
      double result_re = temp_factor * Math.Cos(value.Imaginary);
      double result_im = temp_factor * Math.Sin(value.Imaginary);
      return (new Complex(result_re, result_im));
    }

    public static Complex Sqrt(Complex value) => Complex.FromPolarCoordinates(Math.Sqrt(value.Magnitude), value.Phase / 2.0);

    //? Is it correct?
    public static Complex Pow(Complex value, Complex power)
    {

      if (power == Complex.Zero) {
        return Complex.One;
      }

      if (value == Complex.Zero) {
        return Complex.Zero;
      }

      double a = value.Real;
      double b = value.Imaginary;
      double c = power.Real;
      double d = power.Imaginary;

      double rho = Complex.Abs(value);
      double theta = Math.Atan2(b, a);
      double newRho = c * theta + d * Math.Log(rho);

      double t = Math.Pow(rho, c) * Math.Pow(Math.E, -d * theta);

      return new Complex(t * Math.Cos(newRho), t * Math.Sin(newRho));
    }

    public static Complex Pow(Complex value, double power) => Pow(value, new Complex(power, 0));



    //--------------- SECTION: Private member functions for internal use -----------------------------------//
    private static Complex Scale(Complex value, double factor) => new Complex(factor * value.Real, factor * value.Imaginary);

  }
}
