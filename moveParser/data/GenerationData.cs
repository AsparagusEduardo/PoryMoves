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
        public int genNumber;
        public string genNumberRoman;
        public bool isLatestGen;
        public string dbFilename;
        public string gameFullName;
        public int moveTutorColumn;

        public GenerationData(int num, string roman, bool lastgen, string dbfile, string fullname, int tutorcol)
        {
            genNumber = num;
            genNumberRoman = roman;
            isLatestGen = lastgen;
            dbFilename = dbfile;
            gameFullName = fullname;
            moveTutorColumn = tutorcol;
        }
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
