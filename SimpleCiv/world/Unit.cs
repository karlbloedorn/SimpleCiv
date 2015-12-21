using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{ 

    public enum UnitType
    {
        None,
        Test,
        Archer,
        Artillery,
        Battleship,
        Bomber,
        Cavalry,
        Cannon,
        Caravan,
        Carrier,
        Catapult,
        Chariot,
        Cruiser,
        Destroyer,
        Engineer,
        Explorer,
        Fighter,
        Freight,
        Frigate,
        Galleon,
        Rebel,
        Helicopter,
        Horseman,
        Howitzer,
        Ironclad,
        Knight,
        Legion,
        Marines,
        MechanizedInfantry,
        Musketeer,
        Paratrooper,
        Phalanx,
        Pikeman,
        Riflemen,
        Settler,
        StealthBomber,
        StealthFighter,
        Submarine,
        Tank,
        Transport,
        Trimere,
        Warrior,
        Worker  
    };

    public class Unit
    {
        public Tile currentTile;
        public UnitType currentType;
        public float health = 1;
        public Player player;
       
        public static void EvaluateCombat(Unit attacker, Unit defender)
        {
            attacker.health -= 0.17f;
            defender.health -= 0.25f;

            Debug.WriteLine("attacker ("+ attacker.currentType.ToString() + ") : " + attacker.health);
            Debug.WriteLine("defender ("+ defender.currentType.ToString() + "): " + defender.health);
        }
    }
}
