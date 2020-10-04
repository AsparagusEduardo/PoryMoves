using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static moveParser.data.MoveData;
using hap = HtmlAgilityPack;

namespace moveParser.data
{
    public class MonData
    {
        public List<LevelUpMove> LevelMoves = new List<LevelUpMove>();
        public List<string> TMMoves = new List<string>();
        public List<string> EggMoves = new List<string>();
        public List<string> TutorMoves = new List<string>();
    }
    public class MonName
    {
        public int NatDexNum;
        public string SpeciesName;
        public bool IsBaseForm;
        public string FormName;
        public string VarName;
        public string DefName;
        public MonName(int nat, string og, bool isfrm, string formtm, string var, string def)
        {
            NatDexNum = nat;
            SpeciesName = og;
            IsBaseForm = isfrm;
            FormName = formtm;
            VarName = var;
            DefName = def;
        }
    }

    public class PokemonData
    {
        public static Dictionary<string, MonData> GetMonDataFromFile(string filedir)
        {
            Dictionary<string, MonData> dict;
            if (!File.Exists(filedir))
                return null;
            string text = File.ReadAllText(filedir);

            dict = JsonConvert.DeserializeObject<Dictionary<string, MonData>>(text);


            return dict;
        }

        public static List<MonName> GetMonNamesFromFile(string filedir)
        {
            List<MonName> list;
            string text = File.ReadAllText(filedir);

            list = JsonConvert.DeserializeObject<List<MonName>>(text);


            return list;
        }

