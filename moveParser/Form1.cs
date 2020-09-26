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

                lista.Add(new MonName(number, species, true, species, species, NameToVarFormat(species), NameToDefineFormat(species)));
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

            string html = "https://bulbapedia.bulbagarden.net/w/index.php?title=" + name.SpeciesName + "_(Pok%C3%A9mon)/Generation_VII_learnset&action=edit";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection columns;

            columns = htmlDoc.DocumentNode.SelectNodes("//textarea");

            int column = 0;
            int gamecolumnamount = 1;
            int movetutorcolumn = 2;
            string gameAbv = gen.dbFilename.ToUpper();
            string gametosearch = "Pokémon Ultra Sun and Ultra Moon";

            if (columns != null)
            {
                bool inList = false;
                bool LevelUpListRead = false;
                bool TMListRead = false;
                bool EggListRead = false;
                bool TutorListRead = false;
                string pagetext = columns[0].InnerText.Replace("&lt;br>\n", "&lt;br>");
                string gameText = null, modeText = null, formText = null;

                int rownum = 0;
                List<int> evoMovesId = new List<int>();

                foreach (string textRow in pagetext.Split('\n'))
                {
                    if (!textRow.Trim().Equals(""))
                    {
                        rownum++;

                        if (textRow.Contains(gametosearch))
                            gameText = gametosearch;
                        else if (textRow.Contains("=" + name.SpeciesName + "="))
                            formText = name.SpeciesName;
                        else if (textRow.Contains("=" + name.FormName_TMs + "="))
                            formText = name.FormName_TMs;
                        else if (textRow.Contains("By [[Level|leveling up]]"))
                            modeText = "Level";
                        else if (textRow.Contains("By [[TM]]"))
                            modeText = "TM";
                        else if (textRow.Contains("By {{pkmn|breeding}}"))
                            modeText = "EGG";
                        else if (textRow.Contains("By [[Move Tutor|tutoring]]"))
                            modeText = "TUTOR";
                        else if (textRow.Contains("Pokémon: Let's Go, Pikachu! and Let's Go, Eevee!") || textRow.Contains("Pokémon Ultra Sun and Ultra Moon"))
                            gameText = textRow;

                        else if (textRow.Contains("{{learnlist/levelh/" + gen.genNumber + "|" + name.SpeciesName + "|")
                            || textRow.Contains("{{learnlist/tmh/" + gen.genNumber + "|" + name.SpeciesName + "|")
                            || textRow.Contains("{{learnlist/breedh/" + gen.genNumber + "|" + name.SpeciesName + "|")
                            || textRow.Contains("{{learnlist/tutorh/" + gen.genNumber + "|" + name.SpeciesName + "|"))
                        {
                            if (modeText == null)
                                continue;
                            if (formText == null || formText.Equals(name.FormName_TMs))
                            {
                                if (modeText.Equals("Level") && !LevelUpListRead)
                                {
                                    inList = true;
                                    string[] rowdata = textRow.Split('|');
                                
                                    gamecolumnamount = rowdata.Length - 5;
                                    if (gamecolumnamount == 0)
                                        gamecolumnamount = 1;
                                    for (int i = 0; i < rowdata.Length; i++)
                                    {
                                        string bb = rowdata[i].Replace("}", "");
                                        if (rowdata[i].Replace("}", "").Equals(gameAbv))
                                        {
                                            column = i - 4;
                                        }
                                    }
                                    if (column == 0)
                                        column = 1;
                                }
                                if ((modeText.Equals("TM") && !TMListRead) || (modeText.Equals("EGG") && !EggListRead) || (modeText.Equals("TUTOR") && !TutorListRead))
                                {
                                    inList = true;
                                }
                            }

                        }
                        else if (textRow.Contains("{{learnlist/levelf/" + gen.genNumber + "|" + name.SpeciesName + "|"))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName_TMs))
                                LevelUpListRead = true;
                        }
                        else if (textRow.ToLower().Contains(("{{learnlist/tmf/" + gen.genNumber + "|" + name.SpeciesName + "|").ToLower()))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName_TMs))
                                TMListRead = true;
                        }
                        else if (textRow.ToLower().Contains(("{{learnlist/breedf/" + gen.genNumber + "|" + name.SpeciesName + "|").ToLower()))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName_TMs))
                                EggListRead = true;
                        }
                        else if (textRow.ToLower().Contains(("{{learnlist/tutorf/" + gen.genNumber + "|" + name.SpeciesName + "|").ToLower()))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName_TMs))
                                TutorListRead = true;
                        }
                        else if (inList && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            if (modeText.Equals("Level") && !LevelUpListRead && (formText == null || formText.Equals(name.FormName_TMs)))
                            {
                                string lvltext = textRow.Replace("{{tt|Evo.|Learned upon evolving}}", "0");
                                string[] rowdata = System.Text.RegularExpressions.Regex.Replace(lvltext, "{{tt([^}]+)}}", "").Split('|');
                                string lvl = rowdata[column];
                                string movename = rowdata[gamecolumnamount + 1];

                                if (!lvl.Equals("N/A"))
                                {
                                    if (lvl.Equals("0"))
                                        evoMovesId.Add(SerebiiNameToID[movename]);
                                    else
                                        lvlMovesId.Add(new LevelUpMoveId(int.Parse(lvl), SerebiiNameToID[movename]));
                                }
                            }
                            else if (modeText.Equals("TM") && !TMListRead && (formText == null || formText.Equals(name.FormName_TMs)) && !textRow.Equals("{{learnlist/tm7null}}"))
                            {
                                string[] rowdata = textRow.Split('|');
                                string movename = rowdata[2];

                                TMMovesIds.Add(SerebiiNameToID[movename]);
                            }
                            else if (modeText.Equals("EGG") && !EggListRead && (formText == null || formText.Equals(name.FormName_TMs)) && !textRow.Equals("{{learnlist/breed7null}}"))
                            {
                                string breedtext = textRow.Replace("{{MS|000}}{{tt|*|No legitimate means to pass down move}}", "");
                                string[] rowdata = System.Text.RegularExpressions.Regex.Replace(breedtext, "{{MSP([^}]+)}}", "MON").Split('|');
                                string movename = rowdata[2];

                                if (!movename.Equals("Light Ball}}{{tt") && !(textRow.Contains("†") && !isIncenseBaby(name.SpeciesName)))
                                    EggMovesIds.Add(SerebiiNameToID[movename]);
                            }
                            else if (modeText.Equals("TUTOR") && !TutorListRead && (formText == null || formText.Equals(name.FormName_TMs)) && !textRow.Equals("{{learnlist/tutor7null}}"))
                            {
                                string[] rowdata = System.Text.RegularExpressions.Regex.Replace(textRow, "}}", "").Split('|');
                                //if 
                                string movename = rowdata[1];
                                if (rowdata[8 + movetutorcolumn].Equals("yes"))
                                    TutorMovesIds.Add(SerebiiNameToID[movename]);
                            }

                            //for (int i = 0; )
                        }
                    }

                }
                foreach (int evoid in evoMovesId)
                    lvlMoves.Add(new LevelUpMove(0, "MOVE_" + MoveDefNames[evoid]));
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

        private bool isIncenseBaby(string name)
        {
            switch (name)
            {
                case "Munchlax":
                case "Budew":
                case "Bonsly":
                case "Happiny":
                case "Wynaut":
                case "Azurill":
                case "Mantyke":
                case "Chingling":
                case "Mime Jr.":
                    return true;
                default:
                    return false;
            }
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
            //try
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
            /*
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateLoadingMessage("Couldn't load Pokémon data. (" + current + ")");
            }
            */
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
