using System.Collections.Generic;

public class PieceBuildersBuilder
{
    public Dictionary<int, IPieceBuilder> Build()
    {
        var dict = new Dictionary<int, IPieceBuilder>();

        dict = AddSimplePiece(dict);
        dict = AddEnemyPiece(dict);
        dict = AddObstaclePiece(dict);
        dict = AddOtherPiece(dict);
        
        dict = AddMinePiece(dict);
        dict = AddSawmillPiece(dict);
        dict = AddSheepfoldPiece(dict);
        
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
        dict.Add(PieceType.A8.Id, new SimplePieceBuilder());
        
        dict.Add(PieceType.B1.Id, new SimplePieceBuilder());
        dict.Add(PieceType.B2.Id, new SimplePieceBuilder());
        dict.Add(PieceType.B3.Id, new SimplePieceBuilder());
        dict.Add(PieceType.B4.Id, new SimplePieceBuilder());
        dict.Add(PieceType.B5.Id, new SimplePieceBuilder());
        
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
        dict.Add(PieceType.O2.Id, new GenericPieceBuilder());
        dict.Add(PieceType.O3.Id, new GenericPieceBuilder());
        dict.Add(PieceType.O4.Id, new ObstaclePieceBuilder());
        
        var mask = new List<BoardPosition>();
        var dot = BoardPosition.Zero();
        
        mask.AddRange(GetLine(dot, 5));

        for (int i = 1; i < 5; i++)
        {
            dot = dot.Right;
            mask.AddRange(GetLine(dot, 5));
        }
        
        dict.Add(PieceType.Fog.Id, new FogPieceBuilder
        {
            Mask = mask
        });
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddOtherPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Generic.Id, new GenericPieceBuilder());
        dict.Add(PieceType.Empty.Id, new EmptyPieceBuilder());
        dict.Add(PieceType.Gbox1.Id, new GBoxPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddMinePiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = new List<BoardPosition>
        {
            BoardPosition.Zero().Up,
            BoardPosition.Zero().Right,
            BoardPosition.Zero().Right.Up,
        };
        
        dict.Add(PieceType.Mine1.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Mine2.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Mine3.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Mine4.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Mine5.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Mine6.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Mine7.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSawmillPiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = new List<BoardPosition>
        {
            BoardPosition.Zero().Up,
            BoardPosition.Zero().Right,
            BoardPosition.Zero().Right.Up,
        };
        
        dict.Add(PieceType.Sawmill1.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sawmill2.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sawmill3.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sawmill4.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sawmill5.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sawmill6.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sawmill7.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSheepfoldPiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = new List<BoardPosition>
        {
            BoardPosition.Zero().Up,
            BoardPosition.Zero().Right,
            BoardPosition.Zero().Right.Up,
        };
        
        dict.Add(PieceType.Sheepfold1.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sheepfold2.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sheepfold3.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sheepfold4.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sheepfold5.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sheepfold6.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Sheepfold7.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = mask
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
        
        dict.Add(PieceType.Castle1.Id, new CastlePieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle2.Id, new CastlePieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle3.Id, new CastlePieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle4.Id, new CastlePieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle5.Id, new CastlePieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle6.Id, new CastlePieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle7.Id, new CastlePieceBuilder
        {
            Mask = mask
        });
        
        dict.Add(PieceType.Castle8.Id, new CastlePieceBuilder
        {
            Mask = mask
        });

        dict.Add(PieceType.Castle9.Id, new CastlePieceBuilder
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

    private List<BoardPosition> GetLine(BoardPosition dot, int length)
    {
        var line = new List<BoardPosition> {dot};
        
        for (int i = 1; i < length; i++)
        {
            dot = dot.Up;
            
            line.Add(dot);
        }

        return line;
    }
}