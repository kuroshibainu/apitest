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

namespace webapi
{
    public partial class Form1 : Form
    {
        private const string RANKING = "ランク";
        private const string IMAGE = "画像";
        private const string TITLE = "タイトル";
        private const string URL = "URL";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.rbMale.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = "";
            this.dataGridView1.DataSource = null;

            if(string.IsNullOrEmpty( this.tbYahooDevId.Text))
            {
                MessageBox.Show("アプリケーションIDを入力してください。");
                return;
            }
            var appId = "appid=" + this.tbYahooDevId.Text;

            string gender = this.rbMale.Checked ? "male" : "female";
            string api = string.Format("{0}?{1}{2}"
                                        , "http://shopping.yahooapis.jp/ShoppingWebService/V1/categoryRanking"
                                        , appId
                                        , "&gender=" + gender + "&generation=" + this.nudGeneration.Value.ToString());

            var req = WebRequest.Create(api);

            using (var res = req.GetResponse())
            using (var s = res.GetResponseStream())
            {
                XElement xdoc = XElement.Load(s);
                var ns = xdoc.GetDefaultNamespace();

                var title = from x in xdoc.Descendants(ns + "Result").Elements(ns + "RankingInfo")
                            select new
                            {
                                startDay = x.Element(ns + "StartDate").Value,
                                endDay = x.Element(ns + "EndDate").Value,
                                rsGender = x.Element(ns + "Gender").Value,
                                rsGeneration = x.Element(ns + "Generation").Value,
                            };

                var querys = from x in xdoc.Descendants(ns + "Result").Elements(ns + "RankingData")
                                 // TODO：ソートできない・・・
                                 //orderby x.Attribute("rank") descending
                             select new
                             {
                                 rank = x.Attribute("rank").Value,
                                 name = x.Element(ns + "Name").Value,
                                 url = x.Element(ns + "Url").Value,
                                 imageUrl = x.Element(ns + "Image").Element(ns + "Small").Value
                             };

                StringBuilder sb = new StringBuilder();
                DataTable dt = new DataTable();
                dt.Columns.Add(RANKING, typeof(String));
                dt.Columns.Add(IMAGE, typeof(Image));
                dt.Columns.Add(TITLE, typeof(String));
                dt.Columns.Add(URL, typeof(String));

                foreach (var item in querys)
                {
                    sb.Append(RANKING + item.rank + "位\r\n");
                    sb.Append("名前　　　：　" + item.name + "\r\n");
                    sb.Append("URL　　：　" + item.url + "\r\n");
                    sb.Append("画像URL　　：　" + item.imageUrl + "\r\n\r\n\r\n");

                    DataRow dr = dt.NewRow();

                    PictureBox pb = new PictureBox();
                    WebClient wc = new WebClient();
                    Stream stream = wc.OpenRead(item.imageUrl);
                    Bitmap bitmap = new Bitmap(stream);
                    stream.Close();
                    pb.Image = bitmap;

                    dr[RANKING] = item.rank + "位";
                    dr[IMAGE] = pb.Image;
                    dr[TITLE] = item.name;
                    dr[URL] = item.url;
                    dt.Rows.Add(dr);
                }

                StringBuilder sb2 = new StringBuilder();
                foreach (var item in title)
                {
                    sb2.Append("性別：" + item.rsGender);
                    sb2.Append("　年代：" + item.rsGeneration + "代" + Environment.NewLine);
                    sb2.Append(item.startDay).Append("　～　").Append(item.endDay);
                }

                this.label1.Text = sb2.ToString();
                this.textBox2.Text = sb.ToString();

                this.dataGridView1.DataSource = dt;
                this.dataGridView1.Columns[RANKING].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                this.dataGridView1.Columns[IMAGE].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                this.dataGridView1.Columns[TITLE].Visible = false;
                this.dataGridView1.Columns[URL].Visible = false;
                this.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.PeachPuff;
                // タイトル列はリンクとする
                var column = new DataGridViewLinkColumn();
                column.Name = TITLE;
                column.VisitedLinkColor = Color.DeepPink;
                column.DataPropertyName = dt.Columns[TITLE].ColumnName;
                column.LinkBehavior = LinkBehavior.SystemDefault;
                column.TrackVisitedState = true;
                column.Width = 200;
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                this.dataGridView1.Columns.Add(column);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = sender as DataGridView;

            if (grid.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
            {
                DataGridViewRow row = grid.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex -1];
                Process.Start(cell.Value.ToString());
            }
        }
    }
}
