using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AIComponent
{
    public class SimulatedUnit
    {
        public UnitBehaviour UnitBehaviour;
        public HexBehaviour SimulatedHexBehaviour;
        public int SimulatedHealth;
        public bool currentlyPlaying;

        public SimulatedUnit(UnitBehaviour ub, HexBehaviour hb, int ch, bool cb)
        {
            UnitBehaviour = ub;
            SimulatedHealth = ch;
            SimulatedHexBehaviour = hb;
            currentlyPlaying = cb;
        }
    }
}
