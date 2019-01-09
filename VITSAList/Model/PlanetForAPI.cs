using System;
using System.Collections.Generic;

namespace VITSAList
{
    /// <summary>
    /// API model for planet
    /// </summary>
    public class PlanetForAPI
    {
        public int count;

        public string next;

        public string previous;

        public List<VITSAList.PlanetDetail> results;
    }
}
