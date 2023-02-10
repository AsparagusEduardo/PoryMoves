using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moveParser.data
{
    public class GenerationData
    {
        public int? gameId;
        public int genNumber;
        public string genNumberRoman;
        public bool isLatestGen;
        public string dbFilename;
        public string gameFullName;
        public string gameAvailableName;
        public string gameNameAlt1;
        public string lvlUpColumn;
        public int moveTutorColumn;
        public int maxDexNum;

        public string serebiiDexURL;
        public string serebiiLevelUpTitle;
        public string serebiiMoveTutorTitle;
        public string serebiiMoveTutorTitleAlt;
        public string serebiiLocationsName;
    }

    public class GenerationsData
    {
        public static Dictionary<string, GenerationData> GetGenDataFromFile(string filedir)
        {
            Dictionary<string, GenerationData> dict;
            string text = File.ReadAllText(filedir);

            dict = JsonConvert.DeserializeObject<Dictionary<string, GenerationData>>(text);
            return dict;
        }
    }

}
