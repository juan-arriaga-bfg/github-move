using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IW;
using UnityEngine;
using BFGSDK;

public static class AbTestName
{
    public static readonly string DAILY_REWARD = "daily_reward";
    public static readonly string SHOP_PRICE = "shop_price";
}

public class AbTestDataManager : ECSEntity, IDataManager
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private GameDataManager context;

    private string currentGroup;

    public const string GROUPS = "abcdefghijklmnop";
    
    public Dictionary<string, AbTestItem> Tests {get; private set; }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = (GameDataManager) entity;
        
        Reload();
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }
    
    public void Reload()
    {
        var save = context.UserProfile.GetComponent<AbTestSaveComponent>(AbTestSaveComponent.ComponentGuid);
        
        Tests = new Dictionary<string, AbTestItem>
        {
            {AbTestName.DAILY_REWARD, new AbTestItem {GroupsCount = 3}},
            {AbTestName.SHOP_PRICE, new AbTestItem {GroupsCount = 3}},
        };

        foreach (var pair in Tests)
        {
            string testName = pair.Key;
            
            pair.Value.TestName = testName;

            AbTestItem savedData = save?.Tests?.FirstOrDefault(e => e.TestName == testName);
            
            var group = savedData != null 
                ? savedData.UserGroup 
                : GenerateTestGroup(pair.Value.GroupsCount);
                    
            // if something went wrong with ids, disable tests!
            if (string.IsNullOrEmpty(group))
            {
                IW.Logger.LogError($"[AbTestController] => Reload: group is null or empty for test '{testName}'. All tests will be disabled!");
                
                Tests.Clear();
                break;
            }
                    
            pair.Value.UserGroup = group;

            IW.Logger.Log($"[AbTestController] => Reload: Set group '{group}' for test '{testName}");
        }
    }

    public void ForceSetGroup(string testName, string group)
    {
        AbTestItem item = Tests.Values.FirstOrDefault(e => e.TestName == testName);
        if (item == null)
        {
            IW.Logger.LogError($"[AbTestController] => ForceSetGroupForTest: test '{testName} not found");
            return;
        }

        if (!GROUPS.Contains(group))
        {
            IW.Logger.LogError($"[AbTestController] => ForceSetGroupForTest: Wrong group name  '{testName} not found");
        }

        IW.Logger.Log($"[AbTestController] => ForceSetGroupForTest: Set '{group}' group for '{testName}'"); 
        item.UserGroup = group;
    }
    
    private string GetUserId()
    {
#if UNITY_EDITOR
        return context.UserProfile.ClientID; 
#else 
        return bfgRave.currentRaveId();
#endif
    }
    
    private string GenerateTestGroup(int groupsCount)
    {
        IW.Logger.Log("[AbTestController] => GenerateTestGroup: groupsCount: " + groupsCount);

        if (groupsCount <= 0)
        {
            IW.Logger.LogError("[AbTestController] => GenerateTestGroup: groupsCount <= 0");
            return null; 
        }

        var id = GetUserId();  
        IW.Logger.Log("[AbTestController] => GenerateTestGroup: userID: " + id);

        if (string.IsNullOrEmpty(id))
        {
            IW.Logger.LogError("[AbTestController] => GenerateTestGroup: SocialController.Instance.UserId IsNullOrEmpty");
            return null;
        }   

        char lastChar = id[id.Length - 1];
        
        // Convert the number expressed in base-16 to an integer.
        int value;
        try
        {
            value = Convert.ToInt32(lastChar.ToString(), 16);
        }
        catch (Exception e)
        {
            #if UNITY_EDITOR
                value = 0;
                IW.Logger.LogWarningFormat("[AbTestController] => GenerateTestGroup: Can't convert last char of id '{0}' to HEX value. Force it to 0 [UNITY EDITOR ONLY].", id);
            #else
                IW.Logger.LogWarningFormat("[AbTestController] => GenerateTestGroup: Can't convert last char of id '{0}' to HEX value. Return null", id);
                return null;
            #endif
        }

        // 16 не делится на требуемое количество групп без остатка
        if (16 % groupsCount != 0) 
        {
            int maxAllowedGroup = (16 / groupsCount) * groupsCount;
            if (value > maxAllowedGroup - 1)
            {
                var randomGroup = RandomString.Get(1, GROUPS.Substring(0, groupsCount));

                IW.Logger.Log($"[AbTestController] => GenerateTestGroup: return RANDOM: '{randomGroup}'");
                
                return randomGroup;
            }
        }
        
        int grouplen = 16 / groupsCount;
        StringBuilder groupsSb = new StringBuilder();
        int groupIndex = 0;
        while (groupsSb.Length < 16)
        {
            for (int i = 0; i < grouplen; i++)
            {
                groupsSb.Append(GROUPS[groupIndex]);
            }

            groupIndex++;
        }

        var group = groupsSb[value].ToString();

        IW.Logger.Log($"[AbTestController] => GenerateTestGroup: return: '{group}'");
        
        return group;
    }
}