using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	public partial class Form_Booking : Form
	{
		BalloonTooltip tooltip = new BalloonTooltip();
		User account = new User();
		CafeRestaurantEntities database = new CafeRestaurantEntities();
		Label selectedTable;
		ToolStripComboBox paymentMethod;

		public Form_Booking(User account)
		{
			InitializeComponent();
			this.Account = account;
			grid_HoaDon.ReadOnly = true;
			ApplySalesTheme();
			InitializePaymentControls();
		}

		void InitializePaymentControls()
		{
			var takeawayMenu = new ToolStripMenuItem("Đơn mang đi");
			takeawayMenu.Click += (sender, args) =>
			{
				using (var form = new Form_Takeaway())
				{
					if (form.ShowDialog(this) != DialogResult.OK)
						return;
					var table = new Label
					{
						Name = form.Selected.MaBan.Trim() + "_" + form.Selected.MaTang.Trim(),
						ImageList = list_Table,
						ImageKey = "full"
					};
					ChonBan(table, EventArgs.Empty);
				}
			};
			menuStrip1.Items.Add(takeawayMenu);
			var shiftMenu = new ToolStripMenuItem("Ca bán hàng");
			shiftMenu.Click += (sender, args) =>
			{
				using (var form = new Form_Shift(account))
					form.ShowDialog(this);
			};
			menuStrip1.Items.Add(shiftMenu);
			menuStrip1.Items.Add(new ToolStripLabel("Thanh toán:"));
			paymentMethod = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
			paymentMethod.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản" });
			paymentMethod.SelectedIndexChanged += (sender, args) =>
			{
				tbx_DuaTruoc.ReadOnly = paymentMethod.SelectedIndex == 1;
				if (paymentMethod.SelectedIndex == 1)
					tbx_DuaTruoc.Text = tbx_ThanhTien.Text;
				else
					tbx_DuaTruoc.Clear();
			};
			menuStrip1.Items.Add(paymentMethod);
			paymentMethod.SelectedIndex = 0;
		}

		/// <summary>
		/// Lấy loại tài khoản của người dùng hiện tại
		/// </summary>
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public User Account
		{
			get { return account; }
			set { account = value; }
		}

		#region Events

		private void Form_Booking_Load(object sender, EventArgs e)
		{
			if (User.Type)  //Nếu là nhân viên
			{
				côngCụQuảnTrịToolStripMenuItem.Enabled = false;
				côngCụQuảnTrịToolStripMenuItem.Visible = false;
			}
			else            //Nếu là người quản trị
			{
				côngCụQuảnTrịToolStripMenuItem.Enabled = true;
				côngCụQuảnTrịToolStripMenuItem.Visible = true;
			}
			AddOnClickToTable();
			Load_KhuyenMai();
			Load_ThucUong();
			AddTooltip();
			try
			{
				RefreshDraftIndicators();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể tải đơn chưa thanh toán. Hãy kiểm tra cơ sở dữ liệu và các tập lệnh nâng cấp: " +
					ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
			}
		}

		private void lbl_refresh_Click(object sender, EventArgs e)
		{
			Load_KhuyenMai();
			Load_ThucUong();
		}


		private void thoátToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DialogResult dialog = MessageBox.Show("Bạn có muốn thoát hoàn toàn khỏi ứng dụng?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (dialog == DialogResult.OK)
				Environment.Exit(1);
		}

		private void đăngXuấtToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DialogResult dialog = MessageBox.Show("Bạn có muốn đăng xuất khỏi ứng dụng?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (dialog == DialogResult.OK)
				this.Close();
		}

		private void quảnLýThứcUốngToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!User.Type) //Chỉ có người quản trị mới xem được form này
			{
				Form_DrinkManager form = new Form_DrinkManager();
				form.ShowDialog();
			}
		}

		private void quảnLýNhânViênToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!User.Type) //Chỉ có người quản trị mới xem được form này
			{
				Form_StaffManager form = new Form_StaffManager();
				form.ShowDialog();
			}
		}

		private void quảnLýSựKiệnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!User.Type) //Chỉ có người quản trị mới xem được form này
			{
				Form_EventManager form = new Form_EventManager();
				form.ShowDialog();
			}
		}

		private void thayĐổiThôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Form_ChangeInfo form = new Form_ChangeInfo(Account);
			form.ShowDialog();
		}

		private void Form_Booking_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)   //Nếu thoát bằng cách nhấn nút "x"
				thoátToolStripMenuItem1.PerformClick();
		}

		private void lbl_ChuyenThanhHoaDon_Click(object sender, EventArgs e)
		{
			if (grid_DonHang.Rows.Count == 0)
			{
				MessageBox.Show("Chẳng có gì hết trơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			if (grid_HoaDon.Rows.Count > 0 && CheckBillIsSaved())
			{
				MessageBox.Show("Hóa đơn này đã được lưu", "Thông báo");
				return;
			}
			string maHD = GetBillID();
			if (maHD == null)
				return;
			string ngay = DateTime.Today.ToString("dd/MM/yyyy");
			string maNV = account.MaNhanVien;
			string[] maBan = selectedTable.Name.Split('_');
			int discount = GetDiscountPercent();
			if (discount < 0)
				return;
			int tongTien = GetTotalPrice();
			if (tongTien < 0)
				return;
			int thanhTien = (int)((long)tongTien * (100 - discount) / 100);
			tbx_TongTien.Text = tongTien.ToString();
			tbx_ThanhTien.Text = thanhTien.ToString();
			if (paymentMethod.SelectedIndex == 1)
				tbx_DuaTruoc.Text = tbx_ThanhTien.Text;
			string maSK = discount != 0 ? cbx_KhuyenMai.Text : "NONAME";
			grid_HoaDon.Rows.Clear();
			grid_HoaDon.Rows.Add(maHD, ngay, maNV, maBan[0], maBan[1], thanhTien, maSK);
			foreach (DataGridViewRow order in grid_DonHang.Rows)
				order.Cells[4].Value = maHD;
		}

		private void lbl_InHD_Click(object sender, EventArgs e)
		{
			if (grid_HoaDon.Rows.Count == 0)
			{
				MessageBox.Show("Chưa có hóa đơn", "Thông báo");
				return;
			}
			try
			{
				if (CheckBillIsSaved())
				{
					MessageBox.Show("Hóa đơn này đã được thanh toán.", "Thông báo");
					return;
				}
				if (ShiftService.Current(database) == null)
				{
					MessageBox.Show("Vui lòng mở ca trước khi thanh toán.", "Thông báo");
					return;
				}
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể kiểm tra hóa đơn hoặc ca: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (tbx_DuaTruoc.Text == "")
			{
				MessageBox.Show("Bạn chưa nhập số tiền mà khách đưa", "Thông báo");
				return;
			}
			bool transfer = paymentMethod.SelectedIndex == 1;
			string prompt = transfer
				? "Bạn đã kiểm tra và xác nhận nhận đủ chuyển khoản? Hóa đơn sẽ được lưu."
				: "Xác nhận đã nhận tiền mặt và lưu hóa đơn?";
			DialogResult dialog = MessageBox.Show(prompt, "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (dialog == DialogResult.OK)
			{
				string maHoaDon = grid_HoaDon.Rows[0].Cells[0].Value.ToString();
				try
				{
					using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
					{
						long shiftId = ShiftService.LockOpenShift(database);
						var parts = selectedTable.Name.Split('_');
						var expected = CurrentDraftLines();
						DraftService.Consume(database, parts[0], parts[1], expected);
						if (!AddBillToDb())
							return;
						AddOrderToDb();
						database.Database.ExecuteSqlCommand(
							"INSERT INTO dbo.ThanhToanHoaDon (MaHD, MaCa, PhuongThuc, LucThanhToan) VALUES (@id, @shift, @method, SYSDATETIME())",
							new SqlParameter("@id", maHoaDon), new SqlParameter("@shift", shiftId),
							new SqlParameter("@method", transfer ? "TRANSFER" : "CASH"));
						transaction.Commit();
					}
				}
				catch (SqlException ex)
				{
					ResetDatabaseAfterFailure(ex.Message);
					return;
				}
				catch (DbUpdateException ex)
				{
					ResetDatabaseAfterFailure(ex.GetBaseException().Message);
					return;
				}
				catch (EntityException ex)
				{
					ResetDatabaseAfterFailure(ex.GetBaseException().Message);
					return;
				}
				catch (InvalidOperationException ex)
				{
					ResetDatabaseAfterFailure(ex.Message);
					return;
				}
				selectedTable.ImageKey = "full";
				AddTooltipToTable(maHoaDon);
				RefreshTableAppearance();
				try
				{
					ShowOrderByID(maHoaDon);
					PrintBill(maHoaDon);
				}
				catch (SqlException ex)
				{
					MessageBox.Show("Hóa đơn đã lưu, nhưng không thể tải để in: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (System.Drawing.Printing.InvalidPrinterException ex)
				{
					MessageBox.Show("Hóa đơn đã lưu, nhưng không thể in: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (InvalidOperationException ex)
				{
					MessageBox.Show("Hóa đơn đã lưu, nhưng không thể in: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		void ResetDatabaseAfterFailure(string message)
		{
			database.Dispose();
			database = new CafeRestaurantEntities();
			MessageBox.Show("Không thể lưu hóa đơn: " + message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void lbl_ChuyenBan_Click(object sender, EventArgs e)
		{
			try
			{
				if (IsTakeaway(selectedTable))
				{
					MessageBox.Show("Đơn mang đi không thể chuyển bàn.", "Thông báo");
					return;
				}
				if (selectedTable == null || grid_DonHang.Rows.Count == 0 ||
					(grid_HoaDon.Rows.Count > 0 && CheckBillIsSaved()))
				{
					MessageBox.Show("Hãy chọn bàn có đơn hàng chưa thanh toán để chuyển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				var source = selectedTable;
				var knownTables = database.Ban.AsNoTracking().Select(b => new { b.MaBan, b.MaTang }).ToList()
					.Select(b => b.MaBan.Trim() + "_" + b.MaTang.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
				var destinations = table_Tang0.Controls.Cast<Label>().Concat(table_Tang1.Controls.Cast<Label>())
					.Where(table => table != source && table.ImageKey == "empty" &&
						tooltip.GetTooltip(table) == "" && knownTables.Contains(table.Name))
					.ToList();
				if (destinations.Count == 0)
				{
					MessageBox.Show("Không có bàn trống để chuyển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				using (var form = new Form_Moving(source, destinations))
				{
					if (form.ShowDialog(this) != DialogResult.OK)
						return;
					MoveDraftTo(form.Destination);
				}
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể chuyển bàn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (EntityException ex)
			{
				MessageBox.Show("Không thể chuyển bàn: " + ex.GetBaseException().Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		bool MoveDraftTo(Label destination)
		{
			if (selectedTable == null || grid_DonHang.Rows.Count == 0 ||
				(grid_HoaDon.Rows.Count > 0 && CheckBillIsSaved()) ||
				destination == null || destination == selectedTable ||
				!table_Tang0.Controls.Contains(destination) && !table_Tang1.Controls.Contains(destination) ||
				destination.ImageKey != "empty" || tooltip.GetTooltip(destination) != "")
			{
				MessageBox.Show("Không thể chuyển đơn. Hãy chọn lại bàn trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			string[] parts = destination.Name.Split('_');
			if (parts.Length != 2)
			{
				MessageBox.Show("Bàn đích không có trong cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			string maBan = parts[0];
			string maTang = parts[1];
			if (!database.Ban.Any(b => b.MaBan == maBan && b.MaTang == maTang))
			{
				MessageBox.Show("Bàn đích không có trong cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			var sourceParts = selectedTable.Name.Split('_');
			DraftService.Move(sourceParts[0], sourceParts[1], maBan, maTang);
			selectedTable.ImageKey = "empty";
			destination.ImageKey = "full";
			if (grid_HoaDon.Rows.Count > 0)
			{
				grid_HoaDon.Rows[0].Cells[3].Value = maBan;
				grid_HoaDon.Rows[0].Cells[4].Value = maTang;
			}
			selectedTable = destination;
			lbl_BanChon.Text = destination.Name;
			tooltip.SetToopTip(lbl_BanChon, destination.Name);
			RefreshTableAppearance();
			return true;
		}

		private void lbl_XoaBan_Click(object sender, EventArgs e)
		{
			if (selectedTable == null)
			{
				MessageBox.Show("Bạn chưa chọn bàn", "Thông báo");
				return;
			}
			bool hasDraft;
			try
			{
				var tableParts = selectedTable.Name.Split('_');
				hasDraft = tooltip.GetTooltip(selectedTable) == "" && database.Database.SqlQuery<int>(
					"SELECT COUNT(*) FROM dbo.DonTam WHERE MaBan = @table AND MaTang = @floor",
					new SqlParameter("@table", tableParts[0]), new SqlParameter("@floor", tableParts[1])).Single() != 0;
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể kiểm tra đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (hasDraft)
			{
				if (User.Type)
				{
					MessageBox.Show("Chỉ quản trị viên được hủy đơn chưa thanh toán.", "Thông báo");
					return;
				}
				if (MessageBox.Show("Hủy toàn bộ đơn chưa thanh toán của bàn này?", "Xác nhận",
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
					return;
				try
				{
					var parts = selectedTable.Name.Split('_');
					DraftService.Cancel(parts[0], parts[1], account.MaNhanVien);
				}
				catch (SqlException ex)
				{
					MessageBox.Show("Không thể hủy đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				catch (InvalidOperationException ex)
				{
					MessageBox.Show(ex.Message, "Thông báo");
					return;
				}
			}
			selectedTable.ImageKey = "empty";
			tooltip.SetToopTip(selectedTable, "");
			grid_HoaDon.Rows.Clear();
			grid_DonHang.Rows.Clear();
			ClearPaymentFields();
			RefreshTableAppearance();
		}

		private void lbl_XoaTatCa_Click(object sender, EventArgs e)
		{
			// Drafts are never removed by the bulk table-reset action.
			foreach (Label item in table_Tang0.Controls.Cast<Label>().Concat(table_Tang1.Controls.Cast<Label>()))
			{
				if (tooltip.GetTooltip(item) != "")
				{
					item.ImageKey = "empty";
					tooltip.SetToopTip(item, "");
				}
			}
			if (selectedTable != null && selectedTable.ImageKey == "empty")
			{
				grid_HoaDon.Rows.Clear();
				grid_DonHang.Rows.Clear();
				ClearPaymentFields();
			}
			RefreshTableAppearance();
		}

		private void lbl_XemHD_Click(object sender, EventArgs e)
		{
			Form_Bill form = new Form_Bill();
			form.ShowDialog();
		}

		private void btn_XoaDH_Click(object sender, EventArgs e)
		{
			if (grid_DonHang.SelectedRows.Count == 0)
			{
				MessageBox.Show("Bạn chưa chọn đơn hàng", "Thông báo");
				return;
			}
			if (grid_HoaDon.Rows.Count > 0 && CheckBillIsSaved())
			{
				MessageBox.Show("Không thể sửa hóa đơn đã lưu", "Thông báo");
				return;
			}
			if (grid_DonHang.Rows.Count == 1)
			{
				if (User.Type)
				{
					MessageBox.Show("Chỉ quản trị viên được hủy đơn chưa thanh toán.", "Thông báo");
					return;
				}
				if (MessageBox.Show("Xóa món cuối sẽ hủy đơn. Tiếp tục?", "Xác nhận",
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
					return;
			}
			try
			{
				var parts = selectedTable.Name.Split('_');
				long id = Convert.ToInt64(grid_DonHang.SelectedRows[0].Cells[0].Value);
				DraftService.RemoveLine(parts[0], parts[1], id, account.MaNhanVien);
				LoadDraft(parts[0], parts[1]);
				grid_HoaDon.Rows.Clear();
				ClearPaymentFields();
				RefreshDraftIndicators();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể xóa món: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message, "Thông báo");
			}
		}

		private void btn_ThemDH_Click(object sender, EventArgs e)
		{
			if (lbl_BanChon.Text == "")
			{
				MessageBox.Show("Bạn chưa chọn bàn", "Thông báo");
				return;
			}
			else if (nud_SoLuong.Value == 0)
			{
				MessageBox.Show("Bạn chưa chọn số lượng thức uống", "Thông báo");
				return;
			}
			if (grid_HoaDon.Rows.Count > 0 && CheckBillIsSaved())
			{
				MessageBox.Show("Không thể thêm món vào hóa đơn đã lưu. Hãy làm trống bàn trước.", "Thông báo");
				return;
			}
			string selectedDrink = cbx_ThucUong.Text;
			int separator = selectedDrink.LastIndexOf(" - ", StringComparison.Ordinal);
			if (separator <= 0 || separator + 3 >= selectedDrink.Length)
			{
				MessageBox.Show("Vui lòng chọn thức uống trong danh sách", "Thông báo");
				return;
			}
			ThucUong thucUong = GetSelectedDrink(selectedDrink.Substring(0, separator).Trim(), selectedDrink.Substring(separator + 3).Trim());
			if (thucUong == null)
			{
				MessageBox.Show("Không tìm thấy thức uống trong CSDL", "Thông báo");
				return;
			}
			try
			{
				var parts = selectedTable.Name.Split('_');
				DraftService.Add(parts[0], parts[1], thucUong.MaThucUong, thucUong.MaLoai, (byte)nud_SoLuong.Value);
				LoadDraft(parts[0], parts[1]);
				grid_HoaDon.Rows.Clear();
				ClearPaymentFields();
				RefreshDraftIndicators();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể lưu món: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void tbx_DuaTruoc_TextChanged(object sender, EventArgs e)
		{
			decimal thanhTien = 0;
			decimal duaTruoc = 0;
			if (Decimal.TryParse(tbx_ThanhTien.Text, out thanhTien) && Decimal.TryParse(tbx_DuaTruoc.Text, out duaTruoc))
				tbx_ConLai.Text = ((int)(duaTruoc - thanhTien)).ToString(); //Tự động tính tiền thối lại
		}

		private void thứcUốngBánChạyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!User.Type)
			{
				Form_BestSelling form = new Form_BestSelling();
				form.ShowDialog();
			}
		}

		private void thuNhậpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!User.Type)
			{
				Form_InCome form = new Form_InCome();
				form.ShowDialog();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Áp dụng các sự kiện vào label chọn bàn
		/// </summary>
		void AddOnClickToTable()
		{
			foreach (Label label in table_Tang0.Controls.Cast<Control>().Concat(table_Tang1.Controls.Cast<Control>()))
			{
				label.Click += ChonBan;
			}
		}

		private void ChonBan(object sender, EventArgs e)
		{
			if (selectedTable == sender)
				return;
			var table = (Label)sender;
			string billId = tooltip.GetTooltip(table);
			string[] parts = table.Name.Split('_');
			try
			{
				if (billId == "")
				{
					LoadDraft(parts[0], parts[1]);
					grid_HoaDon.Rows.Clear();
				}
				else
				{
					ShowBillByID(billId);
					ShowOrderByID(billId);
				}
				lbl_BanChon.Text = IsTakeaway(table) ? "Mang đi: " + parts[0] : table.Name;
				tooltip.SetToopTip(lbl_BanChon, table.Name);
				selectedTable = table;
				ClearPaymentFields();
				RefreshDraftIndicators();
			}
			catch (SqlException ex)
			{
				MessageBox.Show("Không thể tải đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void LoadDraft(string table, string floor)
		{
			var lines = DraftService.Lines(database, table, floor);
			grid_DonHang.Rows.Clear();
			foreach (var line in lines)
				grid_DonHang.Rows.Add(line.MaDong, line.MaThucUong, line.MaLoai, line.SoLuong, "");
		}

		List<DraftLine> CurrentDraftLines()
		{
			return grid_DonHang.Rows.Cast<DataGridViewRow>().Select(row => new DraftLine
			{
				MaDong = Convert.ToInt64(row.Cells[0].Value),
				MaThucUong = row.Cells[1].Value.ToString(),
				MaLoai = row.Cells[2].Value.ToString(),
				SoLuong = Convert.ToByte(row.Cells[3].Value)
			}).ToList();
		}

		void RefreshDraftIndicators()
		{
			var drafts = DraftService.Tables(database)
				.Select(t => t.MaBan.Trim() + "_" + t.MaTang.Trim())
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			foreach (Label table in table_Tang0.Controls.Cast<Label>().Concat(table_Tang1.Controls.Cast<Label>()))
			{
				if (tooltip.GetTooltip(table) == "")
					table.ImageKey = drafts.Contains(table.Name) ? "full" : "empty";
			}
			if (IsTakeaway(selectedTable) && tooltip.GetTooltip(selectedTable) == "")
				selectedTable.ImageKey = drafts.Contains(selectedTable.Name) ? "full" : "empty";
			RefreshTableAppearance();
		}

		bool IsTakeaway(Label table)
		{
			return table != null && !table_Tang0.Controls.Contains(table) && !table_Tang1.Controls.Contains(table);
		}

		/// <summary>
		/// Load các khuyến mãi hiện hành vào cbx_KhuyenMai
		/// </summary>
		void Load_KhuyenMai()
		{
			var today = DateTime.Today;
			var tomorrow = today.AddDays(1);
			var list = (from data in database.SuKien
						where data.MaSuKien.Equals("NONAME") ||
						(data.ThoiGian_BD < tomorrow && data.ThoiGian_KT >= today && data.SoLuong > 0 && data.TyLeGiam <= 100)
						select data.MaSuKien).ToList();
			cbx_KhuyenMai.DataSource = list;
			cbx_KhuyenMai.Text = "NONAME";
		}

		/// <summary>
		/// Load các thức uống hiện hành vào cbx_ThucUong
		/// </summary>
		void Load_ThucUong()
		{
			var list = (from data in database.ThucUong
						where data.KhaDung == true  //Thức uống khả dụng (còn hàng)
						select data.TenThucUong + " - " + data.MaLoai).ToList();
			cbx_ThucUong.DataSource = list;
		}

		/// <summary>
		/// Tìm kiếm và trả về một thức uống có trong CSDL
		/// </summary>
		/// <param name="tenTU">Tên thức uống</param>
		/// <param name="maLoai">Mã thức uống</param>
		/// <returns>Thức uống tương ứng</returns>
		ThucUong GetSelectedDrink(string tenTU, string maLoai)
		{
			ThucUong ketQua = null; //Kết quả mặc định nếu không tìm thấy
			ketQua = database.ThucUong.AsNoTracking().Where(d => d.TenThucUong.Equals(tenTU) && d.MaLoai.Equals(maLoai) && d.KhaDung == true).FirstOrDefault();
			return ketQua;
		}

		/// <summary>
		/// Tìm kiếm và trả về một thức uống có trong CSDL
		/// </summary>
		/// <param name="maTU">Mã thức uống</param>
		/// <param name="maLoai">Mã loại</param>
		/// <returns>Thức uống tương ứng</returns>
		ThucUong GetDrink(string maTU, string maLoai)
		{
			ThucUong ketQua = null;
			ketQua = database.ThucUong.AsNoTracking().Where(d => d.MaThucUong.Equals(maTU) && d.MaLoai.Equals(maLoai)).SingleOrDefault();
			return ketQua;
		}

		/// <summary>
		/// Lấy phần trăm giảm giá
		/// </summary>
		/// <returns>0 nếu mã giảm giá hết hạn, hết lượt khuyến mãi hoặc không áp dụng mã nào, ngược lại là số % được giảm</returns>
		int GetDiscountPercent()
		{
			string maGiamGia = cbx_KhuyenMai.Text;
			if (maGiamGia == "NONAME")
				return 0;
			var today = DateTime.Today;
			var tomorrow = today.AddDays(1);
			var suKien = database.SuKien.AsNoTracking().SingleOrDefault(s => s.MaSuKien == maGiamGia &&
				s.SoLuong > 0 && s.ThoiGian_BD < tomorrow && s.ThoiGian_KT >= today);
			if (suKien == null || !suKien.TyLeGiam.HasValue || suKien.TyLeGiam > 100)
			{
				MessageBox.Show("Khuyến mãi không hợp lệ hoặc đã hết hạn", "Thông báo");
				return -1;
			}
			return suKien.TyLeGiam.Value;
		}

		/// <summary>
		/// Tính tổng giá sau khuyến mãi của các đơn hàng hiện tại
		/// </summary>
		/// <returns>Tổng tiền phải thanh toán</returns>
		int GetTotalPrice()
		{
			decimal sum = 0;
			for (int i = 0; i < grid_DonHang.Rows.Count; i++)
			{
				DataGridViewRow row = grid_DonHang.Rows[i];
				ThucUong thucUong = GetDrink(row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());
				if (thucUong == null)
				{
					MessageBox.Show("Không tìm thấy thức uống trong CSDL", "Thông báo");
					return -1;
				}
				if (!thucUong.DonGia.HasValue || thucUong.DonGia < 0)
				{
					MessageBox.Show("Đơn giá không hợp lệ", "Thông báo");
					return -1;
				}
				sum += thucUong.DonGia.Value * int.Parse(row.Cells[3].Value.ToString());
			}
			if (sum > int.MaxValue || sum != decimal.Truncate(sum))
			{
				MessageBox.Show("Tổng tiền phải là số nguyên trong giới hạn cho phép", "Thông báo");
				return -1;
			}
			return (int)sum;
		}

		/// <summary>
		/// Lấy mã đơn hàng khả dụng
		/// </summary>
		/// <returns>Mã đơn hàng DHddmmyyyxxxx</returns>
		string GetOrderID()
		{
			try
			{
				string result = database.Database.SqlQuery<string>("select dbo.fn_SinhMaDH()").FirstOrDefault();
				if (result == null)
					MessageBox.Show("Đã hết mã đơn hàng trong ngày", "Thông báo");
				return result;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				return null;
			}
		}

		/// <summary>
		/// Lấy mã hóa đơn khả dụng
		/// </summary>
		/// <returns>Mã hóa đơn HDddmmyyyxxxx</returns>
		string GetBillID()
		{
			try
			{
				string result = database.Database.SqlQuery<string>("select dbo.fn_SinhMaHD()").FirstOrDefault();
				if (result == null)
					MessageBox.Show("Đã hết mã hóa đơn trong ngày", "Thông báo");
				return result;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				return null;
			}
		}

		/// <summary>
		/// Tạo một id tiếp theo dựa vào ID ban đầu (áp dụng cho cả hóa đơn và đơn hàng)
		/// </summary>
		/// <param name="currentID">ID ban đầu (HDddmmyyxxxx hoặc DHddmmyyxxxx)</param>
		/// <returns>ID kế tiếp</returns>
		string GetNextID(string currentID)
		{
			try
			{
				string result = database.Database.SqlQuery<string>("select dbo.fn_TaoID_TiepTheo(@id)",
					new SqlParameter("@id", currentID)).FirstOrDefault();
				if (result == null)
					MessageBox.Show("Không thể tạo mã đơn hàng tiếp theo", "Thông báo");
				return result;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				return null;
			}
		}

		/// <summary>
		/// Thêm tooltip cho các controls
		/// </summary>
		void AddTooltip()
		{
			tooltip.SetToopTip(btn_ThemDH, "Thêm lựa chọn hiện tại vào danh sách đơn hàng");
			tooltip.SetToopTip(btn_XoaDH, "Xóa một lựa chọn ra khỏi danh sách đơn hàng");
		}

		/// <summary>
		/// Kiểm tra thông tin một hóa đơn nằm trên giao diện chính có lưu vào hệ thống chưa
		/// </summary>
		/// <returns>True nếu hóa đơn đó đã được lưu rồi hoặc khi trống, False nếu tồn tại mà chưa lưu</returns>
		bool CheckBillIsSaved()
		{
			if (grid_HoaDon.Rows.Count > 0)
			{
				DataGridViewRow row = grid_HoaDon.Rows[0];
				string maHD = row.Cells[0].Value.ToString();
				var result = (from data in database.HoaDon  //Tìm kiếm hóa đơn có trong CSDL không?
							  where data.MaHD.Equals(maHD)
							  select data).ToList();
				if (result.Count == 0)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Thêm hóa đơn vào CSDL
		/// </summary>
		/// <returns>True nếu thành công, False nếu gặp vấn đề</returns>
		bool AddBillToDb()
		{
			DataGridViewRow row = grid_HoaDon.Rows[0];
			string maHoaDon = row.Cells[0].Value.ToString();
			int duaTruocValue;
			int tongTien = GetTotalPrice();
			int discount = GetDiscountPercent();
			if (tongTien < 0 || discount < 0)
				return false;
			int thanhTienValue = (int)((long)tongTien * (100 - discount) / 100);
			string maSuKien = discount == 0 ? "NONAME" : cbx_KhuyenMai.Text;
			if (row.Cells[6].Value.ToString() != maSuKien ||
				Convert.ToInt32(row.Cells[5].Value) != thanhTienValue)
			{
				MessageBox.Show("Giá hoặc khuyến mãi đã thay đổi. Vui lòng chuyển lại thành hóa đơn.", "Thông báo");
				return false;
			}
			if (!int.TryParse(tbx_DuaTruoc.Text, out duaTruocValue) || duaTruocValue < thanhTienValue)
			{
				MessageBox.Show("Tiền khách đưa không hợp lệ hoặc chưa đủ", "Thông báo");
				return false;
			}
			if (database.HoaDon.Any(h => h.MaHD == maHoaDon))
			{
				MessageBox.Show("Hóa đơn đã được lưu", "Thông báo");
				return false;
			}
			SqlParameter maHD = new SqlParameter("@MaHD", maHoaDon);
			SqlParameter ngayHD = new SqlParameter("@NgayHD", DateTime.Today);
			SqlParameter maNV = new SqlParameter("@MaNV", row.Cells[2].Value.ToString());
			SqlParameter maBan = new SqlParameter("@MaBan", row.Cells[3].Value.ToString());
			SqlParameter maTang = new SqlParameter("@MaTang", row.Cells[4].Value.ToString());
			SqlParameter thanhTien = new SqlParameter("@TongTien", thanhTienValue);
			SqlParameter maSK = new SqlParameter("@MaSuKien", row.Cells[6].Value.ToString());
			SqlParameter duaTruoc = new SqlParameter("@DuaTruoc", duaTruocValue);
			SqlParameter conLai = new SqlParameter("@ConLai", duaTruocValue - thanhTienValue);
			database.Database.ExecuteSqlCommand("exec usp_ThemHoaDon @MaHD, @NgayHD, @MaNV, @MaBan, @MaTang, @TongTien, @MaSuKien, @DuaTruoc, @ConLai",
				maHD, ngayHD, maNV, maBan, maTang, thanhTien, maSK, duaTruoc, conLai);
			if (!database.HoaDon.Any(h => h.MaHD == maHoaDon))
				throw new InvalidOperationException("Không thể tạo hóa đơn trong cơ sở dữ liệu.");
			return true;
		}

		/// <summary>
		/// Thêm hóa đơn vào CSDL
		/// </summary>
		/// <returns>True nếu thành công, False nếu gặp vấn đề</returns>
		void AddOrderToDb()
		{
			string nextId = GetOrderID();
			if (nextId == null)
				throw new InvalidOperationException("Không thể tạo mã đơn hàng.");
			var added = new List<DonHang>();
			foreach (DataGridViewRow row in grid_DonHang.Rows)
			{
				DonHang donHang = new DonHang
				{
					MaDonHang = nextId,
					MaThucUong = row.Cells[1].Value.ToString(),
					MaLoai = row.Cells[2].Value.ToString(),
					SoLuong = byte.Parse(row.Cells[3].Value.ToString()),
					MaHD = grid_HoaDon.Rows[0].Cells[0].Value.ToString()
				};
				database.DonHang.Add(donHang);
				added.Add(donHang);
				if (added.Count < grid_DonHang.Rows.Count)
				{
					nextId = GetNextID(nextId);
					if (nextId == null)
						throw new InvalidOperationException("Không thể tạo mã đơn hàng tiếp theo.");
				}
			}
			database.SaveChanges();
			foreach (var order in added)
			{
				int count = database.Database.ExecuteSqlCommand(
					@"INSERT INTO dbo.GiaBanDonHang (MaDonHang, DonGia)
					  SELECT @id, DonGia FROM dbo.ThucUong
					  WHERE MaThucUong = @drink AND MaLoai = @type AND DonGia IS NOT NULL AND DonGia >= 0",
					new SqlParameter("@id", order.MaDonHang),
					new SqlParameter("@drink", order.MaThucUong),
					new SqlParameter("@type", order.MaLoai));
				if (count != 1)
					throw new InvalidOperationException("Không thể lưu đơn giá của món.");
			}
		}

		/// <summary>
		/// In hóa đơn
		/// </summary>
		/// <param name="hoaDon">Hóa đơn cần in</param>
		private void PrintBill(string maHD)
		{
			string receipt = BillReceipt.Load(maHD);
			using (var form = new Form_BillDetail(receipt))
				form.ShowDialog(this);
		}

		/// <summary>
		/// Thêm tooltip chính là mã hóa đơn của vị khách đang ngồi tại bàn
		/// </summary>
		/// <param name="text"></param>
		void AddTooltipToTable(string text)
		{
			tooltip.SetToopTip(selectedTable, text);
		}

		/// <summary>
		/// Hiển thị thông tin hóa đơn dựa theo mã hóa đơn
		/// </summary>
		/// <param name="id">Mã hóa đơn</param>
		void ShowBillByID(string id)
		{
			var table = from data in database.HoaDon
						where data.MaHD.Equals(id)
						select new
						{
							MãHóaĐơn = data.MaHD,
							NgàyHóaĐơn = data.NgayHD,
							MãNhânViên = data.MaNV,
							MãBàn = data.MaBan,
							MãTầng = data.MaTang,
							TổngTiền = data.TongTien,
							MãSựKiện = data.MaSuKien
						};
			grid_HoaDon.Rows.Clear();
			foreach (var item in table.ToList())
				grid_HoaDon.Rows.Add(item.MãHóaĐơn, item.NgàyHóaĐơn, item.MãNhânViên, item.MãBàn,
					item.MãTầng, item.TổngTiền, item.MãSựKiện);
		}

		/// <summary>
		/// Hiển thị thông tin đơn hàng dựa vào mã hóa đơn
		/// </summary>
		/// <param name="id">Mã hóa đơn</param>
		void ShowOrderByID(string id)
		{
			var table = from data in database.DonHang
						where data.MaHD.Equals(id)
						select new
						{
							MãĐHàng = data.MaDonHang,
							MãThứcUống = data.MaThucUong,
							MãLoại = data.MaLoai,
							SốLượng = data.SoLuong,
							MãHĐơn = data.MaHD
						};
			grid_DonHang.Rows.Clear();
			foreach (var item in table.ToList())
				grid_DonHang.Rows.Add(item.MãĐHàng, item.MãThứcUống, item.MãLoại, item.SốLượng, item.MãHĐơn);
		}

		#endregion

	}
}
