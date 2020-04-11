﻿using System;
using MatrixSpace;
using PolynomialSpace;
using ExceptionSpace;
using ConstantsSpace;

namespace Lab10 {
  class Program {
    static void Main(string[] args) {
      Matrix m1 = new Matrix(2, new double[,] {
        { 2, 2 },
        { 2, 2 }
      });
      Matrix m2 = new Matrix(2, new double[,] {
        { 3, 4 },
        { 5, 6 }
      });
      Matrix m3 = new Matrix(3, new double[,] {
        { 6, -2, 1 },
        { 7, 4, -1 },
        { -3, 5, 1 }
      });

      Polynomial<double> p1 = new Polynomial<double>();
      p1.AddMonom(2, 1);
      p1.AddMonom(1, 1);
      p1.AddMonom(1, 0);
      p1.AddMonom(2, -2);

      Polynomial<double> p2 = new Polynomial<double>();
      p2.AddMonom(2, 1);
      p2.AddMonom(1, 1);
      p2.AddMonom(1, 0);
      p2.AddMonom(2, -2);
      p2.AddMonom(3, 1);

      Console.WriteLine(p1 < p2);

      Polynomial<Matrix> pm1 = new Polynomial<Matrix>(new System.Collections.Generic.SortedDictionary<int, Matrix>{
        { 1, m2 }
      });
      Console.WriteLine(pm1.ValueMatrix(m2.Inverse()));

      Matrix e1 = new Matrix(1, new double[,] {
        { 1 }
      });
      Matrix e2 = new Matrix(1, new double[,] {
        { 2 }
      });
      Polynomial<Matrix> P = new Polynomial<Matrix>(new System.Collections.Generic.SortedDictionary<int, Matrix> {
        { 2, e1 },
        { 1, e2 },
        { 0, e1 }
      });
      Polynomial<Matrix> p = new Polynomial<Matrix>(new System.Collections.Generic.SortedDictionary<int, Matrix> {
        { 1, e1 },
        { 0, e1 }
      });
      Console.WriteLine(P / p / p);

      Console.WriteLine(P.ValueMatrix(new Matrix(1, 0)));
    }
  }
}
