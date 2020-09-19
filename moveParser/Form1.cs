using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using hap = HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace moveParser
{
    public partial class Form1 : Form
    {
        public class LevelUpMove
        {
            public int Level;
            public string Move;
        }
        public class MonData
        {
            public string VarName;
            public string DefName;
            public List<LevelUpMove> LevelMoves;
            public List<string> ExtraMoves;
        }

        public Form1()
        {
            InitializeComponent();
        }

        protected Dictionary<string, string> LoadPkmnNameListFromSerebii()
        {
            Dictionary<string, string> pkmnList = new Dictionary<string, string>();
            string html = "https://www.serebii.net/pokemon/nationalpokedex.shtml";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='dextable']/tr");

            for(int i = 2; i < nodes.Count; i++)
            {
                hap.HtmlNode nodo = nodes[i];
                string number = nodo.ChildNodes[1].InnerHtml.Trim();
                string species = nodo.ChildNodes[5].ChildNodes[1].InnerHtml.Trim();
                pkmnList.Add(number, species);
            }

            return pkmnList;
        }

        protected List<LevelUpMove> LoadLevelUpMoves(string pkmnName)
        {
            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();

            string html = "https://serebii.net/pokedex-swsh/" + pkmnName.ToLower() + "/index.shtml";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='dextable']/tr/td/h3");

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    hap.HtmlNodeCollection moves;
                    hap.HtmlNode nodo = nodes[i];
                    if (nodo.InnerText.Equals("Standard Level Up"))
                    {
                        moves = nodo.ParentNode.ParentNode.ParentNode.ChildNodes;
                        int move_num = 0;
                        string move_lvl;
                        foreach (hap.HtmlNode move in moves)
                        {
                            LevelUpMove lmove = new LevelUpMove();
                            if (move_num % 3 == 2)
                            {
                                move_lvl = move.ChildNodes[1].InnerText;
                                if (move_lvl.Equals("&#8212;"))
                                    lmove.Level = 1;
                                else if (move_lvl.Equals("Evolve"))
                                    lmove.Level = 0;
                                else
                                    lmove.Level = int.Parse(move_lvl);
                                lmove.Move = "MOVE_" + move.ChildNodes[3].ChildNodes[0].InnerText.ToUpper().Replace(" ", "_");

                                lvlMoves.Add(lmove);
                            }

                            //name = move.InnerText;
                            move_num++;
                        }

                    }
                }
            }
            return lvlMoves;
        }

        private void btnLoadFromSerebii_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<MonData> Database = new List<MonData>();
            Dictionary<string, string> nameList = LoadPkmnNameListFromSerebii();

            int namecount = nameList.Count;
            //Dictionary<string, string> nameList = new Dictionary<string, string>();
            //nameList.Add("#001", "Bulbasaur");
            int i = 1;
            foreach (KeyValuePair<string, string> item in nameList)
            {
                //if (i < 100)
                {
                    MonData mon = new MonData();
                    mon.VarName = item.Value;
                    mon.DefName = "SPECIES_" + item.Value.ToUpper();
                    mon.LevelMoves = LoadLevelUpMoves(item.Value);

                    Database.Add(mon);
                }
                backgroundWorker1.ReportProgress(i * 100 / namecount);
                i++;
            }

            File.WriteAllText("output/db.json", JsonConvert.SerializeObject(Database, Formatting.Indented));
            /*
            for (int i = 1; i <= 100; i++)
            {
                // Wait 100 milliseconds.
                Thread.Sleep(100);
                // Report progress.
                backgroundWorker1.ReportProgress(i);
            }
            */
        }

        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            pbar1.Value = e.ProgressPercentage;
            // Set the text.
            this.Text = e.ProgressPercentage.ToString();
        }
    }
}
