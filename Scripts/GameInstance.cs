using Godot;
using mine;

public class GameInstance : Node
{
    public static World CurrentWorld { get; set; }   
    public static PlayerController Player { get; set; }

    public override void _Ready()
    {
        Player.Position = new Vector2(8000, 8000);
    }

    public static void ChangeWorld(Node2D world)
    {
        Player.Position = new Vector2(8000, 8000);

        GameInstance G = CurrentWorld.GetTree().CurrentScene as GameInstance;
        MainContainer mainContainer = CurrentWorld.GetNode<MainContainer>("MainContainer");

        Option.RemoveFromParent(Player);
        Option.RemoveFromParent(mainContainer);
        CurrentWorld.QueueFree();
        CurrentWorld = (World)world;
        CurrentWorld.AddChild(Player);
        CurrentWorld.AddChild(mainContainer);

        G.AddChild(CurrentWorld);
        
        mainContainer.Position = Player.Position + new Vector2(20,10);     
    }
}

public interface Iinteractbl
{

}
