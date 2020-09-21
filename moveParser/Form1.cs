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
using moveParser.data;
using static moveParser.data.MoveData;

namespace moveParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cmbGeneration.SelectedIndex = 0;
            //LoadPkmnNameListFromSerebii();
        }
        public class GenerationData
        {
            public int genNumber;
            public string dexPage;
            public string indexFormat;
            public string tableNodes;
            public string lvlUpTitle1;
            public string lvlUpTitle2;
            public string lvlUpTitle3;
            public string tmHmTrTitle;
            public string moveTutorTitle1;
            public string moveTutorTitle2;
            public string eggMoveTitle;

            public string dbFilename;
            public GenerationData(int num, string dbfile, string dxpage, string idxformat, string tabnode,
                                    string lvltitle1, string lvltitle2, string lvltitle3,
                                    string tmtitle, string tutortitle1, string tutortitle2,
                                    string eggtitle)
            {
                genNumber = num;
                dbFilename = dbfile;
                dexPage = dxpage;
                indexFormat = idxformat;
                tableNodes = tabnode;
                lvlUpTitle1 = lvltitle1;
                lvlUpTitle2 = lvltitle2;
                lvlUpTitle3 = lvltitle3;
                tmHmTrTitle = tmtitle;
                moveTutorTitle1 = tutortitle1;
                moveTutorTitle2 = tutortitle2;
                eggMoveTitle = eggtitle;
            }
        }

        protected Dictionary<string, GenerationData> GenData = new Dictionary<string, GenerationData>()
        {
            {"Gen VIII", new GenerationData(8, "swsh", "-swsh", "{1}/index", "//table[@class='dextable']",
                                            "Standard Level Up", "Standard Level Up", "Standard Level Up",
                                            "TM & HM Attacks", "Move Tutor Attacks", "Isle of Armor Move Tutor Attacks",
                                            "Egg Moves (Details)") },
            {"Gen VII", new GenerationData(7, "usum", "-sm", "{0}", "//table[@class='dextable']",
                                            "Generation VII Level Up", "Standard Level Up", "Standard Level Up",
                                            "TM & HM Attacks", "Move Tutor Attacks", "Ultra Sun/Ultra Moon Move Tutor Attacks",
                                            "Egg Moves (Details)") },
        };


        protected void LoadPkmnNameListFromSerebii()
        {
            List<MonName> lista = new List<MonName>();
            //string texto = "";

            //Dictionary<string, string> pkmnList = new Dictionary<string, string>();
            string html = "https://www.serebii.net/pokemon/nationalpokedex.shtml";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='dextable']/tr");

            for(int i = 2; i < nodes.Count; i++)
            {
                hap.HtmlNode nodo = nodes[i];
                string number = nodo.ChildNodes[1].InnerHtml.Trim().Replace("#", "");
                string species = nodo.ChildNodes[5].ChildNodes[1].InnerHtml.Trim();
                //pkmnList.Add(number, species);

                lista.Add(new MonName(number, species, null, NameToVarFormat(species), NameToDefineFormat(species)));
                //texto += "{new MonData(\"" + NameToDefineFormat(species) + "\", \"" + NameToVarFormat(species) + "\") },\n";
            }

            if (!Directory.Exists("db"))
                Directory.CreateDirectory("db");
            File.WriteAllText("db/monNames.json", JsonConvert.SerializeObject(lista, Formatting.Indented));
            //File.WriteAllText("db/monNames.json", texto);
            //return pkmnList;
        }

        protected MonData LoadMonData(MonName name, GenerationData gen8)
        {
            MonData mon = new MonData();
            mon.DefName = "SPECIES_" + name.DefName;
            mon.VarName = name.VarName;

            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();
            List<int> ExtraMovesIds = new List<int>();
            List<string> ExtraMoves = new List<string>();

            int number = int.Parse(name.NatDexNum);
            string pokedex, identifier;
            string lvlUpTitle, lvlUpTitle2, lvlUpTitle3;
            string tmHmTrTitle;
            string moveTutorTitle1, moveTutorTitle2;
            string eggMoveTitle;

            pokedex = gen8.dexPage;
            identifier = String.Format(gen8.indexFormat, name.NatDexNum, name.OriginalName.ToLower());
            lvlUpTitle = gen8.lvlUpTitle1;

            if (gen8.genNumber == 7 && (number == 808 || number == 809))
                lvlUpTitle2 = "Let's Go Level Up";
            else
                lvlUpTitle2 = "Ultra Sun/Ultra Moon Level Up";

            lvlUpTitle3 = gen8.lvlUpTitle3;
            tmHmTrTitle = gen8.tmHmTrTitle;
            moveTutorTitle1 = gen8.moveTutorTitle1;
            moveTutorTitle2 = gen8.moveTutorTitle2;
            eggMoveTitle = gen8.eggMoveTitle;

            string html = "https://serebii.net/pokedex" + pokedex + "/" + identifier + ".shtml";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection nodes;

            if (gen8.genNumber == 7 && number <= 151)
                nodes = htmlDoc.DocumentNode.SelectNodes("//li[@title='Sun/Moon/Ultra Sun/Ultra Moon']/table[@class='dextable']");
            else
                nodes = htmlDoc.DocumentNode.SelectNodes(gen8.tableNodes);

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    hap.HtmlNodeCollection moves;
                    hap.HtmlNode nodo = nodes[i];

                    //Checks for Level Up Moves
                    if (nodo.ChildNodes[0].InnerText.Equals(lvlUpTitle) || nodo.ChildNodes[0].InnerText.Equals(lvlUpTitle2) || nodo.ChildNodes[0].InnerText.Equals(lvlUpTitle3))
                    {
                        moves = nodo.ChildNodes;
                        int move_num = 0;
                        string move_lvl;

                        List<string> evoMoves = new List<string>();

                        foreach (hap.HtmlNode move in moves)
                        {
                            LevelUpMove lmove = new LevelUpMove();
                            if (move_num % 3 == 2)
                            {
                                int exMoveId = MoveData.SerebiiNameToID[move.ChildNodes[3].ChildNodes[0].InnerText];
                                ExtraMovesIds.Add(exMoveId);
                                lmove.Move = "MOVE_" + MoveData.MoveDefNames[exMoveId];

                                move_lvl = move.ChildNodes[1].InnerText;
                                if (move_lvl.Equals("&#8212;"))
                                    lmove.Level = 1;
                                else if (move_lvl.Equals("Evolve"))
                                {
                                    lmove.Level = 0;
                                    evoMoves.Add(lmove.Move);
                                }
                                else
                                    lmove.Level = int.Parse(move_lvl);


                                if (lmove.Level > 0 && evoMoves.Count > 0)
                                {
                                    foreach (string evo in evoMoves)
                                        if (!lvlMoves.Contains(new LevelUpMove(1, evo)))
                                            lvlMoves.Add(new LevelUpMove(1, evo));
                                    evoMoves.Clear();
                                }
                                if (!InList(lvlMoves,lmove))
                                    lvlMoves.Add(lmove);
                            }
                            move_num++;
                        }
                    }
                    //Checks for TM/HM/TR
                    else if (nodo.ChildNodes[0].InnerText.Equals(tmHmTrTitle))
                    {
                        moves = nodo.ChildNodes;
                        int move_num = 0;
                        foreach (hap.HtmlNode move in moves)
                        {
                            if (move_num % 3 == 2)
                            {
                                bool addMove = false;
                                if (move.ChildNodes.Count >= 17)
                                {
                                    foreach (hap.HtmlNode form in move.ChildNodes[16].ChildNodes[0].ChildNodes[0].ChildNodes)
                                    {
                                        string formname = form.ChildNodes[0].Attributes["alt"].Value;
                                        if (formname.Equals(name.FormName) || (name.FormName == null && formname.Equals("Normal")))
                                            addMove = true;
                                    }
                                }
                                else
                                    addMove = true;
                                if (addMove)
                                {
                                    int exMoveId = MoveData.SerebiiNameToID[move.ChildNodes[2].ChildNodes[0].InnerText];
                                    ExtraMovesIds.Add(exMoveId);
                                }
                            }
                            move_num++;
                        }
                    }
                    //Checks for move tutors.
                    else if ((nodo.ChildNodes[0].ChildNodes.Count > 0) && (nodo.ChildNodes[0].ChildNodes[0].InnerText.Equals(moveTutorTitle1))
                        || (nodo.ChildNodes[0].ChildNodes.Count > 0) && (nodo.ChildNodes[0].ChildNodes[0].InnerText.Equals(moveTutorTitle2)))
                    {
                        moves = nodo.ChildNodes[0].ChildNodes;
                        int move_num = 0;
                        foreach (hap.HtmlNode move in moves)
                        {
                            if (move_num != 0 && move_num % 2 == 0)
                            {
                                bool addMove = false;
                                if (move.ChildNodes.Count >= 8)
                                {
                                    foreach (hap.HtmlNode form in move.ChildNodes[8].ChildNodes[0].ChildNodes[0].ChildNodes)
                                        if (form.ChildNodes[0].Attributes["alt"].Value.Equals(name.OriginalName))
                                            addMove = true;
                                }
                                else
                                    addMove = true;
                                if (addMove)
                                {
                                    int exMoveId = MoveData.SerebiiNameToID[move.ChildNodes[0].InnerText];
                                    ExtraMovesIds.Add(exMoveId);
                                }

                            }
                            move_num++;
                        }
                    }
                    //Checks for Egg Moves
                    else if ((nodo.ChildNodes[0].ChildNodes.Count > 0) && (nodo.ChildNodes[0].ChildNodes[0].InnerText.Equals(eggMoveTitle)))
                    {
                        moves = nodo.ChildNodes;
                        int move_num = 0;
                        foreach (hap.HtmlNode move in moves)
                        {
                            if (move_num % 3 == 2)
                            {
                                bool addMove = false;
                                if (move.ChildNodes[14].ChildNodes.Count > 1)
                                {
                                    foreach (hap.HtmlNode form in move.ChildNodes[14].ChildNodes)
                                        if ((form.Attributes["alt"] != null) && (form.Attributes["alt"].Value.Equals("Normal")))
                                            addMove = true;
                                }
                                else
                                    addMove = true;
                                if (addMove)
                                {
                                    int exMoveId = MoveData.SerebiiNameToID[move.ChildNodes[0].InnerText.Replace("USUM Only", "")];
                                    ExtraMovesIds.Add(exMoveId);
                                }
                            }
                            move_num++;
                        }
                    }
                }
                ExtraMovesIds = ExtraMovesIds.Distinct().ToList();
                ExtraMovesIds.Sort();
                foreach(int exMoveId in ExtraMovesIds)
                {
                    ExtraMoves.Add("MOVE_" + MoveData.MoveDefNames[exMoveId]);
                }
            }
            else
            {
                return null;
            }
            mon.LevelMoves = lvlMoves;
            mon.ExtraMoves = ExtraMoves;

            return mon;
        }

        private string NameToDefineFormat(string oldname)
        {

            oldname = oldname.Replace("&eacute;", "E");
            oldname = oldname.Replace("-o", "_O");
            oldname = oldname.ToUpper();
            oldname = oldname.Replace(" ", "_");
            oldname = oldname.Replace("'", "");
            oldname = oldname.Replace("-", "_");
            oldname = oldname.Replace(".", "");
            oldname = oldname.Replace("&#9792;", "_F");
            oldname = oldname.Replace("&#9794;", "_M");
            oldname = oldname.Replace(":", "");

            return oldname;
        }

        private string NameToVarFormat(string oldname)
        {
            oldname = oldname.Replace("&eacute;", "e");
            oldname = oldname.Replace("-o", "O");
            oldname = oldname.Replace(" ", "_");
            oldname = oldname.Replace("-", "_");
            oldname = oldname.Replace("'", "");
            oldname = oldname.Replace(".", "");
            oldname = oldname.Replace("&#9792;", "F");
            oldname = oldname.Replace("&#9794;", "M");
            oldname = oldname.Replace(":", "");

            string[] str = oldname.Split('_');
            string final = "";
            foreach (string s in str)
                final += s;

            return final;
        }
        bool InList(List<LevelUpMove> list, LevelUpMove element)
        {
            foreach (LevelUpMove entry in list)
                if (entry.Level == element.Level && entry.Move == element.Move)
                    return true;
            return false;
        }

        private void btnLoadFromSerebii_Click(object sender, EventArgs e)
        {
            btnLoadFromSerebii.Enabled = false;
            backgroundWorker1.RunWorkerAsync(cmbGeneration.SelectedItem.ToString());
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<MonName> nameList = new List<MonName>();
            List<MonData> Database = new List<MonData>();
            string namesFile = "db/monNames.json";

            UpdateLoadingMessage("Loading species...");

            if (!File.Exists(namesFile))
                LoadPkmnNameListFromSerebii();
            nameList = PokemonData.GetMonNamesFromFile(namesFile);

            GenerationData generation = GenData[(string)e.Argument];

            int namecount = nameList.Count;

            int i = 1;
            foreach (MonName item in nameList)
            {
                if (i < 31)
                {
                    MonData mon = LoadMonData(item, generation);
                    if (mon != null)
                        Database.Add(mon);
                }
                backgroundWorker1.ReportProgress(i * 100 / namecount);
                // Set the text.
                UpdateLoadingMessage(i.ToString() + " out of " + namecount + " Pokémon loaded.");
                i++;
            }
            if (!Directory.Exists("db"))
                Directory.CreateDirectory("db");

            File.WriteAllText("db/" + generation.dbFilename + ".json", JsonConvert.SerializeObject(Database, Formatting.Indented));
            FinishMoveDataLoading();
        }

        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            pbar1.Value = e.ProgressPercentage;
            // Set the text.
            //this.Text = e.ProgressPercentage.ToString();
        }

        public void UpdateLoadingMessage(string newMessage)
        {
            this.Invoke((MethodInvoker)delegate {
                this.lblLoading.Text = newMessage;
            });
        }

        public void FinishMoveDataLoading()
        {
            this.Invoke((MethodInvoker)delegate {
                this.btnLoadFromSerebii.Enabled = true;
                this.pbar1.Value = 0;
            });
        }
    }
}
