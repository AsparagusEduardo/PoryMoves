using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static moveParser.data.MoveData;

namespace moveParser.data
{
    public class MonData
    {
        public string VarName;
        public string DefName;
        public List<LevelUpMove> LevelMoves;
        public List<string> ExtraMoves;
    }
    public class MonName
    {
        public string NatDexNum;
        public string OriginalName;
        public string VarName;
        public string DefName;
        public MonName(string nat, string og, string var, string def)
        {
            NatDexNum = nat;
            OriginalName = og;
            VarName = var;
            DefName = def;
        }
    }

    public class PokemonData
    {
        public static List<MonData> GetMonDataFromFile(string filedir)
        {
            List<MonData> list;
            string text = File.ReadAllText(filedir);

            list = JsonConvert.DeserializeObject<List<MonData>>(text);


            return list;
        }

        public static List<MonName> GetMonNamesFromFile(string filedir)
        {
            List<MonName> list;
            string text = File.ReadAllText(filedir);

            list = JsonConvert.DeserializeObject<List<MonName>>(text);


            return list;
        }
    }
}
