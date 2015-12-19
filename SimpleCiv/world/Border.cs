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
        //SetTriangle(0, v1, v2, v0);  UpRight
        //SetTriangle(1, v2, v3, v0);  UpLeft
        //SetTriangle(2, v3, v4, v0); Left
        //SetTriangle(3, v4, v5, v0);  DownLeft
        //SetTriangle(4, v5, v6, v0);  DownRight
        //SetTriangle(5, v6, v1, v0);  Right
        DownRight = 0,
        DownLeft = 1,
        Left = 2,
        UpLeft = 3,
        UpRight = 4,
        Right = 5,
        None = 6
    };

    public class Border
    {
        public BorderType type;
        public Tile tile;
        public Player owner;
    }
}
