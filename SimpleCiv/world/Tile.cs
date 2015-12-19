using GlmNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{

    public enum TileType
    {
        None,
        Grass,
        Water,
        Lava,
        DeepWater,
        Mountain,
        Desert
    };

    public class Tile
    {
        static float s = 1.0f;
        static float r = (float)Math.Cos((Math.PI / 180.0)*30)*s;
        static float h = (float)Math.Sin((Math.PI / 180.0)*30)*s;
        static float m = h / r;
        static float gridWidth = 2 * r;
        static float halfWidth = r;
        static float gridHeight = h + s;

        static float tileWidth = 2*r;
        static float tileHeight = 1.5f;
        static float offsetRow = 0.8660f;
        public static vec3 GetTileCoord(int x, int z)
        {
            var xOffset = 0.0f;
            if (z % 2 == 1)
            {
                xOffset = offsetRow;
            }
            return new vec3(x * tileWidth + xOffset, 0, tileHeight * z);
        }
        public static void GetTileIndex(float rawX, float rawZ, out int x, out int z)
        {
            var xPixel = rawX + r;
            var zPixel = rawZ + (s / 2.0f + h);

            int row = (int)(zPixel / gridHeight);
            int column;
            bool rowIsOdd = row % 2 == 1;

            if (rowIsOdd)
            {
                column = (int)((xPixel - halfWidth) / gridWidth);
            } else
            {
                column = (int)(xPixel / gridWidth);
            }

            double relY = zPixel - (row * gridHeight);
            double relX;

            if (rowIsOdd)
                relX = (xPixel - (column * gridWidth)) - halfWidth;
            else
                relX = xPixel - (column * gridWidth);

            if (relY < (-m * relX) + h) // LEFT edge
            {
                row--;
                if (!rowIsOdd)
                    column--;
            }
            else if (relY < (m * relX) - h) // RIGHT edge
            {
                row--;
                if (rowIsOdd)
                    column++;
            }

            z = row;
            x = column;
            return;
        }

        public Tile parent; // Used only for pathing search
        public float distance; //Used only for pathing search

        public vec3 centerPos;
        public int xPos;
        public int zPos;
        public TileType currentType;
        public Unit currentUnit;
        public Player owner;

        //public List<Tile> neighbors;
        //public Dictionary<Tile, int> neighbors;  // int is the cost of movement to this neighbor.

        public List<Neighbor> neighbors;
        public Dictionary<BorderType,Neighbor> neighborsDict;

    }
}
