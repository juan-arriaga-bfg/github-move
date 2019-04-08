using System.Collections.Generic;
using System.Linq;

public static class PathfindIgnores
{
   private static Dictionary<int, HashSet<int>> dictLinks = InitLinks();
   private static HashSet<int> defaultIgnores = InitDefault();

   private static HashSet<int> allPieceTypes;
   
   private static HashSet<int> InitDefault()
   {
      var hashSet = new HashSet<int>() {PieceType.Fog.Id};
      var obstacles = PieceType.GetIdsByFilter(PieceTypeFilter.Obstacle);
      foreach (var obst in obstacles)
      {
         if (PieceType.GetDefById(obst).Filter.HasFlag(PieceTypeFilter.ProductionField) == false)
         {
            hashSet.Add(obst);
         }
      }

      var ignorable = GetReverseSet(hashSet);
      
      return ignorable;
   }
   
   private static Dictionary<int, HashSet<int>> InitLinks()
   {
      var links = new Dictionary<int, HashSet<int>>(); 
      
      InitFog(links);
      return links;
   }

   private static HashSet<int> GetReverseSet(HashSet<int> nonIgnorableItems)
   {
      if (allPieceTypes == null)
      {
         allPieceTypes = new HashSet<int>(PieceType.Abbreviations.Values.Distinct());
      }
         
      return new HashSet<int>(allPieceTypes.Except(nonIgnorableItems));
   }

   private static void InitFog(Dictionary<int, HashSet<int>> dict)
   {
      dict.Add(PieceType.Fog.Id, GetReverseSet(new HashSet<int>
      {
         PieceType.Fog.Id,
      }));
   }

   public static HashSet<int> GetIgnoreList(int pieceTypeId)
   {
      if (dictLinks.ContainsKey(pieceTypeId))
      {
         return dictLinks[pieceTypeId];
      }
      
      return defaultIgnores;
   }
}
