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
        public string dexPage;
        public string indexFormat;
        public string tableNodes;
        public string lvlUpTitle_Generation;
        public string lvlUpTitle_Game;
        public string lvlUpTitle_Forms;
        public string lvlUpTitle_RegionalForms;
        public string tmHmTrTitle;
        public string tmHmTrTitle2;
        public string moveTutorTitle1;
        public string moveTutorTitle2;
        public string eggMoveTitle;

        public string dbFilename;
        public GenerationData(int num, string dbfile, string dxpage, string idxformat, string tabnode,
                                string lvlgen, string lvlgame, string lvlforms, string lvlregforms,
                                string tmtitle, string tmtitle2, string tutortitle1, string tutortitle2,
                                string eggtitle)
        {
            genNumber = num;
            dbFilename = dbfile;
            dexPage = dxpage;
            indexFormat = idxformat;
            tableNodes = tabnode;
            lvlUpTitle_Generation = lvlgen;
            lvlUpTitle_Game = lvlgame;
            lvlUpTitle_Forms = lvlforms;
            lvlUpTitle_RegionalForms = lvlregforms;
            tmHmTrTitle = tmtitle;
            tmHmTrTitle2 = tmtitle2;
            moveTutorTitle1 = tutortitle1;
            moveTutorTitle2 = tutortitle2;
            eggMoveTitle = eggtitle;
        }
    }
}
