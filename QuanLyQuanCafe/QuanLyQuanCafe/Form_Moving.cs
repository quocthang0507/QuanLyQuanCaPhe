using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
	public partial class Form_Moving : Form
	{
		readonly List<Label> availableDestinations;

		public Form_Moving(Label source, IEnumerable<Label> destinations)
		{
			InitializeComponent();
			availableDestinations = destinations.ToList();
			cbx_BanChuyen.DropDownStyle = ComboBoxStyle.DropDownList;
			cbx_BanChuyen.Items.Add(source.Name);
			cbx_BanChuyen.SelectedIndex = 0;
			cbx_BanChuyen.Enabled = false;
			cbx_BanDen.DropDownStyle = ComboBoxStyle.DropDownList;
			cbx_BanDen.Items.AddRange(availableDestinations.Select(table => table.Name).ToArray());
			if (cbx_BanDen.Items.Count > 0)
				cbx_BanDen.SelectedIndex = 0;
			btn_Chuyen.Enabled = cbx_BanDen.Items.Count > 0;
			AcceptButton = btn_Chuyen;
			btn_Chuyen.Location = new Point(90, 125);
			var cancel = new Button
			{
				Text = "Hủy",
				Location = new Point(194, 125),
				Size = btn_Chuyen.Size,
				DialogResult = DialogResult.Cancel,
				FlatStyle = FlatStyle.Flat,
				BackColor = Color.White,
				ForeColor = Color.FromArgb(66, 45, 33),
				UseVisualStyleBackColor = false
			};
			Controls.Add(cancel);
			CancelButton = cancel;
			StartPosition = FormStartPosition.CenterParent;
			BackColor = Color.FromArgb(250, 246, 239);
			ForeColor = Color.FromArgb(66, 45, 33);
			btn_Chuyen.FlatStyle = FlatStyle.Flat;
			btn_Chuyen.BackColor = Color.FromArgb(111, 73, 49);
			btn_Chuyen.ForeColor = Color.White;
			btn_Chuyen.FlatAppearance.BorderSize = 0;
			btn_Chuyen.UseVisualStyleBackColor = false;
		}

		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public Label Destination
		{
			get
			{
				int index = cbx_BanDen.SelectedIndex;
				return index >= 0 && index < availableDestinations.Count ? availableDestinations[index] : null;
			}
		}

		private void btn_Chuyen_Click(object sender, EventArgs e)
		{
			if (Destination == null)
			{
				MessageBox.Show("Vui lòng chọn bàn trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
