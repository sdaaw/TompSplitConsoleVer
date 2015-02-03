using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace splittest2
{
    class Game
    {
        public static List<string> possibleGames = new List<string>() { "tomb2", "tomb3", "tr3gold" };

        #region Level Names Dictionary
        private static Dictionary<int, string[]> levelNames = new Dictionary<int, string[]>()
        {
            {0, 
            new string[] {"Title",                                                          
            "The Great Wall",
            "Venice",                                 
            "Bartoli's Hideout",
            "Opera House",
            "Offshore Rig",
            "Diving Area",
            "40 Fathoms",
            "Wreck of the Maria Doria",
            "Living Quarters",
            "The Deck",
            "Tibetan Foothills",
            "Barkhang Monastery",
            "Catacombs of the Talion", 
            "Ice Palace",
            "Temple of Xian",
            "Floating Islands",
            "Dragon's Lair",
            "Home Sweet Home",
            "DEMO 1",
            "DEMO 2",
            "DEMO 3",
            "DEMO 4"}},

            {1, 
            new string[] {"Title",                                                          
            "Jungle",
            "Temple Ruins",                                 
            "The River Ganges",
            "Caves of Kaliya",
            "Coastal Village",
            "Crash Site",
            "Madubu Gorge",
            "Temple of Puna",
            "Thames Wharf",
            "Aldwych",
            "Lud's Gate",
            "City",
            "Nevada Desert",
            "High Security Compound",
            "Area 51",
            "Antarctica",
            "RX-Tech Mines",
            "Lost City of Tinnos",
            "Meteorite Cavern",
            "All Hallows"
            }},

            {2, 
            new string[] {"Title",                                                          
            "Highland Fling",
            "Willard's Lair",                                 
            "Shakespeare Cliff",
            "Sleeping with the Fishes",
            "It's a Madhouse!",
            "Reunion"}}
        };
        #endregion

        private static int[] lastLevelTime = { 0x521724, 0x6D7220, 0x6CFE40 };
        private static int[] currentLevel = { 0x4D9EB0, 0x4C561C, 0x4C05FE };
        private static int[] titleFlag = { 0x51BD90, 0x6A1B78, 0x69AA04 };

        public string ProcessName;
        private int Id;
        public string[] Levels;
        public Process ActualProcess;
        public int CurrentLevel;
        public int TitleFlag;
        public int LastLevelTime;

        public Game(int id)
        {
            Id = id;
            ProcessName = possibleGames[id];
            Levels = levelNames[id];
            CurrentLevel = currentLevel[id];
            TitleFlag = titleFlag[id];
            LastLevelTime = lastLevelTime[id];
            ActualProcess = ProcessHandler.getTombProcess(ProcessName);
        }
    }
}
