using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBuildersBuilder
{
    public Dictionary<int, IPieceBuilder> Build()
    {
        var dict = new Dictionary<int, IPieceBuilder>();

        dict = AddSimplePiece(dict);
        dict = AddEnemyPiece(dict);
        dict = AddObstaclePiece(dict);
        dict = AddOtherPiece(dict);
        dict = AddSawmillPiece(dict);
        dict = AddMinePiece(dict);
        dict = AddCastlePiece(dict);
        dict = AddTavernPiece(dict);
        dict = AddChestPiece(dict);

        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSimplePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.A1.Id, new SimplePieceBuilder());
        dict.Add(PieceType.A2.Id, new SimplePieceBuilder());
        dict.Add(PieceType.A3.Id, new SimplePieceBuilder());
        dict.Add(PieceType.A4.Id, new SimplePieceBuilder());
        dict.Add(PieceType.A5.Id, new SimplePieceBuilder());
        dict.Add(PieceType.A6.Id, new SimplePieceBuilder());
        dict.Add(PieceType.A7.Id, new SimplePieceBuilder());
        
        dict.Add(PieceType.B1.Id, new SimplePieceBuilder());
        dict.Add(PieceType.B2.Id, new SimplePieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddEnemyPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.E1.Id, new EnemyPieceBuilder());
        dict.Add(PieceType.E2.Id, new EnemyPieceBuilder());
        dict.Add(PieceType.E3.Id, new EnemyPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddObstaclePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.O1.Id, new GenericPieceBuilder());
        dict.Add(PieceType.O2.Id, new ObstaclePieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddOtherPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Generic.Id, new GenericPieceBuilder());
        dict.Add(PieceType.Empty.Id, new EmptyPieceBuilder());
        dict.Add(PieceType.Gbox1.Id, new GBoxPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSawmillPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Sawmill1.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Sawmill2.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Sawmill3.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Sawmill4.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Sawmill5.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Sawmill6.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Sawmill7.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddMinePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.M1.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
            }
        });
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddCastlePiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = new List<BoardPosition>
        {
            BoardPosition.Zero().Up,
            BoardPosition.Zero().Up.Up,
            BoardPosition.Zero().Right,
            BoardPosition.Zero().Right.Up,
            BoardPosition.Zero().Right.Up.Up,
            BoardPosition.Zero().Right.Right,
            BoardPosition.Zero().Right.Right.Up,
            BoardPosition.Zero().Right.Right.Up.Up,
        };
        
        dict.Add(PieceType.Castle1.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle2.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle3.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle4.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle5.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle6.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle7.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle8.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });

        dict.Add(PieceType.Castle9.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddTavernPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Tavern1.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern2.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern3.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern4.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern5.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern6.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern7.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern8.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.Tavern9.Id, new TavernPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddChestPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Chest1.Id, new ChestPieceBuilder());
        dict.Add(PieceType.Chest2.Id, new ChestPieceBuilder());
        dict.Add(PieceType.Chest3.Id, new ChestPieceBuilder());
        
        return dict;
    }
}