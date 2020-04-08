using System;

/// Exceptions are left to add

namespace Matrix {
  public class Matrix : ICloneable {
    private double[,] data;
    public int Size { get; }

    public double this[int i, int j] { get => data[i, j]; private set => data[i, j] = value; }

    public Matrix(int size) {
      if (size < 1)
        throw new NotImplementedException();

      Size = size;
      data = new double[size, size];
      FillByZeros();
    }

    public Matrix(int size, double[,] array) {
      if (size < 1)
        //TODO CustomException: Size
        throw new NotImplementedException();

      if (array == null || array.GetLength(0) != size || array.GetLength(1) != size)
        //TODO CustomException: Array
        throw new NotImplementedException();

      Size = size;
      data = (double[,])array.Clone();
    }

    public Matrix(Matrix matrix) : this(matrix.Size, (double[,])matrix.data.Clone()) {}

    public void ActionOverData(Action<int, int> action) {
      for (int i = 0; i < Size; i++)
        for (int j = 0; j < Size; j++)
          action(i, j);     
    }

    public void FillByZeros() => ActionOverData((i, j) => this[i, j] = 0);
    public void SetIdentity() => ActionOverData((i, j) => this[i, j] = (i == j ? 1 : 0));

    public static Matrix Add(Matrix lhs, Matrix rhs) {
      Matrix result = new Matrix(lhs);
      result.ActionOverData((i, j) => result[i, j] += rhs[i, j]);
      return result;
    }

    public static Matrix Subtract(Matrix lhs, Matrix rhs) {
      Matrix result = new Matrix(lhs);
      result.ActionOverData((i, j) => result[i, j] -= rhs[i, j]);
      return result;
    }

    public static Matrix Multiply(Matrix lhs, Matrix rhs) {
      Matrix result = new Matrix(lhs);
      result.ActionOverData((x, y) => {
        double accumulator = 0;
        for (int i = 0; i < lhs.Size; i++) 
          accumulator += lhs[x, i] * rhs[i, y];
        result[x, y] = accumulator;
      });
      return result;
    }

    public static Matrix Divide(Matrix lhs, Matrix rhs) {
      return Multiply(lhs, rhs.Inverse());
    }

    public static Matrix operator +(Matrix lhs, Matrix rhs) {
      return Add(lhs, rhs);
    }

    public static Matrix operator -(Matrix lhs, Matrix rhs) {
      return Subtract(lhs, rhs);
    }

    public static Matrix operator *(Matrix lhs, Matrix rhs) {
      return Multiply(lhs, rhs);
    }

    public static Matrix operator /(Matrix lhs, Matrix rhs) {
      return Divide(lhs, rhs);
    }

    public Matrix Transpose() {
      Matrix transposed = new Matrix(Size);
      for (int i = 0; i < Size; i++) {
        for (int j = 0; j < Size; j++) {
          transposed[i, j] = this[j, i];
        }
      }
      return transposed;
    }

    public double Determinant() {
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

    public Matrix Inverse() {
      // Нулевой определитель == необратимость матрицы
      if (Math.Abs(Determinant()) < Constants.Eps)
        //TODO throw inversion_error();
        throw new NotImplementedException();

      Matrix process = new Matrix(this);
      Matrix result = new Matrix(this.Size);
      result.SetIdentity();

      int size = this.Size;

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

    public object Clone() {
      return new Matrix(this);
    }
  }
}
