using Godot;

public class ContainerSlot : VBoxContainer
{
    public TextureRect ItemTexture { get; set; }
    public Label ItemLabel { get; set; }

    public int Count;

    public Texture Texture;

    public override void _Ready()
    {
        ItemTexture = GetNode<TextureRect>("ItemIcon");
        ItemLabel = GetNode<Label>("ItemIcon/Count");      
    }

    public void UpdateContainerSlot()
    {
        ItemTexture.Texture = Texture;
        ItemLabel.Text = Count.ToString();    
    }
}
