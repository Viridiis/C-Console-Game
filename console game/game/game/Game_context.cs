using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Game_context
    {
        public required Unit user { get; init; }
        public required Unit enemy { get; init; }
    }
}
