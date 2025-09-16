using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace game
{
    [Usable_unit(ally_team_unit = false)]
    public class Imp : Unit
    {
        public Imp() : base(nameof(Imp), 20, 20, 10, 10, 10, 10, 5, Weapon_List.weapons[3])
        {

        }
    }

    [Usable_unit]
    public class Jockey : Unit
    {
        public Jockey() : base(nameof(Jockey), 20, 20, 10, 10, 10, 10, 5, Weapon_List.weapons[3])
        {

        }

    }

    [Usable_unit]
    public class Bruce : Unit
    {
        public Bruce() : base(nameof(Bruce), 30, 30, 10, 10, 10, 10, 5, Weapon_List.weapons[0])
        {

        }
    }

    [Usable_unit]
    public class Frencis_with_shotgun : Unit
    {
        public Frencis_with_shotgun() : base(nameof(Frencis_with_shotgun), 30, 30, 400, 10, 10, 10, 5, Weapon_List.weapons[4])
        {

        }
    }
}
