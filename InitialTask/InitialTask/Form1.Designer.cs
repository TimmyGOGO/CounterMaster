namespace InitialTask
{
    partial class ContourFind
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnPerformHaff = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(400, 400);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnPerformHaff
            // 
            this.btnPerformHaff.Location = new System.Drawing.Point(427, 78);
            this.btnPerformHaff.Name = "btnPerformHaff";
            this.btnPerformHaff.Size = new System.Drawing.Size(265, 61);
            this.btnPerformHaff.TabIndex = 1;
            this.btnPerformHaff.Text = "Perform Haff Transformation";
            this.btnPerformHaff.UseVisualStyleBackColor = true;
            this.btnPerformHaff.Click += new System.EventHandler(this.btnPerformHaff_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(427, 13);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(265, 59);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load Image";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // ContourFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 416);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnPerformHaff);
            this.Controls.Add(this.pictureBox1);
            this.MaximumSize = new System.Drawing.Size(800, 550);
            this.MinimumSize = new System.Drawing.Size(739, 455);
            this.Name = "ContourFind";
            this.Text = "ContourFind";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnPerformHaff;
        private System.Windows.Forms.Button btnLoad;
    }
}

