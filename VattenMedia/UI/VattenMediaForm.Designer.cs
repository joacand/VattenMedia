namespace VattenMedia
{
    partial class VattenMediaForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VattenMediaForm));
            this.OAuthButton = new System.Windows.Forms.Button();
            this.RefreshChannelsButton = new System.Windows.Forms.Button();
            this.OAuthAvailableLabel = new System.Windows.Forms.Label();
            this.ChannelGridView = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.RadioQualityHigh = new System.Windows.Forms.RadioButton();
            this.RadioQualityMedium = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.RadioQualityLow = new System.Windows.Forms.RadioButton();
            this.LaunchChannelButton = new System.Windows.Forms.Button();
            this.textBox_URL = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChannelGridView)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OAuthButton
            // 
            this.OAuthButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OAuthButton.Location = new System.Drawing.Point(969, 542);
            this.OAuthButton.Name = "OAuthButton";
            this.OAuthButton.Size = new System.Drawing.Size(75, 23);
            this.OAuthButton.TabIndex = 0;
            this.OAuthButton.Text = "OAuth";
            this.OAuthButton.UseVisualStyleBackColor = true;
            this.OAuthButton.Click += new System.EventHandler(this.OAuthButton_Click);
            // 
            // RefreshChannelsButton
            // 
            this.RefreshChannelsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RefreshChannelsButton.Location = new System.Drawing.Point(12, 513);
            this.RefreshChannelsButton.Name = "RefreshChannelsButton";
            this.RefreshChannelsButton.Size = new System.Drawing.Size(110, 23);
            this.RefreshChannelsButton.TabIndex = 1;
            this.RefreshChannelsButton.Text = "Refresh channels";
            this.RefreshChannelsButton.UseVisualStyleBackColor = true;
            this.RefreshChannelsButton.Click += new System.EventHandler(this.RefreshChannelsButton_Click);
            // 
            // OAuthAvailableLabel
            // 
            this.OAuthAvailableLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OAuthAvailableLabel.AutoSize = true;
            this.OAuthAvailableLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OAuthAvailableLabel.ForeColor = System.Drawing.Color.Red;
            this.OAuthAvailableLabel.Location = new System.Drawing.Point(854, 546);
            this.OAuthAvailableLabel.Name = "OAuthAvailableLabel";
            this.OAuthAvailableLabel.Size = new System.Drawing.Size(109, 15);
            this.OAuthAvailableLabel.TabIndex = 3;
            this.OAuthAvailableLabel.Text = "Access token NOK";
            // 
            // ChannelGridView
            // 
            this.ChannelGridView.AllowUserToAddRows = false;
            this.ChannelGridView.AllowUserToDeleteRows = false;
            this.ChannelGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChannelGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(20)))), ((int)(((byte)(31)))));
            this.ChannelGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChannelGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.ChannelGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(20)))), ((int)(((byte)(31)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ChannelGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ChannelGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ChannelGridView.Cursor = System.Windows.Forms.Cursors.Default;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(20)))), ((int)(((byte)(31)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(20)))), ((int)(((byte)(31)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ChannelGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.ChannelGridView.EnableHeadersVisualStyles = false;
            this.ChannelGridView.Location = new System.Drawing.Point(13, 12);
            this.ChannelGridView.MultiSelect = false;
            this.ChannelGridView.Name = "ChannelGridView";
            this.ChannelGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ChannelGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.ChannelGridView.RowHeadersVisible = false;
            this.ChannelGridView.RowTemplate.Height = 60;
            this.ChannelGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ChannelGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ChannelGridView.Size = new System.Drawing.Size(1031, 495);
            this.ChannelGridView.TabIndex = 4;
            this.ChannelGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ChannelGridView_CellContentClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 568);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1056, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // RadioQualityHigh
            // 
            this.RadioQualityHigh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RadioQualityHigh.AutoSize = true;
            this.RadioQualityHigh.Checked = true;
            this.RadioQualityHigh.Location = new System.Drawing.Point(61, 542);
            this.RadioQualityHigh.Name = "RadioQualityHigh";
            this.RadioQualityHigh.Size = new System.Drawing.Size(47, 17);
            this.RadioQualityHigh.TabIndex = 6;
            this.RadioQualityHigh.TabStop = true;
            this.RadioQualityHigh.Text = "High";
            this.RadioQualityHigh.UseVisualStyleBackColor = true;
            // 
            // RadioQualityMedium
            // 
            this.RadioQualityMedium.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RadioQualityMedium.AutoSize = true;
            this.RadioQualityMedium.Location = new System.Drawing.Point(114, 542);
            this.RadioQualityMedium.Name = "RadioQualityMedium";
            this.RadioQualityMedium.Size = new System.Drawing.Size(62, 17);
            this.RadioQualityMedium.TabIndex = 7;
            this.RadioQualityMedium.Text = "Medium";
            this.RadioQualityMedium.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 544);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Quality:";
            // 
            // RadioQualityLow
            // 
            this.RadioQualityLow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RadioQualityLow.AutoSize = true;
            this.RadioQualityLow.Location = new System.Drawing.Point(182, 542);
            this.RadioQualityLow.Name = "RadioQualityLow";
            this.RadioQualityLow.Size = new System.Drawing.Size(45, 17);
            this.RadioQualityLow.TabIndex = 9;
            this.RadioQualityLow.Text = "Low";
            this.RadioQualityLow.UseVisualStyleBackColor = true;
            // 
            // LaunchChannelButton
            // 
            this.LaunchChannelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LaunchChannelButton.Location = new System.Drawing.Point(128, 513);
            this.LaunchChannelButton.Name = "LaunchChannelButton";
            this.LaunchChannelButton.Size = new System.Drawing.Size(110, 23);
            this.LaunchChannelButton.TabIndex = 10;
            this.LaunchChannelButton.Text = "Launch from URL";
            this.LaunchChannelButton.UseVisualStyleBackColor = true;
            this.LaunchChannelButton.Click += new System.EventHandler(this.LaunchChannelButton_Click);
            // 
            // textBox_URL
            // 
            this.textBox_URL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_URL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_URL.Location = new System.Drawing.Point(244, 515);
            this.textBox_URL.Name = "textBox_URL";
            this.textBox_URL.Size = new System.Drawing.Size(800, 20);
            this.textBox_URL.TabIndex = 11;
            this.textBox_URL.Text = "http://www.twitch.tv/channel";
            // 
            // VattenMediaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 590);
            this.Controls.Add(this.textBox_URL);
            this.Controls.Add(this.LaunchChannelButton);
            this.Controls.Add(this.RadioQualityLow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RadioQualityMedium);
            this.Controls.Add(this.RadioQualityHigh);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ChannelGridView);
            this.Controls.Add(this.OAuthAvailableLabel);
            this.Controls.Add(this.RefreshChannelsButton);
            this.Controls.Add(this.OAuthButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "VattenMediaForm";
            this.Text = "VattenMedia";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ChannelGridView)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OAuthButton;
        private System.Windows.Forms.Button RefreshChannelsButton;
        private System.Windows.Forms.Label OAuthAvailableLabel;
        private System.Windows.Forms.DataGridView ChannelGridView;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.RadioButton RadioQualityHigh;
        private System.Windows.Forms.RadioButton RadioQualityMedium;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton RadioQualityLow;
        private System.Windows.Forms.Button LaunchChannelButton;
        private System.Windows.Forms.TextBox textBox_URL;
    }
}

