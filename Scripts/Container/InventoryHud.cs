using Godot;
using System.Collections.Generic;

public class InventoryHud : HBoxContainer
{
    public PackedScene Slot;
    public Dictionary<int, ContainerSlot> inventoryHud = new Dictionary<int, ContainerSlot>();
    private Ore baseore = new Ore();

    public override void _Ready()
    {
        Slot = ResourceLoader.Load<PackedScene>("res://Scenes/Container/ContainerSlot.tscn");
    }

    public void AddItem(int id)
    {
        if (inventoryHud.ContainsKey(id))
        {
            inventoryHud[id].Count++;

            inventoryHud[id].UpdateContainerSlot();
        }
        else
        {
            GD.Print("CreateNewSlot");

            ContainerSlot slot = Slot.Instance<ContainerSlot>(); 

            inventoryHud.Add(id, slot);
            AddChild(slot);    
            
            inventoryHud[id].Texture = baseore.Init(id);
            inventoryHud[id].Count = 1;
            inventoryHud[id].UpdateContainerSlot();
        }
    }

    public void RemoveItem(int id)
    {
        if (inventoryHud.ContainsKey(id))
        {
            if (inventoryHud[id].Count >= 2)
            {
                inventoryHud[id].Count--;

                inventoryHud[id].UpdateContainerSlot();
            }
            else
            {
                inventoryHud[id].UpdateContainerSlot();
                RemoveChild(inventoryHud[id]);
                inventoryHud.Remove(id);             
            }
        }
    }
}
