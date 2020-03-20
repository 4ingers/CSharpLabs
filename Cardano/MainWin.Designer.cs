namespace WindowsFormsApplication1
{
    partial class Mainwin
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_A = new System.Windows.Forms.TextBox();
            this.tb_B = new System.Windows.Forms.TextBox();
            this.tb_C = new System.Windows.Forms.TextBox();
            this.tb_D = new System.Windows.Forms.TextBox();
            this.lA = new System.Windows.Forms.Label();
            this.lFormula = new System.Windows.Forms.Label();
            this.lB = new System.Windows.Forms.Label();
            this.lC = new System.Windows.Forms.Label();
            this.lD = new System.Windows.Forms.Label();
            this.bt_Calc = new System.Windows.Forms.Button();
            this.tb_x1 = new System.Windows.Forms.TextBox();
            this.tb_x2 = new System.Windows.Forms.TextBox();
            this.tb_x3 = new System.Windows.Forms.TextBox();
            this.l_X1 = new System.Windows.Forms.Label();
            this.l_X2 = new System.Windows.Forms.Label();
            this.l_X3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb_A
            // 
            this.tb_A.Location = new System.Drawing.Point(93, 43);
            this.tb_A.Name = "tb_A";
            this.tb_A.Size = new System.Drawing.Size(100, 20);
            this.tb_A.TabIndex = 0;
            // 
            // tb_B
            // 
            this.tb_B.Location = new System.Drawing.Point(93, 79);
            this.tb_B.Name = "tb_B";
            this.tb_B.Size = new System.Drawing.Size(100, 20);
            this.tb_B.TabIndex = 1;
            // 
            // tb_C
            // 
            this.tb_C.Location = new System.Drawing.Point(93, 117);
            this.tb_C.Name = "tb_C";
            this.tb_C.Size = new System.Drawing.Size(100, 20);
            this.tb_C.TabIndex = 2;
            // 
            // tb_D
            // 
            this.tb_D.Location = new System.Drawing.Point(93, 160);
            this.tb_D.Name = "tb_D";
            this.tb_D.Size = new System.Drawing.Size(100, 20);
            this.tb_D.TabIndex = 3;
            // 
            // lA
            // 
            this.lA.AutoSize = true;
            this.lA.Location = new System.Drawing.Point(58, 46);
            this.lA.Name = "lA";
            this.lA.Size = new System.Drawing.Size(23, 13);
            this.lA.TabIndex = 4;
            this.lA.Text = "A =";
            // 
            // lFormula
            // 
            this.lFormula.AutoSize = true;
            this.lFormula.Location = new System.Drawing.Point(58, 18);
            this.lFormula.Name = "lFormula";
            this.lFormula.Size = new System.Drawing.Size(135, 13);
            this.lFormula.TabIndex = 5;
            this.lFormula.Text = "AX^3 + BX^2 + CX + D = 0";
            // 
            // lB
            // 
            this.lB.AutoSize = true;
            this.lB.Location = new System.Drawing.Point(58, 82);
            this.lB.Name = "lB";
            this.lB.Size = new System.Drawing.Size(23, 13);
            this.lB.TabIndex = 6;
            this.lB.Text = "B =";
            // 
            // lC
            // 
            this.lC.AutoSize = true;
            this.lC.Location = new System.Drawing.Point(58, 120);
            this.lC.Name = "lC";
            this.lC.Size = new System.Drawing.Size(23, 13);
            this.lC.TabIndex = 7;
            this.lC.Text = "C =";
            // 
            // lD
            // 
            this.lD.AutoSize = true;
            this.lD.Location = new System.Drawing.Point(58, 163);
            this.lD.Name = "lD";
            this.lD.Size = new System.Drawing.Size(24, 13);
            this.lD.TabIndex = 8;
            this.lD.Text = "D =";
            // 
            // bt_Calc
            // 
            this.bt_Calc.Location = new System.Drawing.Point(188, 202);
            this.bt_Calc.Name = "bt_Calc";
            this.bt_Calc.Size = new System.Drawing.Size(100, 23);
            this.bt_Calc.TabIndex = 9;
            this.bt_Calc.Text = "Calc";
            this.bt_Calc.UseVisualStyleBackColor = true;
            this.bt_Calc.Click += new System.EventHandler(this.bt_Calc_Click);
            // 
            // tb_x1
            // 
            this.tb_x1.Location = new System.Drawing.Point(333, 43);
            this.tb_x1.Name = "tb_x1";
            this.tb_x1.Size = new System.Drawing.Size(100, 20);
            this.tb_x1.TabIndex = 10;
            // 
            // tb_x2
            // 
            this.tb_x2.Location = new System.Drawing.Point(333, 79);
            this.tb_x2.Name = "tb_x2";
            this.tb_x2.Size = new System.Drawing.Size(100, 20);
            this.tb_x2.TabIndex = 11;
            // 
            // tb_x3
            // 
            this.tb_x3.Location = new System.Drawing.Point(333, 117);
            this.tb_x3.Name = "tb_x3";
            this.tb_x3.Size = new System.Drawing.Size(100, 20);
            this.tb_x3.TabIndex = 12;
            // 
            // l_X1
            // 
            this.l_X1.AutoSize = true;
            this.l_X1.Location = new System.Drawing.Point(294, 50);
            this.l_X1.Name = "l_X1";
            this.l_X1.Size = new System.Drawing.Size(27, 13);
            this.l_X1.TabIndex = 13;
            this.l_X1.Text = "x1 =";
            // 
            // l_X2
            // 
            this.l_X2.AutoSize = true;
            this.l_X2.Location = new System.Drawing.Point(294, 82);
            this.l_X2.Name = "l_X2";
            this.l_X2.Size = new System.Drawing.Size(27, 13);
            this.l_X2.TabIndex = 14;
            this.l_X2.Text = "x2 =";
            // 
            // l_X3
            // 
            this.l_X3.AutoSize = true;
            this.l_X3.Location = new System.Drawing.Point(294, 120);
            this.l_X3.Name = "l_X3";
            this.l_X3.Size = new System.Drawing.Size(27, 13);
            this.l_X3.TabIndex = 15;
            this.l_X3.Text = "x3 =";
            // 
            // Mainwin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 262);
            this.Controls.Add(this.l_X3);
            this.Controls.Add(this.l_X2);
            this.Controls.Add(this.l_X1);
            this.Controls.Add(this.tb_x3);
            this.Controls.Add(this.tb_x2);
            this.Controls.Add(this.tb_x1);
            this.Controls.Add(this.bt_Calc);
            this.Controls.Add(this.lD);
            this.Controls.Add(this.lC);
            this.Controls.Add(this.lB);
            this.Controls.Add(this.lFormula);
            this.Controls.Add(this.lA);
            this.Controls.Add(this.tb_D);
            this.Controls.Add(this.tb_C);
            this.Controls.Add(this.tb_B);
            this.Controls.Add(this.tb_A);
            this.Name = "Mainwin";
            this.Text = "Cardano";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_A;
        private System.Windows.Forms.TextBox tb_B;
        private System.Windows.Forms.TextBox tb_C;
        private System.Windows.Forms.TextBox tb_D;
        private System.Windows.Forms.Label lA;
        private System.Windows.Forms.Label lFormula;
        private System.Windows.Forms.Label lB;
        private System.Windows.Forms.Label lC;
        private System.Windows.Forms.Label lD;
        private System.Windows.Forms.Button bt_Calc;
        private System.Windows.Forms.TextBox tb_x1;
        private System.Windows.Forms.TextBox tb_x2;
        private System.Windows.Forms.TextBox tb_x3;
        private System.Windows.Forms.Label l_X1;
        private System.Windows.Forms.Label l_X2;
        private System.Windows.Forms.Label l_X3;
    }
}

