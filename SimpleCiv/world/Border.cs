using SimpleCiv.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    public enum BorderType
    {
       None,
       Left,
       Right,
       UpLeft,
       UpRight,
       DownLeft,
       DownRight
    };

    public class Border
    {
        public BorderType type;
        public Tile tile;
        public Player owner;
    }
}
