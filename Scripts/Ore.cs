using Godot;

public class Ore : Area2D , Iinteractbl
{
    public int ID;   
    public Sprite Sprite;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite>("OreSprite");
        
    }

    public Texture Init(int iD)
    {
        ID = iD;
        if (iD == (int)Item.Copper)
        {        
            return ResourceLoader.Load<Texture>("res://Content/Item/Ore/copper_ore.png");
        }
        else if (ID == (int)Item.Iron)
        {
            return ResourceLoader.Load<Texture>("res://Content/Item/Ore/iron_ore.png");
        }
        else if(ID == (int)Item.Gold)
        {
            return ResourceLoader.Load<Texture>("res://Content/Item/Ore/gold_ore.png");
        }
        else
            return null;
    }

    public void SetID(int id)
    {
        ID = id;
    }
}
