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
        dict = AddResourcePiece(dict);
        dict = AddOtherPiece(dict);
        dict = AddSawmillPiece(dict);
        dict = AddMinePiece(dict);
        dict = AddHeroPiece(dict);
        dict = AddCastlePiece(dict);
        dict = AddTavernPiece(dict);

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
    
    private Dictionary<int, IPieceBuilder> AddResourcePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.C1.Id, new ResourcePieceBuilder());
        
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
        dict.Add(PieceType.S1.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Right,
            }
        });
        
        dict.Add(PieceType.S2.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Right,
            }
        });
        
        dict.Add(PieceType.S3.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Right,
            }
        });
        
        dict.Add(PieceType.S4.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Right,
            }
        });
        
        dict.Add(PieceType.S5.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Right,
            }
        });
        
        dict.Add(PieceType.S6.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Right,
            }
        });
        
        dict.Add(PieceType.S7.Id, new MulticellularSpawnPieceBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Right,
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
    
    private Dictionary<int, IPieceBuilder> AddHeroPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.H1.Id, new HeroHouseBuilder
        {
            Mask = new List<BoardPosition>
            {
                BoardPosition.Zero().Up,
                BoardPosition.Zero().Right,
                BoardPosition.Zero().Right.Up,
            }
        });
        
        dict.Add(PieceType.H2.Id, new HeroHouseBuilder
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
    
    private Dictionary<int, IPieceBuilder> AddCastlePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Castle1.Id, new HeroHouseBuilder
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
    
    private Dictionary<int, IPieceBuilder> AddTavernPiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict.Add(PieceType.Tavern1.Id, new HeroHouseBuilder
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
}