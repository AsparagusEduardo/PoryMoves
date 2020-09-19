namespace moveParser
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLoadFromSerebii = new System.Windows.Forms.Button();
            this.pbar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblLoading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnLoadFromSerebii
            // 
            this.btnLoadFromSerebii.Location = new System.Drawing.Point(12, 12);
            this.btnLoadFromSerebii.Name = "btnLoadFromSerebii";
            this.btnLoadFromSerebii.Size = new System.Drawing.Size(144, 34);
            this.btnLoadFromSerebii.TabIndex = 0;
            this.btnLoadFromSerebii.Text = "Load from Serebii";
            this.btnLoadFromSerebii.UseVisualStyleBackColor = true;
            this.btnLoadFromSerebii.Click += new System.EventHandler(this.btnLoadFromSerebii_Click);
            // 
            // pbar1
            // 
            this.pbar1.Location = new System.Drawing.Point(12, 52);
            this.pbar1.Name = "pbar1";
            this.pbar1.Size = new System.Drawing.Size(776, 23);
            this.pbar1.TabIndex = 1;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // lblLoading
            // 
            this.lblLoading.Location = new System.Drawing.Point(162, 12);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(626, 32);
            this.lblLoading.TabIndex = 2;
            this.lblLoading.Text = "Welcome!";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.pbar1);
            this.Controls.Add(this.btnLoadFromSerebii);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadFromSerebii;
        private System.Windows.Forms.ProgressBar pbar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblLoading;
    }
}

