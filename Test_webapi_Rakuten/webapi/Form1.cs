using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Codeplex.Data;
using System.Net;
using System.Web;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace webapi {
	public partial class Form1 : Form {
		#region Const　===================
		/// <summary>ランク</summary>
		private const string RANKING = "ランク";
		/// <summary>画像</summary>
		private const string IMAGE = "画像";
		/// <summary>タイトル</summary>
		private const string TITLE = "タイトル";
		/// <summary>URL</summary>
		private const string URL = "URL";
		#endregion

		#region Constructor　===================
		/// <summary>
		/// Constructor
		/// </summary>
		public Form1() {
			InitializeComponent();
		}
		#endregion

		#region Event　===================

		#region Form1_Load　：　ロードイベント
		/// <summary>
		/// ロードイベント
		/// </summary>
		private void Form1_Load(object sender, EventArgs e) {
			this.comboBox1.DataSource = this.Init();
		}
		#endregion

		#region button1_Click　：　ボタン1　クリックイベント
		/// <summary>
		/// ボタン1　クリックイベント
		/// </summary>
		private void button1_Click(object sender, EventArgs e) {
			this.dataGridView1.DataSource = null;

			if (string.IsNullOrEmpty(this.tbDevId.Text)) {
				MessageBox.Show("アプリケーションIDを入力してください。");
				return;
			}
			var appId = "&applicationId=" + this.tbDevId.Text;

			string api = string.Format("{0}{1}{2}"
										, "https://app.rakuten.co.jp/services/api/Gora/GoraPlanSearch/20150706?format=json"
										, appId
										//, "&gender=" + gender + "&generation=" + this.nudGeneration.Value.ToString());
										, "&playDate=" + DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd") + "&areaCode=40&hits=5&sort=price");

			var req = WebRequest.Create(api);

			using (var res = req.GetResponse())
			using (var s = res.GetResponseStream()) {
				dynamic json = DynamicJson.Parse(s);

				try {
					dynamic items = json.Items;
					StringBuilder sb = new StringBuilder();
					for (int ix = 0; ix < 5; ix++) {
						dynamic item = items[ix].Item;
						sb.Append(item.golfCourseName).Append("\r\n");
					}
					MessageBox.Show(sb.ToString());
					//	string iconUrl = today.image.url;

					//	string dateLabel = today.dateLabel;
					//	string date = today.date;
					//	string telop = today.telop;

					//	var sbTempMax = new StringBuilder();
					//dynamic todayTemperatureMax = today.temperature.max;
				} catch (Exception ex) {
				}


			}
		}
		#endregion

		#region dataGridView1_CellContentClick　：　グリッド　セルクリックイベント
		/// <summary>
		/// グリッド　セルクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {
			DataGridView grid = sender as DataGridView;

			if (grid.Columns[e.ColumnIndex] is DataGridViewLinkColumn) {
				DataGridViewRow row = grid.Rows[e.RowIndex];
				DataGridViewCell cell = row.Cells[e.ColumnIndex - 1];
				Process.Start(cell.Value.ToString());
			}
		}
		#endregion

		#endregion

		#region Method　===================

		#region Init　：　初期処理
		/// <summary>
		/// 初期処理
		/// </summary>
		/// <returns>DataTable</returns>
		private DataTable Init() {
			DataTable dt = new DataTable();
			// TODO:20160116
			dt.Columns.Add("id", typeof(string));
			dt.Columns.Add("name", typeof(string));

			Dictionary<string, string> dict = new Dictionary<string, string>();
			dict.Add("011000", "稚内");
			dict.Add("016010", "札幌");
			dict.Add("020010", "青森");
			dict.Add("070010", "福島");
			dict.Add("110010", "さいたま");
			dict.Add("130010", "東京");
			dict.Add("180010", "福井");
			dict.Add("230010", "名古屋");
			dict.Add("270000", "大阪");
			dict.Add("310010", "鳥取");
			dict.Add("390010", "高知");
			dict.Add("400010", "福岡");
			dict.Add("460010", "鹿児島");
			dict.Add("471010", "沖縄");

			foreach (string id in dict.Keys) {
				DataRow dr = dt.NewRow();
				dr["id"] = id;
				dr["name"] = dict[id];
				dt.Rows.Add(dr);
			}

			return dt;
		}
		#endregion

		#endregion
	}
}
