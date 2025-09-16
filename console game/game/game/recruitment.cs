using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class recruitment
    {
        private static Type[] recruitable_units;
        static recruitment()
        {
            recruitable_units = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttribute<Usable_unit>() is not null).ToArray();
        }

        public static IEnumerable<Type> get_recruitable_ally_units()
        {
            return recruitable_units.Where(x => x.GetCustomAttribute<Usable_unit>().ally_team_unit);
        }

        public static IEnumerable<Type> get_recruitable_enemy_units()
        {
            return recruitable_units.Where(x => x.GetCustomAttribute<Usable_unit>().enemy_team_unit);
        }
    }
}
