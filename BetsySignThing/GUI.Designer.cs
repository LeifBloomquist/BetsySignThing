﻿namespace BetsySignThing
{
  partial class GUI
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
            this.BetsyPictureBox = new System.Windows.Forms.PictureBox();
            this.TimerLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.BetsyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // BetsyPictureBox
            // 
            this.BetsyPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BetsyPictureBox.Location = new System.Drawing.Point(0, 0);
            this.BetsyPictureBox.Name = "BetsyPictureBox";
            this.BetsyPictureBox.Size = new System.Drawing.Size(162, 108);
            this.BetsyPictureBox.TabIndex = 0;
            this.BetsyPictureBox.TabStop = false;
            // 
            // TimerLabel
            // 
            this.TimerLabel.AutoSize = true;
            this.TimerLabel.Location = new System.Drawing.Point(13, 190);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(35, 13);
            this.TimerLabel.TabIndex = 1;
            this.TimerLabel.Text = "label1";
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 247);
            this.Controls.Add(this.TimerLabel);
            this.Controls.Add(this.BetsyPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GUI";
            this.Text = "Betsy Sign Controller";
            ((System.ComponentModel.ISupportInitialize)(this.BetsyPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox BetsyPictureBox;
    private System.Windows.Forms.Label TimerLabel;
  }
}

