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
      InitCharacters(links);
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
         PieceType.OB1_A.Id,
         PieceType.OB2_A.Id,
         PieceType.OB3_A.Id,
         PieceType.OB4_A.Id,
         PieceType.OB5_A.Id,
         PieceType.OB6_A.Id,
         PieceType.OB7_A.Id,
         PieceType.OB8_A.Id,
         PieceType.OB9_A.Id,
         PieceType.Fog.Id
      });
      
      dict.Add(PieceType.OB1_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB2_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB3_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB4_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB5_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB6_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB7_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB8_A.Id, obstacleIgnorePices);
      dict.Add(PieceType.OB9_A.Id, obstacleIgnorePices);
   }
   
   private static void InitMines(Dictionary<int, HashSet<int>> dict)
   {
      var mineIgnorePieces = GetReverseSet(new HashSet<int>
      {
         PieceType.OB1_A.Id,
         PieceType.OB2_A.Id,
         PieceType.OB3_A.Id,
         PieceType.OB4_A.Id,
         PieceType.OB5_A.Id,
         PieceType.OB6_A.Id,
         PieceType.OB7_A.Id,
         PieceType.OB8_A.Id,
         PieceType.OB9_A.Id,
         PieceType.Fog.Id
      });
      
      dict.Add(PieceType.MN_B.Id, mineIgnorePieces);
      dict.Add(PieceType.MN_C.Id, mineIgnorePieces);
      dict.Add(PieceType.MN_D.Id, mineIgnorePieces);
   }

   private static void InitCharacters(Dictionary<int, HashSet<int>> dict)
   {
      var charIgnorePieces = GetReverseSet(new HashSet<int>
      {
         PieceType.OB1_A.Id,
         PieceType.OB2_A.Id,
         PieceType.OB3_A.Id,
         PieceType.OB4_A.Id,
         PieceType.OB5_A.Id,
         PieceType.OB6_A.Id,
         PieceType.OB7_A.Id,
         PieceType.OB8_A.Id,
         PieceType.OB9_A.Id,
         PieceType.Fog.Id
      });
      
      dict.Add(PieceType.NPC_SleepingBeauty.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_Rapunzel.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_PussInBoots.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_4.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_5.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_6.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_7.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_8.Id, charIgnorePieces);
      dict.Add(PieceType.NPC_9.Id, charIgnorePieces);
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
      
      Debug.LogWarning($"[PathfindIgnoreBuilder] id {pieceTypeId} not initialized");
      return PathfindIgnoreComponent.Empty;
   }
}
