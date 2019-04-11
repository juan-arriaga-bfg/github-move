using System.Collections.Generic;

public static class BoardTiles
{
    private static readonly Dictionary<int, BoardTileDef> defs;

    private static bool isInited;
    
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
                100, new BoardTileDef // Water
                {
                    Height = 0,
                    IsLock = true,
                }
            },
            {
                200, new BoardTileDef // Border
                {
                    Height = 0,
                    IsLock = true,
                }
            },

            // HEIGHT 0    

            {
                1000, new BoardTileDef
                {
                    Height = 0,
                    IsLock = false,
                    SpriteName = "tile_grass_1",
                    SpriteNameChess = "tile_grass_2"
                }
            },
            {
                1001, new BoardTileDef
                {
                    Height = 0,
                    IsLock = false,
                    SpriteName = "tile_sand_1",
                    SpriteNameChess = "tile_sand_2"
                }
            },
            {
                1002, new BoardTileDef
                {
                    Height = 0,
                    IsLock = false,
                    SpriteName = "tile_clay_1",
                    SpriteNameChess = "tile_clay_2"
                }
            },

            // HEIGHT 1  
            {
                2000, new BoardTileDef
                {
                    Height = 1,
                    IsLock = false,
                    SpriteName = "tile_grass_1",
                    SpriteNameChess = "tile_grass_2"
                }
            },
            {
                2001, new BoardTileDef
                {
                    Height = 1,
                    IsLock = false,
                    SpriteName = "tile_sand_1",
                    SpriteNameChess = "tile_sand_2"
                }
            },
            {
                2002, new BoardTileDef
                {
                    Height = 1,
                    IsLock = false,
                    SpriteName = "tile_clay_1",
                    SpriteNameChess = "tile_clay_2"
                }
            }

            // HEIGHT 2
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