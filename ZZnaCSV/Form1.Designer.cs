
using System.Windows.Forms;

namespace DekretZSql
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Miesiace = new System.Windows.Forms.ComboBox();
            this.Lata = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Miesiace
            // 
            this.Miesiace.Location = new System.Drawing.Point(317, 53);
            this.Miesiace.Name = "Miesiace";
            this.Miesiace.Size = new System.Drawing.Size(68, 21);
            this.Miesiace.TabIndex = 6;
            // 
            // Lata
            // 
            this.Lata.Location = new System.Drawing.Point(317, 104);
            this.Lata.Name = "Lata";
            this.Lata.Size = new System.Drawing.Size(68, 21);
            this.Lata.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(12, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(297, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Proszę wybrać rok do przeksięgowania VAT: ";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(297, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Proszę wybrać miesiąc do przeksięgowania VAT:  ";
            // 
            // button
            // 
            this.button.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button.Location = new System.Drawing.Point(53, 156);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(298, 46);
            this.button.TabIndex = 4;
            this.button.Text = "Generuj";
            this.button.Click += new System.EventHandler(this.button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 230);
            this.Controls.Add(this.Miesiace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Lata);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generowanie dekretu z VAT";
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.ComboBox Miesiace;
        private Label label1;
        private System.Windows.Forms.ComboBox Lata;
        private Label label2;
        private Button button;
    }
}