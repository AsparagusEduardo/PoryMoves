using Newtonsoft.Json;
using PoryMoves.entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static moveParser.data.Move;
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
        public bool CanHatchFromEgg;
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

        enum readingMode
        {
            NONE = 0,
            LEVEL,
            TM,
            EGG,
            TUTOR,
            TRANSFER
        }

        public static MonData LoadMonData(MonName name, GenerationData gen, Dictionary<string, Move> MoveData)
        {
            if (gen.dbFilename.Equals("lgpe") && (name.NatDexNum > 151 && (name.NatDexNum != 808 && name.NatDexNum != 809)))
                return null;

            MonData mon = new MonData();

            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();
            List<Move> TMMoves = new List<Move>();
            List<Move> EggMoves = new List<Move>();
            List<Move> TutorMoves = new List<Move>();

            if (gen.genNumber < 7 && name.FormName.Contains("Alola"))
                return null;

            if (gen.maxDexNum < name.NatDexNum)
                return null;

            string html = "https://bulbapedia.bulbagarden.net/w/index.php?title=" + name.SpeciesName + "_(Pokémon)";
            if (!gen.isLatestGen)
                html += "/Generation_" + gen.genNumberRoman + "_learnset";
            html += "&action=edit";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = null;

            int connectionTries = 1;
            int totalTries = 0;
            for(int i = 0; i < connectionTries; i++)
            {
                try
                {
                    htmlDoc = web.Load(html);
                    i = connectionTries;
                }
                catch (System.Net.WebException)
                {
                    //return null;
                }
                totalTries++;
                if (i == connectionTries - 1 && htmlDoc == null)
                {
                    DialogResult res = MessageBox.Show("There was an issue connecting to " + name.SpeciesName + "'s Bulbapedia Page. Retry? (Total tries: " + totalTries + ")", "Missing Data", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                        connectionTries = -1;
                }
            }
            
            hap.HtmlNodeCollection columns;

            columns = htmlDoc.DocumentNode.SelectNodes("//textarea");

            int column = 0;
            int gamecolumnamount = 1;
            int movetutorcolumn = gen.moveTutorColumn;
            string gameAbv = gen.lvlUpColumn;
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
                string gameText = null, formText = null;
                bool isSelectedGame = true;
                bool isSelectedForm = true;

                readingMode modeText = readingMode.NONE;

                int rownum = 0;
                List<Move> evoMovesId = new List<Move>();

                foreach (string textRow in pagetext.Split('\n'))
                {
                    bool gameTextException = false;
                    string textRowLower = textRow.ToLower();

                    switch (gen.genNumber)
                    {
                        case 1:
                            if (name.NatDexNum == 134) // Vaporeon
                                gameTextException = true;
                            break;
                        case 2:
                            if (name.NatDexNum == 89) // Muk
                                gameTextException = true;
                            break;
                        case 4:
                            if (textRow.Contains("{{tt|60|70 in Pokémon Diamond and Pearl and Pokémon Battle Revolution}}"))
                                gameTextException = true;
                            break;
                    }

                    if (name.NatDexNum == 438) // Bonsly
                        gameTextException = true;

                    if (textRow.Contains("=Learnset="))
                        readingLearnsets = true;
                    else if (textRow.Contains("=Side game data="))
                        readingLearnsets = false;

                    if (readingLearnsets && !textRow.Trim().Equals(""))
                    {
                        if (gen.genNumber > 7)
                        {
                            if (textRow.Contains("is available"))
                                if (!textRow.Contains(gen.gameAvailableName))
                                    return null;
                        }
                        else if (textRow.Contains("Pokémon") && !gameTextException)
                        {
                            gameText = textRow;
                        }


                        rownum++;
                        if (textRowLower.Contains("{{learnlist/movena|"))
                            return null;
                        else if (textRowLower.Contains("by [[level|leveling"))
                        {
                            modeText = readingMode.LEVEL;
                            isSelectedGame = true;
                            isSelectedForm = matchForm(formText, name.FormName);
                        }
                        else if (textRow.Contains("By [[TM]]"))
                        {
                            modeText = readingMode.TM;
                            isSelectedGame = true;
                            isSelectedForm = matchForm(formText, name.FormName);
                        }
                        else if (textRow.Contains("By {{pkmn|breeding}}"))
                        {
                            modeText = readingMode.EGG;
                            isSelectedGame = true;
                            isSelectedForm = matchForm(formText, name.FormName);
                        }
                        else if (textRow.Contains("By [[transfer]] from"))
                        {
                            modeText = readingMode.TRANSFER;
                            isSelectedGame = true;
                            isSelectedForm = matchForm(formText, name.FormName);
                        }
                        else if (gen.moveTutorColumn != 0 && textRowLower.Contains("by [[move tutor|tutoring]]"))
                        {
                            modeText = readingMode.TUTOR;
                            isSelectedGame = true;
                            isSelectedForm = matchForm(formText, name.FormName);
                        }
                        else if (textRow.Contains("gameabbrev"))
                        {
                            if (textRow.Contains(gen.lvlUpColumn))
                                isSelectedGame = true;
                            else
                                isSelectedGame = false;
                        }
                        else if (textRow.Contains("====") && !readingLevelUp && !textRow.Contains("Pokémon")
                            && !textRow.Contains("By a prior [[evolution]]") && !textRow.Contains("Special moves")
                            && !textRow.Contains("By {{pkmn2|event}}s"))
                        {
                            formText = Regex.Replace(textRow.Replace("=", ""), "{{sup([^{]*)([A-Z][a-z]*)}}", "");
                            isSelectedGame = true;
                        }

                        else if ((textRowLower.Contains("{{learnlist/levelh")
                                                    || textRowLower.Contains("{{learnlist/tmh")
                                                    || textRowLower.Contains("{{learnlist/breedh")
                                                    || textRowLower.Contains("{{learnlist/tutorh"))
                        )
                        {
                            string aa = name.SpeciesName;
                            if (modeText == readingMode.NONE)
                                continue;
                            if (isSelectedForm && isSelectedGame)
                            {
                                if (modeText == readingMode.LEVEL && !LevelUpListRead)
                                {
                                    inList = true;
                                    readingLevelUp = true;
                                    string[] rowdata = textRow.Split('|');

                                    int toMinus = 0;

                                    for (int i = 0; i < rowdata.Length; i++)
                                    {
                                        string header = rowdata[i].Replace("}", "");
                                        int a;

                                        if (header.Contains("xy=") || header.Equals("") || int.TryParse(header, out a))
                                            toMinus++;
                                        if (header.Equals("V"))
                                        {
                                            toMinus--;
                                            if (gen.lvlUpColumn.Equals("BW"))
                                                column = 1;
                                            else
                                                column = 2;
                                        }

                                        if (header.Equals(gameAbv))
                                        {
                                            column = i - 3 - toMinus;
                                        }
                                    }

                                    gamecolumnamount = rowdata.Length - 4 - toMinus;

                                    if (gamecolumnamount <= 0)
                                        gamecolumnamount = 1;

                                    if (column == 0)
                                        column = 1;
                                }
                                else if ((modeText == readingMode.TM && !TMListRead) || (modeText == readingMode.EGG && !EggListRead) || (modeText == readingMode.TUTOR && !TutorListRead))
                                {
                                    inList = true;
                                }
                            }

                        }
                        else if (textRowLower.Contains("{{learnlist/levelf") && isSelectedGame && isSelectedForm)
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                LevelUpListRead = true;
                            formText = null;
                            readingLevelUp = false;
                        }
                        else if (textRowLower.Contains("{{learnlist/tmf") && isSelectedGame && isSelectedForm)
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                TMListRead = true;
                            formText = null;
                        }
                        else if (textRowLower.Contains("{{learnlist/breedf") && isSelectedGame && isSelectedForm)
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                EggListRead = true;
                            formText = null;
                        }
                        else if (textRowLower.Contains("{{learnlist/tutorf") && isSelectedGame && isSelectedForm)
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                TutorListRead = true;
                            formText = null;
                        }
                        else if (inList && isSelectedGame)
                        {
                            if (modeText == readingMode.LEVEL && !LevelUpListRead && (formText == null || formText.Equals(name.FormName)))
                            {
                                bool skipMove = false;
                                if (textRow.Contains("{{sup/" + gen.genNumber))
                                    if (!textRow.Contains(gen.lvlUpColumn))
                                        skipMove = true;
                                if (!skipMove)
                                {
                                    string lvltext = textRow.Replace("{{tt|Evo.|Learned upon evolving}}", "0");
                                    lvltext = lvltext.Replace("{{tt|60|70 in Pokémon Diamond and Pearl and Pokémon Battle Revolution}}", "60");
                                    string[] rowdata = System.Text.RegularExpressions.Regex.Replace(lvltext, "{{tt([^}]+)}}", "").Split('|');
                                    string lvl = rowdata[column].Replace("*", "");
                                    string movename = rowdata[gamecolumnamount + 1];

                                    if (!lvl.Equals("N/A"))
                                    {
                                        Move mo = MoveData[movename];

                                        if (mo.moveId == 617)
                                        {
                                            if (name.SpeciesName.Equals("FLOETTE_ETERNAL_FLOWER"))
                                                lvlMoves.Add(new LevelUpMove(int.Parse(lvl), "MOVE_" + mo.defineName));
                                        }
                                        else
                                        {
                                            if (lvl.Equals("0"))
                                                evoMovesId.Add(mo);
                                            else
                                                lvlMoves.Add(new LevelUpMove(int.Parse(lvl), "MOVE_" + mo.defineName));
                                        }
                                    }
                                }

                                
                            }
                            else if (modeText == readingMode.TM && !TMListRead && (formText == null || formText.Equals(name.FormName)) && !Regex.IsMatch(textRowLower, "{{learnlist/t[mr].+null}}"))
                            {
                                string[] rowdata = textRow.Split('|');
                                string movename = rowdata[2];

                                //TMMovesIds.Add(SerebiiNameToID[movename]);
                                try
                                {
                                    TMMoves.Add(MoveData[movename]);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("There's no move data in db/moveNames.json for " + movename + ". Skipping move.", "Missing Data", MessageBoxButtons.OK);
                                }
                            }
                            else if (modeText == readingMode.EGG && !EggListRead && (formText == null || formText.Equals(name.FormName)) && !Regex.IsMatch(textRowLower, "{{learnlist/breed.+null"))
                            {
                                string breedtext = textRow.Replace("{{tt|*|No legitimate means to pass down move}}", "");
                                breedtext = breedtext.Replace("{{tt|*|Male-only, and none of the evolutions can learn this move legitimately}}", "");
                                breedtext = breedtext.Replace("{{tt|*|No legitimate father to pass down move}}", "");
                                breedtext = breedtext.Replace("{{tt|*|No legitimate means to pass down the move}}", "");
                                breedtext = breedtext.Replace("{{tt|*|Paras learns Sweet Scent as an Egg move in Gold and Silver; in Crystal, the only fathers that can be learn the move learn it via TM}}", "");
                                string[] rowdata = System.Text.RegularExpressions.Regex.Replace(breedtext, "{{sup(.*)\v([A-Z]*)}}|{{MS([^}]+)}}", "MON").Split('|');
                                string movename = rowdata[2];

                                if (gen.genNumber == 4 && rowdata.Length >= 13 && !rowdata[12].Trim().Equals("") && !breedtext.Contains(gen.lvlUpColumn))
                                    continue;
                                else if (Regex.IsMatch(breedtext, "{{sup(.*)\\\u007C([A-Z]*)}}") && !breedtext.Contains(gen.dbFilename.ToUpper()))
                                    continue;
                                else if (!movename.Equals("Light Ball}}{{tt") && !(textRow.Contains("†") && !isIncenseBaby(name.SpeciesName)))
                                    //EggMovesIds.Add(SerebiiNameToID[movename]);
                                    EggMoves.Add(MoveData[movename]);
                            }
                            else if (modeText == readingMode.TUTOR && !TutorListRead && !Regex.IsMatch(textRowLower, "{{learnlist/tutor.+null}}")
                                && isSelectedForm)
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

                                    if (gen.genNumber == 1 || gen.genNumber == 2)
                                    {
                                        //int modeid = SerebiiNameToID[movename];
                                        //TutorMovesIds.Add(modeid);
                                        TutorMoves.Add(MoveData[movename]);
                                    }
                                    else if (rowdata[tutorpad + movetutorcolumn].Equals("yes"))
                                    {
                                        Move mov = MoveData[movename];
                                        if (mov.moveId == 520 && name.SpeciesName.Equals("Silvally"))
                                        {
                                            //TutorMovesIds.Add(518);
                                            //TutorMovesIds.Add(519);
                                            TutorMoves.Add(MoveData["Water Pledge"]);
                                            TutorMoves.Add(MoveData["Fire Pledge"]);
                                        }
                                        TutorMoves.Add(mov);
                                    }
                                }
                                catch (IndexOutOfRangeException) { }
                            }

                            //for (int i = 0; )
                        }
                    }

                }
                foreach (Move moe in evoMovesId)
                    lvlMoves.Insert(0,new LevelUpMove(0, "MOVE_" + moe.defineName));

                TMMoves = TMMoves.Distinct().ToList();
                EggMoves = EggMoves.Distinct().ToList();
                TutorMoves = TutorMoves.Distinct().ToList();
            }
            else
            {
                return null;
            }
            mon.LevelMoves = lvlMoves;
            foreach (Move m in TMMoves)
                mon.TMMoves.Add("MOVE_" + m.defineName);
            foreach (Move m in EggMoves)
                mon.EggMoves.Add("MOVE_" + m.defineName);
            foreach (Move m in TutorMoves)
                mon.TutorMoves.Add("MOVE_" + m.defineName);

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
