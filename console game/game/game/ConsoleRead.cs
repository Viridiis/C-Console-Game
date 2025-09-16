using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class ConsoleRead
    {
        public static bool try_read_int(out int value)
        {
            value = 0;
            string s = Console.ReadLine();
            if (int.TryParse(s, out value)) return true;
            return false;
        }
    }
}
