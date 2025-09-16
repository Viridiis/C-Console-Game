using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class Enemy_team
    {
        public static Type[,] enemy_team_1 = new Type[3, 2]
        {
            {typeof(Jockey), null},
            {null, typeof(Jockey)},
            {typeof(Bruce), typeof(Bruce)}
        };
    }
}
