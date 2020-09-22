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
        public List<string> TMMoves;
        public List<string> EggMoves;
        public List<string> TutorMoves;
    }
    public class MonName
    {
        public string NatDexNum;
        public string SpeciesName;
        public bool IsBaseForm;             //Alolan Forms were previously called "Alola Form", and some stuff on Serebii still have the previous name.
        public string FormName_TMs;         //- Used for "Alola Form".
        public string FormName_EggTutor;    //- Used for all other forms.
        public string VarName;
        public string DefName;
        public MonName(string nat, string og, bool isfrm, string formtm, string formegg, string var, string def)
        {
            NatDexNum = nat;
            SpeciesName = og;
            IsBaseForm = isfrm;
            FormName_TMs = formtm;
            FormName_EggTutor = formegg;
            VarName = var;
            DefName = def;
        }
    }

    public class PokemonData
    {
        public static Dictionary<string, MonData> GetMonDataFromFile(string filedir)
        {
            Dictionary<string, MonData> dict;
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
    }
}
