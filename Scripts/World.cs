using Godot;
using System;
using System.ComponentModel.Design.Serialization;
using System.Threading;

public class World : Node2D
{
    
    public Vector2 PlayerPosistion;
    public PlayerController Player;   
    public TileMap BottomMap;
    public TileMap TopMap;
    public TileMap OreMap;
    public OpenSimplexNoise noise;
    [Export]
    public float Weight;
    [Export]
    public float Height;
    Vector2 map_size;
    public Vector2 GlobalPositions;
    [Export]
    public int SizeFirstZone = 100;
    [Export]
    public int SizeSecondZone = 150;
    
    public float grass_cap = 0.5f;
    public Vector2 road_caps = new Vector2(0.3f, 0.05f);
    public Vector3 Ore_caps = new Vector3(0.4f, 0.3f, 0.04f);
    public Vector3 Iron_caps = new Vector3(0.25f, 0.23f, 0.01f);
    
    Random rnd = new Random();

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()

    {
        map_size = new Vector2(Weight, Height);
        
        GameInstance.CurrentWorld= this;
        Player = GetNode<PlayerController>("Player");         
        BottomMap = GetNode<TileMap>("BottomMap");
        TopMap = GetNode<TileMap>("TopMap");
        OreMap = GetNode<TileMap>("OreMap");
        noise = new OpenSimplexNoise();
        noise.Seed = rnd.Next();
        noise.Octaves = 1;
        noise.Period = 12;
        Vector2 localPosition = BottomMap.ToLocal(Player.GlobalPosition);
        Vector2 cell = BottomMap.WorldToMap(localPosition);
       
       
        
        

        make_Earn();
        make_void();
        SetStartTile(cell);
        make_Ore(cell);           
        UpdateMap();
    }
    public void SetStartTile(Vector2 cell)
    {
        BottomMap.SetCell((int)cell.x, (int)cell.y, (int)Tile.Trail);
        BottomMap.SetCell((int)cell.x + 1, (int)cell.y, (int)Tile.Trail);
        BottomMap.SetCell((int)cell.x - 1, (int)cell.y, (int)Tile.Trail);
        BottomMap.SetCell((int)cell.x, (int)cell.y + 1, (int)Tile.Trail);
        BottomMap.SetCell((int)cell.x , (int)cell.y - 1, (int)Tile.Trail);
        UpdateMap();
    }
    public void make_Earn()
    {
        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                var a = noise.GetNoise2d(x, y);
                if (a < grass_cap)
                {
                    if ((x >= Weight/2 - SizeFirstZone/2 && x <= Weight / 2 + SizeFirstZone / 2) && (y >= Height / 2 - SizeFirstZone / 2 && y <= Height / 2 + SizeFirstZone / 2))
                        BottomMap.SetCell(x, y, (int)Tile.Earth);
                    else if((x >= ((Weight / 2) -3) - (SizeFirstZone / 2) -3) && ( x <= ((Weight / 2) + 3) + (SizeFirstZone / 2) + 3) && (y >= ((Height / 2) -3) - (SizeFirstZone / 2) -3) && (y <= ((Height / 2) +3) + ((SizeFirstZone / 2) + 3)))
                    {
                        int num = rnd.Next() % 2;
                        if(num == 0)
                            BottomMap.SetCell(x, y, (int)Tile.Earth);
                        else if(num == 1)
                            BottomMap.SetCell(x, y, (int)Tile.Stone);
                    }                   
                    else if ((x>=Weight/2 - SizeSecondZone/2 && x<= Weight / 2 + SizeSecondZone / 2) && (y>= Height/2 - SizeSecondZone/2 && y <= Height / 2 + SizeSecondZone / 2))
                        BottomMap.SetCell(x, y, (int)Tile.Stone);
                    else if ((x >= ((Weight / 2) - 3) - (SizeSecondZone / 2) - 3) && (x <= ((Weight / 2) + 3) + (SizeSecondZone / 2) + 3) && (y >= ((Height / 2) - 3) - (SizeSecondZone / 2) - 3) && (y <= ((Height / 2) + 3) + ((SizeSecondZone / 2) + 3)))
                    {
                        int num = rnd.Next() % 2;
                        if(num == 0)
                            BottomMap.SetCell(x, y, (int)Tile.Stone);
                        else if(num == 1)
                            BottomMap.SetCell(x, y, (int)Tile.SolidStone);
                    }
                    else
                        BottomMap.SetCell(x, y, 1);
                }
                if(x == 1 || x == Weight -1 || y == 1 || y == Height - 1)
                {

                }
            }
        }
        
    }
    public void UpdateMap()
    {
        BottomMap.UpdateBitmaskRegion(new Vector2(0,0), new Vector2(map_size.x, map_size.y));        
    }
    public void UpdateTile(Vector2 cell)
    {
        Vector2 localPosition = BottomMap.ToLocal(Player.GlobalPosition);
        Vector2 MapPosition = BottomMap.WorldToMap(localPosition);
        Vector2 UpdatePosition = new Vector2(MapPosition.x - 3, MapPosition.y - 3);
        BottomMap.UpdateBitmaskRegion(UpdatePosition,cell);        
    }
    public void RemoveTail(Vector2 cell)
    {
        BottomMap.SetCell((int)cell.x, (int)cell.y,(int)Tile.Trail);
        OreMap.SetCell((int)cell.x, (int)cell.y, (int)Tile.Trail);
        TopMap.SetCell((int)cell.x, (int)cell.y, (int)Tile.Trail);
    }
    public void make_void()
    {
        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                var a = noise.GetNoise2d(x, y);
                if (a > grass_cap)
                {
                    BottomMap.SetCell(x, y, (int)Tile.Trail);
                }
            }
        }      
    }
    public void make_Ore(Vector2 Cell)
    {
        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                var a = noise.GetNoise2d(x, y);
                if (a < Ore_caps.x && (a > Ore_caps.y || a < Ore_caps.z) && a < grass_cap)
                {
                    if (x != Cell.x && y != Cell.y && x!=Cell.x + 1 && x!=Cell.x -1 && y != Cell.y +1 && y != Cell.y-1 ) 
                    {
                        var chance = (rnd.Next() % 100);
                        if ((x >= Weight / 2 - SizeFirstZone / 2 && x <= Weight / 2 + SizeFirstZone / 2) && (y >= Height / 2 - SizeFirstZone / 2 && y <= Height / 2 + SizeFirstZone / 2))
                        {

                            if (chance < 1)
                            {
                                //int num = (rnd.Next(1, 20));
                                //if (num == 1)
                                OreMap.SetCell(x, y, (int)Tile.Iron);
                            }
                            else if (chance < 3)
                            {
                                OreMap.SetCell(x, y, (int)Tile.Copper);
                            }
                        }
                        else if ((x >= Weight / 2 - SizeSecondZone / 2 && x <= Weight / 2 + SizeSecondZone / 2) && (y >= Height / 2 - SizeSecondZone / 2 && y <= Height / 2 + SizeSecondZone / 2))
                        {
                            if (chance < 1)
                            {
                                int num = (rnd.Next(1, 20));
                                if (num == 1)
                                    OreMap.SetCell(x, y, (int)Tile.Gold);
                            }
                            else if (chance < 3)
                            {
                                OreMap.SetCell(x, y, (int)Tile.Iron);
                            }
                            else if (chance < 5)
                            {
                                OreMap.SetCell(x, y, (int)Tile.Copper);
                            }
                        }
                        else
                        {
                            if (chance < 1)
                            {
                                OreMap.SetCell(x, y, (int)Tile.Gold);
                            }
                            else if (chance < 2)
                            {
                                OreMap.SetCell(x, y, (int)Tile.Iron);
                            }
                            else if (chance < 3)
                            {
                                OreMap.SetCell(x, y, (int)Tile.Copper);
                            }
                        }
                    }
                }
            }      
        }
    }
}
