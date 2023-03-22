using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine
{
    public class Worlds
    {
        public static Node2D Mine()
        {
            return ResourceLoader.Load<PackedScene>("res://Scenes/World.tscn").Instance<Node2D>();

        }
    }
}
