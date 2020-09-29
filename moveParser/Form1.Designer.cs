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
            this.lblOptions = new System.Windows.Forms.Label();
            this.cmbGeneration = new System.Windows.Forms.ComboBox();
            this.lblBaseMovesets = new System.Windows.Forms.Label();
            this.btnWriteLvlLearnsets = new System.Windows.Forms.Button();
            this.bwrkExportLvl = new System.ComponentModel.BackgroundWorker();
            this.cListTMMoves = new System.Windows.Forms.CheckedListBox();
            this.gBoxOptionsTM = new System.Windows.Forms.GroupBox();
            this.chkTM_IncludeTutor = new System.Windows.Forms.CheckBox();
            this.chkTM_IncludeEgg = new System.Windows.Forms.CheckBox();
            this.btnExportTM = new System.Windows.Forms.Button();
            this.chkTM_IncludeLvl = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkLvl_LevelUpEnd = new System.Windows.Forms.CheckBox();
            this.cListLevelUp = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.cListEggMoves = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.cListTutorMoves = new System.Windows.Forms.CheckedListBox();
            this.gBoxOptionsTM.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadFromSerebii
            // 
            this.btnLoadFromSerebii.Location = new System.Drawing.Point(660, 4);
            this.btnLoadFromSerebii.Name = "btnLoadFromSerebii";
            this.btnLoadFromSerebii.Size = new System.Drawing.Size(169, 34);
            this.btnLoadFromSerebii.TabIndex = 0;
            this.btnLoadFromSerebii.Text = "Load from Bulbapedia";
            this.btnLoadFromSerebii.UseVisualStyleBackColor = true;
            this.btnLoadFromSerebii.Click += new System.EventHandler(this.btnLoadFromSerebii_Click);
            // 
            // pbar1
            // 
            this.pbar1.Location = new System.Drawing.Point(12, 44);
            this.pbar1.Name = "pbar1";
            this.pbar1.Size = new System.Drawing.Size(817, 23);
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
            this.lblLoading.Location = new System.Drawing.Point(260, 6);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(378, 32);
            this.lblLoading.TabIndex = 2;
            this.lblLoading.Text = "Welcome!";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOptions
            // 
            this.lblOptions.AutoSize = true;
            this.lblOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOptions.Location = new System.Drawing.Point(9, 79);
            this.lblOptions.Name = "lblOptions";
            this.lblOptions.Size = new System.Drawing.Size(115, 17);
            this.lblOptions.TabIndex = 3;
            this.lblOptions.Text = "Export Options";
            // 
            // cmbGeneration
            // 
            this.cmbGeneration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGeneration.FormattingEnabled = true;
            this.cmbGeneration.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cmbGeneration.Location = new System.Drawing.Point(162, 10);
            this.cmbGeneration.Name = "cmbGeneration";
            this.cmbGeneration.Size = new System.Drawing.Size(92, 24);
            this.cmbGeneration.TabIndex = 5;
            // 
            // lblBaseMovesets
            // 
            this.lblBaseMovesets.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBaseMovesets.Location = new System.Drawing.Point(12, 13);
            this.lblBaseMovesets.Name = "lblBaseMovesets";
            this.lblBaseMovesets.Size = new System.Drawing.Size(144, 23);
            this.lblBaseMovesets.TabIndex = 6;
            this.lblBaseMovesets.Text = "Load moves from:";
            // 
            // btnWriteLvlLearnsets
            // 
            this.btnWriteLvlLearnsets.Location = new System.Drawing.Point(6, 291);
            this.btnWriteLvlLearnsets.Name = "btnWriteLvlLearnsets";
            this.btnWriteLvlLearnsets.Size = new System.Drawing.Size(180, 27);
            this.btnWriteLvlLearnsets.TabIndex = 7;
            this.btnWriteLvlLearnsets.Text = "Export Level Up Moves";
            this.btnWriteLvlLearnsets.UseVisualStyleBackColor = true;
            this.btnWriteLvlLearnsets.Click += new System.EventHandler(this.btnWriteLvlLearnsets_Click);
            // 
            // bwrkExportLvl
            // 
            this.bwrkExportLvl.WorkerReportsProgress = true;
            this.bwrkExportLvl.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwrkExportLvl_DoWork);
            this.bwrkExportLvl.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // cListTMMoves
            // 
            this.cListTMMoves.CheckOnClick = true;
            this.cListTMMoves.Enabled = false;
            this.cListTMMoves.FormattingEnabled = true;
            this.cListTMMoves.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cListTMMoves.Location = new System.Drawing.Point(6, 25);
            this.cListTMMoves.Name = "cListTMMoves";
            this.cListTMMoves.Size = new System.Drawing.Size(180, 174);
            this.cListTMMoves.TabIndex = 12;
            // 
            // gBoxOptionsTM
            // 
            this.gBoxOptionsTM.Controls.Add(this.chkTM_IncludeTutor);
            this.gBoxOptionsTM.Controls.Add(this.chkTM_IncludeEgg);
            this.gBoxOptionsTM.Controls.Add(this.btnExportTM);
            this.gBoxOptionsTM.Controls.Add(this.chkTM_IncludeLvl);
            this.gBoxOptionsTM.Controls.Add(this.cListTMMoves);
            this.gBoxOptionsTM.Location = new System.Drawing.Point(220, 108);
            this.gBoxOptionsTM.Name = "gBoxOptionsTM";
            this.gBoxOptionsTM.Size = new System.Drawing.Size(199, 330);
            this.gBoxOptionsTM.TabIndex = 13;
            this.gBoxOptionsTM.TabStop = false;
            this.gBoxOptionsTM.Text = "TM/HM/TR Moves";
            // 
            // chkTM_IncludeTutor
            // 
            this.chkTM_IncludeTutor.AutoSize = true;
            this.chkTM_IncludeTutor.Enabled = false;
            this.chkTM_IncludeTutor.Location = new System.Drawing.Point(6, 260);
            this.chkTM_IncludeTutor.Name = "chkTM_IncludeTutor";
            this.chkTM_IncludeTutor.Size = new System.Drawing.Size(158, 21);
            this.chkTM_IncludeTutor.TabIndex = 16;
            this.chkTM_IncludeTutor.Text = "Include Tutor Moves";
            this.chkTM_IncludeTutor.UseVisualStyleBackColor = true;
            // 
            // chkTM_IncludeEgg
            // 
            this.chkTM_IncludeEgg.AutoSize = true;
            this.chkTM_IncludeEgg.Enabled = false;
            this.chkTM_IncludeEgg.Location = new System.Drawing.Point(6, 232);
            this.chkTM_IncludeEgg.Name = "chkTM_IncludeEgg";
            this.chkTM_IncludeEgg.Size = new System.Drawing.Size(149, 21);
            this.chkTM_IncludeEgg.TabIndex = 15;
            this.chkTM_IncludeEgg.Text = "Include Egg Moves";
            this.chkTM_IncludeEgg.UseVisualStyleBackColor = true;
            // 
            // btnExportTM
            // 
            this.btnExportTM.Enabled = false;
            this.btnExportTM.Location = new System.Drawing.Point(6, 291);
            this.btnExportTM.Name = "btnExportTM";
            this.btnExportTM.Size = new System.Drawing.Size(180, 27);
            this.btnExportTM.TabIndex = 14;
            this.btnExportTM.Text = "Export TM Moves";
            this.btnExportTM.UseVisualStyleBackColor = true;
            // 
            // chkTM_IncludeLvl
            // 
            this.chkTM_IncludeLvl.AutoSize = true;
            this.chkTM_IncludeLvl.Enabled = false;
            this.chkTM_IncludeLvl.Location = new System.Drawing.Point(6, 205);
            this.chkTM_IncludeLvl.Name = "chkTM_IncludeLvl";
            this.chkTM_IncludeLvl.Size = new System.Drawing.Size(180, 21);
            this.chkTM_IncludeLvl.TabIndex = 13;
            this.chkTM_IncludeLvl.Text = "Include Level Up Moves";
            this.chkTM_IncludeLvl.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkLvl_LevelUpEnd);
            this.groupBox1.Controls.Add(this.cListLevelUp);
            this.groupBox1.Controls.Add(this.btnWriteLvlLearnsets);
            this.groupBox1.Location = new System.Drawing.Point(15, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(199, 330);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Level Up Moves";
            // 
            // chkLvl_LevelUpEnd
            // 
            this.chkLvl_LevelUpEnd.AutoSize = true;
            this.chkLvl_LevelUpEnd.Location = new System.Drawing.Point(6, 205);
            this.chkLvl_LevelUpEnd.Name = "chkLvl_LevelUpEnd";
            this.chkLvl_LevelUpEnd.Size = new System.Drawing.Size(166, 21);
            this.chkLvl_LevelUpEnd.TabIndex = 17;
            this.chkLvl_LevelUpEnd.Text = "Add LEVEL_UP_END";
            this.chkLvl_LevelUpEnd.UseVisualStyleBackColor = true;
            // 
            // cListLevelUp
            // 
            this.cListLevelUp.CheckOnClick = true;
            this.cListLevelUp.FormattingEnabled = true;
            this.cListLevelUp.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cListLevelUp.Location = new System.Drawing.Point(6, 25);
            this.cListLevelUp.Name = "cListLevelUp";
            this.cListLevelUp.Size = new System.Drawing.Size(180, 174);
            this.cListLevelUp.TabIndex = 12;
            this.cListLevelUp.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cListLevelUp_ItemCheck);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox4);
            this.groupBox2.Controls.Add(this.checkBox5);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.checkBox6);
            this.groupBox2.Controls.Add(this.cListEggMoves);
            this.groupBox2.Location = new System.Drawing.Point(425, 108);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(199, 330);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Egg Moves";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Enabled = false;
            this.checkBox4.Location = new System.Drawing.Point(6, 260);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(158, 21);
            this.checkBox4.TabIndex = 16;
            this.checkBox4.Text = "Include Tutor Moves";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Enabled = false;
            this.checkBox5.Location = new System.Drawing.Point(6, 232);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(144, 21);
            this.checkBox5.TabIndex = 15;
            this.checkBox5.Text = "Include TM Moves";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(6, 291);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 27);
            this.button1.TabIndex = 14;
            this.button1.Text = "Export Egg Moves";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Enabled = false;
            this.checkBox6.Location = new System.Drawing.Point(6, 205);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(180, 21);
            this.checkBox6.TabIndex = 13;
            this.checkBox6.Text = "Include Level Up Moves";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // cListEggMoves
            // 
            this.cListEggMoves.CheckOnClick = true;
            this.cListEggMoves.Enabled = false;
            this.cListEggMoves.FormattingEnabled = true;
            this.cListEggMoves.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cListEggMoves.Location = new System.Drawing.Point(6, 25);
            this.cListEggMoves.Name = "cListEggMoves";
            this.cListEggMoves.Size = new System.Drawing.Size(180, 174);
            this.cListEggMoves.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox7);
            this.groupBox3.Controls.Add(this.checkBox8);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.checkBox9);
            this.groupBox3.Controls.Add(this.cListTutorMoves);
            this.groupBox3.Location = new System.Drawing.Point(630, 108);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(199, 330);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tutor Moves";
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Enabled = false;
            this.checkBox7.Location = new System.Drawing.Point(6, 260);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(144, 21);
            this.checkBox7.TabIndex = 16;
            this.checkBox7.Text = "Include TM Moves";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Enabled = false;
            this.checkBox8.Location = new System.Drawing.Point(6, 232);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(149, 21);
            this.checkBox8.TabIndex = 15;
            this.checkBox8.Text = "Include Egg Moves";
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(6, 291);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(180, 27);
            this.button2.TabIndex = 14;
            this.button2.Text = "Export Tutor Moves";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Enabled = false;
            this.checkBox9.Location = new System.Drawing.Point(6, 205);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(180, 21);
            this.checkBox9.TabIndex = 13;
            this.checkBox9.Text = "Include Level Up Moves";
            this.checkBox9.UseVisualStyleBackColor = true;
            // 
            // cListTutorMoves
            // 
            this.cListTutorMoves.CheckOnClick = true;
            this.cListTutorMoves.Enabled = false;
            this.cListTutorMoves.FormattingEnabled = true;
            this.cListTutorMoves.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cListTutorMoves.Location = new System.Drawing.Point(6, 25);
            this.cListTutorMoves.Name = "cListTutorMoves";
            this.cListTutorMoves.Size = new System.Drawing.Size(180, 174);
            this.cListTutorMoves.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 450);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gBoxOptionsTM);
            this.Controls.Add(this.lblBaseMovesets);
            this.Controls.Add(this.cmbGeneration);
            this.Controls.Add(this.lblOptions);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.pbar1);
            this.Controls.Add(this.btnLoadFromSerebii);
            this.Name = "Form1";
            this.Text = "Form1";
            this.gBoxOptionsTM.ResumeLayout(false);
            this.gBoxOptionsTM.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadFromSerebii;
        private System.Windows.Forms.ProgressBar pbar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Label lblOptions;
        private System.Windows.Forms.ComboBox cmbGeneration;
        private System.Windows.Forms.Label lblBaseMovesets;
        private System.Windows.Forms.Button btnWriteLvlLearnsets;
        private System.ComponentModel.BackgroundWorker bwrkExportLvl;
        private System.Windows.Forms.CheckedListBox cListTMMoves;
        private System.Windows.Forms.GroupBox gBoxOptionsTM;
        private System.Windows.Forms.CheckBox chkTM_IncludeLvl;
        private System.Windows.Forms.Button btnExportTM;
        private System.Windows.Forms.CheckBox chkTM_IncludeEgg;
        private System.Windows.Forms.CheckBox chkTM_IncludeTutor;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox cListLevelUp;
        private System.Windows.Forms.CheckBox chkLvl_LevelUpEnd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckedListBox cListEggMoves;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.CheckedListBox cListTutorMoves;
    }
}