        public static MonData LoadMonData(MonName name, GenerationData gen)
        {
            if (gen.dbFilename.Equals("lgpe") && (name.NatDexNum > 151 && (name.NatDexNum != 808 && name.NatDexNum != 809)))
                return null;

            MonData mon = new MonData();

            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();
            List<LevelUpMoveId> lvlMovesId = new List<LevelUpMoveId>();
            List<int> TMMovesIds = new List<int>();
            List<string> TMMoves = new List<string>();

            List<int> EggMovesIds = new List<int>();
            List<string> EggMoves = new List<string>();

            List<int> TutorMovesIds = new List<int>();
            List<string> TutorMoves = new List<string>();

            if (gen.genNumber < 7 && name.FormName.Contains("Alola"))
                return null;

            if (gen.maxDexNum < name.NatDexNum)
                return null;

            string html = "https://bulbapedia.bulbagarden.net/w/index.php?title=" + name.SpeciesName + "_(Pokémon)";
            if (!gen.isLatestGen)
                html += "/Generation_" + gen.genNumberRoman + "_learnset";
            html += "&action=edit";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc;
            try
            {
                htmlDoc = web.Load(html);
            }
            catch (System.Net.WebException)
            {
                return null;
            }
            
            hap.HtmlNodeCollection columns;

            columns = htmlDoc.DocumentNode.SelectNodes("//textarea");

            int column = 0;
            int gamecolumnamount = 1;
            int movetutorcolumn = gen.moveTutorColumn;
            string gameAbv = gen.dbFilename.ToUpper();
            string gametosearch = gen.gameFullName;

            if (columns != null)
            {
                bool inList = false;
                bool readingLearnsets = !gen.isLatestGen;
                bool readingLevelUp = false;
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
                    if (readingLearnsets && textRow.Contains("Pokémon") && !name.SpeciesName.Equals("Bonsly"))
                        gameText = textRow;
                    else if (textRow.Contains("=Learnset="))
                        readingLearnsets = true;
                    else if (textRow.Contains("=Side game data="))
                        readingLearnsets = false;

                    if (readingLearnsets && !textRow.Trim().Equals(""))
                    {
                        rownum++;
                        if (textRow.ToLower().Contains("{{learnlist/movena|"))
                            return null;
                        else if (textRow.ToLower().Contains("by [[level|leveling up]]"))
                            modeText = "Level";
                        else if (textRow.Contains("By [[TM]]"))
                            modeText = "TM";
                        else if (textRow.Contains("By {{pkmn|breeding}}"))
                            modeText = "EGG";
                        else if (textRow.ToLower().Contains("by [[move tutor|tutoring]]"))
                            modeText = "TUTOR";
                        else if (textRow.Contains("====") && !readingLevelUp && !textRow.Contains("Pokémon")
                            && !textRow.Contains("By a prior [[evolution]]") && !textRow.Contains("Special moves") && !textRow.Contains("By {{pkmn2|event}}s"))
                            formText = Regex.Replace(textRow.Replace("=", ""), "{{sup([^{]*)([A-Z][a-z]*)}}", "");

                        else if (textRow.ToLower().Contains("{{learnlist/levelh")
                            || textRow.ToLower().Contains("{{learnlist/tmh")
                            || textRow.ToLower().Contains("{{learnlist/breedh")
                            || textRow.ToLower().Contains("{{learnlist/tutorh"))
                        {
                            if (modeText == null)
                                continue;
                            if (matchForm(formText, name.FormName) && (gameText == null || gameText.Contains(gametosearch)))
                            {
                                if (modeText.Equals("Level") && !LevelUpListRead)
                                {
                                    inList = true;
                                    readingLevelUp = true;
                                    string[] rowdata = textRow.Split('|');

                                    gamecolumnamount = rowdata.Length - 5;
                                    if (rowdata[rowdata.Length - 1].Contains("xy="))
                                        gamecolumnamount--;
                                    if (gamecolumnamount <= 0)
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
                        else if (textRow.ToLower().Contains("{{learnlist/levelf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                LevelUpListRead = true;
                            formText = null;
                            readingLevelUp = false;
                        }
                        else if (textRow.ToLower().Contains("{{learnlist/tmf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                TMListRead = true;
                            formText = null;
                        }
                        else if (textRow.ToLower().Contains("{{learnlist/breedf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                EggListRead = true;
                            formText = null;
                        }
                        else if (textRow.ToLower().Contains("{{learnlist/tutorf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                TutorListRead = true;
                            formText = null;
                        }
                        else if (inList && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            if (modeText.Equals("Level") && !LevelUpListRead && (formText == null || formText.Equals(name.FormName)))
                            {
                                string lvltext = textRow.Replace("{{tt|Evo.|Learned upon evolving}}", "0");
                                string[] rowdata = System.Text.RegularExpressions.Regex.Replace(lvltext, "{{tt([^}]+)}}", "").Split('|');
                                string lvl = rowdata[column];
                                string movename = rowdata[gamecolumnamount + 1];

                                if (!lvl.Equals("N/A"))
                                {
                                    int moveId = SerebiiNameToID[movename];

                                    if (moveId == 617)
                                    {
                                        if (name.SpeciesName.Equals("FLOETTE_ETERNAL_FLOWER"))
                                            lvlMovesId.Add(new LevelUpMoveId(int.Parse(lvl), moveId));
                                    }
                                    else
                                    {
                                        if (lvl.Equals("0"))
                                            evoMovesId.Add(moveId);
                                        else
                                            lvlMovesId.Add(new LevelUpMoveId(int.Parse(lvl), moveId));
                                    }
                                }
                            }
                            else if (modeText.Equals("TM") && !TMListRead && (formText == null || formText.Equals(name.FormName)) && !Regex.IsMatch(textRow.ToLower(), "{{learnlist/t[mr].+null}}"))
                            {
                                string[] rowdata = textRow.Split('|');
                                string movename = rowdata[2];

                                TMMovesIds.Add(SerebiiNameToID[movename]);
                            }
                            else if (modeText.Equals("EGG") && !EggListRead && (formText == null || formText.Equals(name.FormName)) && !Regex.IsMatch(textRow.ToLower(), "{{learnlist/breed.+null"))
                            {
                                string breedtext = textRow.Replace("{{tt|*|No legitimate means to pass down move}}", "");
                                breedtext = breedtext.Replace("{{tt|*|Male-only, and none of the evolutions can learn this move legitimately}}", "");
                                string[] rowdata = System.Text.RegularExpressions.Regex.Replace(breedtext, "{{sup(.*)\v([A-Z]*)}}|{{MS([^}]+)}}", "MON").Split('|');
                                string movename = rowdata[2];

                                if (Regex.IsMatch(breedtext, "{{sup(.*)\\\u007C([A-Z]*)}}") && !breedtext.Contains(gen.dbFilename.ToUpper()))
                                    continue;
                                if (!movename.Equals("Light Ball}}{{tt") && !(textRow.Contains("†") && !isIncenseBaby(name.SpeciesName)))
                                    EggMovesIds.Add(SerebiiNameToID[movename]);
                            }
                            else if (modeText.Equals("TUTOR") && !TutorListRead && !Regex.IsMatch(textRow.ToLower(), "{{learnlist/tutor.+null}}")
                                 && matchForm(formText, name.FormName))
                            {
                                string tutortext = textRow.Replace("{{tt|*|", "");
                                string[] rowdata = System.Text.RegularExpressions.Regex.Replace(tutortext, "}}", "").Split('|');
                                //if 
                                string movename = rowdata[1];
                                try
                                {
                                    int tutorpad;
                                    if (gen.genNumber == 3 || gen.genNumber == 4)
                                        tutorpad = 10;
                                    else
                                        tutorpad = 8;


                                    if (rowdata[tutorpad + movetutorcolumn].Equals("yes"))
                                    {
                                        int modeid = SerebiiNameToID[movename];
                                        if (modeid == 520 && name.SpeciesName.Equals("Silvally"))
                                        {
                                            TutorMovesIds.Add(518);
                                            TutorMovesIds.Add(519);
                                        }
                                        TutorMovesIds.Add(modeid);
                                    }
                                }
                                catch (IndexOutOfRangeException) { }
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
        public static bool isIncenseBaby(string name)
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

        public static bool matchForm(string currentForm, string formToCheck)
        {
            if (currentForm == null)
                return true;
            if (currentForm.Equals(formToCheck))
                return true;
            if (currentForm.Equals(formToCheck + " / Defense Forme"))
                return true;
            if (currentForm.Equals("Attack Forme / " + formToCheck))
                return true;
            return false;
        }
    }
}
