using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
namespace game
{
    static class Game
    {
        static Hero hero;
        static Unit enemy;
        static int player_choice;
        static int skill_points = 4;
        static string skill_distribution;
        static List<Spell> spells_known;
        static string item_choosed = "";
        public static Combat_board board = new Combat_board();
        static int units_to_choose = 3;
        

        public static void new_game()
        {
            //board.draw_board();
            create_hero();
            choose_weapon();
            add_stats();
            choose_units();
            hero.currentHp = hero.maxHp;
            
            Spawner.spawn(Enemy_team.enemy_team_1, board); 
            loop_game();
        }

        public static void create_hero()
        {
            Console.Write("An adventure beggins. ");
            name:
            Console.Write("What is your name, hero?\n");
            string hero_name = Console.ReadLine();

            if (String.IsNullOrEmpty(hero_name))
            {
                Console.WriteLine("Empty name");
                goto name;
            }

            Cords:
            Console.WriteLine("What tile you want to play your hero at? ");
            if (!int.TryParse(Console.ReadLine(), out int tile))
            {
                Console.WriteLine("Incorrect input");
                goto Cords;
            }
            if (tile <= 0 || tile > 6)
            {
                Console.WriteLine("Too big number");
                goto Cords;
            }

            hero = new Hero(hero_name);
            (int x, int y) = board.unit_cords(tile);
            board.board1[y, x] = hero;


            hero.give_item(Inventory_Item_List.items[0]);
            hero.give_item(Inventory_Item_List.items[0]);
            hero.give_item(Inventory_Item_List.items[1]);
            hero.give_item(Inventory_Item_List.items[2]);

        }

        public static void choose_units()
        {
            List<Type> ally_team = new List<Type>();
            while (units_to_choose > 0)
            {
                Console.WriteLine($"\nYou have {units_to_choose} units to choose:");
                int i = 1;
                foreach (Type t in recruitment.get_recruitable_ally_units())
                {
                    Console.WriteLine($"{i} - {t.Name}");
                    i++;
                }

                if (int.TryParse(Console.ReadLine(), out int unit_chosen))
                {
                    unit_chosen--;
                    Type choosen_unit = recruitment.get_recruitable_ally_units().ElementAtOrDefault(unit_chosen);
                    if (choosen_unit is null)
                    {
                        Console.WriteLine("Incorrect Unit id");
                        continue;
                    }
                    Console.WriteLine($"You have choosen: {choosen_unit.Name}\n");
                    units_to_choose--;
                    ally_team.Add(choosen_unit);
                }
            }

            List<int> unit_coords = new List<int>(ally_team.Count);
            Console.Write("Your units are: ");
            foreach (Type ally in ally_team)
            {
                Console.Write($"{ally.Name}, \n");
            }

            for (int i = 0; i < ally_team.Count; i++)
            {
                int tile = 0;

                Cords:
                Console.WriteLine($"\nWhat tile you want {ally_team[i].Name} to place at?");
                if (!int.TryParse(Console.ReadLine(), out tile))
                {
                    Console.WriteLine("Incorrect input");
                    goto Cords;
                }
                if (tile <= 0 || tile > 6)
                {
                    Console.WriteLine("Too big number");
                    goto Cords;
                }
                if (unit_coords.Contains(tile) || board.is_unit_at_tile(tile, Team.ally_team))
                {
                    Console.WriteLine("There's already unit on this tile");
                    goto Cords;
                }                                                            
                unit_coords.Add(tile);
            }

            for(int i = 0; i < ally_team.Count; i++)
            {
                Type allyType = ally_team[i];
                (int x, int y) = board.unit_cords(unit_coords[i]);
                Unit ally_unit = Activator.CreateInstance(allyType) as Unit;
                board.board1[y, x] = ally_unit;
            }
        }

