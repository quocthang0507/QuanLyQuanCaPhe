namespace QuanLyQuanCafe
{
	partial class Form_Moving
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Moving));
			this.cbx_BanChuyen = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cbx_BanDen = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btn_Chuyen = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// cbx_BanChuyen
			// 
			this.cbx_BanChuyen.FormattingEnabled = true;
			this.cbx_BanChuyen.Location = new System.Drawing.Point(183, 29);
			this.cbx_BanChuyen.Margin = new System.Windows.Forms.Padding(4);
			this.cbx_BanChuyen.Name = "cbx_BanChuyen";
			this.cbx_BanChuyen.Size = new System.Drawing.Size(160, 25);
			this.cbx_BanChuyen.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(32, 32);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Bàn cần chuyển:";
			// 
			// cbx_BanDen
			// 
			this.cbx_BanDen.FormattingEnabled = true;
			this.cbx_BanDen.Location = new System.Drawing.Point(183, 73);
			this.cbx_BanDen.Margin = new System.Windows.Forms.Padding(4);
			this.cbx_BanDen.Name = "cbx_BanDen";
			this.cbx_BanDen.Size = new System.Drawing.Size(160, 25);
			this.cbx_BanDen.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(54, 73);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Bàn cần đến:";
			// 
			// btn_Chuyen
			// 
			this.btn_Chuyen.Location = new System.Drawing.Point(149, 125);
			this.btn_Chuyen.Name = "btn_Chuyen";
			this.btn_Chuyen.Size = new System.Drawing.Size(95, 37);
			this.btn_Chuyen.TabIndex = 6;
			this.btn_Chuyen.Text = "Chuyển";
			this.btn_Chuyen.UseVisualStyleBackColor = true;
			this.btn_Chuyen.Click += new System.EventHandler(this.btn_Chuyen_Click);
			// 
			// Form_Moving
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(379, 175);
			this.Controls.Add(this.btn_Chuyen);
			this.Controls.Add(this.cbx_BanDen);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cbx_BanChuyen);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.Name = "Form_Moving";
			this.Text = "CHUYỂN BÀN";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbx_BanChuyen;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbx_BanDen;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btn_Chuyen;
	}
}