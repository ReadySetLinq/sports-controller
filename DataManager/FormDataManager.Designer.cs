namespace DataManager
{
    partial class FormDataManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyleMain = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyleAlternate = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDataManager));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dataGridManage = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridManage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSave.Location = new System.Drawing.Point(0, 425);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(782, 38);
            this.btnSave.TabIndex = 36;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnLoad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLoad.Location = new System.Drawing.Point(0, 0);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(782, 38);
            this.btnLoad.TabIndex = 37;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // dataGridManage
            // 
            this.dataGridManage.AllowDrop = true;
            dataGridViewCellStyleAlternate.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyleAlternate.BackColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyleAlternate.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyleAlternate.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyleAlternate.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridManage.AlternatingRowsDefaultCellStyle = dataGridViewCellStyleAlternate;
            this.dataGridManage.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridManage.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyleMain.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyleMain.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyleMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyleMain.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyleMain.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyleMain.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyleMain.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridManage.BackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.dataGridManage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridManage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridManage.EnableHeadersVisualStyles = false;
            this.dataGridManage.Location = new System.Drawing.Point(0, 38);
            this.dataGridManage.MultiSelect = false;
            this.dataGridManage.Name = "dataGridManage";
            this.dataGridManage.RowsDefaultCellStyle = dataGridViewCellStyleMain;
            this.dataGridManage.Size = new System.Drawing.Size(782, 387);
            this.dataGridManage.TabIndex = 38;
            this.dataGridManage.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridManage_CellEndEdit);
            // 
            // FormDataManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(782, 463);
            this.Controls.Add(this.dataGridManage);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormDataManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReadySetLinq - DataManager";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridManage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dataGridManage;
    }
}

