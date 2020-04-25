using ConstantsSpace;
using ExceptionSpace;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace MatrixSpace {
  public class Matrix : ICloneable, IEquatable<Matrix>, IEnumerable, IComparable<Matrix> {
    
    private readonly double[,] data;
    
    public int Size { get; }


    public double this[int i, int j] { get => data[i, j]; set => data[i, j] = value; }

    public Matrix(int size) {
      if (size < 1)
        throw new ArgumentException("Wrong matrix size");

      Size = size;
      data = new double[size, size];
      FillByNum(0);
    }

    public Matrix(int size, double num) {
      if (size < 1)
        throw new ArgumentException("Wrong matrix size");

      Size = size;
      data = new double[size, size];
      FillByNum(num);
    }

    public Matrix(int size, double[,] array) {
      if (size < 1)
        throw new ArgumentException("Wrong matrix size");
      if (array is null)
        throw new ArgumentNullException("There is no initializer");
      if (array.GetLength(0) != size || array.GetLength(1) != size)
        throw new RankException("Sizes mismatch");

      Size = size;
      data = (double[,])array.Clone();
    }

    public Matrix(Matrix matrix) : this(matrix.Size, matrix.data) { }

    public void ActionOverData(Action<int, int> action) {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      for (int i = 0; i < Size; i++)
        for (int j = 0; j < Size; j++)
          action(i, j);
    }

    public void FillByNum(double num) => ActionOverData((i, j) => this[i, j] = num);

    public void SetIdentity() {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      ActionOverData((i, j) => this[i, j] = (i == j ? 1 : 0));
    }

    public static Matrix Add(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      Matrix result = new Matrix(left);
      result.ActionOverData((i, j) => result[i, j] += right[i, j]);
      return result;
    }

    public static Matrix Add(Matrix left, double right) {
      if (left is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Matrix result = new Matrix(left);
      result.ActionOverData((i, j) => result[i, j] += right);
      return result;
    }

    public static Matrix Subtract(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      Matrix result = new Matrix(left);
      result.ActionOverData((i, j) => result[i, j] -= right[i, j]);
      return result;
    }

    public static Matrix Multiply(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      Matrix result = new Matrix(left);
      result.ActionOverData((x, y) => {
        double accumulator = 0;
        for (int i = 0; i < left.Size; i++)
          accumulator += left[x, i] * right[i, y];
        result[x, y] = accumulator;
      });
      return result;
    }

    public static Matrix Multiply(Matrix left, double right) {
      if (left is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Matrix result = new Matrix(left);
      result.ActionOverData((i, j) => result[i, j] *= right);
      return result;
    }

    public static Matrix Divide(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Multiply(left, Inverse(right));
    }

    public static Matrix Transpose(Matrix matrix) {
      if (matrix is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      Matrix result = new Matrix(matrix.Size);
      matrix.ActionOverData((i, j) => result[i, j] = matrix[j, i]);
      return result;
    }

    public double Determinant() {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      double determinant = 1;

      Matrix processMatrix = new Matrix(this);

      for (int i = 0; i < Size; ++i) {
        int k = i;
        for (int j = i + 1; j < Size; j++)
          if (Math.Abs(processMatrix[j, i]) > Math.Abs(processMatrix[k, i]))
            k = j;
        if (Math.Abs(processMatrix[k, i]) < Constants.Eps) {
          determinant = 0;
          break;
        }
        for (int m = 0; m < Size; m++) {
          double tmp = processMatrix[i, m];
          processMatrix[i, m] = processMatrix[k, m];
          processMatrix[k, m] = tmp;
        }
        if (i != k)
          determinant = -determinant;
        determinant *= processMatrix[i, i];
        for (int j = i + 1; j < Size; ++j)
          processMatrix[i, j] /= processMatrix[i, i];
        for (int j = 0; j < Size; ++j) {
          if (j != i && Math.Abs(processMatrix[j, i]) > Constants.Eps) {
            for (int l = i + 1; l < Size; ++l)
              processMatrix[j, l] -= processMatrix[i, l] * processMatrix[j, i];
          }
        }
      }
      return determinant;
    }

    public static Matrix Inverse(Matrix matrix) {
      if (matrix is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (Math.Abs(matrix.Determinant()) < Constants.Eps)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Determinant is 0");

      Matrix process = new Matrix(matrix);
      Matrix result = new Matrix(matrix.Size);
      result.SetIdentity();

      int size = matrix.Size;

      for (int i = 0; i < size - 1; i++) {
        int tmp = i;
        while (Math.Abs(matrix[tmp, i]) < Constants.Eps && tmp < size)
          tmp++;
        for (int j = 0; j < size; ++j) {
          double tmpMean = process[i, j];
          process[i, j] = process[tmp, j];
          process[tmp, j] = tmpMean;

          tmpMean = result[i, j];
          result[i, j] = result[tmp, j];
          result[tmp, j] = tmpMean;
        }
        double tmpCoef = process[i, i];
        for (int j = 0; j < size; ++j) {
          process[i, j] /= tmpCoef;
          result[i, j] /= tmpCoef;
        }
        for (int k = i + 1; k < size; k++) {
          double coef = process[k, i];
          for (int j = 0; j < size; ++j) {
            process[k, j] -= process[i, j] * coef;
            result[k, j] -= result[i, j] * coef;
          }
        }
      }
      double tmpCoeff = process[size - 1, size - 1];
      for (int j = 0; j < size; ++j) {
        process[size - 1, j] /= tmpCoeff;
        result[size - 1, j] /= tmpCoeff;
      }
      for (int i = size - 1; i > 0; i--) {
        for (int k = i - 1; k >= 0; k--) {
          double koef = process[k, i];
          for (int j = 0; j < size; ++j)
            result[k, j] -= result[i, j] * koef;
        }
      }
      return result;
    }


    public object Clone() => new Matrix(this);

    public override bool Equals(object obj) {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is null)
        return false;

      return Equals(obj as Matrix);
    }

    public bool Equals(Matrix other) {
      if (other! is Matrix)
        return false;

      return CompareTo(other) == 0;
    }

    public int CompareTo(object obj) {
      if (obj is null)
        throw new ArgumentNullException("Object is null");

      return CompareTo(obj as Matrix);
    }

    public int CompareTo(Matrix other) {
      if (!(other is Matrix))
        throw new ArgumentException("Object is not a Polynomial");

      double difference = this.Determinant() - other.Determinant();

      if (Math.Abs(difference) < Constants.Eps)
        return 0;
      else if (difference > 0)
        return 1;
      else
        return -1;
    }

    public IEnumerator GetEnumerator() => data.GetEnumerator();

    public override string ToString() {
      if (this is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (data.GetLength(0) == 1)
        return $"({Math.Round(data[0, 0], 3)})";

      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < data.GetLength(0); i++) {
        sb.Append("(");
        for (int j = 0; j < data.GetLength(0) - 1; j++)
          sb.Append($"{Math.Round(this[i, j], 3)}, ");
        sb.Append($"{Math.Round(this[i, data.GetLength(0) - 1], 3)})\n");
      }
      return sb.ToString();
    }


    // --- Operators ---
    public static Matrix operator +(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Add(left, right);
    }

    public static Matrix operator +(Matrix left, double right) {
      if (left is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Add(left, right);
    }

    public static Matrix operator -(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Subtract(left, right);
    }

    public static Matrix operator *(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Multiply(left, right);
    }

    public static Matrix operator *(Matrix left, double right) {
      if (left is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      return Multiply(left, right);
    }

    public static Matrix operator /(Matrix left, Matrix right) {
      if (left is null || right is null)
        throw new ArgumentNullException(System.Reflection.MethodBase.GetCurrentMethod().Name);

      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Divide(left, right);
    }

    public static bool operator ==(Matrix left, Matrix right) => left.CompareTo(right) == 0;

    public static bool operator ==(Matrix left, double right) => Math.Abs(left.Determinant() - right) < Constants.Eps;

    public static bool operator !=(Matrix left, Matrix right) => !(left == right);

    public static bool operator !=(Matrix left, double right) => !(left == right);

    public static bool operator <(Matrix left, Matrix right) => left.CompareTo(right) < 0;

    public static bool operator <=(Matrix left, Matrix right) => left.CompareTo(right) <= 0;

    public static bool operator >(Matrix left, Matrix right) => left.CompareTo(right) > 0;

    public static bool operator >=(Matrix left, Matrix right) => left.CompareTo(right) >= 0;

  }
}
