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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            public string PSName;
            public string DefName;
            public List<LevelUpMove> LevelMoves;
            public List<string> ExtraMoves;
        }

        public Form1()
        {
            InitializeComponent();
            JObject pkmn_ps_data = JObject.Parse(File.ReadAllText("data/PS/learnsets.js"));
            JObject species_defines = JObject.Parse(File.ReadAllText("data/pokeemerald/species.txt"));
            JObject move_defines = JObject.Parse(File.ReadAllText("data/pokeemerald/moves.txt"));

            List<MonData> Database = new List<MonData>();

            bool expandedTMs = true;
            bool expandedLevelUpMoves = true;

            string tmhm_learnsets_text;
            string levelup_learnsets_text;

            if (expandedTMs)
            {
                tmhm_learnsets_text = "#define TMHM(tmhm) ((u8) ((ITEM_##tmhm) - ITEM_TM01_FOCUS_PUNCH))\n";
            }
            else
            {
                tmhm_learnsets_text = "#define TMHM_LEARNSET(moves) {(u32)(moves), ((u64)(moves) >> 32)}\n"
                    + "#define TMHM(tmhm) ((u64)1 << (ITEM_##tmhm - ITEM_TM01_FOCUS_PUNCH))\n\n"
                    + "// This table determines which TMs and HMs a species is capable of learning.\n"
                    + "// Each entry is a 64-bit bit array spread across two 32-bit values, with\n"
                    + "// each bit corresponding to a TM or HM.\n"
                    + "const u32 gTMHMLearnsets[][2] =\n"
                    + "{\n";
            }

            if (expandedLevelUpMoves)
            {
                levelup_learnsets_text = "#define LEVEL_UP_MOVE(lvl, moveLearned) {.move = moveLearned, .level = lvl}\n";
            }
            else
            {
                levelup_learnsets_text = "#define TMHM_LEARNSET(moves) {(u32)(moves), ((u64)(moves) >> 32)}\n"
                    + "#define TMHM(tmhm) ((u64)1 << (ITEM_##tmhm - ITEM_TM01_FOCUS_PUNCH))\n\n"
                    + "// This table determines which TMs and HMs a species is capable of learning.\n"
                    + "// Each entry is a 64-bit bit array spread across two 32-bit values, with\n"
                    + "// each bit corresponding to a TM or HM.\n"
                    + "const u32 gTMHMLearnsets[][2] =\n"
                    + "{\n";
            }

            File.WriteAllText("output/tmhm_learnsets.h", tmhm_learnsets_text);

            foreach (KeyValuePair<string, JToken> mon in pkmn_ps_data)
            {
                string mon_define = "SPECIES_NONE";
                bool exists;
                try
                {
                    mon_define = species_defines[mon.Key].ToString();
                    exists = true;
                }
                catch (NullReferenceException)
                {
                    exists = false;
                }
                if (exists)
                {
                    MonData md = new MonData();
                    md.PSName = mon.Key;
                    md.DefName = mon_define;
                    md.LevelMoves = new List<LevelUpMove>();
                    md.ExtraMoves = new List<string>();

                    foreach (KeyValuePair<string, JToken> movedata in JObject.Parse(JObject.Parse(mon.Value.ToString())["learnset"].ToString()))
                    {
                        string movedef = "MOVE_NONE";
                        bool move_exists;
                        string[] arra = JsonConvert.DeserializeObject<string[]>(movedata.Value.ToString());


                        try
                        {
                            movedef = move_defines[movedata.Key].ToString();
                            move_exists = true;
                        }
                        catch (NullReferenceException)
                        {
                            move_exists = false;
                        }
                        if (move_exists)
                        {
                            foreach (string movedatapoint in arra)
                            {
                                if (movedatapoint.Contains("7L"))
                                {
                                    LevelUpMove lvl = new LevelUpMove();
                                    lvl.Level = int.Parse(movedatapoint.Replace("7L", ""));
                                    lvl.Move = movedef;
                                    md.LevelMoves.Add(lvl);
                                }
                                else if (movedatapoint.Contains("M"))
                                {

                                }
                            }
                        }
                    }
                    md.LevelMoves.Sort((p, q) => p.Level.CompareTo(q.Level));

                    Database.Add(md);

                    if (expandedTMs)
                    {

                    }
                }
            }

            File.WriteAllText("output/aaa..json", JsonConvert.SerializeObject(Database, Formatting.Indented));


        }
    }
}
