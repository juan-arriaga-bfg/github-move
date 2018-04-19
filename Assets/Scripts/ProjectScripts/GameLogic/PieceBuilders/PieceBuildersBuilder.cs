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
        
        dict = AddMinePiece(dict);
        dict = AddSawmillPiece(dict);
        dict = AddSheepfoldPiece(dict);
        
        dict = AddCastlePiece(dict);
//        dict = AddTavernPiece(dict);
        dict = AddChestPiece(dict);

        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddSimplePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.A1.Id, PieceType.A9.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.B1.Id, PieceType.B5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.C1.Id, PieceType.C9.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.D1.Id, PieceType.D5.Id, dict);
        dict = AddSimplePiece<SimplePieceBuilder>(PieceType.E1.Id, PieceType.E5.Id, dict);
        dict.Add(PieceType.E6.Id, new SpawnPieceBuilder());
        
        return dict;
    }
    
    private Dictionary<int, IPieceBuilder> AddEnemyPiece(Dictionary<int, IPieceBuilder> dict)
    {
        return AddSimplePiece<EnemyPieceBuilder>(PieceType.Enemy1.Id, PieceType.Enemy3.Id, dict);
    }
    
    private Dictionary<int, IPieceBuilder> AddObstaclePiece(Dictionary<int, IPieceBuilder> dict)
    {
        dict = AddSimplePiece<SimpleObstaclePieceBuilder>(PieceType.O1.Id, PieceType.O5.Id, dict);
        dict = AddSimplePiece<SimpleObstaclePieceBuilder>(PieceType.OX1.Id, PieceType.OX5.Id, dict);
        
//        dict.Add(PieceType.O3.Id, new GenericPieceBuilder());
        
        
        dict.Add(PieceType.Quest.Id, new ObstaclePieceBuilder());
        
        dict.Add(PieceType.Fog.Id, new FogPieceBuilder
        {
            Mask = GetRect(5, 5)
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
        
        return AddMulticellularPiece<MulticellularSpawnPieceBuilder>(PieceType.Mine1.Id, PieceType.Mine7.Id, mask, dict);
    }
    
    private Dictionary<int, IPieceBuilder> AddSawmillPiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = new List<BoardPosition>
        {
            BoardPosition.Zero().Up,
            BoardPosition.Zero().Right,
            BoardPosition.Zero().Right.Up,
        };
        
        return AddMulticellularPiece<MulticellularSpawnPieceBuilder>(PieceType.Sawmill1.Id, PieceType.Sawmill7.Id, mask, dict);
    }
    
    private Dictionary<int, IPieceBuilder> AddSheepfoldPiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = new List<BoardPosition>
        {
            BoardPosition.Zero().Up,
            BoardPosition.Zero().Right,
            BoardPosition.Zero().Right.Up,
        };
        
        return AddMulticellularPiece<MulticellularSpawnPieceBuilder>(PieceType.Sheepfold1.Id, PieceType.Sheepfold7.Id, mask, dict);
    }
    
    private Dictionary<int, IPieceBuilder> AddCastlePiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = GetRect(3, 3);
        
        return AddMulticellularPiece<CastlePieceBuilder>(PieceType.Castle1.Id, PieceType.Castle9.Id, mask, dict);
    }
    
    private Dictionary<int, IPieceBuilder> AddTavernPiece(Dictionary<int, IPieceBuilder> dict)
    {
        var mask = new List<BoardPosition>
        {
            BoardPosition.Zero().Up,
            BoardPosition.Zero().Right,
            BoardPosition.Zero().Right.Up,
        };
        
        return AddMulticellularPiece<TavernPieceBuilder>(PieceType.Tavern1.Id, PieceType.Tavern9.Id, mask, dict);
    }
    
    private Dictionary<int, IPieceBuilder> AddChestPiece(Dictionary<int, IPieceBuilder> dict)
    {
        return AddSimplePiece<ChestPieceBuilder>(PieceType.Chest1.Id, PieceType.Chest9.Id, dict);
    }

    private Dictionary<int, IPieceBuilder> AddSimplePiece<T>(int idMin, int idMax, Dictionary<int, IPieceBuilder> dict) where T : IPieceBuilder, new()
    {
        for (var id = idMin; id < idMax + 1; id++)
        {
            dict.Add(id, new T());
        }
        
        return dict;
    }

    private Dictionary<int, IPieceBuilder> AddMulticellularPiece<T>(int idMin, int idMax, List<BoardPosition> mask, Dictionary<int, IPieceBuilder> dict) where T : MulticellularPieceBuilder, new()
    {
        for (var id = idMin; id < idMax + 1; id++)
        {
            dict.Add(id, new T {Mask = mask});
        }
        
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

    private List<BoardPosition> GetRect(int width, int height)
    {
        var mask = new List<BoardPosition>();
        var dot = BoardPosition.Zero();
        
        mask.AddRange(GetLine(dot, width));

        for (int i = 1; i < height; i++)
        {
            dot = dot.Right;
            mask.AddRange(GetLine(dot, width));
        }

        return mask;
    }
}