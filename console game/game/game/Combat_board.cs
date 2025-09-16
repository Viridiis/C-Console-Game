using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{

    public enum Team
    {
        ally_team,
        enemy_team
    }


    public class Combat_board
    {

        public Unit[,] board1 = new Unit[3, 2];
        public Unit[,] board2 = new Unit[3, 2];

        public (int, int) unit_cords(int tile)
        {
            int x = tile / 3.0d > 1.0d ? 1 : 0;
            int y = (tile - 1) % 3;
  
            return (x, y);
        }

        public IEnumerable<Unit> get_all_units()
        {
            Unit[,] board = board1;
            foreach (Unit unit in board) 
            {
                if (unit is not null) yield return unit;
            }
            board = board2;
            foreach (Unit unit in board)
            {
                if (unit is not null) yield return unit;
            }
            yield break;
        }

        public IEnumerable<Unit> get_all_units_in_team(Team team)
        {
            Unit[,] board = team == Team.ally_team ? board1 : board2;
            foreach (Unit unit in board)
            {
                if (unit is not null) yield return unit;
            }
            yield break;
        }


        public bool is_unit_at_tile(int tile, Team checked_team)
        {
            if(tile <= 0 || tile > 6) return false;
            (int x, int y) = unit_cords(tile);
            Unit[,] board = checked_team == Team.ally_team ? board1 : board2;
            return board[y, x] is not null;
        }

        public Unit get_unit_at_tile(int tile, Team checked_team)
        {
            if (tile <= 0 || tile > 6) return null;
            (int x, int y) = unit_cords(tile);
            Unit[,] board = checked_team == Team.ally_team ? board1 : board2;
            return board[y, x];
        }

        public Team get_unit_team(Unit unit)
        {
            ArgumentNullException.ThrowIfNull(unit);
            for (int i = 1; i <= 6; i++)
            {
                Unit unitAtTile = get_unit_at_tile(i, Team.ally_team);
                if (unitAtTile == unit) return Team.ally_team;
            }
            for (int i = 1; i <= 6; i++)
            {
                Unit unitAtTile = get_unit_at_tile(i, Team.enemy_team);
                if (unitAtTile == unit) return Team.enemy_team;
            }
            throw new ArgumentException("Unit is not part of the battle");
        }

        public int get_unit_tile(Unit unit, OptionalParameter<Team> team = default)
        {
            if (team.HasValue)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Unit unitAtTile = get_unit_at_tile(i, team);
                    if (unitAtTile == unit) return i;
                }
                return -1;
            }
            for (int i = 1; i <= 6; i++)
            {
                Unit unitAtTile = get_unit_at_tile(i, Team.ally_team);
                if (unitAtTile == unit) return i;
            }
            for (int i = 1; i <= 6; i++)
            {
                Unit unitAtTile = get_unit_at_tile(i, Team.enemy_team);
                if (unitAtTile == unit) return i;
            }
            return -1;
        }

        public void draw_board()
        {
            /*
            board1[0, 0] = '1';
            board1[0, 1] = '4';
            board1[1, 0] = '2';
            board1[1, 1] = '5';
            board1[2, 0] = '3';
            board1[2, 1] = '6';

            board2[0, 0] = '1';
            board2[0, 1] = '4';
            board2[1, 0] = '2';
            board2[1, 1] = '5';
            board2[2, 0] = '3';
            board2[2, 1] = '6';
            */

            Console.WriteLine("+-----+-----+          +-----+-----+");
            for (int y = 0; y < board1.GetLength(0); y++)
            {

                Console.WriteLine($"|  {(board1[y, 1] is null ? " " : !board1[y, 1].is_alive() ? "x" : y + 1 + board1.GetLength(0))}  |  {(board1[y, 0] is null ? " " : !board1[y, 0].is_alive()? "x" : y + 1)}  |          |  {(board2[y, 0] is null ? " " : !board2[y, 0].is_alive() ? "x" : y + 1)}  |  {(board2[y, 1] is null ? " " : !board2[y, 1].is_alive() ? "x" : y + 1 + board2.GetLength(0))}  |");

                Console.WriteLine("+-----+-----+          +-----+-----+");
            }

           /*
            Console.WriteLine("+-----+-----+          +-----+-----+");
            Console.WriteLine("|  " + board1[0, 1] + "  |  " + board1[0, 0] + "  |          |  " + board2[0, 0] + "  |  " + board2[0, 1] + "  |");
            Console.WriteLine("+-----+-----+          +-----+-----+");
            Console.WriteLine("|  " + board1[1, 1] + "  |  " + board1[1, 0] + "  |          |  " + board2[1, 0] + "  |  " + board2[1, 1] + "  |");
            Console.WriteLine("+-----+-----+          +-----+-----+");
            Console.WriteLine("|  " + board1[2, 1] + "  |  " + board1[2, 0] + "  |          |  " + board2[2, 0] + "  |  " + board2[2, 1] + "  |");
            Console.WriteLine("+-----+-----+          +-----+-----+");
           */


            Console.WriteLine();

        }

    }
}
