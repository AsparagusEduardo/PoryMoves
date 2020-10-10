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
        protected Dictionary<string, GenerationData> GenData;

        public Form1()
        {
            InitializeComponent();
            LoadGenerationData();
            cmbGeneration.SelectedIndex = 0;
            //LoadPkmnNameListFromSerebii();
        }

        protected void LoadGenerationData()
        {
            GenData = GenerationsData.GetGenDataFromFile("db/generations.json");
#if DEBUG
            File.WriteAllText("../../db/generations.json", JsonConvert.SerializeObject(GenData, Formatting.Indented));
#endif

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

                Dictionary<string, MonData> gen = PokemonData.GetMonDataFromFile("db/gen/" + item.Value.dbFilename + ".json");

                allGensData.Add(item.Key, gen);
            }
            //cListLevelUp.SetItemChecked(0, true);
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
                string species = nodo.ChildNodes[5].ChildNodes[1].InnerHtml.Trim().Replace("&eacute", "é").Replace("&#9792;", "♀").Replace("&#9794;", "♂");

                lista.Add(new MonName(int.Parse(number), species, true, species, NameToVarFormat(species), NameToDefineFormat(species)));
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
#if !DEBUG
            try
#endif
            {
                List<MonName> nameList = new List<MonName>();
                Dictionary<string, MonData> Database = new Dictionary<string, MonData>();
                string namesFile = "db/monNames.json";

                UpdateLoadingMessage("Loading species...");

                if (!File.Exists(namesFile))
                    LoadPkmnNameListFromSerebii();
                nameList = PokemonData.GetMonNamesFromFile(namesFile);
#if DEBUG
                File.WriteAllText("../../db/monNames.json", JsonConvert.SerializeObject(nameList, Formatting.Indented));
#endif

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
                if (!Directory.Exists("db/gen"))
                    Directory.CreateDirectory("db/gen");

                File.WriteAllText("db/gen/" + generation.dbFilename + ".json", JsonConvert.SerializeObject(Database, Formatting.Indented));
#if DEBUG
                File.WriteAllText("../../db/gen/" + generation.dbFilename + ".json", JsonConvert.SerializeObject(Database, Formatting.Indented));
#endif

                allGensData.Remove(generation.dbFilename.ToUpper());
                allGensData.Add(generation.dbFilename.ToUpper(), PokemonData.GetMonDataFromFile("db/gen/" + generation.dbFilename + ".json"));

                UpdateLoadingMessage("Pokémon data loaded.");
            }
#if !DEBUG
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error loading " + current, MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateLoadingMessage("Couldn't load Pokémon data. (" + current + ")");
            }
