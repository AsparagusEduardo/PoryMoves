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
using System.Text.RegularExpressions;

namespace moveParser
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Dictionary<string, MonData>> allGensData = new Dictionary<string, Dictionary<string, MonData>>();
        Dictionary<string, MonData> customGenData = new Dictionary<string, MonData>();

        public Form1()
        {
            InitializeComponent();
            LoadGenerationData();
            cmbGeneration.SelectedIndex = 0;
            //LoadPkmnNameListFromSerebii();
        }

        protected Dictionary<string, GenerationData> GenData = new Dictionary<string, GenerationData>()
        {
            {"SWSH", new GenerationData(8, "VIII", true, "swsh", "Pokémon Sword and Shield", 1) },
            {"USUM", new GenerationData(7, "VII", false, "usum", "Pokémon Ultra Sun and Ultra Moon", 2) },
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

                allGensData.Add(item.Key, PokemonData.GetMonDataFromFile("db/" + item.Value.dbFilename + ".json"));
            }
            cListLevelUp.SetItemChecked(0, true);
        }

        protected void LoadPkmnNameListFromSerebii()
        {
            List<MonName> lista = new List<MonName>();
            string html = "https://www.serebii.net/pokemon/nationalpokedex.shtml";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = web.Load(html);
            hap.HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='dextable']/tr");

            for(int i = 2; i < nodes.Count; i++)
            {
                hap.HtmlNode nodo = nodes[i];
                string number = nodo.ChildNodes[1].InnerHtml.Trim().Replace("#", "");
                string species = nodo.ChildNodes[5].ChildNodes[1].InnerHtml.Trim();

                lista.Add(new MonName(number, species, true, species, NameToVarFormat(species), NameToDefineFormat(species)));
            }

            if (!Directory.Exists("db"))
                Directory.CreateDirectory("db");
            File.WriteAllText("db/monNames.json", JsonConvert.SerializeObject(lista, Formatting.Indented));
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
                        MonData mon = PokemonData.LoadMonData(item, generation);
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
                MessageBox.Show(ex.ToString(), "Error loading " + current, MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateLoadingMessage("Couldn't load Pokémon data. (" + current + ")");
            }
            FinishMoveDataLoading();
        }

        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {
                pbar1.Value = e.ProgressPercentage;
            });
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
            SetEnableForAllElements(false);
            bwrkGroupMovesets.RunWorkerAsync();
        }
        private void bwrkExportLvl_DoWork(object sender, DoWorkEventArgs e)
        {
            SetEnableForAllElements(false);

            List<MonName> nameList = PokemonData.GetMonNamesFromFile("db/monNames.json");

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
                    mon = customGenData[name.DefName];
                    //mon = CustomData[name.DefName];
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

            bwrkExportLvl.ReportProgress(0);
            // Set the text.
            UpdateLoadingMessage(namecount + " Level Up movesets exported.");

            MessageBox.Show("Level Up moves exported to \"output/level_up_learnsets.h\"", "Success!", MessageBoxButtons.OK);
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

        private void cListLevelUp_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            /*
            for (int ix = 0; ix < cListLevelUp.Items.Count; ++ix)
                if (ix != e.Index) cListLevelUp.SetItemChecked(ix, false);
            */
        }

        private void bwrkGroupMovesets_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateLoadingMessage("Grouping movesets...");
            List<MonName> nameList = PokemonData.GetMonNamesFromFile("db/monNames.json");

            customGenData.Clear();

            int i = 1;
            int namecount = nameList.Count;
            foreach (MonName name in nameList)
            {
                MonData monToAdd = new MonData();
                monToAdd.LevelMoves = new List<LevelUpMove>();

                List<LevelUpMove> evoMoves = new List<LevelUpMove>();
                List<LevelUpMove> lvl1Moves = new List<LevelUpMove>();

                Dictionary<string, List<Tuple<int, int>>> OtherLvlMoves = new Dictionary<string, List<Tuple<int, int>>>();
                List<LevelUpMove> oLvlMoves = new List<LevelUpMove>();


                foreach (string item in cListLevelUp.CheckedItems)
                {
                    GenerationData gen = GenData[item];
                    MonData mon;
                    try
                    {
                        mon = allGensData[item][name.DefName];
                    }
                    catch (KeyNotFoundException)
                    {
                        mon = new MonData();
                    }
                    foreach (LevelUpMove move in mon.LevelMoves)
                    {
                        if (move.Level == 0)
                            evoMoves.Add(move);
                        else if (move.Level == 1)
                            lvl1Moves.Add(move);
                        else
                        {
                            if (!OtherLvlMoves.ContainsKey(move.Move))
                                OtherLvlMoves.Add(move.Move, new List<Tuple<int, int>> { new Tuple<int, int>(gen.genNumber, move.Level) });
                            else
                                OtherLvlMoves[move.Move].Add(new Tuple<int, int>(gen.genNumber, move.Level));
                        }
                    }

                }
                evoMoves = evoMoves.GroupBy(elem => elem.Move).Select(group => group.First()).ToList();
                //evoMoves = evoMoves.Distinct().ToList();
                foreach (LevelUpMove move in evoMoves)
                    monToAdd.LevelMoves.Add(move);

                lvl1Moves = lvl1Moves.GroupBy(elem => elem.Move).Select(group => group.First()).ToList();
                foreach (LevelUpMove move in lvl1Moves)
                    monToAdd.LevelMoves.Add(move);

                foreach (KeyValuePair<string, List<Tuple<int, int>>> item in OtherLvlMoves)
                {
                    int weightedSum = 0;
                    int sum = 0;

                    foreach (Tuple<int, int> l in item.Value)
                    {
                        weightedSum += l.Item1 * l.Item2;
                        sum += l.Item1;
                    }
                    //oLvlMoves.Add(new LevelUpMove(Math.Max((int)item.Value.Average(), 2), item.Key));
                    monToAdd.LevelMoves.Add(new LevelUpMove(Math.Max((int)(weightedSum / sum), 2), item.Key));
                }
                //oLvlMoves = oLvlMoves.OrderBy(o => o.Level).ToList();
                monToAdd.LevelMoves = monToAdd.LevelMoves.OrderBy(o => o.Level).ToList();

                customGenData.Add(name.DefName, monToAdd);

                i++;
                int percent = i * 100 / namecount;
                bwrkGroupMovesets.ReportProgress(percent);
            }
            bwrkExportLvl.RunWorkerAsync();
        }
    }
}
