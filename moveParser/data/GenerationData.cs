using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moveParser.data
{
    public class GenerationData
    {
        public int genNumber;
        public string dbFilename;

        public GenerationData(int num, string dbfile)
        {
            genNumber = num;
            dbFilename = dbfile;
        }
    }
}
