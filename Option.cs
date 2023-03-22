using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine
{
    public static class Option
    {
        public static bool RemoveFromParent(Node target)
        {
            Node parent = target.GetParent();
            if (parent != null)
            {
                parent.RemoveChild(target);
                return true;
            }
            return false;
        }
    }
}
