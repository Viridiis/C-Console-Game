using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public interface Inventory_item
    {
        string name { get; }
        public void use_item (Game_context context);
    }
}
