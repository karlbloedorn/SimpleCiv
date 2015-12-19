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

    public class UnitDetail
    {
        public string objectFile;
        public float scaleSize;
        public bool singleUnit;
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

        [JsonConverter(typeof(StringEnumConverter)), JsonRequired]
        public UnitType name {
            get; set;
        }
    }
}
