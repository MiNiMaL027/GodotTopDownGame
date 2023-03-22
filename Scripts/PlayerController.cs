using Godot;
using System;

public class PlayerController : KinematicBody2D
{
    #region Variable
    #region Tail
    public Timer Timer;
    public int TileReinforced = 100;
    public const int DigReinforcedTile = 0;
    public int PicaxeStrenght = 2;
    public const int BaseReinforsed = 100;
    public int procentDestroy = 0;
    public int procentToDestroy = 0;
    public float TileBreackDivide;
    public int CountOre = 0;
    #endregion
    #region Inventory
    public PackedScene ore;
    int ContainerCount = 1;
    int CurrentContainerCount = 0;
    public Ore Inventory = null;
    public Sprite Slot;
    #endregion

    bool isAtacking = false;
    float AttackSpeed = 2f;
    
    public Vector2 LineTraysRotator = new Vector2(0, 0);
    public Vector2 CurrentCell = new Vector2(0, 0);

    [Export] float Speed = 80.0f;

    #endregion Variable

    public override void _Ready()
    {
        GameInstance.Player = this;

        Timer = GetNode<Timer>("AttackSpeed");
        Slot = GetNode<Sprite>("InventorySlot");

        ore = ResourceLoader.Load<PackedScene>("res://Scenes/Item/Ore.tscn");

        Timer.WaitTime = 1f/AttackSpeed;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (isAtacking && Timer.IsStopped())
        {
            Timer.Start();

            var spaceState = GetWorld2d().DirectSpaceState;
            Vector2 FinishPoint = this.GlobalPosition + LineTraysRotator * 15;

            var result = spaceState.IntersectRay(this.GlobalPosition, FinishPoint, new Godot.Collections.Array { this }, this.CollisionMask, true, true);

            if (result.Count > 0)
            {
                if (result["collider"] is TileMap m)
                {
                    Vector2 FinishPosition = (Vector2)result["position"];

                    if (LineTraysRotator == new Vector2(-1, 0))
                        FinishPosition = new Vector2(FinishPosition.x - 1, FinishPosition.y);

                    if (LineTraysRotator == new Vector2(0, -1))
                        FinishPosition = new Vector2(FinishPosition.x, FinishPosition.y - 1);

                    Vector2 localPosition = m.ToLocal(FinishPosition);
                    Vector2 cell = m.WorldToMap(localPosition);

                    // перевірки на TailID

                    if (m.GetCellv(cell) == (int)Tile.Copper)
                    {
                        ChangeTileInfo(m.GetCellv(cell), cell, m, FinishPosition);
                    }
                    else if (m.GetCellv(cell) == (int)Tile.Iron)
                    {
                        ChangeTileInfo(m.GetCellv(cell), cell, m, FinishPosition);
                    }
                    else if (m.GetCellv(cell) == (int)Tile.Gold)
                    {
                        ChangeTileInfo(m.GetCellv(cell), cell, m, FinishPosition);
                    }
                    else if (m.GetCellv(cell) == (int)Tile.Earth)
                    {
                        ChangeTileInfo(m.GetCellv(cell), cell, m, FinishPosition);
                    }
                    else if (m.GetCellv(cell) == (int)Tile.Stone)
                    {
                        ChangeTileInfo(m.GetCellv(cell), cell, m, FinishPosition);
                    }
                }
            }
        }
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("Interact"))
        {
            if (CurrentContainerCount == 1)
            {
                Ore ore = Inventory;

                ((World)this.GetParent()).AddChild(ore);

                Inventory = null;
                ore.GlobalPosition = this.GlobalPosition;
                Slot.Texture = null;

                CurrentContainerCount--;
            }
            else
            {
                var spaceState = GetWorld2d().DirectSpaceState;
                Vector2 FinishPoint = this.GlobalPosition + LineTraysRotator * 15;

                var result = spaceState.IntersectRay(this.GlobalPosition, FinishPoint, new Godot.Collections.Array { this }, this.CollisionMask, true, true);

                if (result.Count > 0)
                {
                    if (result["collider"] is Container container)
                    {
                        if (container.ContainerInventory.Count > 0)
                        {
                            Ore CurrentOre = container.ContainerInventory.Pop();

                            container.RemoveItem(CurrentOre);

                            Inventory = CurrentOre;
                            Slot.Texture = Inventory.Sprite.Texture;
                            CurrentContainerCount++;
                        }
                    }

                    if (result["collider"] is Ore ore)
                    {
                        if (CheckContainer())
                        {
                            Texture CurrentOreSprite = ore.Sprite.Texture;
                            Inventory = ore;

                            ((World)this.GetParent()).RemoveChild(ore);

                            Slot.Texture = CurrentOreSprite;
                            CurrentContainerCount++;
                        }
                    }
                }
            }
        }

        if (Input.IsActionPressed("Attack") && Inventory == null)
        {
            isAtacking = true;
            //GameInstance.ChangeWorld(Worlds.Mine());
        }

        if (Input.IsActionJustReleased("Attack"))
        {
            isAtacking = false;
        }

        AnimationPlayer Anim = GetNode<AnimationPlayer>("AnimationPlayer");

        Vector2 velocity = Vector2.Zero;

        if (Input.IsActionPressed("Move_Right"))
        {
            velocity.x += 1;
            Anim.Play("walk_right");
            LineTraysRotator = new Vector2(1, 0);
        }
        else if (Input.IsActionPressed("Move_Left"))
        {
            velocity.x -= 1;
            Anim.Play("walk_left");
            LineTraysRotator = new Vector2(-1, 0);
        }
        else if (Input.IsActionPressed("Move_Up"))
        {
            velocity.y -= 1;
            Anim.Play("walk_up");
            LineTraysRotator = new Vector2(0, -1);
        }
        else if (Input.IsActionPressed("Move_Down"))
        {
            velocity.y += 1;
            Anim.Play("walk_down");
            LineTraysRotator = new Vector2(0, 1);
        }
        else
        {
            Anim.Play("Idle");
        }

