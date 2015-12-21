using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCiv.world
{
    public enum UnitRole
    {
        LandRanged,
        NavalRanged,
        Air,
        Siege,
        Land,
        Naval,
    }

    public class CombatStrength
    {
        public int offensive { get; set; }
        public int defensive { get; set; }
    }

    public class UnitDetail
    {
        public string attackSound;
        public string moveSound;

        [JsonConverter(typeof(StringEnumConverter)), JsonRequired]
        public UnitRole role {
            get; set;
        }
        [JsonRequired]
        public string objectFile
        {
            get; set;
        }
        [JsonRequired]
        public float scaleSize
        {
            get; set;
        }
        [JsonRequired]
        public bool singleUnit
        {
            get; set;
        }

        [JsonRequired]
        public CombatStrength combatStrength
        {
            get; set;
        }

        [JsonConverter(typeof(StringEnumConverter)), JsonRequired]
        public UnitType name {
            get; set;
        }
    }
}
