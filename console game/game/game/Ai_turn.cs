using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class Ai_turn
    {
        public static void ai_execute_turn(Unit unit, int tile)
        {
            int[] attackable_tiles = Game.get_attackable_tiles(unit, tile, Team.enemy_team);
            if (attackable_tiles.Length <= 0)
            {
                unit.defend();
                Console.WriteLine($"{unit.name} defends");
                return;
            }
            int picked_tile = Random.Shared.Next(0, attackable_tiles.Length);
            int attacked_tile = attackable_tiles[picked_tile];
            unit.attack(Game.board.get_unit_at_tile(attacked_tile, Team.ally_team), attacked_tile, tile);
            
        }        
    }
}
