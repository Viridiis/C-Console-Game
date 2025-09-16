using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Spawner
    {

        public static void spawn(Type[,] enemy_team, Combat_board board)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    Type enemyType = enemy_team[y,x];
                    if (enemyType is null) continue;
                    Unit enemy_unit = Activator.CreateInstance(enemyType) as Unit;
                    board.board2[y, x] = enemy_unit;
                }
            }
                

        }
    }
}
