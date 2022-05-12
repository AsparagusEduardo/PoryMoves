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
        public string SerebiiFormName;
        public string SerebiiFormNameAlt;
        public string SerebiiURL;
        public string SerebiiLevelUpTableName;
        public string SerebiiTMTableName;
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
        public static MonData LoadMonDataSerebii(MonName name, GenerationData gen, Dictionary<string, Move> MoveData)
        {
            MonData mon = new MonData();

            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();
            List<Move> evoMoves = new List<Move>();
            List<Move> TMMoves = new List<Move>();
            List<Move> EggMoves = new List<Move>();
            List<Move> TutorMoves = new List<Move>();

            if (gen.maxDexNum < name.NatDexNum)
                return null;

            /*
            if (name.NatDexNum != 876)
                return null;
            //*/

            string html;
            if (name.SerebiiURL != null)
                html = "https://serebii.net/pokedex" + gen.serebiiDexURL + "/" + name.SerebiiURL + "/";
            else
                html = "https://serebii.net/pokedex" + gen.serebiiDexURL + "/" + name.SpeciesName.ToLower() + "/";

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc = null;

            int connectionTries = 1;
            int totalTries = 0;
            for (int i = 0; i < connectionTries; i++)
            {
                try
                {
                    htmlDoc = web.Load(html);
                    htmlDoc.DocumentNode.InnerHtml = htmlDoc.DocumentNode.InnerHtml.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("> <", "><");
                    i = connectionTries;
                }
                catch (System.Net.WebException)
                {
                    //return null;
                }
                totalTries++;
                if (i == connectionTries - 1 && htmlDoc == null)
                {
                    DialogResult res = MessageBox.Show("There was an issue connecting to " + name.SpeciesName + "'s Serebii Page. Retry? (Total tries: " + totalTries + ")", "Missing Data", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                        connectionTries = -1;
                }
            }

            hap.HtmlNodeCollection columns;
            columns = htmlDoc.DocumentNode.SelectNodes("//table[@class='dextable']");

            int idSeccionLevelUp = 0;
            if (columns != null)
            {

                int tabNum = 1;
                foreach (hap.HtmlNode nodo1 in columns)
                {
                    int nroSeccion = 0;
                    foreach (hap.HtmlNode nodo2 in nodo1.ChildNodes[0].ChildNodes)
                    {
                        nroSeccion++;
                        if (nodo2.InnerText.Contains("Level Up"))
                        {
                            if (//(gen.gameNameAlt1 != null && nodo2.InnerText.Contains(gen.gameNameAlt1))
                                //|| 
                                (nodo1.ParentNode.Id.Equals("legends") == gen.dbFilename.Equals("la")) &&
                                (
                                    nodo2.InnerText.Equals(name.SerebiiLevelUpTableName)
                                    || (name.IsBaseForm && nodo2.InnerText.Equals(gen.serebiiLevelUpTitle))
                                    || (name.IsBaseForm && nodo2.InnerText.Equals("Standard Level Up"))
                                )
                                )
                            {
                                
                                int tableRow = 2;
                                while (tableRow < nodo1.ChildNodes.Count)
                                {
                                    string lvlText = nodo1.ChildNodes[tableRow].ChildNodes[0].InnerText.Replace("&#8212;", "1");
                                    if (!lvlText.Contains("Evo"))
                                    {
                                        int lvl = int.Parse(lvlText);
                                        string movename = nodo1.ChildNodes[tableRow].ChildNodes[1].InnerText;
                                        Move mo = MoveData[movename];
                                        lvlMoves.Add(new LevelUpMove(lvl, "MOVE_" + mo.defineName));
                                    }
                                    else
                                    {
                                        string movename = nodo1.ChildNodes[tableRow].ChildNodes[1].InnerText;
                                        Move mo = MoveData[movename];
                                        evoMoves.Add(mo);
                                    }
                                    tableRow += 2;

                                }
                            }
                        }
                        else if (nodo2.InnerText.Equals(gen.gameNameAlt1 + " Technical Machine Attacks")
                                || nodo2.InnerText.Equals("Technical Record Attacks")
                                || nodo2.InnerText.Equals(name.SerebiiTMTableName)
                                || (name.IsBaseForm && nodo2.InnerText.Equals("Technical Machine Attacks"))
                                )
                        {
                            int tableRow = 2;
                            while (tableRow < nodo1.ChildNodes.Count)
                            {
                                if (nodo1.ChildNodes[tableRow].ChildNodes.Count <= 8 || name.SerebiiFormName == null
                                     || nodo1.ChildNodes[tableRow].ChildNodes[8].ChildNodes[0].ChildNodes[0].OuterHtml.Contains("alt=\"" + name.SerebiiFormName + "\"")
                                     || nodo1.ChildNodes[tableRow].ChildNodes[8].ChildNodes[0].ChildNodes[0].OuterHtml.Contains("alt=\"" + name.SerebiiFormNameAlt + "\"")
                                    )
                                {
                                    string movename = nodo1.ChildNodes[tableRow].ChildNodes[1].InnerText;
                                    Move mo = MoveData[movename];
                                    TMMoves.Add(mo);
                                }
                                tableRow += 2;
                            }
                        }
                        else if (nodo2.InnerText.Contains("Egg Moves"))
                        {
                            int tableRow = 2;
                            bool hasFormDifferences = false;

                            while (tableRow < nodo1.ChildNodes.Count && !hasFormDifferences)
                            {
                                if (nodo1.ChildNodes[tableRow].ChildNodes[7].OuterHtml.Contains("alt=\""))
                                    hasFormDifferences = true;
                                tableRow += 2;
                            }

                            tableRow = 2;
                            while (tableRow < nodo1.ChildNodes.Count)
                            {
                                if ((nodo1.ChildNodes[tableRow].ChildNodes[0].ChildNodes.Count < 2 ||
                                    nodo1.ChildNodes[tableRow].ChildNodes[0].InnerText.ToLower().Contains(gen.dbFilename + " only"))
                                        &&
                                    (name.SerebiiFormName == null
                                     || !hasFormDifferences
                                     || nodo1.ChildNodes[tableRow].ChildNodes[7].OuterHtml.Contains("alt=\"" + name.SerebiiFormName + "\"")
                                     || nodo1.ChildNodes[tableRow].ChildNodes[7].OuterHtml.Contains("alt=\"" + name.SerebiiFormNameAlt + "\""))
                                    )
                                {
                                    string movename = nodo1.ChildNodes[tableRow].ChildNodes[0].ChildNodes[0].InnerText;
                                    Move mo = MoveData[movename];

                                    if (!(mo.moveId == 344 && (name.NatDexNum == 25 || name.NatDexNum == 26 || name.NatDexNum == 172))) //Volt Tackle Pichu
                                        EggMoves.Add(mo);
                                }
                                tableRow += 2;
                            }
                        }
                        else if (nodo2.InnerText.Equals(gen.serebiiMoveTutorTitle))
                        {
                            int tableRow = 2;
                            while (tableRow < nodo1.ChildNodes[0].ChildNodes.Count)
                            {
                                if (nodo1.ChildNodes[0].ChildNodes[tableRow].ChildNodes.Count > 7)
                                    ;

                                if (nodo1.ChildNodes[0].ChildNodes[tableRow].ChildNodes.Count <= 7 || name.SerebiiFormName == null
                                     || nodo1.ChildNodes[0].ChildNodes[tableRow].ChildNodes[7].ChildNodes[0].ChildNodes[0].OuterHtml.Contains("alt=\"" + name.SerebiiFormName + "\"")
                                    )
                                {
                                    string movename = nodo1.ChildNodes[0].ChildNodes[tableRow].ChildNodes[0].ChildNodes[0].InnerText;
                                    Move mo = MoveData[movename];
                                    if (mo.moveId == 520 && name.SpeciesName.Equals("Silvally"))
                                    {
                                        TutorMoves.Add(MoveData["Water Pledge"]);
                                        TutorMoves.Add(MoveData["Fire Pledge"]);
                                    }
                                    TutorMoves.Add(mo);
                                }

                                tableRow += 2;
                            }
                        }
                        else if (nodo2.InnerText.Contains("Locations"))
                        {
                            bool isInGame = false;
                            int tableRow = 1;
                            while (tableRow < nodo1.ChildNodes.Count)
                            {
                                if (nodo1.ChildNodes[tableRow].ChildNodes[0].InnerText.Equals(gen.serebiiLocationsName))
                                    isInGame = true;
                                tableRow++;
                            }
                            if (!isInGame)
                                return null;

                        }
                    }
                }

                foreach (Move moe in evoMoves)
                    lvlMoves.Insert(0, new LevelUpMove(0, "MOVE_" + moe.defineName));

                TMMoves = TMMoves.Distinct().ToList();
                EggMoves = EggMoves.Distinct().OrderBy(x => x.defineName).ToList();
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

        public static MonData LoadMonDataBulbapedia(MonName name, GenerationData gen, Dictionary<string, Move> MoveData)
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

                    if (readingLearnsets && textRow.Contains("Pokémon") && !gameTextException)
                        gameText = textRow;
                    else if (textRow.Contains("=Learnset="))
                        readingLearnsets = true;
                    else if (textRow.Contains("=Side game data="))
                        readingLearnsets = false;

                    if (readingLearnsets && !textRow.Trim().Equals(""))
                    {
                        rownum++;
                        if (textRowLower.Contains("{{learnlist/movena|"))
                            return null;
                        else if (textRowLower.Contains("by [[level|leveling"))
                            modeText = readingMode.LEVEL;
                        else if (textRow.Contains("By [[TM]]"))
                            modeText = readingMode.TM;
                        else if (textRow.Contains("By {{pkmn|breeding}}"))
                            modeText = readingMode.EGG;
                        else if (textRow.Contains("By [[transfer]] from"))
                            modeText = readingMode.TRANSFER;
                        else if (gen.moveTutorColumn != 0 && textRowLower.Contains("by [[move tutor|tutoring]]"))
                            modeText = readingMode.TUTOR;
                        else if (textRow.Contains("====") && !readingLevelUp && !textRow.Contains("Pokémon")
                            && !textRow.Contains("By a prior [[evolution]]") && !textRow.Contains("Special moves") && !textRow.Contains("By {{pkmn2|event}}s"))
                            formText = Regex.Replace(textRow.Replace("=", ""), "{{sup([^{]*)([A-Z][a-z]*)}}", "");

                        else if (textRowLower.Contains("{{learnlist/levelh")
                            || textRowLower.Contains("{{learnlist/tmh")
                            || textRowLower.Contains("{{learnlist/breedh")
                            || textRowLower.Contains("{{learnlist/tutorh"))
                        {
                            if (modeText == readingMode.NONE)
                                continue;
                            if (matchForm(formText, name.FormName) && (gameText == null || gameText.Contains(gametosearch)))
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
                        else if (textRowLower.Contains("{{learnlist/levelf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                LevelUpListRead = true;
                            formText = null;
                            readingLevelUp = false;
                        }
                        else if (textRowLower.Contains("{{learnlist/tmf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                TMListRead = true;
                            formText = null;
                        }
                        else if (textRowLower.Contains("{{learnlist/breedf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                EggListRead = true;
                            formText = null;
                        }
                        else if (textRowLower.Contains("{{learnlist/tutorf") && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            inList = false;
                            if (formText == null || formText.Equals(name.FormName))
                                TutorListRead = true;
                            formText = null;
                        }
                        else if (inList && (gameText == null || gameText.Contains(gametosearch)))
                        {
                            if (modeText == readingMode.LEVEL && !LevelUpListRead && (formText == null || formText.Equals(name.FormName)))
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
