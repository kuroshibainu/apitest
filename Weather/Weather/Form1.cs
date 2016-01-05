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

namespace webapi
{
    public partial class Form1 : Form
    {
        private const string NO_VALUE = "---";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dt = CreateDt();

            this.listBox1.SelectedValueChanged -= new System.EventHandler(this.listBox1_SelectedValueChanged);

            this.listBox1.DataSource = dt;
            this.listBox1.DisplayMember = "name";
            this.listBox1.ValueMember = "id";

            this.listBox1.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);

            this.listBox1.SetSelected(0, true);

            this.ActiveControl = this.listBox1;
        }

        /// <summary>
        /// listBox1_SelectedValueChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            this.GetWeatherText();
        }

        private void GetWeatherText()
        {
            var url = "http://weather.livedoor.com/forecast/webservice/json/v1?city=" + this.listBox1.SelectedValue;
            var req = WebRequest.Create(url);

            using (var res = req.GetResponse())
            using (var s = res.GetResponseStream())
            {
                dynamic json = DynamicJson.Parse(s);
                try
                {
                    for (int ix = 0; ix < 3; ix++)
                    {
                        //天気(今日)
                        dynamic today = json.forecasts[ix];
                        string iconUrl = today.image.url;

                        string dateLabel = today.dateLabel;
                        string date = today.date;
                        string telop = today.telop;

                        var sbTempMax = new StringBuilder();
                        dynamic todayTemperatureMax = today.temperature.max;
                        if (todayTemperatureMax != null)
                        {
                            sbTempMax.AppendFormat("{0}℃", todayTemperatureMax.celsius);
                        }
                        else
                        {
                            sbTempMax.Append(NO_VALUE);
                        }

                        var sbTempMin = new StringBuilder();
                        dynamic todayTemperatureMin = today.temperature.min;
                        if (todayTemperatureMin != null)
                        {
                            sbTempMin.AppendFormat("{0}℃", todayTemperatureMin.celsius);
                        }
                        else
                        {
                            sbTempMin.Append(NO_VALUE);
                        }

                        //天気概況文
                        var situation = "\r\n【天気概況】\r\n" + json.description.text;

                        //area
                        string location = json.location.prefecture + "・" + json.location.city;

                        string tenki = string.Format("\r\n　　　【天気】　{0}\r\n\r\n【最高気温】　{1}\r\n\r\n【最低気温】　{2}",
                            telop,
                            sbTempMax.ToString(),
                            sbTempMin.ToString()
                            );

                        this.Place.Text = location;

                        if (ix == 0)
                        {
                            this.Today.Text = DateTime.Parse(date).ToLongDateString() + "　（今日）";
                            this.textBox1.Text = tenki;
                            this.pictureBox1.ImageLocation = iconUrl;
                        }
                        else if (ix == 1)
                        {
                            this.Tomorrow.Text = DateTime.Parse(date).ToLongDateString() + "　（明日）";
                            this.textBox2.Text = tenki;
                            this.pictureBox2.ImageLocation = iconUrl;
                        }
                        else if (ix == 2)
                        {
                            this.DayAfterTomorrow.Text = DateTime.Parse(date).ToLongDateString() + "　（明後日）";
                            this.textBox3.Text = tenki;
                            this.pictureBox3.ImageLocation = iconUrl;
                        }

                        this.textBox4.Text = situation;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// CreateDt
        /// </summary>
        /// <returns>DataTable</returns>
        private DataTable CreateDt()
        {
            DataTable dt = new DataTable();
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

            foreach ( string id in dict.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = id;
                dr["name"] = dict[id];
                dt.Rows.Add(dr);
            }

            return dt;
        }

    }
}
