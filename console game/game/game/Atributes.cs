using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    // Attribute class af
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Usable_unit : Attribute
    {
        public bool ally_team_unit  { get; init; } = true;
        public bool enemy_team_unit { get; init; } = true;
    }

}
