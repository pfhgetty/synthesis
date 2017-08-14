namespace EditorsLibrary
{
    partial class ExporterSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExporterSettingsForm));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupboxSkeleton = new System.Windows.Forms.GroupBox();
            this.HighlightParentsCheckBox = new System.Windows.Forms.CheckBox();
            this.ChildHighlight = new System.Windows.Forms.Button();
            this.ParentHighlight = new System.Windows.Forms.Button();
            this.ParentLabel = new System.Windows.Forms.Label();
            this.ChildLabel = new System.Windows.Forms.Label();
            this.groupboxSkeleton.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(13, 118);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(120, 29);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(155, 118);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(120, 29);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // groupboxSkeleton
            // 
            this.groupboxSkeleton.Controls.Add(this.HighlightParentsCheckBox);
            this.groupboxSkeleton.Controls.Add(this.ChildHighlight);
            this.groupboxSkeleton.Controls.Add(this.ParentHighlight);
            this.groupboxSkeleton.Controls.Add(this.ParentLabel);
            this.groupboxSkeleton.Controls.Add(this.ChildLabel);
            this.groupboxSkeleton.ForeColor = System.Drawing.Color.Gray;
            this.groupboxSkeleton.Location = new System.Drawing.Point(11, 9);
            this.groupboxSkeleton.Margin = new System.Windows.Forms.Padding(2);
            this.groupboxSkeleton.Name = "groupboxSkeleton";
            this.groupboxSkeleton.Padding = new System.Windows.Forms.Padding(2);
            this.groupboxSkeleton.Size = new System.Drawing.Size(262, 105);
            this.groupboxSkeleton.TabIndex = 12;
            this.groupboxSkeleton.TabStop = false;
            this.groupboxSkeleton.Text = "Inventor Settings";
            // 
            // HighlightParentsCheckBox
            // 
            this.HighlightParentsCheckBox.AutoSize = true;
            this.HighlightParentsCheckBox.ForeColor = System.Drawing.Color.Black;
            this.HighlightParentsCheckBox.Location = new System.Drawing.Point(2, 83);
            this.HighlightParentsCheckBox.Name = "HighlightParentsCheckBox";
            this.HighlightParentsCheckBox.Size = new System.Drawing.Size(135, 17);
            this.HighlightParentsCheckBox.TabIndex = 10;
            this.HighlightParentsCheckBox.Text = "Highlight Parent Nodes";
            this.HighlightParentsCheckBox.UseVisualStyleBackColor = true;
            this.HighlightParentsCheckBox.CheckedChanged += new System.EventHandler(this.HighlightParentsCheckBox_CheckedChanged);
            // 
            // ChildHighlight
            // 
            this.ChildHighlight.BackColor = System.Drawing.Color.Black;
            this.ChildHighlight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChildHighlight.Location = new System.Drawing.Point(126, 21);
            this.ChildHighlight.Margin = new System.Windows.Forms.Padding(2);
            this.ChildHighlight.Name = "ChildHighlight";
            this.ChildHighlight.Size = new System.Drawing.Size(76, 20);
            this.ChildHighlight.TabIndex = 9;
            this.ChildHighlight.UseVisualStyleBackColor = false;
            this.ChildHighlight.Click += new System.EventHandler(this.ChildHighlight_Click);
            // 
            // ParentHighlight
            // 
            this.ParentHighlight.BackColor = System.Drawing.Color.Black;
            this.ParentHighlight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ParentHighlight.Location = new System.Drawing.Point(126, 51);
            this.ParentHighlight.Margin = new System.Windows.Forms.Padding(2);
            this.ParentHighlight.Name = "ParentHighlight";
            this.ParentHighlight.Size = new System.Drawing.Size(76, 20);
            this.ParentHighlight.TabIndex = 8;
            this.ParentHighlight.UseVisualStyleBackColor = false;
            this.ParentHighlight.Click += new System.EventHandler(this.ParentHighlight_Click);
            // 
            // ParentLabel
            // 
            this.ParentLabel.AutoSize = true;
            this.ParentLabel.ForeColor = System.Drawing.Color.Black;
            this.ParentLabel.Location = new System.Drawing.Point(2, 54);
            this.ParentLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ParentLabel.Name = "ParentLabel";
            this.ParentLabel.Size = new System.Drawing.Size(109, 13);
            this.ParentLabel.TabIndex = 7;
            this.ParentLabel.Text = "Parent Highlight Color";
            // 
            // ChildLabel
            // 
            this.ChildLabel.AutoSize = true;
            this.ChildLabel.ForeColor = System.Drawing.Color.Black;
            this.ChildLabel.Location = new System.Drawing.Point(2, 24);
            this.ChildLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ChildLabel.Name = "ChildLabel";
            this.ChildLabel.Size = new System.Drawing.Size(101, 13);
            this.ChildLabel.TabIndex = 6;
            this.ChildLabel.Text = "Child Highlight Color";
            // 
            // ExporterSettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(286, 162);
            this.Controls.Add(this.groupboxSkeleton);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExporterSettingsForm";
            this.Text = "Exporter Settings";
            this.groupboxSkeleton.ResumeLayout(false);
            this.groupboxSkeleton.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupboxSkeleton;
        private System.Windows.Forms.CheckBox HighlightParentsCheckBox;
        private System.Windows.Forms.Button ChildHighlight;
        private System.Windows.Forms.Button ParentHighlight;
        private System.Windows.Forms.Label ParentLabel;
        private System.Windows.Forms.Label ChildLabel;
    }
}