using System.Collections.Generic;

public static class BoardTiles
{
    private static readonly Dictionary<int, BoardTileDef> defs;

    private static bool isInited;
    
    public static readonly int WATER_TILE_ID = 10; 
      
    public static Dictionary<int, BoardTileDef> GetDefs()
    {
        if (!isInited)
        {
            Init();
        }

        return defs;
    }
    
    static BoardTiles()
    {
        defs = new Dictionary<int, BoardTileDef>
        {
            {
                -1, new BoardTileDef // Cell outside of border
                {
                    Height = 0,
                    IsLock = true,
                }
            },
            {
                10, new BoardTileDef // Water
                {
                    Height = 0,
                    IsLock = true,
                }
            },
            {
                20, new BoardTileDef // Floor 0 -> 1
                {
                    Height = 0,
                    IsLock = true,
                }
            },
            {
                21, new BoardTileDef // Floor 1 -> 2
                {
                    Height = 1,
                    IsLock = true,
                }
            },

            // HEIGHT 0    
            {
                100, new BoardTileDef
                {
                    Height = 0,
                    IsLock = false,
                    SpriteName = "tile_grass_1",
                    SpriteNameChess = "tile_grass_2"
                }
            },
            {
                101, new BoardTileDef
                {
                    Height = 0,
                    IsLock = false,
                    SpriteName = "tile_sand_1",
                    SpriteNameChess = "tile_sand_2"
                }
            },
            {
                102, new BoardTileDef
                {
                    Height = 0,
                    IsLock = false,
                    SpriteName = "tile_clay_1",
                    SpriteNameChess = "tile_clay_2"
                }
            },            
            {
                103, new BoardTileDef
                {
                    Height = 0,
                    IsLock = false,
                    SpriteName = "tile_foliage_1",
                    SpriteNameChess = "tile_foliage_2"
                }
            },

            // HEIGHT 1  
            {
                200, new BoardTileDef
                {
                    Height = 1,
                    IsLock = false,
                    SpriteName = "tile_grass_1",
                    SpriteNameChess = "tile_grass_2"
                }
            },
            {
                201, new BoardTileDef
                {
                    Height = 1,
                    IsLock = false,
                    SpriteName = "tile_sand_1",
                    SpriteNameChess = "tile_sand_2"
                }
            },
            {
                202, new BoardTileDef
                {
                    Height = 1,
                    IsLock = false,
                    SpriteName = "tile_clay_1",
                    SpriteNameChess = "tile_clay_2"
                }
            },            
            {
                203, new BoardTileDef
                {
                    Height = 1,
                    IsLock = false,
                    SpriteName = "tile_foliage_1",
                    SpriteNameChess = "tile_foliage_2"
                }
            },

            // HEIGHT 2 
            {
                300, new BoardTileDef
                {
                    Height = 2,
                    IsLock = false,
                    SpriteName = "tile_grass_1",
                    SpriteNameChess = "tile_grass_2"
                }
            },
            {
                301, new BoardTileDef
                {
                    Height = 2,
                    IsLock = false,
                    SpriteName = "tile_sand_1",
                    SpriteNameChess = "tile_sand_2"
                }
            },
            {
                302, new BoardTileDef
                {
                    Height = 2,
                    IsLock = false,
                    SpriteName = "tile_clay_1",
                    SpriteNameChess = "tile_clay_2"
                }
            },
            {
                303, new BoardTileDef
                {
                    Height = 2,
                    IsLock = false,
                    SpriteName = "tile_foliage_1",
                    SpriteNameChess = "tile_foliage_2"
                }
            }
        };
    }

    private static void Init()
    {
        var iconManager = IconService.Current;

        foreach (var pair in defs)
        {
            int key = pair.Key;
            BoardTileDef value = pair.Value;

            value.Id = key;
            
            if (value.SpriteName != null)
            {
                value.Sprite = iconManager.GetSpriteById(value.SpriteName);
            }
            
            if (value.SpriteNameChess != null)
            {
                value.SpriteChess = iconManager.GetSpriteById(value.SpriteNameChess);
            }
        }
        
        isInited = true;
    }
}