        MoveAndSlide(velocity * Speed);

    }
    private void ChangeTileInfo(int Cellid, Vector2 cell, TileMap m, Vector2 GlobalCell)                         // TileRenforsed. це змінна яка відповідає за міцність тайла,коли вона 0(DigTailRenforsed) .
    {

        // Значення TileReinforsed означає Кількість ударів киркою по блоку.Але TileReinforsed не обновляється при ініціалізації,тому по блоку буде лишній удар
        if (TileReinforced == BaseReinforsed)                                          // По дефолту TileReinforsed + 1(TileReinforsed = 3(4 удари))
        {
            CurrentCell = cell;

            SetTileReinforced(Cellid);

            TileBreackDivide = SetBreackDivide(TileReinforced);

            GameInstance.CurrentWorld.TopMap.SetCell((int)cell.x, (int)cell.y, (int)((float)TileReinforced / TileBreackDivide));
            procentDestroy = 0;
            procentToDestroy = TileReinforced;

            if (TileReinforced <= PicaxeStrenght)
            {
                TileReinforced = 0;

                DestroyTile(Cellid, cell, m, GlobalCell);
            }
        }
        else if (CurrentCell != cell)
        {
            GameInstance.CurrentWorld.TopMap.SetCell((int)CurrentCell.x, (int)CurrentCell.y, -1);   
                                                                                                                    //Якщо починає ламатись інший блок то цей блок стає цілим
            CurrentCell = cell;
            SetTileReinforced(Cellid);

            TileBreackDivide = SetBreackDivide(TileReinforced);

            GameInstance.CurrentWorld.TopMap.SetCell((int)cell.x, (int)cell.y, (int)((float)TileReinforced / TileBreackDivide));

            procentDestroy = 0;
            procentToDestroy = TileReinforced;

            if (TileReinforced <= PicaxeStrenght)
            {
                TileReinforced = 0;
                DestroyTile(Cellid, cell, m, GlobalCell);
            }
        }
        else
        {
            DestroyTile(Cellid, cell, m, GlobalCell);
        }

    }

    private void SetTileReinforced(int CellID)
    {
        if (CellID == (int)Tile.Earth)
            TileReinforced = 2;
        else if (CellID == (int)Tile.Stone)
            TileReinforced = 3;
        else if (CellID == (int)Tile.Copper)
            TileReinforced = 4;
        else if (CellID == (int)Tile.Iron)
            TileReinforced = 5;
        else if (CellID == (int)Tile.Gold)
            TileReinforced = 10;
    }

    private float SetBreackDivide(float TileReinforced)
    {
        if (TileReinforced < 4)
            return 0.5f;
        else if (TileReinforced == 5)
            return 1;
        else if (TileReinforced > 5 && TileReinforced <= 10)
            return 2;
        else if (TileReinforced > 10 && TileReinforced <= 15)
            return 3;
        else if (TileReinforced > 15 && TileReinforced <= 20)
            return 4;
        else if (TileReinforced > 20 && TileReinforced <= 25)
            return 5;
        else
            return 1;

    }

    bool CheckContainer()
    {
        if (ContainerCount > CurrentContainerCount)
        {
            return true;
        }
        else
            return false;
    }

    public void DestroyTile(int Cellid, Vector2 cell, TileMap m, Vector2 GlobalCell)
    {
        procentDestroy++;
        procentDestroy += PicaxeStrenght;

        if (procentDestroy >= procentToDestroy)
        {
            TileReinforced--;
            GameInstance.CurrentWorld.TopMap.SetCell((int)cell.x, (int)cell.y, (int)((float)TileReinforced / TileBreackDivide));
            procentDestroy = 0;

            if (TileBreackDivide >= 2 && TileReinforced > 0 && TileReinforced <= 2)
                GameInstance.CurrentWorld.TopMap.SetCell((int)cell.x, (int)cell.y, 1);
        }

        if (TileReinforced <= DigReinforcedTile)
        {
            GameInstance.CurrentWorld.RemoveTail(cell);
            m.GetParent<World>().UpdateTile(cell);
            TileReinforced = 100;

            if (Cellid > (int)Tile.Earth && Cellid < (int)Tile.Stone)
            {
                Ore o = ore.Instance<Ore>();

                ((World)this.GetParent()).AddChild(o);
                o.SetID(Cellid);

                o.Sprite.Texture = o.Init(Cellid);
                o.GlobalPosition = GlobalCell + LineTraysRotator * 15;
            }
            else if (Cellid == (int)Tile.Stone)
            {
                Random rnd = new Random();
                int num = rnd.Next(0, 100);

                if (num == 1)
                {
                    Ore o = ore.Instance<Ore>();

                    ((World)this.GetParent()).AddChild(o);
                    o.SetID(Cellid);

                    o.Sprite.Texture = o.Init(Cellid);
                    o.GlobalPosition = GlobalCell + LineTraysRotator * 15;
                }
            }
        }
    }
}

enum Tile
{    
    Trail=-1,
    Earth= 0,
    Copper =2,
    Iron =3,
    Gold = 4,
    Stone = 5,   
    SolidStone = 6,
    Wall = 7
}

enum Item
{
    Copper = 2,
    Iron = 3,
    Gold = 4,
    Stone = 5
}
