using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public enum player_action_result
    {
        action_fail,
        action_success,
        action_wait
    }

    public static class Player_turn
    {
        static (string, Func<Player_action_args, player_action_result>)[] ally_units_actions =
            [
            ("attack", attack_choice),
            ("defend", defend_choice),
            ("wait", wait_choice)
            ];
        static (string, Func<Player_action_args, player_action_result>)[] hero_actions = 
            [
            ("attack", attack_choice),
            ("defend", defend_choice),
            ("wait", wait_choice),
            ("item bag", null),
            ("spellbook", null)
            ];
        public static player_action_result player_execute_turn(Unit unit, int tile)
        {
            var available_actions = ally_units_actions; // = ally_ just as temporary value so var doesnt break

            //ValueTuple<string, int> e;
            if (unit is Hero hero)
            {
                available_actions = hero_actions;     
            }
            else
            {
                available_actions = ally_units_actions;
            }

            failed_input:
            Console.WriteLine($"Current unit: {unit.name} at tile {tile}");

            string action_description = available_actions[0].Item1;
            bool is_healing_weapon = unit.weapon.type.is_healing_type();
            if (is_healing_weapon)
            {
                action_description = "heal";
            }
            Console.WriteLine($"1. {action_description}");

            {
                int i = 1;
                int j = 2;
                while (i < available_actions.Length)
                {
                    action_description = available_actions[i].Item1;
                    if (action_description == "wait" && unit.is_waiting)
                    {
                        i++;
                        continue;
                    }
                    Console.WriteLine($"{j}. {action_description}");
                    j++;
                    i++;
                }
            }
                         
            if(!ConsoleRead.try_read_int(out int choice))
            {
                Console.WriteLine("Incorrect input");
                goto failed_input;
            }

            int action_count = available_actions.Length;
            if (unit.is_waiting)
            {
                action_count--;
            }

            choice--;
            if (choice <0 || choice > action_count)
            {
                Console.WriteLine("Incorrect input");
                goto failed_input;
            }
          
            //need var here for the reason that its likely we will change types around as we are coding this and dont wanna constantly
            //come back to this line to edit it
            // var = python af
            Func<Player_action_args, player_action_result> selected_action = null;
            if (unit.is_waiting)
            {
                selected_action = available_actions.Where(x => x.Item1 != "wait").ElementAt(choice).Item2;
            }
            else
            {
                selected_action = available_actions[choice].Item2;
            }

            if (selected_action is null)
            {
                Console.WriteLine($"Action: {available_actions[choice].Item1} is not implemented");
                return player_action_result.action_fail;
            }
            Player_action_args arg = new Player_action_args(unit, tile);
            player_action_result action_result = selected_action(arg);

            switch(action_result)
            {
                case player_action_result.action_fail:
                    goto failed_input;
                case player_action_result.action_wait: return action_result;
                default:
                    unit.is_waiting = false;
                    return action_result;
            }           
        }

        public static player_action_result attack_choice(Player_action_args arg)  // arg stands for both arguments from function
        {
            Unit unit = arg.unit;
            int tile = arg.tile;
            bool is_healing_weapon = unit.weapon.type.is_healing_type();
            int[] attackable_tiles = Game.get_attackable_tiles(unit, tile, Team.ally_team);
            
            if (attackable_tiles is null)
            {
                if (unit.weapon is not null)
                {
                    if (is_healing_weapon)
                    {
                        if(!Game.board.get_all_units_in_team(Team.ally_team).Where(x => x.is_alive()).Any(x => x.currentHp != x.maxHp))
                        {
                            Console.WriteLine("There are no tiles to heal");
                            return player_action_result.action_fail;
                        }
                        unit.aoe_heal(tile);
                        return player_action_result.action_success;
                    }
                    if (unit.weapon.type == weapon_type.magic)
                    {
                        unit.mage_attack(tile);
                        return player_action_result.action_success;
                    }
                }

                throw new InvalidProgramException();
            }
            if (attackable_tiles.Length <= 0)
            {
                Console.WriteLine(is_healing_weapon ? "There are no tiles to heal" : "There are no tiles to attack");
                return player_action_result.action_fail;
            }

            if(is_healing_weapon && !attackable_tiles.Any(x => { Unit u = Game.board.get_unit_at_tile(x, Team.ally_team); return u.currentHp != u.maxHp; }))
            {
                Console.WriteLine("There are no tiles to heal");
                return player_action_result.action_fail;
            }
            failed_input:
            Game.board.draw_board();
            if (!ConsoleRead.try_read_int(out int attacked_tile))
            {
                Console.WriteLine("Incorrect input");
                goto failed_input;
            }
            if (!attackable_tiles.Contains(attacked_tile))
            {
                Console.WriteLine("Incorrect input");
                goto failed_input;
            }
            if(is_healing_weapon)
            {
                unit.heal(Game.board.get_unit_at_tile(attacked_tile, Team.ally_team), attacked_tile, tile);
                return player_action_result.action_success;
            }
            unit.attack(Game.board.get_unit_at_tile(attacked_tile, Team.enemy_team), attacked_tile, tile);
            return player_action_result.action_success;
        }

        public static player_action_result defend_choice(Player_action_args arg)
        {
            arg.unit.defend();
            return player_action_result.action_success;
        }

        public static player_action_result wait_choice(Player_action_args arg)
        {  
            arg.unit.is_waiting = true;
            return player_action_result.action_wait;
        }
    }



    public class Player_action_args
    {
        public Unit unit;
        public int tile;
        public Player_action_args(Unit unit, int tile)
        {
            this.unit = unit;
            this.tile = tile;
        }
    }
}
