using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	internal sealed class Form_Takeaway : Form
	{
		private readonly ListBox drafts;
		internal DraftTable Selected { get; private set; }

		internal Form_Takeaway()
		{
			Text = "Đơn mang đi chưa thanh toán";
			StartPosition = FormStartPosition.CenterParent;
			ClientSize = new Size(460, 300);
			MinimizeBox = false;
			MaximizeBox = false;
			drafts = new ListBox { Left = 15, Top = 15, Width = 430, Height = 220 };
			var choose = new Button { Left = 15, Top = 245, Width = 200, Height = 35, Text = "Chọn đơn" };
			var create = new Button { Left = 230, Top = 245, Width = 215, Height = 35, Text = "Tạo đơn mang đi" };
			choose.Click += Choose;
			create.Click += Create;
			drafts.DoubleClick += Choose;
			Controls.AddRange(new Control[] { drafts, choose, create });
			Load += (sender, args) => RefreshDrafts();
		}

		private void RefreshDrafts()
		{
			try
			{
				drafts.DataSource = TakeawayService.Drafts();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể tải đơn mang đi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
			}
		}

		private void Choose(object sender, EventArgs e)
		{
			var item = drafts.SelectedItem as TakeawayDraft;
			if (item == null)
			{
				MessageBox.Show("Vui lòng chọn đơn mang đi.", "Thông báo");
				return;
			}
			Selected = new DraftTable { MaBan = item.MaBan.Trim(), MaTang = item.MaTang.Trim() };
			DialogResult = DialogResult.OK;
			Close();
		}

		private void Create(object sender, EventArgs e)
		{
			try
			{
				Selected = TakeawayService.Reserve();
				DialogResult = DialogResult.OK;
				Close();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể tạo đơn mang đi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Thông báo");
			}
		}
	}
}
