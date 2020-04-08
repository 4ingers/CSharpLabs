using System;

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

    public static Matrix Inverse(Matrix matrix) {
      // Нулевой определитель == необратимость матрицы
      if (Math.Abs(matrix.Determinant()) < Constants.Eps)
        //TODO throw inversion_error();
        throw new NotImplementedException();

      Matrix transit = new Matrix(matrix);
      Matrix res = new Matrix(matrix.Size);
      res.SetIdentity();

      int size = matrix.Size;

      for (int i = 0; i < size - 1; i++) {
        int tmp = i;
        while (Math.Abs(matrix[tmp, i]) < Constants.Eps && tmp < size) 
          tmp++;
        for (int j = 0; j < size; ++j) {
          double tmpMean = transit[i, j];
          transit[i, j] = transit[tmp, j];
          transit[tmp, j] = tmpMean;

          tmpMean = res[i, j];
          res[i, j] = res[tmp, j];
          res[tmp, j] = tmpMean;
        }
        double tmpCoef = transit[i, i];
        for (int j = 0; j < size; ++j) {
          transit[i, j] = transit[i, j] / tmpCoef;
          res[i, j] = res[i, j] / tmpCoef;
        }
        for (int k = i + 1; k < size; k++) {
          double coef = transit[k, i];
          for (int j = 0; j < size; ++j) {
            transit[k, j] = transit[k, j] - transit[i, j] * coef;
            res[k, j] = res[k, j] - res[i, j] * coef;
          }
        }
      }
      double tmpCoeff = transit[size - 1, size - 1];
      for (int j = 0; j < size; ++j) {
        transit[size - 1, j] = transit[size - 1, j] / tmpCoeff;
        res[size - 1, j] = res[size - 1, j] / tmpCoeff;
      }
      for (int i = size - 1; i > 0; i--) {
        for (int k = i - 1; k >= 0; k--) {
          double koef = transit[k, i];
          for (int j = 0; j < size; ++j)
            res[k, j] = res[k, j] - res[i, j] * koef;
        }
      }
      Matrix inverted = new Matrix(size);
      inverted.ActionOverData((i, j) => inverted[i, j] = res[i, j]);
      return res;
    }

    public object Clone() {
      return new Matrix(this);
    }
  }
}
