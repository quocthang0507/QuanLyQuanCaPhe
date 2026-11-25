using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	public partial class Form_Booking
	{
		private static readonly Color CanvasColor = Color.FromArgb(250, 246, 239);
		private static readonly Color SurfaceColor = Color.FromArgb(255, 253, 249);
		private static readonly Color CoffeeColor = Color.FromArgb(111, 73, 49);
		private static readonly Color TextColor = Color.FromArgb(66, 45, 33);
		private static readonly Color SelectedColor = Color.FromArgb(236, 212, 180);

		void ApplySalesTheme()
		{
			BackColor = CanvasColor;
			ForeColor = TextColor;
			menuStrip1.BackColor = SurfaceColor;
			menuStrip1.ForeColor = TextColor;
			foreach (var group in new[] { group_Tang0, group_Tang1, groupBox3, groupBox4 })
			{
				group.BackColor = SurfaceColor;
				group.ForeColor = TextColor;
			}
			table_Tang0.BackColor = SurfaceColor;
			table_Tang1.BackColor = SurfaceColor;
			lbl_BanChon.BackColor = SelectedColor;
			lbl_BanChon.ForeColor = TextColor;
			lbl_BanChon.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			lbl_BanChon.AccessibleName = "Bàn đang chọn";
			tableLayoutPanel3.ColumnStyles[0].Width = 115F;
			groupBox3.Text = "Gọi món";
			groupBox4.Text = "Thanh toán";
			StyleGrid(grid_DonHang);
			StyleGrid(grid_HoaDon);
			MãĐHàng.FillWeight = 130F;
			MãHĐơn.FillWeight = 130F;
			MãHóaĐơn.FillWeight = 145F;
			StyleButton(btn_ThemDH, true);
			StyleButton(btn_XoaDH, false);

			tableLayoutPanel6.ColumnCount = 7;
			tableLayoutPanel6.ColumnStyles.Clear();
			for (int i = 0; i < 7; i++)
				tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 7));
			ReplaceAction(lbl_refresh, "Làm mới", "Làm mới danh sách món và khuyến mãi", lbl_refresh_Click, false);
			ReplaceAction(lbl_ApDung, "Tạo hóa đơn", "Tạo hóa đơn từ đơn hàng đang chọn", lbl_ChuyenThanhHoaDon_Click, false);
			ReplaceAction(lbl_InHD, "Thanh toán", "Lưu và hiển thị hóa đơn", lbl_InHD_Click, true);
			ReplaceAction(lbl_ChuyenBan, "Chuyển bàn", "Chuyển đơn chưa thanh toán sang bàn trống", lbl_ChuyenBan_Click, false);
			ReplaceAction(lbl_XoaBan, "Làm trống bàn", "Làm trống bàn hoặc hủy đơn nháp (quản trị viên)", lbl_XoaBan_Click, false);
			ReplaceAction(lbl_XoaTatCa, "Trống bàn đã trả", "Làm trống các bàn đã thanh toán; giữ nguyên đơn nháp", lbl_XoaTatCa_Click, false);
			ReplaceAction(lbl_XemHD, "Xem hóa đơn", "Xem danh sách hóa đơn", lbl_XemHD_Click, false);
			RefreshTableAppearance();
		}

		void ReplaceAction(Label oldControl, string text, string hint, EventHandler click, bool primary)
		{
			int column = tableLayoutPanel6.GetColumn(oldControl);
			tableLayoutPanel6.Controls.Remove(oldControl);
			oldControl.Dispose();
			var button = new Button
			{
				Name = oldControl.Name + "Button",
				Text = text,
				AccessibleName = text,
				Dock = DockStyle.Fill,
				Margin = new Padding(3),
				Font = new Font("Segoe UI", 9F, FontStyle.Bold),
				TabIndex = column
			};
			StyleButton(button, primary);
			button.Click += click;
			tooltip.SetToopTip(button, hint);
			tableLayoutPanel6.Controls.Add(button, column, 0);
		}

		static void StyleButton(Button button, bool primary)
		{
			button.FlatStyle = FlatStyle.Flat;
			button.UseVisualStyleBackColor = false;
			button.BackColor = primary ? CoffeeColor : SurfaceColor;
			button.ForeColor = primary ? Color.White : TextColor;
			button.FlatAppearance.BorderColor = primary ? CoffeeColor : Color.FromArgb(210, 192, 172);
			button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(133, 87, 55) : SelectedColor;
		}

		static void StyleGrid(DataGridView grid)
		{
			grid.BackgroundColor = SurfaceColor;
			grid.BorderStyle = BorderStyle.None;
			grid.AllowUserToDeleteRows = false;
			grid.GridColor = Color.FromArgb(231, 220, 207);
			grid.EnableHeadersVisualStyles = false;
			grid.ColumnHeadersDefaultCellStyle.BackColor = CoffeeColor;
			grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
			grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = CoffeeColor;
			grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
			grid.DefaultCellStyle.BackColor = SurfaceColor;
			grid.DefaultCellStyle.ForeColor = TextColor;
			grid.DefaultCellStyle.SelectionBackColor = SelectedColor;
			grid.DefaultCellStyle.SelectionForeColor = TextColor;
			grid.AlternatingRowsDefaultCellStyle.BackColor = CanvasColor;
		}

		void RefreshTableAppearance()
		{
			foreach (Label table in table_Tang0.Controls.Cast<Label>().Concat(table_Tang1.Controls.Cast<Label>()))
			{
				table.BackColor = table == selectedTable ? SelectedColor :
					table.ImageKey == "full" ? Color.FromArgb(239, 225, 209) : SurfaceColor;
				table.ForeColor = TextColor;
				table.AccessibleName = table.Name;
				table.AccessibleDescription = table == selectedTable ? "Bàn đang chọn" :
					table.ImageKey == "full" ? "Bàn đang sử dụng" : "Bàn trống";
			}
		}

		void ClearPaymentFields()
		{
			tbx_TongTien.Clear();
			tbx_ThanhTien.Clear();
			tbx_DuaTruoc.Clear();
			tbx_ConLai.Clear();
		}
	}
}
