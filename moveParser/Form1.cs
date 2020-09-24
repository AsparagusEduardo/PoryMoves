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
            LoadGenerationData();
            cmbGeneration.SelectedIndex = 0;
            //LoadPkmnNameListFromSerebii();
        }

        protected Dictionary<string, GenerationData> GenData = new Dictionary<string, GenerationData>()
        {
            {"SWSH", new GenerationData(8, "swsh", "-swsh", "{1}/index", "//table[@class='dextable']",
                                            "Standard Level Up", "Standard Level Up", "Level Up - {0}", "{0} Level Up",
                                            "Technical Machine Attacks", "Technical Record Attacks",
                                            "Move Tutor Attacks", "Isle of Armor Move Tutor Attacks",
                                            "Egg Moves (Details)") },
            {"USUM", new GenerationData(7, "usum", "-sm", "{0}", "//table[@class='dextable']",
                                            "Generation VII Level Up", "Standard Level Up", "Sun / Moon Level Up - {0}", "{0} Level Up",
                                            "TM & HM Attacks", "TM & HM Attacks",
                                            "Move Tutor Attacks", "Ultra Sun/Ultra Moon Move Tutor Attacks",
                                            "Egg Moves (Details)") },
        };

        protected void LoadGenerationData()
        {
            cmbGeneration.Items.Clear();
            cListLevelUp.Items.Clear();
            cListTMMoves.Items.Clear();
            cListEggMoves.Items.Clear();
            cListTutorMoves.Items.Clear();
            foreach(KeyValuePair<string, GenerationData> item in GenData)
            {
                cmbGeneration.Items.Add(item.Key);
                cListLevelUp.Items.Add(item.Key);
                cListTMMoves.Items.Add(item.Key);
                cListEggMoves.Items.Add(item.Key);
                cListTutorMoves.Items.Add(item.Key);
            }
        }

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

            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();
            List<LevelUpMoveId> lvlMovesId = new List<LevelUpMoveId>();
            List<int> TMMovesIds = new List<int>();
            List<string> TMMoves = new List<string>();

            List<int> EggMovesIds = new List<int>();
            List<string> EggMoves = new List<string>();

            List<int> TutorMovesIds = new List<int>();
            List<string> TutorMoves = new List<string>();

            int number = int.Parse(name.NatDexNum);
            string pokedex, identifier;
            string lvlUpTitle_Gen, lvlUpTitle_Game, lvlUpTitle_Form, lvlUpTitle_RegionalForm;
            string tmHmTrTitle, tmHmTrTitle2;
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
            tmHmTrTitle2 = gen.tmHmTrTitle2;
            moveTutorTitle1 = gen.moveTutorTitle1;
            moveTutorTitle2 = gen.moveTutorTitle2;
            eggMoveTitle = gen.eggMoveTitle;

            string html = "https://pokemondb.net/pokedex/" + name.SpeciesName.ToLower() + "/moves/7";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection columns;

            columns = htmlDoc.DocumentNode.SelectNodes("//div[@id='tabs-moves-16']/div/div[@class='grid-col span-lg-6']");

            if (columns != null)
            {
                string currentRead = "";
                foreach (hap.HtmlNode column in columns)
                {
                    for(int i = 0; i < column.ChildNodes.Count; i++)
                    {
                        hap.HtmlNode block = column.ChildNodes[i];

                        bool isMoveTable = false;
                        if (block.Name.Equals("h3"))
                            currentRead = block.InnerHtml;
                        else if (block.HasClass("tabset-moves-game-form") || block.HasClass("resp-scroll"))
                            isMoveTable = true;

                        if (isMoveTable)
                        {
                            int formId = 0;
                            hap.HtmlNode moveTable; 
                            if (block.HasClass("tabset-moves-game-form"))
                                moveTable = block.ChildNodes[3].ChildNodes[formId].ChildNodes[0];
                            else
                                moveTable = block;
                            foreach (hap.HtmlNode move in moveTable.SelectNodes(".//table/tbody/tr"))
                            {
                                if (currentRead.Equals("Moves learnt by level up"))
                                {
                                    int lvl = int.Parse(move.ChildNodes[0].ChildNodes[0].InnerHtml);
                                    int moveId = SerebiiNameToID[move.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerHtml];
                                    string movename = "MOVE_" + MoveDefNames[moveId];
                                    //lvlMoves.Add(new LevelUpMove(lvl, movename));
                                    lvlMovesId.Add(new LevelUpMoveId(lvl, moveId));
                                }
                                else if (currentRead.Equals("Egg moves"))
                                {
                                    int moveId = SerebiiNameToID[move.ChildNodes[0].ChildNodes[0].ChildNodes[0].InnerHtml];
                                    EggMovesIds.Add(moveId);
                                }
                                else if (currentRead.Equals("Move Tutor moves"))
                                {
                                    int moveId = SerebiiNameToID[move.ChildNodes[0].ChildNodes[0].ChildNodes[0].InnerHtml];
                                    TutorMovesIds.Add(moveId);
                                }
                                else if (currentRead.Equals("Moves learnt by TM"))
                                {
                                    int moveId = SerebiiNameToID[move.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerHtml];
                                    TMMovesIds.Add(moveId);
                                }
                            }
                        }

                    }

                }
                //lvlMovesId.Sort(new Comparison<LevelUpMoveId>(CompareLvlUpMoveIds));
                foreach (LevelUpMoveId lvlid in lvlMovesId)
                    lvlMoves.Add(new LevelUpMove(lvlid.Level, "MOVE_" + MoveDefNames[lvlid.MoveId]));

                TMMovesIds = TMMovesIds.Distinct().ToList();
                TMMovesIds.Sort();
                foreach (int exMoveId in TMMovesIds)
                    TMMoves.Add("MOVE_" + MoveData.MoveDefNames[exMoveId]);

                EggMovesIds = EggMovesIds.Distinct().ToList();
                EggMovesIds.Sort();
                foreach (int exMoveId in EggMovesIds)
                    EggMoves.Add("MOVE_" + MoveData.MoveDefNames[exMoveId]);

                TutorMovesIds = TutorMovesIds.Distinct().ToList();
                TutorMovesIds.Sort();
                foreach (int exMoveId in TutorMovesIds)
                    TutorMoves.Add("MOVE_" + MoveData.MoveDefNames[exMoveId]);
            }
            else
            {
                return null;
            }
            mon.LevelMoves = lvlMoves;
            mon.TMMoves = TMMoves;
            mon.EggMoves = EggMoves;
            mon.TutorMoves = TutorMoves;

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
            SetEnableForAllElements(false);
            backgroundWorker1.RunWorkerAsync(cmbGeneration.SelectedItem.ToString());
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string current = "";
            try
            {
                List<MonName> nameList = new List<MonName>();
                Dictionary<string, MonData> Database = new Dictionary<string, MonData>();
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
                    current = item.DefName;
                    //if (i < 31)
                    {
                        MonData mon = LoadMonData(item, generation);
                        if (mon != null)
                        {
                            try
                            {
                                Database.Add(item.DefName, mon);
                            }
                            catch (ArgumentException ex)
                            {
                                File.AppendAllText("errorLog.txt", "[" + DateTime.Now.ToString() + "] Error adding " + item.DefName + ": " + ex.Message);
                            }
                        }
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
                UpdateLoadingMessage("Couldn't load Pokémon data. (" + current + ")");
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

        public void SetEnableForAllElements(bool value)
        {
            SetEnableForAllButtons(value);
            SetEnableForAllCombobox(value);
        }

        public void FinishMoveDataLoading()
        {
            SetEnableForAllElements(true);

            this.Invoke((MethodInvoker)delegate {
                this.pbar1.Value = 0;
            });
        }


        private void btnWriteLvlLearnsets_Click(object sender, EventArgs e)
        {
            bwrkExportLvl.RunWorkerAsync();
        }
        private void bwrkExportLvl_DoWork(object sender, DoWorkEventArgs e)
        {
            SetEnableForAllElements(false);

            List<MonName> nameList = PokemonData.GetMonNamesFromFile("db/monNames.json");

            Dictionary<string, MonData> Data = PokemonData.GetMonDataFromFile("db/usum.json");

            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");

            // file header
            string sets = "#define LEVEL_UP_MOVE(lvl, moveLearned) {.move = moveLearned, .level = lvl}\n";
            if (chkLvl_LevelUpEnd.Checked)
                sets += "#define LEVEL_UP_END (0xffff)\n";

            // iterate over mons
            int namecount = nameList.Count;
            int i = 1;
            foreach (MonName name in nameList)
            {
                MonData mon = new MonData();
                try
                {
                    mon = Data[name.DefName];
                }
                catch (KeyNotFoundException) {}

                // begin learnset
                sets += $"\nstatic const struct LevelUpMove s{name.VarName}LevelUpLearnset[] = {{\n";

                if (mon.LevelMoves.Count == 0)
                    sets += "    LEVEL_UP_MOVE( 1, MOVE_POUND),\n";

                foreach (LevelUpMove move in mon.LevelMoves)
                {
                    sets += $"    LEVEL_UP_MOVE({move.Level,-2}, {move.Move}),\n";
                    //sets += "    LEVEL_UP_MOVE(" + ((move.Level < 10) ? move.Level.ToString().PadLeft(2) : move.Level.ToString()) + ", " + move.Move + "),\n";
                }
                sets += "    LEVEL_UP_END\n};\n";

                int percent = i * 100 / namecount;
                bwrkExportLvl.ReportProgress(percent);
                // Set the text.
                UpdateLoadingMessage(i.ToString() + " out of " + namecount + " Level Up movesets exported.");
                i++;
            }
            // write to file
            File.WriteAllText("output/level_up_learnsets.h", sets);

            MessageBox.Show("Level Up moves exported to \"output/level_up_learnsets.h", "Success!", MessageBoxButtons.OK);
            SetEnableForAllElements(true);
        }

        private void SetEnableForAllButtons(bool value)
        {
            this.Invoke((MethodInvoker)delegate {
                btnLoadFromSerebii.Enabled = value;
                btnWriteLvlLearnsets.Enabled = value;
            });
        }

        private void SetEnableForAllCombobox(bool value)
        {
            this.Invoke((MethodInvoker)delegate {
                cmbGeneration.Enabled = value;
            });
        }
    }
}
