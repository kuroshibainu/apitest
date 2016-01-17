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
		/// <summary>コース</summary>
		private const string COURSE = "コース";
		/// <summary>平日価格</summary>
		private const string WEEKDAY_PRICE = "平日価格";
		/// <summary>画像URL</summary>
		private const string IMG_URL = "画像URL";
		/// <summary>緯度</summary>
		private const string LATITUDE = "緯度";
		/// <summary>経度</summary>
		private const string LONGITUDE = "経度";
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
			this.comboBox1.DisplayMember = "name";
			this.comboBox1.ValueMember = "id";

			this.comboBox1.SelectedIndex = 0;
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
			var getCnt = 30;

			string api = string.Format("{0}{1}{2}"
										, "https://app.rakuten.co.jp/services/api/Gora/GoraPlanSearch/20150706?format=json"
										, appId
										, "&playDate=" + DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd")
										+ "&areaCode=" + this.comboBox1.SelectedValue
										+ "&hits=" + getCnt
										+ "&sort=price"
										+ "&formatVersion=2");

			var req = WebRequest.Create(api);

			using (var res = req.GetResponse())
			using (var s = res.GetResponseStream()) {
				dynamic json = DynamicJson.Parse(s);

				try {
					dynamic items = json.Items;
					DataTable dt = new DataTable();
					dt.Columns.Add(COURSE, typeof(String));
					//dt.Columns.Add(WEEKDAY_PRICE, typeof(String));
					//dt.Columns.Add(IMG_URL, typeof(String));
					dt.Columns.Add(IMG_URL, typeof(Image));
					//dt.Columns.Add(LATITUDE, typeof(decimal));
					//dt.Columns.Add(LONGITUDE, typeof(decimal));

					for (int ix = 0; ix < json.hits; ix++) {
						DataRow dr = dt.NewRow();
						dr[COURSE] = items[ix].golfCourseName
										+ Environment.NewLine
										+ Environment.NewLine
										+ items[ix].golfCourseCaption;
						//dr[WEEKDAY_PRICE] = items[ix].displayWeekdayMinBasePrice;
						//dr[IMG_URL] = items[ix].golfCourseImageUrl;
						PictureBox pb = new PictureBox();
						WebClient wc = new WebClient();
						Stream stream = wc.OpenRead(items[ix].golfCourseImageUrl);
						Bitmap bitmap = new Bitmap(stream);
						stream.Close();
						pb.Image = bitmap;
						dr[IMG_URL] = pb.Image;

						//dr[LATITUDE] = items[ix].latitude;
						//dr[LONGITUDE] = items[ix].longitude;
						dt.Rows.Add(dr);
					}

					this.dataGridView1.DataSource = dt;
					//this.dataGridView1.Columns[COURSE].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
					this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
					this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
					this.dataGridView1.Columns[COURSE].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
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
			dt.Columns.Add("id", typeof(string));
			dt.Columns.Add("name", typeof(string));

			Dictionary<string, string> dict = new Dictionary<string, string>();
			dict.Add("40", "福岡県");
			dict.Add("41", "佐賀県");
			dict.Add("42", "長崎県");
			dict.Add("43", "熊本県");
			dict.Add("44", "大分県");
			dict.Add("45", "宮崎県");
			dict.Add("46", "鹿児島県");
			dict.Add("47", "沖縄県");

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
