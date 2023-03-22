using Godot;
using System.Collections.Generic;

public class Container : StaticBody2D
{
    Control InventoryHud;

    public Stack<Ore> ContainerInventory = new Stack<Ore>();

    public override void _Ready()
    {
        InventoryHud = GetNode<Control>("ContainerHud");
    }

    public void AddItem(Ore item)
    {
       ContainerInventory.Push(item);
       ((InventoryHud)InventoryHud).AddItem(item.ID);
    }

    public void RemoveItem(Ore item)
    {
       ((InventoryHud)InventoryHud).RemoveItem(item.ID);
    }
}
