using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class PathfindIgnoreBuilder
{
   [JsonIgnore]
   private static Dictionary<int, HashSet<int>> dictLinks = InitLinks();

   private static Dictionary<int, HashSet<int>> InitLinks()
   {
      var links = new Dictionary<int, HashSet<int>>();
      
      InitMines(links);
      InitFog(links);
      InitObstacles(links);
      
      return links;
   }

   private static HashSet<int> allPieceTypes;
   private static HashSet<int> GetReverseSet(HashSet<int> nonIgnorableItems)
   {
      if (allPieceTypes == null)
         allPieceTypes = new HashSet<int>(PieceType.Abbreviations.Values.Distinct());
      return new HashSet<int>(allPieceTypes.Except(nonIgnorableItems));
   }

   private static void InitObstacles(Dictionary<int, HashSet<int>> dict)
   {
      var obstacleIgnorePices = GetReverseSet(new HashSet<int>
      {
         PieceType.O1.Id,
         PieceType.O2.Id,
         PieceType.O3.Id,
         PieceType.O4.Id,
         PieceType.O5.Id,
         PieceType.O6.Id,
         PieceType.O7.Id,
         PieceType.O8.Id,
         PieceType.O9.Id,
         PieceType.OX1.Id,
         PieceType.OX2.Id,
         PieceType.OX3.Id,
         PieceType.OX4.Id,
         PieceType.OX5.Id,
         PieceType.OX6.Id,
         PieceType.OX7.Id,
         PieceType.OX8.Id,
         PieceType.OX9.Id,
         PieceType.Fog.Id
      });
      
      dict.Add(PieceType.O1.Id, obstacleIgnorePices);
      dict.Add(PieceType.O2.Id, obstacleIgnorePices);
      dict.Add(PieceType.O3.Id, obstacleIgnorePices);
      dict.Add(PieceType.O4.Id, obstacleIgnorePices);
      dict.Add(PieceType.O5.Id, obstacleIgnorePices);
      dict.Add(PieceType.O6.Id, obstacleIgnorePices);
      dict.Add(PieceType.O7.Id, obstacleIgnorePices);
      dict.Add(PieceType.O8.Id, obstacleIgnorePices);
      dict.Add(PieceType.O9.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX1.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX2.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX3.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX4.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX5.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX6.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX7.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX8.Id, obstacleIgnorePices);
      dict.Add(PieceType.OX9.Id, obstacleIgnorePices);
   }
   
   private static void InitMines(Dictionary<int, HashSet<int>> dict)
   {
      var mineIgnorePieces = GetReverseSet(new HashSet<int>
      {
         PieceType.O1.Id,
         PieceType.O2.Id,
         PieceType.O3.Id,
         PieceType.O4.Id,
         PieceType.O5.Id,
         PieceType.O6.Id,
         PieceType.O7.Id,
         PieceType.O8.Id,
         PieceType.O9.Id,
         PieceType.OX1.Id,
         PieceType.OX2.Id,
         PieceType.OX3.Id,
         PieceType.OX4.Id,
         PieceType.OX5.Id,
         PieceType.OX6.Id,
         PieceType.OX7.Id,
         PieceType.OX8.Id,
         PieceType.OX9.Id,
         PieceType.Fog.Id
      });
      
      dict.Add(PieceType.Mine1.Id, mineIgnorePieces);
      dict.Add(PieceType.Mine2.Id, mineIgnorePieces);
      dict.Add(PieceType.Mine3.Id, mineIgnorePieces);
      dict.Add(PieceType.Mine4.Id, mineIgnorePieces);
      dict.Add(PieceType.Mine5.Id, mineIgnorePieces);
      dict.Add(PieceType.Mine6.Id, mineIgnorePieces);
      dict.Add(PieceType.Mine7.Id, mineIgnorePieces);
      dict.Add(PieceType.MineC.Id, mineIgnorePieces);
      dict.Add(PieceType.MineX.Id, mineIgnorePieces);
      dict.Add(PieceType.MineY.Id, mineIgnorePieces);
   }

   private static void InitFog(Dictionary<int, HashSet<int>> dict)
   {
      dict.Add(PieceType.Fog.Id, GetReverseSet(new HashSet<int>
      {
         PieceType.Fog.Id,
      }));
   }

   public static PathfindIgnoreComponent Build(int pieceTypeId)
   {
      if(dictLinks.ContainsKey(pieceTypeId))
         return new PathfindIgnoreComponent(dictLinks[pieceTypeId]);
      
      Debug.LogWarning(string.Format("[PathfindIgnoreBuilder] id {0} not initialized", pieceTypeId));
      return PathfindIgnoreComponent.Empty;
   }
}
