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
        public bool IsBaseForm;
        public string FormName_TMs;
        public string FormName_EggTutor;
        public string VarName;
        public string DefName;
        public MonName(string nat, string og, bool isfrm, string formtm, string formegg, string var, string def)
        {
            NatDexNum = nat;
            OriginalName = og;
            IsBaseForm = isfrm;
            FormName_TMs = formtm;
            FormName_EggTutor = formegg;
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
