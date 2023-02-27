namespace SonicFrontiersPuzzle
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BTopRight = new System.Windows.Forms.TextBox();
            this.ATopLeft = new System.Windows.Forms.TextBox();
            this.DBottomRight = new System.Windows.Forms.TextBox();
            this.CBottomLeft = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.MOD = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BTopRight
            // 
            this.BTopRight.Location = new System.Drawing.Point(444, 64);
            this.BTopRight.Name = "BTopRight";
            this.BTopRight.Size = new System.Drawing.Size(145, 27);
            this.BTopRight.TabIndex = 0;
            this.BTopRight.Text = "1";
            // 
            // ATopLeft
            // 
            this.ATopLeft.Location = new System.Drawing.Point(128, 64);
            this.ATopLeft.Name = "ATopLeft";
            this.ATopLeft.Size = new System.Drawing.Size(125, 27);
            this.ATopLeft.TabIndex = 1;
            this.ATopLeft.Text = "0";
            // 
            // DBottomRight
            // 
            this.DBottomRight.Location = new System.Drawing.Point(444, 233);
            this.DBottomRight.Name = "DBottomRight";
            this.DBottomRight.Size = new System.Drawing.Size(145, 27);
            this.DBottomRight.TabIndex = 2;
            this.DBottomRight.Text = "6";
            // 
            // CBottomLeft
            // 
            this.CBottomLeft.Location = new System.Drawing.Point(128, 233);
            this.CBottomLeft.Name = "CBottomLeft";
            this.CBottomLeft.Size = new System.Drawing.Size(125, 27);
            this.CBottomLeft.TabIndex = 3;
            this.CBottomLeft.Text = "6";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(207, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "a";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(547, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "b";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 263);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "c";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(531, 263);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "d";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 341);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(112, 343);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(634, 27);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = "0 3 4 0 3 4 0 3 4 0 3 4 0 3 4 0 3 4 0 3 4 0 3 4 0 3 4";
            // 
            // MOD
            // 
            this.MOD.Location = new System.Drawing.Point(2, 2);
            this.MOD.Name = "MOD";
            this.MOD.Size = new System.Drawing.Size(125, 27);
            this.MOD.TabIndex = 11;
            this.MOD.Text = "8";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 450);
            this.Controls.Add(this.MOD);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CBottomLeft);
            this.Controls.Add(this.DBottomRight);
            this.Controls.Add(this.ATopLeft);
            this.Controls.Add(this.BTopRight);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox BTopRight;
        private TextBox ATopLeft;
        private TextBox DBottomRight;
        private TextBox CBottomLeft;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button button1;
        private TextBox textBox1;
        private TextBox MOD;
    }
}