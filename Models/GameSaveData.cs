using System;
using System.Collections.Generic;

namespace TopDownGame.Models
{
    [Serializable]
    public class GameSaveData
    {
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public int Gold { get; set; }
        public Dictionary<string, int> InventoryItems { get; set; }
        public List<string> CompletedQuests { get; set; }
        public List<string> ActiveQuests { get; set; }
        public ZoneType CurrentZone { get; set; }
        public Dictionary<string, bool> NpcStates { get; set; }
    }
}