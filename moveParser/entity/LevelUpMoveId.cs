using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoryMoves.entity
{
    public class LevelUpMoveId
    {
        public int Level;
        public int MoveId;
        public LevelUpMoveId()
        {

        }
        public LevelUpMoveId(int lvl, int mv)
        {
            Level = lvl;
            MoveId = mv;
        }
    }
}
