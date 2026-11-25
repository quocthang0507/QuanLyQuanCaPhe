using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;

namespace QuanLyQuanCafe
{
	public partial class Form_Bill : Form
	{
		CafeRestaurantEntities database = new CafeRestaurantEntities();

		public Form_Bill()
		{
			InitializeComponent();
			flowLayoutPanel1.AutoScroll = true;
			var details = new Button
			{
				Text = "Chi tiết và in hóa đơn",
				Size = new Size(284, 40),
				AccessibleName = "Chi tiết và in hóa đơn"
			};
			details.Click += ShowReceipt;
			flowLayoutPanel1.Controls.Add(details);
			if (!User.Type)
			{
				var refund = new Button { Text = "Hoàn toàn bộ hóa đơn", Size = new Size(284, 40) };
				refund.Click += Refund;
				flowLayoutPanel1.Controls.Add(refund);
			}
		}

		private void Refund(object sender, EventArgs e)
		{
			if (grid_Bill.CurrentRow == null || grid_Bill.CurrentRow.Cells[0].Value == null)
			{
				MessageBox.Show("Vui lòng chọn hóa đơn.", "Thông báo");
				return;
			}
			string billId = grid_Bill.CurrentRow.Cells[0].Value.ToString();
			try
			{
				using (var form = new Form_Refund(billId, RefundService.PaymentMethod(billId)))
					form.ShowDialog(this);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể tải thông tin thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ShowReceipt(object sender, EventArgs e)
		{
			if (grid_Bill.CurrentRow == null || grid_Bill.CurrentRow.Cells[0].Value == null)
			{
				MessageBox.Show("Vui lòng chọn hóa đơn.", "Thông báo");
				return;
			}
			try
			{
				string receipt = BillReceipt.Load(grid_Bill.CurrentRow.Cells[0].Value.ToString());
				using (var form = new Form_BillDetail(receipt))
					form.ShowDialog(this);
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể tải hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (System.Drawing.Printing.InvalidPrinterException ex)
			{
				MessageBox.Show("Không thể in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show("Không thể xem hoặc in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#region Events

		private void Form_Bill_Load(object sender, EventArgs e)
		{
			btn_Xuat.PerformClick();
			AddTooltip();
		}

		private void btn_Xuat_Click(object sender, EventArgs e)
		{
			exportTable();
			AddDataBinding();
		}

		private void btn_TimTheoMaHD_Click(object sender, EventArgs e)
		{
			string maHD = tbx_MaHD.Text;
			if (maHD != "")
			{
				var table = from data in database.HoaDon
							where data.MaHD.Contains(maHD)
							select new
							{
								MãHóaĐơn = data.MaHD,
								NgàyHóaĐơn = data.NgayHD,
								MãNhânViên = data.MaNV,
								MãBàn = data.MaBan,
								MãTầng = data.MaTang,
								TổngTiền = data.TongTien,
								MãSựKiện = data.MaSuKien,
								ĐưaTrước = data.DuaTruoc,
								CònLại = data.ConLai
							};
				grid_Bill.DataSource = table.ToList();
				AddDataBinding();
			}
		}

		private void btn_TimTheoMaSK_Click(object sender, EventArgs e)
		{
			string maSK = tbx_MaSK.Text;
			if (maSK != "")
			{
				var table = from data in database.HoaDon
							where data.MaSuKien.Contains(maSK)
							select new
							{
								MãHóaĐơn = data.MaHD,
								NgàyHóaĐơn = data.NgayHD,
								MãNhânViên = data.MaNV,
								MãBàn = data.MaBan,
								MãTầng = data.MaTang,
								TổngTiền = data.TongTien,
								MãSựKiện = data.MaSuKien,
								ĐưaTrước = data.DuaTruoc,
								CònLại = data.ConLai
							};
				grid_Bill.DataSource = table.ToList();
				AddDataBinding();
			}
		}

		private void btn_TimTheoNhanVien_Click(object sender, EventArgs e)
		{
			string maNV = tbx_MaNV.Text;
			if (maNV != "")
			{
				var table = from data in database.HoaDon
							where data.MaNV.Contains(maNV)
							select new
							{
								MãHóaĐơn = data.MaHD,
								NgàyHóaĐơn = data.NgayHD,
								MãNhânViên = data.MaNV,
								MãBàn = data.MaBan,
								MãTầng = data.MaTang,
								TổngTiền = data.TongTien,
								MãSựKiện = data.MaSuKien,
								ĐưaTrước = data.DuaTruoc,
								CònLại = data.ConLai
							};
				grid_Bill.DataSource = table.ToList();
				AddDataBinding();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Xuất bảng HoaDon vào DataGridView
		/// </summary>
		void exportTable()
		{
			var table = from data in database.HoaDon
						select new
						{
							MãHóaĐơn = data.MaHD,
							NgàyHóaĐơn = data.NgayHD,
							MãNhânViên = data.MaNV,
							MãBàn = data.MaBan,
							MãTầng = data.MaTang,
							TổngTiền = data.TongTien,
							MãSựKiện = data.MaSuKien,
							ĐưaTrước = data.DuaTruoc,
							CònLại = data.ConLai
						};
			grid_Bill.DataSource = table.ToList();
		}

		/// <summary>
		/// Xóa các DataBindings
		/// </summary>
		void ResetDatabinding()
		{
			tbx_MaHD.DataBindings.Clear();
			tbx_MaNV.DataBindings.Clear();
			tbx_MaSK.DataBindings.Clear();
		}

		/// <summary>
		/// Thêm các DataBinding
		/// </summary>
		void AddDataBinding()
		{
			ResetDatabinding();
			tbx_MaHD.DataBindings.Add(new Binding("Text", grid_Bill.DataSource, "MãHóaĐơn"));
			tbx_MaNV.DataBindings.Add(new Binding("Text", grid_Bill.DataSource, "MãNhânViên"));
			tbx_MaSK.DataBindings.Add(new Binding("Text", grid_Bill.DataSource, "MãSựKiện"));
		}

		/// <summary>
		/// Thêm tooltip vào các button
		/// </summary>
		void AddTooltip()
		{
			BalloonTooltip tip = new BalloonTooltip(10000);
			tip.SetToopTip(btn_Xuat, "Xuất toàn bộ các hóa đơn");
			tip.SetToopTip(btn_TimTheoMaHD, "Tìm kiếm một hóa đơn dựa trên mã hóa đơn tương đối\r\n"+
				"Mẹo: Bạn có thể tìm được ngày của hóa đơn nhờ vào thông tin này vì mã hóa đơn có dạng HDddmmyyxxxx\r\n"+
				"Trong đó: ddmmyy là ngày tháng năm, xxxx là số thứ tự hóa đơn trong ngày");
			tip.SetToopTip(btn_TimTheoMaSK, "Tìm kiếm các hóa đơn dựa theo mã sự kiện tương đối");
			tip.SetToopTip(btn_TimTheoNhanVien, "Tìm kiếm các hóa đơn của một nhân viên theo mã nhân viên tương đối");
		}

		#endregion
	}
}