        public static void choose_weapon()
        {  
            while(true)
            {
                Console.WriteLine("\nChoose your weapon:");
                int weapon_id = 1;
                foreach (Weapon wep in Weapon_List.weapons)
                {
                    Console.WriteLine($"{weapon_id}: {wep.name} ({wep.minDmg} - {wep.maxDmg})");
                    weapon_id++;
                }
                if (int.TryParse(Console.ReadLine(), out int weapon_choosed))    // check if string conversion didnt fail (like typing 'a')
                {
                    weapon_choosed--;
                    if ((uint)weapon_choosed >= (uint)Weapon_List.weapons.Count)  // check if int is outside list
                    {
                        Console.WriteLine("Incorrect weapon id");
                        continue;
                    }
                    hero.weapon = Weapon_List.weapons[weapon_choosed];
                    Console.WriteLine("u choosed " + hero.weapon.name);
                    break;
                }
                Console.WriteLine("u typed bs, stop trolling and hop on l4d2");
            } 
        }

        public static void add_stats()
        {
            distribution:
            Console.WriteLine("\nSkill points: " + skill_points);
            hero.display_stats();
            Console.WriteLine("Write s/d/i/c/w and numbers");
            skill_distribution = Console.ReadLine().ToLowerInvariant();
            int distributed = 0;
            int no_digit_index = 0;
            Stat_Snapshot snapshot = hero.take_stats_snapshot();

            foreach (string c in skill_distribution.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                no_digit_index = c.FindIndexOf(x => !char.IsDigit(x));
                if (no_digit_index < 0)
                {
                    Console.WriteLine("No skill selected");
                    hero.reset_stats(snapshot);
                    goto distribution;
                }
                ReadOnlySpan<char> span = c.AsSpan();
                ReadOnlySpan<char> span_char = span.Slice(no_digit_index);
                ReadOnlySpan<char> span_number = span.Slice(0, no_digit_index);
             

                if (!int.TryParse(span_number, out int stat_points))
                {
                    Console.WriteLine("No skills distrubution found in input, try again");
                    hero.reset_stats(snapshot);
                    goto distribution;
                }

                distributed += stat_points;
                if (distributed > skill_points)
                {
                    Console.WriteLine("\nToo many skill points given, try again");
                    hero.reset_stats(snapshot);
                    goto distribution;
                }

                switch (span_char)
                {
                    case "s":
                        hero.strength = hero.strength + stat_points;
                        break;
                    case "d":
                        hero.dexterity = hero.dexterity + stat_points;
                        break;
                    case "i":
                        hero.inteligence = hero.inteligence + stat_points;
                        break;
                    case "c":
                        hero.consitution = hero.consitution + stat_points;
                        hero.maxHp = (int)(hero.maxHp + (hero.maxHp * (0.06 * stat_points)));
                        break;
                    case "w":
                        hero.wits = hero.wits + stat_points;
                        hero.iniciative = hero.iniciative + stat_points;
                        hero.crit_chance = hero.crit_chance + stat_points;
                        break;
                    default:
                        Console.WriteLine("Incorrect input");
                        hero.reset_stats(snapshot);
                        goto distribution;
                }
            }

            skill_points -= distributed;

            if (skill_points <= 0) { return; }
            if (skill_distribution == "exit") { return; }
            if (skill_distribution != "exit") { goto distribution; }
        }

        public static bool is_team_alive(Team team)
        {
            for (int i = 1; i <= 6; i++)
            {
                Unit unit = board.get_unit_at_tile(i, team);
                if (unit is null) continue;
                if (unit.is_alive()) return true;
            }
            return false;
        }

        public static bool are_teams_alive()
        {
            return is_team_alive(Team.ally_team) && is_team_alive(Team.enemy_team);
        }

