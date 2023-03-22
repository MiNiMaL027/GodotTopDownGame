using Godot;

public class MainContainer : Area2D
{
    public AnimationPlayer Anim;
    public Container container;
    public InventoryHud InventoryHud;
    
    public override void _Ready()
    {
        Anim = GetNode<AnimationPlayer>("AnimationPlayer");
        container = GetNode<Container>("Container");
        InventoryHud = GetNode<InventoryHud>("Container/ContainerHud");

        Connect("area_entered", this, nameof(ChekArea));
        Connect("body_entered", this, nameof(ChekBody));
        Connect("body_exited", this, nameof(ChekBodyExited));

        Position = new Vector2(8020, 8010);
    }

    public void ChekArea(Area2D Ore)
    {
        if(Ore is Iinteractbl)
        {
            container.AddItem((Ore)Ore);
            ((World)GetParent()).RemoveChild(Ore);
        }     
    }

    public void ChekBody(Node Body)
    {  
        Anim.Play("Open");
        InventoryHud.Visible= true;
    }

    public void ChekBodyExited(Node Body)
    {
        Anim.Play("Close");
        InventoryHud.Visible = false;
    }
}
