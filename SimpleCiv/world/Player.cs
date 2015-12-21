using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    public class Player
    {
        public Color borderColor;
        public string name;

        public List<City> cities;

        public Player() { 
            cities = new List<City>();
        }
    }
}
