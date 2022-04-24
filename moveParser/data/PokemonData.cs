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

        enum tableReadMode{
            NONE,
            LEVEL,
            EVO,
            TM,
            HM,
            TR,
            EGG,
            TUTOR

        }

        public static MonData LoadMonData(MonName name, GenerationData gen, Dictionary<string, Move> MoveData)
        {
            if (gen.dbFilename.Equals("lgpe") && (name.NatDexNum > 151 && (name.NatDexNum != 808 && name.NatDexNum != 809)))
                return null;

            MonData mon = new MonData();

            List<LevelUpMove> lvlMoves = new List<LevelUpMove>();
            //List<LevelUpMoveId> lvlMovesId = new List<LevelUpMoveId>();


            List<Move> TMMovesNew = new List<Move>();
            List<Move> HMMovesNew = new List<Move>();
            List<Move> EggMovesNew = new List<Move>();
            List<Move> TutorMovesNew = new List<Move>();

            if (gen.genNumber < 7 && name.FormName.Contains("Alola"))
                return null;

            if (gen.maxDexNum < name.NatDexNum)
                return null;

            string html = "https://pokemondb.net/pokedex/" + name.SpeciesName + "/moves/" + gen.genNumber;

            hap.HtmlWeb web = new hap.HtmlWeb();
            hap.HtmlDocument htmlDoc;
            try
            {
                htmlDoc = web.Load(html);
                htmlDoc.DocumentNode.InnerHtml = htmlDoc.DocumentNode.InnerHtml.Replace("\n", "").Replace("> <", "><");
            }
            catch (System.Net.WebException)
            {
                return null;
            }
            
            hap.HtmlNodeCollection columns;

            columns = htmlDoc.DocumentNode.SelectNodes("//div[@class='tabset-moves-game sv-tabs-wrapper']");

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
                string pagetext = columns[0].InnerHtml.Replace("&lt;br>\n", "&lt;br>");
                string gameText = null, modeText = null, formText = null;

                int tabNum = 0;

                tableReadMode readMode = tableReadMode.NONE;
                foreach (hap.HtmlNode nodo1 in columns[0].ChildNodes)
                {
                    if (nodo1.Attributes["class"].Value.Equals("sv-tabs-tab-list"))
                    {
                        foreach (hap.HtmlNode nodo2 in nodo1.ChildNodes)
                        {
                            tabNum++;
                            if (nodo2.InnerText.Equals(gen.gameAvailableName))
                                break;
                        }
                    }
                    else if (nodo1.Attributes["class"].Value.Equals("sv-tabs-panel-list"))
                    {
                        hap.HtmlNode nodo2 = nodo1.ChildNodes[tabNum - 1];
                        foreach(hap.HtmlNode nodo3 in nodo2.ChildNodes)
                        {
                            if (nodo3.Attributes["class"].Value.Equals("grid-row"))
                            {
                                foreach(hap.HtmlNode nodo4 in nodo3.ChildNodes)
                                {
                                    foreach (hap.HtmlNode nodo5 in nodo4.ChildNodes)
                                    {
                                        if (nodo5.InnerText.Equals("Moves learnt by level up"))
                                        {
                                            readMode = tableReadMode.LEVEL;
                                        }
                                        else if (nodo5.InnerText.Equals("Moves learnt by TM"))
                                        {
                                            readMode = tableReadMode.TM;
                                        }
                                        else if (nodo5.InnerText.Equals("Moves learnt by HM"))
                                        {
                                            readMode = tableReadMode.HM;
                                        }
                                        else if (nodo5.Attributes["class"] != null && nodo5.Attributes["class"].Value.Equals("text-small"))
                                        {
                                            if (nodo5.InnerText.Contains("does not") || nodo5.InnerText.Contains("cannot"))
                                            {
                                                readMode = tableReadMode.NONE;
                                            }
                                        }
                                        else if (nodo5.Attributes["class"] != null && nodo5.Attributes["class"].Value.Equals("resp-scroll"))
                                        {
                                            foreach (hap.HtmlNode tableRow in nodo5.ChildNodes[0].ChildNodes[1].ChildNodes)
                                            {
                                                string movename;
                                                switch (readMode)
                                                {
                                                    case tableReadMode.LEVEL:
                                                        int lvl = int.Parse(tableRow.ChildNodes[0].InnerText);
                                                        movename = tableRow.ChildNodes[1].InnerText;
                                                        Move mo = MoveData[movename];
                                                        lvlMoves.Add(new LevelUpMove(lvl, "MOVE_" + mo.defineName));
                                                        break;
                                                    case tableReadMode.TM:
                                                        movename = tableRow.ChildNodes[1].InnerText;
                                                        TMMovesNew.Add(MoveData[movename]);
                                                        break;
                                                    case tableReadMode.HM:
                                                        movename = tableRow.ChildNodes[1].InnerText;
                                                        HMMovesNew.Add(MoveData[movename]);
                                                        break;
                                                }
                                            }
                                            readMode = tableReadMode.NONE;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                int rownum = 0;
                List<Move> evoMovesId = new List<Move>();
                foreach (Move moe in evoMovesId)
                    lvlMoves.Insert(0,new LevelUpMove(0, "MOVE_" + moe.defineName));

                TMMovesNew = TMMovesNew.Distinct().ToList();
                EggMovesNew = EggMovesNew.Distinct().ToList();
                TutorMovesNew = TutorMovesNew.Distinct().ToList();
            }
            else
            {
                return null;
            }
            mon.LevelMoves = lvlMoves;
            foreach (Move m in TMMovesNew)
                mon.TMMoves.Add("MOVE_" + m.defineName);
            foreach (Move m in HMMovesNew)
                mon.TMMoves.Add("MOVE_" + m.defineName);
            foreach (Move m in EggMovesNew)
                mon.EggMoves.Add("MOVE_" + m.defineName);
            foreach (Move m in TutorMovesNew)
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