        public static int[] get_attackable_tiles(Unit unit, int tile, Team attacking_team)
        {
            Team opposite_team = attacking_team == Team.ally_team ? Team.enemy_team : Team.ally_team;
            List<int> attackable_tiles = new List<int>(6);
            switch(unit.weapon.type)
            {
                case weapon_type.melee:
                {
                    switch (tile)
                    {
                        case 1:
                            {
                                int i = 1;
                                for (int rows = 1; rows <= 2; rows++)
                                {
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i++;
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i++;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i++;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                }
                                return Array.Empty<int>();
                            }

                        case 2:
                            {
                                for (int rows = 0; rows <= 1; rows++)
                                {
                                    for (int i = 1; i <= 3; i++)
                                    {
                                        int true_index = rows * 3 + i;
                                        if (board.is_unit_at_tile(true_index, opposite_team))
                                        {
                                            if (board.get_unit_at_tile(true_index, opposite_team).is_alive())
                                                attackable_tiles.Add(true_index);
                                        }
                                    }
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                }
                                return Array.Empty<int>();
                            }

                        case 3:
                            {
                                int i = 3;
                                for (int rows = 1; rows <= 2; rows++)
                                {
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i--;
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i--;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i = 6;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                }
                                return Array.Empty<int>();
                            }

                        case 4:
                            {
                                for (int allyTile = 1; allyTile <= 3; allyTile++)
                                {
                                    if (board.is_unit_at_tile(allyTile, attacking_team))
                                    {
                                        if (board.get_unit_at_tile(allyTile, attacking_team).is_alive()) return Array.Empty<int>();
                                    }
                                }

                                int i = 1;
                                for (int rows = 1; rows <= 2; rows++)
                                {
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i++;
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i++;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i++;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                }
                                return Array.Empty<int>();
                            }

                        case 5:
                            {
                                for (int allyTile = 1; allyTile <= 3; allyTile++)
                                {
                                    if (board.is_unit_at_tile(allyTile, attacking_team))
                                    {
                                        if (board.get_unit_at_tile(allyTile, attacking_team).is_alive()) return Array.Empty<int>();
                                    }
                                }

                                for (int rows = 0; rows <= 1; rows++)
                                {
                                    for (int i = 1; i <= 3; i++)
                                    {
                                        int true_index = rows * 3 + i;
                                        if (board.is_unit_at_tile(true_index, opposite_team))
                                        {
                                            if (board.get_unit_at_tile(true_index, opposite_team).is_alive())
                                                attackable_tiles.Add(true_index);
                                        }
                                    }
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                }
                                return Array.Empty<int>();
                            }

                        case 6:
                            {
                                for (int allyTile = 1; allyTile <= 3; allyTile++)
                                {
                                    if (board.is_unit_at_tile(allyTile, attacking_team))
                                    {
                                        if (board.get_unit_at_tile(allyTile, attacking_team).is_alive()) return Array.Empty<int>();
                                    }
                                }

                                int i = 3;
                                for (int rows = 1; rows <= 2; rows++)
                                {
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i--;
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i--;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                    if (board.is_unit_at_tile(i, opposite_team))
                                    {
                                        if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                            attackable_tiles.Add(i);
                                    }
                                    i = 6;
                                    if (attackable_tiles.Count > 0) return attackable_tiles.ToArray();
                                }
                                return Array.Empty<int>();
                            }

                        default:
                            return Array.Empty<int>();
                    }
                }

                case weapon_type.aoe_heal:
                case weapon_type.magic:
                    return null;

                case weapon_type.ranged:
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        if (board.is_unit_at_tile(i, opposite_team))
                        {
                            if (board.get_unit_at_tile(i, opposite_team).is_alive())
                                attackable_tiles.Add(i);
                        }
                    }
                    return attackable_tiles.ToArray();
                }

