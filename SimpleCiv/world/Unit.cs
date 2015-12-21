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
        static public Dictionary<UnitType, UnitDetail> unitTypes = new Dictionary<UnitType, UnitDetail>();
        public Tile currentTile;
        public UnitType currentType;
        public float health = 100;

        private UnitDetail detail
        {
            get
            {
                return unitTypes[currentType];
            }
        }

        public static void EvaluateCombat(Unit attacker, Unit defender)
        {
            var attackerDetails = attacker.detail;
            var defenderDetails = defender.detail;
            var difference = Math.Abs(attacker.detail.combatStrength.offensive - defender.detail.combatStrength.defensive);
            var attackerPercentOfStrength = (difference / (attacker.detail.combatStrength.offensive * 1.0f));
            var defenderPercentOfStrength = (difference / (defender.detail.combatStrength.defensive * 1.0f));

            attacker.health -= attackerPercentOfStrength;
            defender.health -= defenderPercentOfStrength;

            Debug.WriteLine("attacker ("+ attacker.currentType.ToString() + ") : " + attacker.health);
            Debug.WriteLine("defender ("+ defender.currentType.ToString() + "): " + defender.health);
        }
    }
}
