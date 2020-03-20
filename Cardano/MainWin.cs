using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public struct TKomplex
    {
        public double real;
        public double imag;
    };

    public partial class Mainwin : Form
    {
         TKomplex x1, x2, x3;
         double a1, a, b, c, d;

        public double Xroot(double a, double x)
        {
            double i = 1;
            if (a < 0)
                i = -1;
            return (i * Math.Exp( Math.Log(a*i)/x));
        }

        public int Calc_Cardano()  // solve cubic equation according to cardano
        {
            double p, q, u, v;
            double r, alpha;
            int res;
            res = 0;
            if (a1 != 0)
            {
                a = b / a1;
                b = c / a1;
                c = d / a1;

                p = -(a * a / 3.0) + b;
                q = (2.0 / 27.0 * a * a * a) - (a * b / 3.0) + c;
                d = q * q / 4.0 + p * p * p / 27.0;
                if (Math.Abs(d) < Math.Pow(10.0, -11.0))
                    d = 0;
                // 3 cases D > 0, D == 0 and D < 0
                if (d > 1e-20)
                {
                    u = Xroot(-q / 2.0 + Math.Sqrt(d), 3.0);
                    v = Xroot(-q / 2.0 - Math.Sqrt(d), 3.0);
                    x1.real = u + v - a / 3.0;
                    x2.real = -(u + v) / 2.0 - a / 3.0;
                    x2.imag = Math.Sqrt(3.0) / 2.0 * (u - v);
                    x3.real = x2.real;
                    x3.imag = -x2.imag;
                    res = 1;
                }
                if (Math.Abs(d) <= 1e-20)
                {
                    u = Xroot(-q / 2.0, 3.0);
                    v = Xroot(-q / 2.0, 3.0);
                    x1.real = u + v - a / 3.0;
                    x2.real = -(u + v) / 2.0 - a / 3.0;
                    res = 2;
                }
                if (d < -1e-20)
                {
                    r = Math.Sqrt(-p * p * p / 27.0);
                    alpha = Math.Atan(Math.Sqrt(-d) / -q * 2.0);
                    if (q > 0)                         // if q > 0 the angle becomes 2 * PI - alpha
                        alpha = 2.0 * Math.PI - alpha;

                    x1.real = Xroot(r, 3.0) * (Math.Cos((6.0 * Math.PI - alpha) / 3.0) + Math.Cos(alpha / 3.0)) - a / 3.0;
                    x2.real = Xroot(r, 3.0) * (Math.Cos((2.0 * Math.PI + alpha) / 3.0) + Math.Cos((4.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    x3.real = Xroot(r, 3.0) * (Math.Cos((4.0 * Math.PI + alpha) / 3.0) + Math.Cos((2.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    res = 3;
                }
            }
            else
                res = 0;
            return res;
         }

        public Mainwin()
        {

            InitializeComponent();
            x1 = new TKomplex();
            x2 = new TKomplex();
            x3 = new TKomplex();
            x1.real = 0;
            x1.imag = 0;
            x2.real = 0;
            x2.imag = 0;
            x3.real = 0;
            x3.imag = 0;
            // sample for D < 0
            a1 = 3.0;
            b = -10;
            c = 1.0;
            d = 4.0;
            tb_A.Text = Convert.ToString(a1);
            tb_B.Text = Convert.ToString(b);
            tb_C.Text = Convert.ToString(c);
            tb_D.Text = Convert.ToString(d);
        }

        private void bt_Calc_Click(object sender, EventArgs e)
        {
            if (tb_A.Text != "")
                a1 = Convert.ToDouble(tb_A.Text);
            else
                a1 = 0;
            if (tb_B.Text != "")
                b = Convert.ToDouble(tb_B.Text);
            else
                b = 0;
            if (tb_C.Text != "")
                c = Convert.ToDouble(tb_C.Text);
            else
                c = 0;
            if (tb_D.Text != "")
                d = Convert.ToDouble(tb_D.Text);
            else
                d = 0;
            if ((a1 != 0) && (b != 0) && (c != 0) && (d != 0))
            {
                switch (Calc_Cardano())
                {
                    case 0:
                        {
                            tb_x1.Text = "";
                            tb_x2.Text = "";
                            tb_x3.Text = "";
                            break;
                        }
                    case 1:
                        {
                            tb_x1.Text = Convert.ToString(x1.real);
                            tb_x2.Text = "";
                            tb_x3.Text = "";
                            break;
                        }
                    case 2:
                        {
                            tb_x1.Text = Convert.ToString(x1.real);
                            tb_x2.Text = Convert.ToString(x2.real);
                            tb_x3.Text = "";
                            break;
                        }
                    case 3:
                        {
                            tb_x1.Text = Convert.ToString(x1.real);
                            tb_x2.Text = Convert.ToString(x2.real);
                            tb_x3.Text = Convert.ToString(x3.real);
                            break;
                        }
                }
            }
        }
    }
}
