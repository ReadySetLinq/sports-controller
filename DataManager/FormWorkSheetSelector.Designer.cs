namespace DataManager
{
    partial class FormWorkSheetSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWorkSheetSelector));
            this.cmbWorksheet = new System.Windows.Forms.ComboBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbWorksheet
            // 
            this.cmbWorksheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorksheet.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbWorksheet.FormattingEnabled = true;
            this.cmbWorksheet.Location = new System.Drawing.Point(0, 0);
            this.cmbWorksheet.Name = "cmbWorksheet";
            this.cmbWorksheet.Size = new System.Drawing.Size(263, 21);
            this.cmbWorksheet.TabIndex = 0;
            // 
            // btnSelect
            // 
            this.btnSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSelect.Location = new System.Drawing.Point(0, 21);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(263, 36);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // FormWorkSheetSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 57);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.cmbWorksheet);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWorkSheetSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Worksheet";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbWorksheet;
        private System.Windows.Forms.Button btnSelect;
    }
}