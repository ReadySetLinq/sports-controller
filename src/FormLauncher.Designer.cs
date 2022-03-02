namespace SportsController
{
    partial class FormLauncher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLauncher));
            this.btnVolleyball = new System.Windows.Forms.Button();
            this.btnBasketball = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // btnVolleyball
            // 
            this.btnVolleyball.BackColor = System.Drawing.SystemColors.Control;
            this.btnVolleyball.ForeColor = System.Drawing.Color.Black;
            this.btnVolleyball.Location = new System.Drawing.Point(12, 70);
            this.btnVolleyball.Name = "btnVolleyball";
            this.btnVolleyball.Size = new System.Drawing.Size(124, 40);
            this.btnVolleyball.TabIndex = 3;
            this.btnVolleyball.Text = "Volleyball Controller";
            this.btnVolleyball.UseVisualStyleBackColor = false;
            this.btnVolleyball.Click += new System.EventHandler(this.btnVolleyball_Click);
            // 
            // btnBasketball
            // 
            this.btnBasketball.BackColor = System.Drawing.SystemColors.Control;
            this.btnBasketball.ForeColor = System.Drawing.Color.Black;
            this.btnBasketball.Location = new System.Drawing.Point(12, 12);
            this.btnBasketball.Name = "btnBasketball";
            this.btnBasketball.Size = new System.Drawing.Size(124, 40);
            this.btnBasketball.TabIndex = 2;
            this.btnBasketball.Text = "Basketball Controller";
            this.btnBasketball.UseVisualStyleBackColor = false;
            this.btnBasketball.Click += new System.EventHandler(this.btnBasketball_Click);
            // 
            // FormLauncher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(149, 124);
            this.Controls.Add(this.btnVolleyball);
            this.Controls.Add(this.btnBasketball);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormLauncher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ready Set Linq";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnVolleyball;
        private System.Windows.Forms.Button btnBasketball;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

