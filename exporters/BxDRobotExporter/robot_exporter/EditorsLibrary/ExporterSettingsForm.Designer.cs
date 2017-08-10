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
            this.groupboxGeneral = new System.Windows.Forms.GroupBox();
            this.buttonChooseFolder = new System.Windows.Forms.Button();
            this.buttonChooseText = new System.Windows.Forms.Button();
            this.buttonChooseBackground = new System.Windows.Forms.Button();
            this.labelBackgroundColor = new System.Windows.Forms.Label();
            this.labelTextColor = new System.Windows.Forms.Label();
            this.checkboxSaveLog = new System.Windows.Forms.CheckBox();
            this.textboxLogLocation = new System.Windows.Forms.TextBox();
            this.groupboxSkeleton.SuspendLayout();
            this.groupboxGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(11, 254);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(120, 29);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(153, 254);
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
            this.groupboxSkeleton.Location = new System.Drawing.Point(11, 145);
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
            // groupboxGeneral
            // 
            this.groupboxGeneral.Controls.Add(this.buttonChooseFolder);
            this.groupboxGeneral.Controls.Add(this.buttonChooseText);
            this.groupboxGeneral.Controls.Add(this.buttonChooseBackground);
            this.groupboxGeneral.Controls.Add(this.labelBackgroundColor);
            this.groupboxGeneral.Controls.Add(this.labelTextColor);
            this.groupboxGeneral.Controls.Add(this.checkboxSaveLog);
            this.groupboxGeneral.Controls.Add(this.textboxLogLocation);
            this.groupboxGeneral.ForeColor = System.Drawing.Color.Gray;
            this.groupboxGeneral.Location = new System.Drawing.Point(11, 11);
            this.groupboxGeneral.Margin = new System.Windows.Forms.Padding(2);
            this.groupboxGeneral.Name = "groupboxGeneral";
            this.groupboxGeneral.Padding = new System.Windows.Forms.Padding(2);
            this.groupboxGeneral.Size = new System.Drawing.Size(262, 129);
            this.groupboxGeneral.TabIndex = 13;
            this.groupboxGeneral.TabStop = false;
            this.groupboxGeneral.Text = "General options";
            // 
            // buttonChooseFolder
            // 
            this.buttonChooseFolder.ForeColor = System.Drawing.Color.Black;
            this.buttonChooseFolder.Location = new System.Drawing.Point(236, 41);
            this.buttonChooseFolder.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChooseFolder.Name = "buttonChooseFolder";
            this.buttonChooseFolder.Size = new System.Drawing.Size(22, 18);
            this.buttonChooseFolder.TabIndex = 6;
            this.buttonChooseFolder.Text = "...";
            this.buttonChooseFolder.UseVisualStyleBackColor = true;
            this.buttonChooseFolder.Click += new System.EventHandler(this.buttonChooseFolder_Click);
            // 
            // buttonChooseText
            // 
            this.buttonChooseText.BackColor = System.Drawing.Color.Black;
            this.buttonChooseText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonChooseText.Location = new System.Drawing.Point(126, 72);
            this.buttonChooseText.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChooseText.Name = "buttonChooseText";
            this.buttonChooseText.Size = new System.Drawing.Size(76, 20);
            this.buttonChooseText.TabIndex = 5;
            this.buttonChooseText.UseVisualStyleBackColor = false;
            this.buttonChooseText.Click += new System.EventHandler(this.buttonChooseText_Click);
            // 
            // buttonChooseBackground
            // 
            this.buttonChooseBackground.BackColor = System.Drawing.Color.Black;
            this.buttonChooseBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonChooseBackground.Location = new System.Drawing.Point(126, 102);
            this.buttonChooseBackground.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChooseBackground.Name = "buttonChooseBackground";
            this.buttonChooseBackground.Size = new System.Drawing.Size(76, 20);
            this.buttonChooseBackground.TabIndex = 4;
            this.buttonChooseBackground.UseVisualStyleBackColor = false;
            this.buttonChooseBackground.Click += new System.EventHandler(this.buttonChooseBackground_Click);
            // 
            // labelBackgroundColor
            // 
            this.labelBackgroundColor.AutoSize = true;
            this.labelBackgroundColor.ForeColor = System.Drawing.Color.Black;
            this.labelBackgroundColor.Location = new System.Drawing.Point(2, 106);
            this.labelBackgroundColor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBackgroundColor.Name = "labelBackgroundColor";
            this.labelBackgroundColor.Size = new System.Drawing.Size(114, 13);
            this.labelBackgroundColor.TabIndex = 3;
            this.labelBackgroundColor.Text = "Log background color:";
            // 
            // labelTextColor
            // 
            this.labelTextColor.AutoSize = true;
            this.labelTextColor.ForeColor = System.Drawing.Color.Black;
            this.labelTextColor.Location = new System.Drawing.Point(2, 75);
            this.labelTextColor.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTextColor.Name = "labelTextColor";
            this.labelTextColor.Size = new System.Drawing.Size(74, 13);
            this.labelTextColor.TabIndex = 2;
            this.labelTextColor.Text = "Log text color:";
            // 
            // checkboxSaveLog
            // 
            this.checkboxSaveLog.AutoSize = true;
            this.checkboxSaveLog.ForeColor = System.Drawing.Color.Black;
            this.checkboxSaveLog.Location = new System.Drawing.Point(4, 20);
            this.checkboxSaveLog.Margin = new System.Windows.Forms.Padding(2);
            this.checkboxSaveLog.Name = "checkboxSaveLog";
            this.checkboxSaveLog.Size = new System.Drawing.Size(112, 17);
            this.checkboxSaveLog.TabIndex = 1;
            this.checkboxSaveLog.Text = "Save log to folder:";
            this.checkboxSaveLog.UseVisualStyleBackColor = true;
            this.checkboxSaveLog.CheckedChanged += new System.EventHandler(this.checkboxSaveLog_CheckedChanged);
            // 
            // textboxLogLocation
            // 
            this.textboxLogLocation.Location = new System.Drawing.Point(4, 41);
            this.textboxLogLocation.Margin = new System.Windows.Forms.Padding(2);
            this.textboxLogLocation.Name = "textboxLogLocation";
            this.textboxLogLocation.ReadOnly = true;
            this.textboxLogLocation.Size = new System.Drawing.Size(228, 20);
            this.textboxLogLocation.TabIndex = 0;
            // 
            // ExporterSettingsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(286, 294);
            this.Controls.Add(this.groupboxGeneral);
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
            this.groupboxGeneral.ResumeLayout(false);
            this.groupboxGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupboxSkeleton;
        private System.Windows.Forms.GroupBox groupboxGeneral;
        private System.Windows.Forms.Label labelBackgroundColor;
        private System.Windows.Forms.Label labelTextColor;
        private System.Windows.Forms.CheckBox checkboxSaveLog;
        private System.Windows.Forms.TextBox textboxLogLocation;
        private System.Windows.Forms.Button buttonChooseText;
        private System.Windows.Forms.Button buttonChooseBackground;
        private System.Windows.Forms.Button buttonChooseFolder;
        private System.Windows.Forms.CheckBox HighlightParentsCheckBox;
        private System.Windows.Forms.Button ChildHighlight;
        private System.Windows.Forms.Button ParentHighlight;
        private System.Windows.Forms.Label ParentLabel;
        private System.Windows.Forms.Label ChildLabel;
    }
}