                case weapon_type.single_target_heal:
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            if (board.is_unit_at_tile(i, attacking_team))
                            {
                                if (board.get_unit_at_tile(i, attacking_team).is_alive())
                                    attackable_tiles.Add(i); // aka healable tiles
                            }
                        }
                        return attackable_tiles.ToArray();
                    }
            }

            return Array.Empty<int>();


            //catsittingverycomfortablewaitingtouncommentthecode
            /*for (int i = 1; i <= 6; i++)
            {
                if (board.is_unit_at_tile(i, opposite_team))
                {
                    if(board.get_unit_at_tile(i, opposite_team).is_alive())
                        tiles_with_units.Add(i);
                }
            }
            return tiles_with_units.ToArray();*/
        }

        public static void loop_game()
        {
            //combat();
            List<Unit> all_units = new List<Unit>(12);
            all_units.AddRange(board.get_all_units());
            ulong turn = 1; 
            while (are_teams_alive())
            {
                Console.WriteLine($"Turn: {turn}");
                all_units.Sort((a, b) => 
                {
                    int cmp = a.iniciative.CompareTo(b.iniciative);
                    if (cmp == 0)
                    {
                        cmp = Random.Shared.Next(0, 2) == 0 ? -1 : 1;
                    }
                    return cmp * -1;
                    
                });

                for(int i = 0; i < all_units.Count; i++)
                {
                    unitsthatwaited:
                    Unit current_unit = all_units[i];
                    Team current_team = board.get_unit_team(current_unit);
                    int current_unit_tile = board.get_unit_tile(current_unit, current_team);
                    current_unit.stop_defend();
                    switch (current_team)
                    {
                        case Team.ally_team:
                        {
                            player_action_result result = Player_turn.player_execute_turn(current_unit, current_unit_tile);
                            if (result == player_action_result.action_wait)
                            {
                                all_units.RemoveAt(i);
                                all_units.Add(current_unit);
                                goto unitsthatwaited;
                            }
                            break;
                        }
                        case Team.enemy_team:
                            Ai_turn.ai_execute_turn(current_unit, current_unit_tile);                      
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }

                for (int i = 1; i <= 6; i++)
                {
                    Unit ally_unit = board.get_unit_at_tile(i, Team.ally_team);
                    if (ally_unit is null) continue;
                    Console.Write($"{i}. ");
                    board.get_unit_at_tile(i, Team.ally_team).display_base_info();
                }

                Console.Write("\n==========================================================\n\n");
                for (int i = 1; i <= 6; i++)
                {
                    Unit enemy_unit = board.get_unit_at_tile(i, Team.enemy_team);
                    if (enemy_unit is null) continue;
                    Console.Write($"{i}. ");
                    board.get_unit_at_tile(i, Team.enemy_team).display_base_info();
                }
                board.draw_board();
                turn++;
            }
            
        }

        public static void combat()
        {

        }


        /*public static void combat()
        {
            while (hero.is_alive() && enemy.is_alive())
            {
                Console.WriteLine("\nTurn: " + turn_counter + "\n");
                
                enemy.display_base_info();
                Console.WriteLine();

                if (hero.iniciative > enemy.iniciative)
                {
                    // hero starts
                }

                if (enemy.iniciative > hero.iniciative)
                {
                    // enemy starts
                }    

                if (hero.iniciative == enemy.iniciative)
                {
                    // random
                }

                no_atack:
                hero.display_actions();
                player_choice = Convert.ToInt32(Console.ReadLine());
                handle_choice(player_choice, enemy);
                if (player_choice == 4 || player_choice == 5){ goto no_atack; }
                if (!enemy.is_alive()) { break; }

                Console.WriteLine(enemy.name + " attacks " + hero.name);
                enemy.attack(enemy, hero);
                if (!hero.is_alive()) { break; }
                turn_counter++;
            }
        }*/

      /*  public static void handle_choice(int choice, Unit enemy)
        {
            switch(choice)
            {
                case 1:
                    Console.WriteLine("\n" + hero.name + " attacks " + enemy.name);
                    hero.attack(enemy);
                    break;
                case 2:
                    break;
                case 3:
                    Console.WriteLine("\nYour spells: " + hero.weapon_id.spell.name + "\n");
                    break;
                case 4:
                    hero.show_inventory();
                    Console.WriteLine("\nWhich item you want to use? ");
                    item_choosed = Console.ReadLine();
                    Inventory_item itemtouse = hero.inventory[int.Parse(item_choosed) - 1].item;
                    itemtouse.use_item(new Game_context() { user = hero, enemy = enemy });
                    hero.inventory.remove_item(itemtouse);
                    break;
                case 5:
                    break;
                case 6:
                    hero.display_stats();
                    break;
                case 7:
                    enemy.display_stats();
                    break;

               
            }
        }*/
    }
}
