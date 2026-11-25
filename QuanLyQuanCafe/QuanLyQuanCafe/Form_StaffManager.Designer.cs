namespace QuanLyQuanCafe
{
	partial class Form_StaffManager
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_StaffManager));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.grid_Staff = new System.Windows.Forms.DataGridView();
			this.TênĐăngNhập = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.HọTên = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.CMND = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.NgàySinh = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SốĐiệnThoại = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.CònLàm = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.tbx_TenDangNhap = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.tbx_HoTen = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tbx_CMND = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panel8 = new System.Windows.Forms.Panel();
			this.tbx_NgaySinh = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.tbx_SoDT = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.chk_ConLam = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.panel6 = new System.Windows.Forms.Panel();
			this.btn_Xuat = new System.Windows.Forms.Button();
			this.panel7 = new System.Windows.Forms.Panel();
			this.btn_Xoa = new System.Windows.Forms.Button();
			this.btn_Them = new System.Windows.Forms.Button();
			this.btn_CapNhatTrangThai = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btn_TimTheoConLam = new System.Windows.Forms.Button();
			this.btn_TimTheoDT = new System.Windows.Forms.Button();
			this.btn_TimTheoTen = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grid_Staff)).BeginInit();
			this.flowLayoutPanel1.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel8.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel7.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 33);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.grid_Staff);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel1);
			this.splitContainer1.Size = new System.Drawing.Size(836, 552);
			this.splitContainer1.SplitterDistance = 531;
			this.splitContainer1.TabIndex = 3;
			// 
			// grid_Staff
			// 
			this.grid_Staff.AllowUserToResizeRows = false;
			this.grid_Staff.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.grid_Staff.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grid_Staff.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TênĐăngNhập,
            this.HọTên,
            this.CMND,
            this.NgàySinh,
            this.SốĐiệnThoại,
            this.CònLàm});
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.grid_Staff.DefaultCellStyle = dataGridViewCellStyle1;
			this.grid_Staff.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid_Staff.GridColor = System.Drawing.SystemColors.ScrollBar;
			this.grid_Staff.Location = new System.Drawing.Point(0, 0);
			this.grid_Staff.Name = "grid_Staff";
			this.grid_Staff.RowHeadersVisible = false;
			this.grid_Staff.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.grid_Staff.Size = new System.Drawing.Size(531, 552);
			this.grid_Staff.TabIndex = 0;
			// 
			// TênĐăngNhập
			// 
			this.TênĐăngNhập.DataPropertyName = "TênĐăngNhập";
			this.TênĐăngNhập.HeaderText = "Tên đăng nhập";
			this.TênĐăngNhập.Name = "TênĐăngNhập";
			// 
			// HọTên
			// 
			this.HọTên.DataPropertyName = "HọTên";
			this.HọTên.HeaderText = "Họ tên";
			this.HọTên.Name = "HọTên";
			// 
			// CMND
			// 
			this.CMND.DataPropertyName = "CMND";
			this.CMND.HeaderText = "CMND";
			this.CMND.Name = "CMND";
			// 
			// NgàySinh
			// 
			this.NgàySinh.DataPropertyName = "NgàySinh";
			this.NgàySinh.HeaderText = "Ngày sinh";
			this.NgàySinh.Name = "NgàySinh";
			// 
			// SốĐiệnThoại
			// 
			this.SốĐiệnThoại.DataPropertyName = "SốĐiệnThoại";
			this.SốĐiệnThoại.HeaderText = "Số điện thoại";
			this.SốĐiệnThoại.Name = "SốĐiệnThoại";
			// 
			// CònLàm
			// 
			this.CònLàm.DataPropertyName = "CònLàm";
			this.CònLàm.HeaderText = "Còn làm";
			this.CònLàm.Name = "CònLàm";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.panel5);
			this.flowLayoutPanel1.Controls.Add(this.panel2);
			this.flowLayoutPanel1.Controls.Add(this.panel1);
			this.flowLayoutPanel1.Controls.Add(this.panel8);
			this.flowLayoutPanel1.Controls.Add(this.panel4);
			this.flowLayoutPanel1.Controls.Add(this.panel3);
			this.flowLayoutPanel1.Controls.Add(this.panel6);
			this.flowLayoutPanel1.Controls.Add(this.panel7);
			this.flowLayoutPanel1.Controls.Add(this.groupBox1);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(301, 552);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.tbx_TenDangNhap);
			this.panel5.Controls.Add(this.label6);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.Location = new System.Drawing.Point(3, 3);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(290, 41);
			this.panel5.TabIndex = 1;
			// 
			// tbx_TenDangNhap
			// 
			this.tbx_TenDangNhap.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.tbx_TenDangNhap.Location = new System.Drawing.Point(120, 8);
			this.tbx_TenDangNhap.MaxLength = 20;
			this.tbx_TenDangNhap.Name = "tbx_TenDangNhap";
			this.tbx_TenDangNhap.Size = new System.Drawing.Size(144, 26);
			this.tbx_TenDangNhap.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(3, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(111, 21);
			this.label6.TabIndex = 0;
			this.label6.Text = "Tên đăng nhập";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.tbx_HoTen);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(3, 50);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(290, 41);
			this.panel2.TabIndex = 2;
			// 
			// tbx_HoTen
			// 
			this.tbx_HoTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.tbx_HoTen.Location = new System.Drawing.Point(120, 8);
			this.tbx_HoTen.MaxLength = 100;
			this.tbx_HoTen.Name = "tbx_HoTen";
			this.tbx_HoTen.Size = new System.Drawing.Size(144, 26);
			this.tbx_HoTen.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(38, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76, 21);
			this.label3.TabIndex = 0;
			this.label3.Text = "Họ và tên";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tbx_CMND);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(3, 97);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(290, 41);
			this.panel1.TabIndex = 3;
			// 
			// tbx_CMND
			// 
			this.tbx_CMND.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.tbx_CMND.Location = new System.Drawing.Point(120, 8);
			this.tbx_CMND.MaxLength = 15;
			this.tbx_CMND.Name = "tbx_CMND";
			this.tbx_CMND.Size = new System.Drawing.Size(144, 26);
			this.tbx_CMND.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(57, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 21);
			this.label2.TabIndex = 0;
			this.label2.Text = "CMND";
			// 
			// panel8
			// 
			this.panel8.Controls.Add(this.tbx_NgaySinh);
			this.panel8.Controls.Add(this.label7);
			this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel8.Location = new System.Drawing.Point(3, 144);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(287, 41);
			this.panel8.TabIndex = 4;
			// 
			// tbx_NgaySinh
			// 
			this.tbx_NgaySinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.tbx_NgaySinh.Location = new System.Drawing.Point(120, 8);
			this.tbx_NgaySinh.MaxLength = 10;
			this.tbx_NgaySinh.Name = "tbx_NgaySinh";
			this.tbx_NgaySinh.Size = new System.Drawing.Size(164, 26);
			this.tbx_NgaySinh.TabIndex = 4;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(34, 10);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 21);
			this.label7.TabIndex = 0;
			this.label7.Text = "Ngày sinh";
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.tbx_SoDT);
			this.panel4.Controls.Add(this.label5);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel4.Location = new System.Drawing.Point(3, 191);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(287, 41);
			this.panel4.TabIndex = 5;
			// 
			// tbx_SoDT
			// 
			this.tbx_SoDT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.tbx_SoDT.Location = new System.Drawing.Point(120, 8);
			this.tbx_SoDT.MaxLength = 10;
			this.tbx_SoDT.Name = "tbx_SoDT";
			this.tbx_SoDT.Size = new System.Drawing.Size(164, 26);
			this.tbx_SoDT.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(13, 10);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(101, 21);
			this.label5.TabIndex = 0;
			this.label5.Text = "Số điện thoại";
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.chk_ConLam);
			this.panel3.Controls.Add(this.label4);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel3.Location = new System.Drawing.Point(3, 238);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(287, 41);
			this.panel3.TabIndex = 6;
			// 
			// chk_ConLam
			// 
			this.chk_ConLam.AutoSize = true;
			this.chk_ConLam.Location = new System.Drawing.Point(126, 13);
			this.chk_ConLam.Name = "chk_ConLam";
			this.chk_ConLam.Size = new System.Drawing.Size(15, 14);
			this.chk_ConLam.TabIndex = 6;
			this.chk_ConLam.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(46, 10);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 21);
			this.label4.TabIndex = 0;
			this.label4.Text = "Còn làm";
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.btn_Xuat);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel6.Location = new System.Drawing.Point(3, 285);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(287, 41);
			this.panel6.TabIndex = 7;
			// 
			// btn_Xuat
			// 
			this.btn_Xuat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.btn_Xuat.Location = new System.Drawing.Point(80, 3);
			this.btn_Xuat.Name = "btn_Xuat";
			this.btn_Xuat.Size = new System.Drawing.Size(115, 35);
			this.btn_Xuat.TabIndex = 7;
			this.btn_Xuat.Text = "Xuất toàn bộ";
			this.btn_Xuat.UseVisualStyleBackColor = true;
			this.btn_Xuat.Click += new System.EventHandler(this.btn_Xuat_Click);
			// 
			// panel7
			// 
			this.panel7.Controls.Add(this.btn_Xoa);
			this.panel7.Controls.Add(this.btn_Them);
			this.panel7.Controls.Add(this.btn_CapNhatTrangThai);
			this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel7.Location = new System.Drawing.Point(3, 332);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(287, 41);
			this.panel7.TabIndex = 8;
			// 
			// btn_Xoa
			// 
			this.btn_Xoa.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.btn_Xoa.Location = new System.Drawing.Point(91, 3);
			this.btn_Xoa.Name = "btn_Xoa";
			this.btn_Xoa.Size = new System.Drawing.Size(72, 35);
			this.btn_Xoa.TabIndex = 9;
			this.btn_Xoa.Text = "Xóa";
			this.btn_Xoa.UseVisualStyleBackColor = true;
			this.btn_Xoa.Click += new System.EventHandler(this.btn_Xoa_Click);
			// 
			// btn_Them
			// 
			this.btn_Them.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.btn_Them.Location = new System.Drawing.Point(13, 3);
			this.btn_Them.Name = "btn_Them";
			this.btn_Them.Size = new System.Drawing.Size(72, 35);
			this.btn_Them.TabIndex = 8;
			this.btn_Them.Text = "Thêm";
			this.btn_Them.UseVisualStyleBackColor = true;
			this.btn_Them.Click += new System.EventHandler(this.btn_Them_Click);
			//
			// btn_CapNhatTrangThai
			//
			this.btn_CapNhatTrangThai.Location = new System.Drawing.Point(169, 3);
			this.btn_CapNhatTrangThai.Name = "btn_CapNhatTrangThai";
			this.btn_CapNhatTrangThai.Size = new System.Drawing.Size(110, 35);
			this.btn_CapNhatTrangThai.TabIndex = 10;
			this.btn_CapNhatTrangThai.Text = "Lưu trạng thái";
			this.btn_CapNhatTrangThai.UseVisualStyleBackColor = true;
			this.btn_CapNhatTrangThai.Click += new System.EventHandler(this.btn_CapNhatTrangThai_Click);
			//
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btn_TimTheoConLam);
			this.groupBox1.Controls.Add(this.btn_TimTheoDT);
			this.groupBox1.Controls.Add(this.btn_TimTheoTen);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(3, 379);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(284, 91);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tìm kiếm nâng cao";
			// 
			// btn_TimTheoConLam
			// 
			this.btn_TimTheoConLam.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.btn_TimTheoConLam.Location = new System.Drawing.Point(201, 28);
			this.btn_TimTheoConLam.Name = "btn_TimTheoConLam";
			this.btn_TimTheoConLam.Size = new System.Drawing.Size(77, 55);
			this.btn_TimTheoConLam.TabIndex = 13;
			this.btn_TimTheoConLam.Text = "Còn làm";
			this.btn_TimTheoConLam.UseVisualStyleBackColor = true;
			this.btn_TimTheoConLam.Click += new System.EventHandler(this.btn_TimTheoConLam_Click);
			// 
			// btn_TimTheoDT
			// 
			this.btn_TimTheoDT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.btn_TimTheoDT.Location = new System.Drawing.Point(97, 28);
			this.btn_TimTheoDT.Name = "btn_TimTheoDT";
			this.btn_TimTheoDT.Size = new System.Drawing.Size(98, 55);
			this.btn_TimTheoDT.TabIndex = 12;
			this.btn_TimTheoDT.Text = "Tìm theo Số điện thoại";
			this.btn_TimTheoDT.UseVisualStyleBackColor = true;
			this.btn_TimTheoDT.Click += new System.EventHandler(this.btn_TimTheoDT_Click);
			// 
			// btn_TimTheoTen
			// 
			this.btn_TimTheoTen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.btn_TimTheoTen.Location = new System.Drawing.Point(6, 28);
			this.btn_TimTheoTen.Name = "btn_TimTheoTen";
			this.btn_TimTheoTen.Size = new System.Drawing.Size(85, 55);
			this.btn_TimTheoTen.TabIndex = 11;
			this.btn_TimTheoTen.Text = "Tìm theo Họ tên ";
			this.btn_TimTheoTen.UseVisualStyleBackColor = true;
			this.btn_TimTheoTen.Click += new System.EventHandler(this.btn_TimTheoTen_Click);
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(836, 33);
			this.label1.TabIndex = 2;
			this.label1.Text = "QUẢN LÝ DANH SÁCH NHÂN VIÊN";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Form_StaffManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(836, 585);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form_StaffManager";
			this.Text = "QUẢN LÝ NHÂN VIÊN";
			this.Load += new System.EventHandler(this.Form_StaffManager_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grid_Staff)).EndInit();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel8.ResumeLayout(false);
			this.panel8.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataGridView grid_Staff;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox tbx_HoTen;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox tbx_CMND;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.TextBox tbx_SoDT;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chk_ConLam;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Button btn_Xoa;
		private System.Windows.Forms.Button btn_Them;
		private System.Windows.Forms.Button btn_CapNhatTrangThai;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btn_TimTheoDT;
		private System.Windows.Forms.Button btn_TimTheoTen;
		private System.Windows.Forms.Button btn_TimTheoConLam;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.TextBox tbx_TenDangNhap;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Button btn_Xuat;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.TextBox tbx_NgaySinh;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.DataGridViewTextBoxColumn TênĐăngNhập;
		private System.Windows.Forms.DataGridViewTextBoxColumn HọTên;
		private System.Windows.Forms.DataGridViewTextBoxColumn CMND;
		private System.Windows.Forms.DataGridViewTextBoxColumn NgàySinh;
		private System.Windows.Forms.DataGridViewTextBoxColumn SốĐiệnThoại;
		private System.Windows.Forms.DataGridViewTextBoxColumn CònLàm;
	}
}