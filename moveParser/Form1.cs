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

        protected Dictionary<string, GenerationData> GenData = new Dictionary<string, GenerationData>()
        {
            {"Gen VIII", new GenerationData(8, "swsh", "-swsh", "{1}/index", "//table[@class='dextable']",
                                            "Standard Level Up", "Standard Level Up", "Level Up - {0}", "{0} Level Up",
                                            "Technical Machine Attacks", "Technical Record Attacks",
                                            "Move Tutor Attacks", "Isle of Armor Move Tutor Attacks",
                                            "Egg Moves (Details)") },
            {"Gen VII", new GenerationData(7, "usum", "-sm", "{0}", "//table[@class='dextable']",
                                            "Generation VII Level Up", "Standard Level Up", "Sun / Moon Level Up - {0}", "{0} Level Up",
                                            "TM & HM Attacks", "TM & HM Attacks",
                                            "Move Tutor Attacks", "Ultra Sun/Ultra Moon Move Tutor Attacks",
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

                lista.Add(new MonName(number, species, true, null, null, NameToVarFormat(species), NameToDefineFormat(species)));
                //texto += "{new MonData(\"" + NameToDefineFormat(species) + "\", \"" + NameToVarFormat(species) + "\") },\n";
            }

            if (!Directory.Exists("db"))
                Directory.CreateDirectory("db");
            File.WriteAllText("db/monNames.json", JsonConvert.SerializeObject(lista, Formatting.Indented));
            //File.WriteAllText("db/monNames.json", texto);
            //return pkmnList;
        }

        protected MonData LoadMonData(MonName name, GenerationData gen)
        {
            MonData mon = new MonData();
            mon.DefName = "SPECIES_" + name.DefName;
            mon.VarName = name.VarName;

            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();
            List<int> ExtraMovesIds = new List<int>();
            List<string> ExtraMoves = new List<string>();

            int number = int.Parse(name.NatDexNum);
            string pokedex, identifier;
            string lvlUpTitle_Gen, lvlUpTitle_Game, lvlUpTitle_Form, lvlUpTitle_RegionalForm;
            string tmHmTrTitle;
            string moveTutorTitle1, moveTutorTitle2;
            string eggMoveTitle;

            pokedex = gen.dexPage;
            identifier = String.Format(gen.indexFormat, name.NatDexNum, name.SpeciesName.ToLower());
            lvlUpTitle_Gen = gen.lvlUpTitle_Generation;

            if (gen.genNumber == 7 && (number == 808 || number == 809))
                lvlUpTitle_Game = "Let's Go Level Up";
            else
                lvlUpTitle_Game = "Ultra Sun/Ultra Moon Level Up";

            //Checks if it's a form.
            if (name.IsBaseForm)
            {
                lvlUpTitle_Form = String.Format(gen.lvlUpTitle_Forms, "Standard");
                lvlUpTitle_RegionalForm = String.Format(gen.lvlUpTitle_RegionalForms, "Standard");
            }
            else
            {
                lvlUpTitle_Form = String.Format(gen.lvlUpTitle_Forms, name.FormName_EggTutor);
                lvlUpTitle_RegionalForm = String.Format(gen.lvlUpTitle_RegionalForms, name.FormName_EggTutor);
            }

            tmHmTrTitle = gen.tmHmTrTitle;
            moveTutorTitle1 = gen.moveTutorTitle1;
            moveTutorTitle2 = gen.moveTutorTitle2;
            eggMoveTitle = gen.eggMoveTitle;

            string html = "https://serebii.net/pokedex" + pokedex + "/" + identifier + ".shtml";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection nodes;

            if (gen.genNumber == 7 && number <= 151)
                nodes = htmlDoc.DocumentNode.SelectNodes("//li[@title='Sun/Moon/Ultra Sun/Ultra Moon']/table[@class='dextable']");
            else
                nodes = htmlDoc.DocumentNode.SelectNodes(gen.tableNodes);

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    hap.HtmlNodeCollection moves;
                    hap.HtmlNode nodo = nodes[i];

                    //Checks for Level Up Moves
                    if ((nodo.ChildNodes[0].InnerText.Equals(lvlUpTitle_Gen) && name.IsBaseForm)
                        || nodo.ChildNodes[0].InnerText.Equals(lvlUpTitle_Game)
                        || nodo.ChildNodes[0].InnerText.Equals(lvlUpTitle_Form)
                        || nodo.ChildNodes[0].InnerText.Equals(lvlUpTitle_RegionalForm))
                    {
                        moves = nodo.ChildNodes;
                        int move_num = 0;
                        string move_lvl;

                        List<string> evoMoves = new List<string>();

                        foreach (hap.HtmlNode move in moves)
                        {
                            LevelUpMove lmove = new LevelUpMove();

                            //Skips rows without relevant data.
                            if (move_num % 3 == 2)
                            {
                                int exMoveId = MoveData.SerebiiNameToID[move.ChildNodes[3].ChildNodes[0].InnerText];
                                ExtraMovesIds.Add(exMoveId);
                                lmove.Move = "MOVE_" + MoveData.MoveDefNames[exMoveId];

                                move_lvl = move.ChildNodes[1].InnerText;

                                //Dashes ("—") interpreted as level 1 moves.
                                if (move_lvl.Equals("&#8212;"))
                                    lmove.Level = 1;
                                //Adds evolution moves from a separate list.
                                else if (move_lvl.Equals("Evolve"))
                                {
                                    lmove.Level = 0;
                                    evoMoves.Add(lmove.Move);
                                }
                                else
                                    lmove.Level = int.Parse(move_lvl);

                                //Reads evo moves from separate list and adds them as level 1 moves before proper level 1 moves.
                                if (lmove.Level > 0 && evoMoves.Count > 0)
                                {
                                    foreach (string evo in evoMoves)
                                        if (!lvlMoves.Contains(new LevelUpMove(1, evo)))
                                            lvlMoves.Add(new LevelUpMove(1, evo));
                                    evoMoves.Clear();
                                }
                                //Don't add move if it's already on the list.
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
                            //Skips rows without relevant data.
                            if (move_num % 3 == 2)
                            {
                                bool addMove = false;
                                //Checks if it has an extra column for forms.
                                if (move.ChildNodes.Count >= 17)
                                {
                                    //Looks at all the forms listed in this row.
                                    foreach (hap.HtmlNode form in move.ChildNodes[16].ChildNodes[0].ChildNodes[0].ChildNodes)
                                    {
                                        string formname = form.ChildNodes[0].Attributes["alt"].Value;
                                        //It first checks if the current form has the same name as the Pokémon being checked.
                                        //If not, checks if it's a base form and if the form checked is tagged as "Normal" (useful for species with regional forms)
                                        if (formname.Equals(name.FormName_TMs) || (name.IsBaseForm && formname.Equals("Normal")))
                                            addMove = true;
                                    }
                                }
                                //Adds move if it doesn't have a form column.
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
                            //Skips rows without relevant data.
                            if (move_num != 0 && move_num % 2 == 0)
                            {
                                bool addMove = false;
                                //Checks if it has an extra column for forms.
                                if (move.ChildNodes.Count >= 8)
                                {
                                    foreach (hap.HtmlNode form in move.ChildNodes[8].ChildNodes[0].ChildNodes[0].ChildNodes)
                                    {
                                        string formname = form.ChildNodes[0].Attributes["alt"].Value;
                                        //It first checks if the current form has the same name as the Pokémon being checked.
                                        //If not, checks if it's a base form and if the form checked is tagged with the original name (useful for species with regional forms)
                                        if (formname.Equals(name.FormName_EggTutor) || (name.IsBaseForm && formname.Equals(name.SpeciesName)))
                                            addMove = true;
                                    }
                                }
                                //Adds move if it doesn't have a form column.
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
                            //Skips rows without relevant data.
                            if (move_num % 3 == 2)
                            {
                                bool addMove = false;
                                //Checks if it has an extra column for forms.
                                if (move.ChildNodes[14].ChildNodes.Count > 1)
                                {
                                    foreach (hap.HtmlNode form in move.ChildNodes[14].ChildNodes)
                                    {
                                        if (form.Attributes["alt"] != null)
                                        {
                                            string formname = form.Attributes["alt"].Value;
                                            //It first checks if the current form has the same name as the Pokémon being checked.
                                            //If not, checks if it's a base form and if the form checked is tagged as "Normal" (useful for species with regional forms)
                                            if (formname.Equals(name.FormName_EggTutor) || (name.IsBaseForm && formname.Equals("Normal")))
                                                addMove = true;
                                        }
                                    }
                                }
                                //Adds move if it doesn't have a form column.
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
            try
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
                    //if (i < 31)
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
                UpdateLoadingMessage("Pokémon data loaded.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateLoadingMessage("Couldn't load Pokémon data.");
            }
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
