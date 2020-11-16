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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnLoadFromSerebii = new System.Windows.Forms.Button();
            this.pbar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblLoading = new System.Windows.Forms.Label();
            this.lblOptions = new System.Windows.Forms.Label();
            this.cmbGeneration = new System.Windows.Forms.ComboBox();
            this.btnWriteLvlLearnsets = new System.Windows.Forms.Button();
            this.bwrkExportLvl = new System.ComponentModel.BackgroundWorker();
            this.cListTMMoves = new System.Windows.Forms.CheckedListBox();
            this.gBoxOptionsTM = new System.Windows.Forms.GroupBox();
            this.btnTM_All = new System.Windows.Forms.Button();
            this.chkTM_Extended = new System.Windows.Forms.CheckBox();
            this.chkTM_IncludeTutor = new System.Windows.Forms.CheckBox();
            this.chkTM_IncludeEgg = new System.Windows.Forms.CheckBox();
            this.btnExportTM = new System.Windows.Forms.Button();
            this.chkTM_IncludeLvl = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLvl_All = new System.Windows.Forms.Button();
            this.chkLvl_LevelUpEnd = new System.Windows.Forms.CheckBox();
            this.cListLevelUp = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkEgg_Extended = new System.Windows.Forms.CheckBox();
            this.btnEgg_All = new System.Windows.Forms.Button();
            this.chkEgg_IncludeTutor = new System.Windows.Forms.CheckBox();
            this.chkEgg_IncludeTM = new System.Windows.Forms.CheckBox();
            this.btnExportEgg = new System.Windows.Forms.Button();
            this.chkEgg_IncludeLvl = new System.Windows.Forms.CheckBox();
            this.cListEggMoves = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkTutor_Extended = new System.Windows.Forms.CheckBox();
            this.btnTutor_All = new System.Windows.Forms.Button();
            this.chkTutor_IncludeTM = new System.Windows.Forms.CheckBox();
            this.chkTutor_IncludeEgg = new System.Windows.Forms.CheckBox();
            this.btnExportTutor = new System.Windows.Forms.Button();
            this.chkTutor_IncludeLvl = new System.Windows.Forms.CheckBox();
            this.cListTutorMoves = new System.Windows.Forms.CheckedListBox();
            this.bwrkExportTM = new System.ComponentModel.BackgroundWorker();
            this.bwrkExportTutor = new System.ComponentModel.BackgroundWorker();
            this.bwrkExportEgg = new System.ComponentModel.BackgroundWorker();
            this.gBoxOptionsTM.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoadFromSerebii
            // 
            this.btnLoadFromSerebii.Location = new System.Drawing.Point(616, 7);
            this.btnLoadFromSerebii.Name = "btnLoadFromSerebii";
            this.btnLoadFromSerebii.Size = new System.Drawing.Size(169, 34);
            this.btnLoadFromSerebii.TabIndex = 0;
            this.btnLoadFromSerebii.Text = "Load from Bulbapedia";
            this.btnLoadFromSerebii.UseVisualStyleBackColor = true;
            this.btnLoadFromSerebii.Visible = false;
            this.btnLoadFromSerebii.Click += new System.EventHandler(this.btnLoadFromSerebii_Click);
            // 
            // pbar1
            // 
            this.pbar1.Location = new System.Drawing.Point(355, 12);
            this.pbar1.Name = "pbar1";
            this.pbar1.Size = new System.Drawing.Size(476, 23);
            this.pbar1.TabIndex = 1;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // lblLoading
            // 
            this.lblLoading.Location = new System.Drawing.Point(9, 9);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(340, 32);
            this.lblLoading.TabIndex = 2;
            this.lblLoading.Text = "Welcome!";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOptions
            // 
            this.lblOptions.AutoSize = true;
            this.lblOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOptions.Location = new System.Drawing.Point(9, 44);
            this.lblOptions.Name = "lblOptions";
            this.lblOptions.Size = new System.Drawing.Size(115, 17);
            this.lblOptions.TabIndex = 3;
            this.lblOptions.Text = "Export Options";
            // 
            // cmbGeneration
            // 
            this.cmbGeneration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGeneration.FormattingEnabled = true;
            this.cmbGeneration.Location = new System.Drawing.Point(468, 11);
            this.cmbGeneration.Name = "cmbGeneration";
            this.cmbGeneration.Size = new System.Drawing.Size(92, 24);
            this.cmbGeneration.TabIndex = 5;
            this.cmbGeneration.Visible = false;
            // 
            // btnWriteLvlLearnsets
            // 
            this.btnWriteLvlLearnsets.Location = new System.Drawing.Point(6, 425);
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
            this.cListTMMoves.FormattingEnabled = true;
            this.cListTMMoves.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cListTMMoves.Location = new System.Drawing.Point(6, 25);
            this.cListTMMoves.Name = "cListTMMoves";
            this.cListTMMoves.Size = new System.Drawing.Size(180, 259);
            this.cListTMMoves.TabIndex = 12;
            // 
            // gBoxOptionsTM
            // 
            this.gBoxOptionsTM.Controls.Add(this.btnTM_All);
            this.gBoxOptionsTM.Controls.Add(this.chkTM_Extended);
            this.gBoxOptionsTM.Controls.Add(this.chkTM_IncludeTutor);
            this.gBoxOptionsTM.Controls.Add(this.chkTM_IncludeEgg);
            this.gBoxOptionsTM.Controls.Add(this.btnExportTM);
            this.gBoxOptionsTM.Controls.Add(this.chkTM_IncludeLvl);
            this.gBoxOptionsTM.Controls.Add(this.cListTMMoves);
            this.gBoxOptionsTM.Location = new System.Drawing.Point(220, 67);
            this.gBoxOptionsTM.Name = "gBoxOptionsTM";
            this.gBoxOptionsTM.Size = new System.Drawing.Size(199, 458);
            this.gBoxOptionsTM.TabIndex = 13;
            this.gBoxOptionsTM.TabStop = false;
            this.gBoxOptionsTM.Text = "TM/HM/TR Moves";
            // 
            // btnTM_All
            // 
            this.btnTM_All.Location = new System.Drawing.Point(6, 284);
            this.btnTM_All.Name = "btnTM_All";
            this.btnTM_All.Size = new System.Drawing.Size(180, 27);
            this.btnTM_All.TabIndex = 19;
            this.btnTM_All.Text = "Select All";
            this.btnTM_All.UseVisualStyleBackColor = true;
            this.btnTM_All.Click += new System.EventHandler(this.btnTM_All_Click);
            // 
            // chkTM_Extended
            // 
            this.chkTM_Extended.AutoSize = true;
            this.chkTM_Extended.Location = new System.Drawing.Point(6, 317);
            this.chkTM_Extended.Name = "chkTM_Extended";
            this.chkTM_Extended.Size = new System.Drawing.Size(149, 21);
            this.chkTM_Extended.TabIndex = 17;
            this.chkTM_Extended.Text = "Use Extended TMs";
            this.chkTM_Extended.UseVisualStyleBackColor = true;
            // 
            // chkTM_IncludeTutor
            // 
            this.chkTM_IncludeTutor.AutoSize = true;
            this.chkTM_IncludeTutor.Location = new System.Drawing.Point(6, 398);
            this.chkTM_IncludeTutor.Name = "chkTM_IncludeTutor";
            this.chkTM_IncludeTutor.Size = new System.Drawing.Size(158, 21);
            this.chkTM_IncludeTutor.TabIndex = 16;
            this.chkTM_IncludeTutor.Text = "Include Tutor Moves";
            this.chkTM_IncludeTutor.UseVisualStyleBackColor = true;
            // 
            // chkTM_IncludeEgg
            // 
            this.chkTM_IncludeEgg.AutoSize = true;
            this.chkTM_IncludeEgg.Location = new System.Drawing.Point(6, 371);
            this.chkTM_IncludeEgg.Name = "chkTM_IncludeEgg";
            this.chkTM_IncludeEgg.Size = new System.Drawing.Size(149, 21);
            this.chkTM_IncludeEgg.TabIndex = 15;
            this.chkTM_IncludeEgg.Text = "Include Egg Moves";
            this.chkTM_IncludeEgg.UseVisualStyleBackColor = true;
            // 
            // btnExportTM
            // 
            this.btnExportTM.Location = new System.Drawing.Point(6, 425);
            this.btnExportTM.Name = "btnExportTM";
            this.btnExportTM.Size = new System.Drawing.Size(180, 27);
            this.btnExportTM.TabIndex = 14;
            this.btnExportTM.Text = "Export TM Moves";
            this.btnExportTM.UseVisualStyleBackColor = true;
            this.btnExportTM.Click += new System.EventHandler(this.btnExportTM_Click);
            // 
            // chkTM_IncludeLvl
            // 
            this.chkTM_IncludeLvl.AutoSize = true;
            this.chkTM_IncludeLvl.Location = new System.Drawing.Point(6, 344);
            this.chkTM_IncludeLvl.Name = "chkTM_IncludeLvl";
            this.chkTM_IncludeLvl.Size = new System.Drawing.Size(180, 21);
            this.chkTM_IncludeLvl.TabIndex = 13;
            this.chkTM_IncludeLvl.Text = "Include Level Up Moves";
            this.chkTM_IncludeLvl.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnLvl_All);
            this.groupBox1.Controls.Add(this.chkLvl_LevelUpEnd);
            this.groupBox1.Controls.Add(this.cListLevelUp);
            this.groupBox1.Controls.Add(this.btnWriteLvlLearnsets);
            this.groupBox1.Location = new System.Drawing.Point(15, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(199, 458);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Level Up Moves";
            // 
            // btnLvl_All
            // 
            this.btnLvl_All.Location = new System.Drawing.Point(0, 284);
            this.btnLvl_All.Name = "btnLvl_All";
            this.btnLvl_All.Size = new System.Drawing.Size(180, 27);
            this.btnLvl_All.TabIndex = 18;
            this.btnLvl_All.Text = "Select All";
            this.btnLvl_All.UseVisualStyleBackColor = true;
            this.btnLvl_All.Click += new System.EventHandler(this.btnLvl_All_Click);
            // 
            // chkLvl_LevelUpEnd
            // 
            this.chkLvl_LevelUpEnd.AutoSize = true;
            this.chkLvl_LevelUpEnd.Location = new System.Drawing.Point(6, 317);
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
            this.cListLevelUp.Size = new System.Drawing.Size(180, 259);
            this.cListLevelUp.TabIndex = 12;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkEgg_Extended);
            this.groupBox2.Controls.Add(this.btnEgg_All);
            this.groupBox2.Controls.Add(this.chkEgg_IncludeTutor);
            this.groupBox2.Controls.Add(this.chkEgg_IncludeTM);
            this.groupBox2.Controls.Add(this.btnExportEgg);
            this.groupBox2.Controls.Add(this.chkEgg_IncludeLvl);
            this.groupBox2.Controls.Add(this.cListEggMoves);
            this.groupBox2.Location = new System.Drawing.Point(425, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(199, 458);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Egg Moves";
            // 
            // chkEgg_Extended
            // 
            this.chkEgg_Extended.AutoSize = true;
            this.chkEgg_Extended.Location = new System.Drawing.Point(6, 317);
            this.chkEgg_Extended.Name = "chkEgg_Extended";
            this.chkEgg_Extended.Size = new System.Drawing.Size(119, 21);
            this.chkEgg_Extended.TabIndex = 20;
            this.chkEgg_Extended.Text = "Use new Style";
            this.chkEgg_Extended.UseVisualStyleBackColor = true;
            // 
            // btnEgg_All
            // 
            this.btnEgg_All.Location = new System.Drawing.Point(6, 284);
            this.btnEgg_All.Name = "btnEgg_All";
            this.btnEgg_All.Size = new System.Drawing.Size(180, 27);
            this.btnEgg_All.TabIndex = 20;
            this.btnEgg_All.Text = "Select All";
            this.btnEgg_All.UseVisualStyleBackColor = true;
            this.btnEgg_All.Click += new System.EventHandler(this.btnEgg_All_Click);
            // 
            // chkEgg_IncludeTutor
            // 
            this.chkEgg_IncludeTutor.AutoSize = true;
            this.chkEgg_IncludeTutor.Location = new System.Drawing.Point(6, 398);
            this.chkEgg_IncludeTutor.Name = "chkEgg_IncludeTutor";
            this.chkEgg_IncludeTutor.Size = new System.Drawing.Size(158, 21);
            this.chkEgg_IncludeTutor.TabIndex = 16;
            this.chkEgg_IncludeTutor.Text = "Include Tutor Moves";
            this.chkEgg_IncludeTutor.UseVisualStyleBackColor = true;
            // 
            // chkEgg_IncludeTM
            // 
            this.chkEgg_IncludeTM.AutoSize = true;
            this.chkEgg_IncludeTM.Location = new System.Drawing.Point(6, 371);
            this.chkEgg_IncludeTM.Name = "chkEgg_IncludeTM";
            this.chkEgg_IncludeTM.Size = new System.Drawing.Size(144, 21);
            this.chkEgg_IncludeTM.TabIndex = 15;
            this.chkEgg_IncludeTM.Text = "Include TM Moves";
            this.chkEgg_IncludeTM.UseVisualStyleBackColor = true;
            // 
            // btnExportEgg
            // 
            this.btnExportEgg.Location = new System.Drawing.Point(6, 425);
            this.btnExportEgg.Name = "btnExportEgg";
            this.btnExportEgg.Size = new System.Drawing.Size(180, 27);
            this.btnExportEgg.TabIndex = 14;
            this.btnExportEgg.Text = "Export Egg Moves";
            this.btnExportEgg.UseVisualStyleBackColor = true;
            this.btnExportEgg.Click += new System.EventHandler(this.btnExportEgg_Click);
            // 
            // chkEgg_IncludeLvl
            // 
            this.chkEgg_IncludeLvl.AutoSize = true;
            this.chkEgg_IncludeLvl.Location = new System.Drawing.Point(6, 344);
            this.chkEgg_IncludeLvl.Name = "chkEgg_IncludeLvl";
            this.chkEgg_IncludeLvl.Size = new System.Drawing.Size(180, 21);
            this.chkEgg_IncludeLvl.TabIndex = 13;
            this.chkEgg_IncludeLvl.Text = "Include Level Up Moves";
            this.chkEgg_IncludeLvl.UseVisualStyleBackColor = true;
            // 
            // cListEggMoves
            // 
            this.cListEggMoves.CheckOnClick = true;
            this.cListEggMoves.FormattingEnabled = true;
            this.cListEggMoves.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cListEggMoves.Location = new System.Drawing.Point(6, 25);
            this.cListEggMoves.Name = "cListEggMoves";
            this.cListEggMoves.Size = new System.Drawing.Size(180, 259);
            this.cListEggMoves.TabIndex = 12;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkTutor_Extended);
            this.groupBox3.Controls.Add(this.btnTutor_All);
            this.groupBox3.Controls.Add(this.chkTutor_IncludeTM);
            this.groupBox3.Controls.Add(this.chkTutor_IncludeEgg);
            this.groupBox3.Controls.Add(this.btnExportTutor);
            this.groupBox3.Controls.Add(this.chkTutor_IncludeLvl);
            this.groupBox3.Controls.Add(this.cListTutorMoves);
            this.groupBox3.Location = new System.Drawing.Point(630, 67);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(199, 458);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tutor Moves";
            // 
            // chkTutor_Extended
            // 
            this.chkTutor_Extended.AutoSize = true;
            this.chkTutor_Extended.Location = new System.Drawing.Point(6, 317);
            this.chkTutor_Extended.Name = "chkTutor_Extended";
            this.chkTutor_Extended.Size = new System.Drawing.Size(163, 21);
            this.chkTutor_Extended.TabIndex = 22;
            this.chkTutor_Extended.Text = "Use Extended Tutors";
            this.chkTutor_Extended.UseVisualStyleBackColor = true;
            // 
            // btnTutor_All
            // 
            this.btnTutor_All.Location = new System.Drawing.Point(6, 284);
            this.btnTutor_All.Name = "btnTutor_All";
            this.btnTutor_All.Size = new System.Drawing.Size(180, 27);
            this.btnTutor_All.TabIndex = 21;
            this.btnTutor_All.Text = "Select All";
            this.btnTutor_All.UseVisualStyleBackColor = true;
            this.btnTutor_All.Click += new System.EventHandler(this.btnTutor_All_Click);
            // 
            // chkTutor_IncludeTM
            // 
            this.chkTutor_IncludeTM.AutoSize = true;
            this.chkTutor_IncludeTM.Location = new System.Drawing.Point(6, 398);
            this.chkTutor_IncludeTM.Name = "chkTutor_IncludeTM";
            this.chkTutor_IncludeTM.Size = new System.Drawing.Size(144, 21);
            this.chkTutor_IncludeTM.TabIndex = 16;
            this.chkTutor_IncludeTM.Text = "Include TM Moves";
            this.chkTutor_IncludeTM.UseVisualStyleBackColor = true;
            // 
            // chkTutor_IncludeEgg
            // 
            this.chkTutor_IncludeEgg.AutoSize = true;
            this.chkTutor_IncludeEgg.Location = new System.Drawing.Point(6, 371);
            this.chkTutor_IncludeEgg.Name = "chkTutor_IncludeEgg";
            this.chkTutor_IncludeEgg.Size = new System.Drawing.Size(149, 21);
            this.chkTutor_IncludeEgg.TabIndex = 15;
            this.chkTutor_IncludeEgg.Text = "Include Egg Moves";
            this.chkTutor_IncludeEgg.UseVisualStyleBackColor = true;
            // 
            // btnExportTutor
            // 
            this.btnExportTutor.Location = new System.Drawing.Point(6, 425);
            this.btnExportTutor.Name = "btnExportTutor";
            this.btnExportTutor.Size = new System.Drawing.Size(180, 27);
            this.btnExportTutor.TabIndex = 14;
            this.btnExportTutor.Text = "Export Tutor Moves";
            this.btnExportTutor.UseVisualStyleBackColor = true;
            this.btnExportTutor.Click += new System.EventHandler(this.btnExportTutor_Click);
            // 
            // chkTutor_IncludeLvl
            // 
            this.chkTutor_IncludeLvl.AutoSize = true;
            this.chkTutor_IncludeLvl.Location = new System.Drawing.Point(6, 344);
            this.chkTutor_IncludeLvl.Name = "chkTutor_IncludeLvl";
            this.chkTutor_IncludeLvl.Size = new System.Drawing.Size(180, 21);
            this.chkTutor_IncludeLvl.TabIndex = 13;
            this.chkTutor_IncludeLvl.Text = "Include Level Up Moves";
            this.chkTutor_IncludeLvl.UseVisualStyleBackColor = true;
            // 
            // cListTutorMoves
            // 
            this.cListTutorMoves.CheckOnClick = true;
            this.cListTutorMoves.FormattingEnabled = true;
            this.cListTutorMoves.Items.AddRange(new object[] {
            "SWSH",
            "USUM"});
            this.cListTutorMoves.Location = new System.Drawing.Point(6, 25);
            this.cListTutorMoves.Name = "cListTutorMoves";
            this.cListTutorMoves.Size = new System.Drawing.Size(180, 259);
            this.cListTutorMoves.TabIndex = 12;
            // 
            // bwrkExportTM
            // 
            this.bwrkExportTM.WorkerReportsProgress = true;
            this.bwrkExportTM.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwrkExportTM_DoWork);
            this.bwrkExportTM.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.bwrkExportTM.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwrkGroupMovesets_tm_RunWorkerCompleted);
            // 
            // bwrkExportTutor
            // 
            this.bwrkExportTutor.WorkerReportsProgress = true;
            this.bwrkExportTutor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwrkExportTutor_DoWork);
            this.bwrkExportTutor.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // bwrkExportEgg
            // 
            this.bwrkExportEgg.WorkerReportsProgress = true;
            this.bwrkExportEgg.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwrkExportEgg_DoWork);
            this.bwrkExportEgg.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 537);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gBoxOptionsTM);
            this.Controls.Add(this.cmbGeneration);
            this.Controls.Add(this.lblOptions);
            this.Controls.Add(this.btnLoadFromSerebii);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.pbar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "PoryMoves 1.1.0";
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
        private System.Windows.Forms.CheckBox chkEgg_IncludeTutor;
        private System.Windows.Forms.CheckBox chkEgg_IncludeTM;
        private System.Windows.Forms.Button btnExportEgg;
        private System.Windows.Forms.CheckBox chkEgg_IncludeLvl;
        private System.Windows.Forms.CheckedListBox cListEggMoves;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkTutor_IncludeTM;
        private System.Windows.Forms.CheckBox chkTutor_IncludeEgg;
        private System.Windows.Forms.Button btnExportTutor;
        private System.Windows.Forms.CheckBox chkTutor_IncludeLvl;
        private System.Windows.Forms.CheckedListBox cListTutorMoves;
        private System.ComponentModel.BackgroundWorker bwrkExportTM;
        private System.Windows.Forms.CheckBox chkTM_Extended;
        private System.Windows.Forms.Button btnLvl_All;
        private System.Windows.Forms.Button btnTM_All;
        private System.Windows.Forms.Button btnEgg_All;
        private System.Windows.Forms.Button btnTutor_All;
        private System.Windows.Forms.CheckBox chkTutor_Extended;
        private System.ComponentModel.BackgroundWorker bwrkExportTutor;
        private System.ComponentModel.BackgroundWorker bwrkExportEgg;
        private System.Windows.Forms.CheckBox chkEgg_Extended;
    }
}

