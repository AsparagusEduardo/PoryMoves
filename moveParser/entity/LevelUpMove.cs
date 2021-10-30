using moveParser.data;

namespace PoryMoves.entity
{
    public class LevelUpMove
    {
        public int Level;
        public Move Move;
        public LevelUpMove()
        {

        }
        public LevelUpMove(int lvl, Move mv)
        {
            Level = lvl;
            Move = mv;
        }
    }
}