#endif
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
            SetEnableForAllComboBoxList(value);
            SetEnableForAllCheckbox(value);
        }

        private void SetEnableForAllButtons(bool value)
        {
            this.Invoke((MethodInvoker)delegate {
                btnLoadFromSerebii.Enabled = value;

                btnWriteLvlLearnsets.Enabled = value;
                btnLvl_All.Enabled = value;

                btnExportTM.Enabled = value;
                btnTM_All.Enabled = value;

                //btnEgg_All.Enabled = value;

                btnExportTutor.Enabled = value;
                btnTutor_All.Enabled = value;
            });
        }

        private void SetEnableForAllCombobox(bool value)
        {
            this.Invoke((MethodInvoker)delegate {
                cmbGeneration.Enabled = value;
            });
        }

        private void SetEnableForAllComboBoxList(bool value)
        {
            this.Invoke((MethodInvoker)delegate {
                cListLevelUp.Enabled = value;
                cListTMMoves.Enabled = value;
                cListTutorMoves.Enabled = value;
            });
        }

        private void SetEnableForAllCheckbox(bool value)
        {
            this.Invoke((MethodInvoker)delegate {
                chkLvl_LevelUpEnd.Enabled = value;

                chkTM_IncludeEgg.Enabled = value;
                chkTM_IncludeLvl.Enabled = value;
                chkTM_IncludeTutor.Enabled = value;
                chkTM_Extended.Enabled = value;

                chkTutor_Extended.Enabled = value;
                chkTutor_IncludeLvl.Enabled = value;
                chkTutor_IncludeEgg.Enabled = value;
                chkTutor_IncludeTM.Enabled = value;
            });
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
            bwrkExportLvl.RunWorkerAsync();
        }
        private void bwrkExportLvl_DoWork(object sender, DoWorkEventArgs e)
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
                bwrkExportLvl.ReportProgress(percent);
            }

            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");

            // file header
            string sets = "#define LEVEL_UP_MOVE(lvl, moveLearned) {.move = moveLearned, .level = lvl}\n";
            if (chkLvl_LevelUpEnd.Checked)
                sets += "#define LEVEL_UP_END (0xffff)\n";

            // iterate over mons
            i = 1;
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
                    sets += $"    LEVEL_UP_MOVE({move.Level, 2}, {move.Move}),\n";
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

        private void bwrkGroupMovesets_tm_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void bwrkExportTM_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateLoadingMessage("Grouping movesets...");
            List<MonName> nameList = PokemonData.GetMonNamesFromFile("db/monNames.json");

            Dictionary<string, List<string>> lvlMoves = new Dictionary<string, List<string>>();

            customGenData.Clear();

            int i = 0;
            int namecount = nameList.Count;
            foreach (MonName name in nameList)
            {
                MonData monToAdd = new MonData();
                monToAdd.TMMoves = new List<string>();
                lvlMoves.Add(name.DefName, new List<string>());

                foreach (string item in cListTMMoves.CheckedItems)
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
                    foreach (string move in mon.TMMoves)
                        monToAdd.TMMoves.Add(move);
                    if (chkTM_IncludeLvl.Checked)
                    {
                        foreach (LevelUpMove move in mon.LevelMoves)
                            lvlMoves[name.DefName].Add(move.Move);
                    }
                    if (chkTM_IncludeEgg.Checked)
                    {
                        foreach (string move in mon.EggMoves)
                            monToAdd.EggMoves.Add(move);
                    }
                    if (chkTM_IncludeTutor.Checked)
                    {
                        foreach (string move in mon.TutorMoves)
                            monToAdd.TutorMoves.Add(move);
                    }


                }
                monToAdd.TMMoves = monToAdd.TMMoves.GroupBy(elem => elem).Select(group => group.First()).ToList();

                customGenData.Add(name.DefName, monToAdd);

                i++;
                int percent = i * 100 / namecount;
                bwrkExportTM.ReportProgress(percent);
            }
            bool oldStyle = !chkTM_Extended.Checked;

            // load specified TM list
            List<string> tmMoves = File.ReadAllLines("input/tm.txt").ToList();
            // sanity check: old style TM list must be 64 entries or less
            if (tmMoves.Count > 64 && oldStyle)
            {
                MessageBox.Show("Old-style TM learnsets only support up to 64 TMs/HMs.\nConsider using the new format here:\nhttps://github.com/LOuroboros/pokeemerald/commit/6f69765770a52c1a7d6608a117112b78a2afcc22",
                                "FATAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (tmMoves.Count > 255 && !oldStyle)
            {
                MessageBox.Show("FATAL: New-style TM learnsets only support up to 255 TMs/HMs. Consider reducing your TM amount.",
                                "FATAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // build importable TM list
            string tms = "// IMPORTANT: DO NOT PASTE THIS FILE INTO YOUR REPO!\n// Instead, paste the following array into src/data/party_menu.h\n\nconst u16 sTMHMMoves[TMHM_COUNT] =\n{\n";
            foreach (string move in tmMoves)
            {
                tms += $"    MOVE{move.Substring(move.IndexOf('_'))},";
            }
            tms += "};\n";

            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");

            File.WriteAllText("output/party_menu_tm_list.h", tms);

            // file header
            string sets;
            if (oldStyle)
            {
                sets = $"#define TMHM_LEARNSET(moves) {{(u32)(moves), ((u64)(moves) >> 32)}}\n" +
                    $"#define TMHM(tmhm) ((u64)1 << (ITEM_##tmhm - ITEM_{tmMoves[0]}))\n\n" +
                    "// This table determines which TMs and HMs a species is capable of learning.\n" +
                    "// Each entry is a 64-bit bit array spread across two 32-bit values, with\n" +
                    "// each bit corresponding to a TM or HM.\n" +
                    "const u32 gTMHMLearnsets[][2] =\n{\n" +
                    $"    [SPECIES_NONE]        = TMHM_LEARNSET(0),\n";
            }
            else
                sets = $"#define TMHM(tmhm) ((u8) ((ITEM_##tmhm) - ITEM_{tmMoves[0]}))\n\nstatic const u8 sNoneTMHMLearnset[] =\n{{\n    0xFF,\n}};\n";

            i = 1;
            // iterate over mons
            foreach (MonName name in nameList)
            {
                MonData data = customGenData[name.DefName];
                // begin learnset
                if (oldStyle)
                {
                    sets += $"\n    {$"[SPECIES_{name.DefName}]",-22}= TMHM_LEARNSET(";
                    // hacky workaround for first move being on the same line
                    bool first = true;
                    foreach (string move in tmMoves)
                    {
                        string aa = $"MOVE{move.Substring(move.IndexOf('_'))}";
                        if (data.TMMoves.Contains(aa) || lvlMoves[name.DefName].Contains(aa) || data.EggMoves.Contains(aa) || data.TutorMoves.Contains(aa))
                        {
                            if (first)
                            {
                                sets += $"TMHM({move})";
                                first = false;
                            }
                            else
                            {
                                sets += $"\n                                        | TMHM({move})";
                            }
                        }
                    }
                    sets += "),\n";
                }
                else
                {
                    sets += $"\nstatic const u8 s{name.VarName}TMHMLearnset[] =\n{{\n";
                    foreach (string move in tmMoves)
                    {
                        string aa = $"MOVE{move.Substring(move.IndexOf('_'))}";
                        if (data.TMMoves.Contains(aa) || lvlMoves[name.DefName].Contains(aa) || data.EggMoves.Contains(aa) || data.TutorMoves.Contains(aa))
                        {
                            sets += $"    TMHM({move}),\n";
                        }
                    }
                    sets += "    0xFF,\n};\n";
                }

                int percent = i * 100 / namecount;
                bwrkExportTM.ReportProgress(percent);
                // Set the text.
                UpdateLoadingMessage(i.ToString() + " out of " + namecount + " TM movesets exported.");
                i++;
            }
            if (oldStyle)
                sets += "\n};\n";
            else
            {
                sets += "const u8 *const gTMHMLearnsets[] =\n{\n";
                foreach (MonName name in nameList)
                {
                    sets += $"    [SPECIES_{name.DefName}] = s{name.VarName}TMHMLearnset,\n";
                }
                sets += "};";
            }

            // write to file
            File.WriteAllText("output/tmhm_learnsets.h", sets);

            bwrkExportTM.ReportProgress(0);
            // Set the text.
            UpdateLoadingMessage(namecount + " TM movesets exported.");

            MessageBox.Show("TM moves exported to \"output/tmhm_learnsets.h\"", "Success!", MessageBoxButtons.OK);
            SetEnableForAllElements(true);
        }

        private void btnExportTM_Click(object sender, EventArgs e)
        {
            SetEnableForAllElements(false);
            bwrkExportTM.RunWorkerAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Loading from Bulbapedia complete!", "Success!", MessageBoxButtons.OK);
        }

        private void btnLvl_All_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cListLevelUp.Items.Count; ++i)
                cListLevelUp.SetItemChecked(i, true);
        }

        private void btnTM_All_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cListTMMoves.Items.Count; ++i)
                cListTMMoves.SetItemChecked(i, true);
        }

        private void btnEgg_All_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cListEggMoves.Items.Count; ++i)
                cListEggMoves.SetItemChecked(i, true);
        }

        private void btnTutor_All_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cListTutorMoves.Items.Count; ++i)
                cListTutorMoves.SetItemChecked(i, true);
        }

        private void bwrkExportTutor_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateLoadingMessage("Grouping movesets...");
            List<MonName> nameList = PokemonData.GetMonNamesFromFile("db/monNames.json");

            Dictionary<string, List<string>> lvlMoves = new Dictionary<string, List<string>>();

            customGenData.Clear();

            int i = 0;
            int namecount = nameList.Count;
            foreach (MonName name in nameList)
            {
                MonData monToAdd = new MonData();
                monToAdd.TutorMoves = new List<string>();
                lvlMoves.Add(name.DefName, new List<string>());

                foreach (string item in cListTutorMoves.CheckedItems)
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
                    foreach (string move in mon.TutorMoves)
                        monToAdd.TutorMoves.Add(move);
                    if (chkTutor_IncludeLvl.Checked)
                    {
                        foreach (LevelUpMove move in mon.LevelMoves)
                            lvlMoves[name.DefName].Add(move.Move);
                    }
                    if (chkTutor_IncludeEgg.Checked)
                    {
                        foreach (string move in mon.EggMoves)
                            monToAdd.EggMoves.Add(move);
                    }
                    if (chkTutor_IncludeTM.Checked)
                    {
                        foreach (string move in mon.TMMoves)
                            monToAdd.TMMoves.Add(move);
                    }


                }
                monToAdd.TutorMoves = monToAdd.TutorMoves.GroupBy(elem => elem).Select(group => group.First()).ToList();

                customGenData.Add(name.DefName, monToAdd);

                i++;
                int percent = i * 100 / namecount;
                bwrkExportTM.ReportProgress(percent);
            }
            bool oldStyle = !chkTutor_Extended.Checked;

            // load specified tutor list
            List<string> tutorMoves = File.ReadAllLines("input/tutor.txt").ToList();
            // sanity check: old style tutor list must be 32 entries or less
            if (tutorMoves.Count > 32 && oldStyle)
            {
                Console.WriteLine("FATAL: Old-style tutor learnsets only support up to 32 moves.\nConsider using the new format here:\n<commit to be made>");
                return;
            }
            if (tutorMoves.Count > 255 && !oldStyle)
            {
                Console.WriteLine("FATAL: New-style tutor learnsets only support up to 255 tutor moves. Consider reducing your tutor amount.");
            }
            // build importable tutor list
            string tutors = "// IMPORTANT: DO NOT PASTE THIS FILE INTO YOUR REPO!\n// Instead, paste the following list of defines into include/constants/party_menu.h\n\n";
            for (int j = 0; j < tutorMoves.Count; j++)
            {
                string move = tutorMoves[j];
                tutors += $"#define {move,-28} {j,3}\n";
            }
            tutors += $"#define TUTOR_MOVE_COUNT             {tutorMoves.Count,3}";
            File.WriteAllText("party_menu_tutor_list.h", tutors);

            // file header
            string sets = "const u16 gTutorMoves[TUTOR_MOVE_COUNT] =\n{\n";
            foreach (string move in tutorMoves)
            {
                sets += $"    [TUTOR_{move}] = MOVE_{move.Substring(5)},\n";
            }
            if (oldStyle)
            {
                // TODO: double check how the hell old style works	
                sets += "};\n\n#define TUTOR(move) (1u << (TUTOR_##move))\n\nstatic const u32 sTutorLearnsets[TUTOR_MOVE_COUNT] =\n{\n    [SPECIES_NONE]             = (0),\n\n";
            }
            else
            {
                sets += "};\n\n#define TUTOR(move) ((u8) (TUTOR_##move))\n\nstatic const u8 sNoneTutorLearnset[TUTOR_MOVE_COUNT] =\n{\n    0xFF,\n};\n\n";
            }
            // iterate over mons
            i = 1;
            foreach (MonName entry in nameList)
            {
                MonData data = customGenData[entry.DefName];
                // begin learnset
                if (oldStyle)
                {
                    sets += $"\n    {$"[SPECIES_{entry.DefName}]",-27}= (";
                    // hacky workaround for first move being on the same line
                    bool first = true;
                    foreach (string move in tutorMoves)
                    {
                        string aa = $"MOVE{move.Substring(move.IndexOf('_'))}";
                        if (data.TMMoves.Contains(aa) || lvlMoves[entry.DefName].Contains(aa) || data.EggMoves.Contains(aa) || data.TutorMoves.Contains(aa))
                        {
                            if (first)
                            {
                                sets += $"TUTOR({move})";
                                first = false;
                            }
                            else
                            {
                                sets += $"\n                                | TUTOR({move})";
                            }
                        }
                    }
                    sets += "),\n";
                }
                else
                {
                    sets += $"\nstatic const u8 s{entry.VarName}TutorLearnset[] =\n{{\n";
                    foreach (string move in tutorMoves)
                    {
                        string aa = $"MOVE{move.Substring(move.IndexOf('_'))}";
                        if (data.TMMoves.Contains(aa) || lvlMoves[entry.DefName].Contains(aa) || data.EggMoves.Contains(aa) || data.TutorMoves.Contains(aa))
                        {
                            sets += $"    TUTOR({move}),\n";
                        }
                    }
                    sets += "    0xFF,\n};\n";
                }

                int percent = i * 100 / namecount;
                bwrkExportTutor.ReportProgress(percent);
                // Set the text.
                UpdateLoadingMessage(i.ToString() + " out of " + namecount + " Tutor movesets exported.");
                i++;
            }


            if (oldStyle)
                sets += "\n};\n";
            else
            {
                sets += "const u8 *const sTutorLearnsets[] =\n{\n    [SPECIES_NONE] = sNoneTutorLearnset,\n";
                foreach (MonName name in nameList)
                {
                    sets += $"    [SPECIES_{name.DefName}] = s{name.VarName}TutorLearnset,\n";
                }
                sets += "};\n";
            }

            // write to file
            File.WriteAllText("output/tutor_learnsets.h", sets);

            bwrkExportTutor.ReportProgress(0);
            // Set the text.
            UpdateLoadingMessage(namecount + " Tutor movesets exported.");

            MessageBox.Show("Tutor moves exported to \"output/tutor_learnsets.h\"", "Success!", MessageBoxButtons.OK);
            SetEnableForAllElements(true);
        }

        private void btnExportTutor_Click(object sender, EventArgs e)
        {
            SetEnableForAllElements(false);
            bwrkExportTutor.RunWorkerAsync();
        }
    }
}
