using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    public class City
    {
        public Player player;
        public Tile location;
        public string name;

        public void Expand()
        {
            foreach (var neighbor in location.neighbors)
            {
                if (neighbor.tile.owner == null)
                {
                    neighbor.tile.owner = player;
                    return;
                }
            }
        }
    }
}
