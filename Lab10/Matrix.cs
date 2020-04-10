using System;
using ConstantsSpace;


namespace ExceptionSpace {
  public class Matrix : ICloneable, IEquatable<Matrix> {

    private readonly double[,] data;
    public int Size { get; }

    public double this[int i, int j] { get => data[i, j]; private set => data[i, j] = value; }

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
      if (array == null)
        throw new ArgumentNullException("There is no initializer");
      if (array.GetLength(0) != size || array.GetLength(1) != size)
        throw new RankException();

      Size = size;
      data = (double[,])array.Clone();
    }

    public Matrix(Matrix matrix) {
      if (matrix is null)
        throw new ArgumentNullException();
      
      Size = matrix.Size;

      if (Size < 1)
        throw new ArgumentException("Wrong matrix size");
      if (matrix.data == null)
        throw new ArgumentNullException("There is no initializer");
      if (matrix.data.GetLength(0) != Size || matrix.data.GetLength(1) != Size)
        throw new RankException();

      data = (double[,])matrix.data.Clone();
    }

    public void ActionOverData(Action<int, int> action) {
      for (int i = 0; i < Size; i++)
        for (int j = 0; j < Size; j++)
          action(i, j);
    }

    public void FillByNum(double num) => ActionOverData((i, j) => this[i, j] = num);

    public void SetIdentity() => ActionOverData((i, j) => this[i, j] = (i == j ? 1 : 0));

    public static Matrix Add(Matrix left, Matrix right) {
      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      Matrix result = new Matrix(left);
      result.ActionOverData((i, j) => result[i, j] += right[i, j]);
      return result;
    }

    public static Matrix Subtract(Matrix left, Matrix right) {
      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      Matrix result = new Matrix(left);
      result.ActionOverData((i, j) => result[i, j] -= right[i, j]);
      return result;
    }

    public static Matrix Multiply(Matrix left, Matrix right) {
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
      Matrix result = new Matrix(left);
      result.ActionOverData((i, j) => result[i, j] *= right);
      return result;
    }

    public static Matrix Divide(Matrix left, Matrix right) {
      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Multiply(left, right.Inverse());
    }

    public static Matrix operator +(Matrix left, Matrix right) {
      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Add(left, right);
    }

    public static Matrix operator -(Matrix left, Matrix right) {
      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Subtract(left, right);
    }

    public static Matrix operator *(Matrix left, Matrix right) {
      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Multiply(left, right);
    }

    public static Matrix operator *(Matrix left, double right) {
      return Multiply(left, right);
    }

    public static Matrix operator /(Matrix left, Matrix right) {
      if (left.Size != right.Size)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Different sizes");

      return Divide(left, right);
    }

    public static bool operator ==(Matrix left, Matrix right) => left.Equals(right);

    public static bool operator ==(Matrix left, double right) {
      Matrix numberMatrix = new Matrix(left.Size, right);
      return left.Equals(numberMatrix);
    }

    public static bool operator !=(Matrix left, Matrix right) => !(left == right);

    public static bool operator !=(Matrix left, double right) => !(left == right);

    public Matrix Transpose() {
      Matrix transposed = new Matrix(Size);
      ActionOverData((i, j) => transposed[i, j] = this[j, i]);
      return transposed;
    }

    public double Determinant() {
      double determinant = 1;

      if (Size == 1)
        return this[0, 0];

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

    public Matrix Inverse() {
      if (Math.Abs(Determinant()) < Constants.Eps)
        throw new OperationException(System.Reflection.MethodBase.GetCurrentMethod().Name, "Determinant is 0");

      Matrix process = new Matrix(this);
      Matrix result = new Matrix(this.Size);
      result.SetIdentity();

      int size = Size;

      for (int i = 0; i < size - 1; i++) {
        int tmp = i;
        while (Math.Abs(this[tmp, i]) < Constants.Eps && tmp < size)
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

    public object Clone() { return new Matrix(this); }

    public override bool Equals(object obj) { return Equals(obj as Matrix); }

    public bool Equals(Matrix other) {
      if (other == null || Size != other.Size)
        return false;

      for (int i = 0; i < Size; i++) 
        for (int j = 0; j < Size; j++) 
          if (Math.Abs(this[i, j] - other[i, j]) > Constants.Eps)
            return false;

      return true;
    }


    public override int GetHashCode() { throw new NotImplementedException(); }

  }
}
