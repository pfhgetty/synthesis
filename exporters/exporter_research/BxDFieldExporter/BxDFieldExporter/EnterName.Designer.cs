﻿namespace BxDFieldExporter
{
    partial class EnterName
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
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.enterNameLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.reservedLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(105, 12);
            this.nameTextBox.MaxLength = 20;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(249, 22);
            this.nameTextBox.TabIndex = 0;
            this.nameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // enterNameLabel
            // 
            this.enterNameLabel.AutoSize = true;
            this.enterNameLabel.Location = new System.Drawing.Point(12, 15);
            this.enterNameLabel.Name = "enterNameLabel";
            this.enterNameLabel.Size = new System.Drawing.Size(82, 16);
            this.enterNameLabel.TabIndex = 1;
            this.enterNameLabel.Text = "Enter Name:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(156, 40);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(96, 32);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CancleButton_OnClick);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(258, 40);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(96, 32);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OKButton_OnClick);
            // 
            // reservedLabel
            // 
            this.reservedLabel.AutoSize = true;
            this.reservedLabel.ForeColor = System.Drawing.Color.Red;
            this.reservedLabel.Location = new System.Drawing.Point(12, 48);
            this.reservedLabel.Name = "reservedLabel";
            this.reservedLabel.Size = new System.Drawing.Size(118, 16);
            this.reservedLabel.TabIndex = 4;
            this.reservedLabel.Text = "Name is reserved.";
            this.reservedLabel.Visible = false;
            // 
            // EnterName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(366, 84);
            this.Controls.Add(this.reservedLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.enterNameLabel);
            this.Controls.Add(this.nameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterName";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter Property Set Name";          
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label enterNameLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        public System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label reservedLabel;
    }
}