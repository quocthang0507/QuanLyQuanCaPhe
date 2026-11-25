using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	public partial class Form_Moving : Form
	{
		TableLayoutPanel tangTret;
		TableLayoutPanel tangMot;

		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public TableLayoutPanel Tang0
		{
			get { return tangTret; }
			set { tangTret = value; }
		}

		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public TableLayoutPanel Tang1
		{
			get { return tangMot; }
			set { tangMot = value; }
		}

		public Form_Moving(TableLayoutPanel tang0, TableLayoutPanel tang1)
		{
			InitializeComponent();
			Tang0 = tang0;
			Tang1 = tang1;
		}

		#region Events
		private void Form_Moving_Load(object sender, EventArgs e)
		{
			Load_BanChuyen();
			Load_BanDen();
		}

		private void btn_Chuyen_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Tính năng đang phát triển...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Load các bàn có khách vào combobox bàn cần chuyển
		/// </summary>
		void Load_BanChuyen()
		{
			List<String> ds = new List<string>();
			foreach (Label item in tangTret.Controls.Cast<Label>().Concat(tangMot.Controls.Cast<Label>()))
			{
				if (item.ImageKey == "full")
					ds.Add(item.Name);
			}
			cbx_BanChuyen.DataSource = ds;
		}

		/// <summary>
		/// Load các bàn chưa có khách vào combobox bàn cần đến
		/// </summary>
		void Load_BanDen()
		{
			List<String> ds = new List<string>();
			foreach (Label item in tangTret.Controls.Cast<Label>().Concat(tangMot.Controls.Cast<Label>()))
			{
				if (item.ImageKey == "empty")
					ds.Add(item.Name);
			}
			cbx_BanDen.DataSource = ds;
		}

		bool ChuyenBan(string maBan_Nguon, string maTang_Nguon, string maBan_Dich, string maTang_Dich)
		{
			return true;
		}

		#endregion

	}
}
