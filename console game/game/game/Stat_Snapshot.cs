using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public struct Stat_Snapshot
    {
        public int strength;
        public int dexterity;
        public int inteligence;
        public int constitution;
        public int wits;

        public Stat_Snapshot(int strength, int dexterity, int inteligence, int constitution, int wits)
        {
            this.strength = strength;
            this.dexterity = dexterity;
            this.inteligence = inteligence;
            this.constitution = constitution;
            this.wits = wits;
        }

    }
}
