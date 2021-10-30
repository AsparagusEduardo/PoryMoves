using moveParser.data;

namespace PoryMoves.entity
{
    public class LevelUpMove
    {
        public int Level;
        public string Move;
        public LevelUpMove()
        {

        }
        public LevelUpMove(int lvl, string mv)
        {
            Level = lvl;
            Move = mv;
        }
    }
}
