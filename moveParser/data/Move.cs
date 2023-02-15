using Newtonsoft.Json;
using PoryMoves.entity;
using System.Collections.Generic;
using System.IO;

namespace moveParser.data
{
    public class Move
    {
        public int moveId;
        public string defineName;

        public Move(int id, string define)
        {
            moveId = id;
            defineName = define;
        }

        public override int GetHashCode()
        {
            return moveId;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Move);
        }
        public bool Equals(Move obj)
        {
            return obj != null && obj.moveId == this.moveId;
        }
    }

    public class MovesData
    {
        public static Dictionary<string, Move> GetMoveDataFromFile(string filedir)
        {
            Dictionary<string, Move> dict = new Dictionary<string, Move>();
            if (!File.Exists(filedir))
                return dict;
            string text = File.ReadAllText(filedir);

            dict = JsonConvert.DeserializeObject<Dictionary<string, Move>>(text);
            return dict;
        }
        public static int CompareLvlUpMoveIds(LevelUpMoveId a, LevelUpMoveId b)
        {
            int result = a.Level.CompareTo(b.Level);
            if (result == 0)
                return a.MoveId.CompareTo(b.MoveId);
            return result;
        }
    }

}
