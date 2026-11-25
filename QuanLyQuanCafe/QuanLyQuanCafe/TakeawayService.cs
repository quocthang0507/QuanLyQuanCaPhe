using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyQuanCafe
{
	internal sealed class TakeawayDraft
	{
		public string MaBan { get; set; }
		public string MaTang { get; set; }
		public DateTime LucTao { get; set; }
		public int SoMon { get; set; }
		public override string ToString() => $"{MaBan.Trim()} — {LucTao:dd/MM HH:mm} — {SoMon} dòng món";
	}

	internal static class TakeawayService
	{
		internal static List<TakeawayDraft> Drafts()
		{
			using (var database = new CafeRestaurantEntities())
				return database.Database.SqlQuery<TakeawayDraft>(
					@"SELECT d.MaBan, d.MaTang, d.LucTao, COUNT(c.MaDong) AS SoMon
					  FROM dbo.DonTam d JOIN dbo.BanMangDi m ON m.MaBan = d.MaBan AND m.MaTang = d.MaTang
					  LEFT JOIN dbo.ChiTietDonTam c ON c.MaBan = d.MaBan AND c.MaTang = d.MaTang
					  GROUP BY d.MaBan, d.MaTang, d.LucTao ORDER BY d.LucTao, d.MaBan").ToList();
		}

		internal static DraftTable Reserve()
		{
			using (var database = new CafeRestaurantEntities())
			using (var transaction = database.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				var slot = database.Database.SqlQuery<DraftTable>(
					@"SELECT TOP (1) m.MaBan, m.MaTang FROM dbo.BanMangDi m WITH (UPDLOCK, HOLDLOCK)
					  WHERE NOT EXISTS (SELECT 1 FROM dbo.DonTam d WITH (UPDLOCK, HOLDLOCK)
					    WHERE d.MaBan = m.MaBan AND d.MaTang = m.MaTang)
					  ORDER BY m.MaBan").FirstOrDefault();
				if (slot == null)
				{
					var used = database.Database.SqlQuery<string>(
						"SELECT MaBan FROM dbo.Ban WITH (UPDLOCK, HOLDLOCK) WHERE MaTang = 'MANG'")
						.ToList().Select(s => s.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
					string id = Enumerable.Range(1, 9999).Select(i => "M" + i.ToString("D4"))
						.FirstOrDefault(candidate => !used.Contains(candidate));
					if (id == null)
						throw new InvalidOperationException("Không còn mã cho đơn mang đi mới.");
					slot = new DraftTable { MaBan = id, MaTang = "MANG" };
					database.Database.ExecuteSqlCommand(
						"INSERT INTO dbo.Ban (MaBan, MaTang, TenBan, TrangThai) VALUES (@table, @floor, N'Mang đi', 0)",
						new SqlParameter("@table", slot.MaBan), new SqlParameter("@floor", slot.MaTang));
					database.Database.ExecuteSqlCommand(
						"INSERT INTO dbo.BanMangDi (MaBan, MaTang) VALUES (@table, @floor)",
						new SqlParameter("@table", slot.MaBan), new SqlParameter("@floor", slot.MaTang));
				}
				database.Database.ExecuteSqlCommand(
					"INSERT INTO dbo.DonTam (MaBan, MaTang) VALUES (@table, @floor)",
					new SqlParameter("@table", slot.MaBan), new SqlParameter("@floor", slot.MaTang));
				transaction.Commit();
				return slot;
			}
		}
	}
}
