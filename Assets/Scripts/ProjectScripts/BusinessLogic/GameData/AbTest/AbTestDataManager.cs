using System;
using System.Collections.Generic;
using System.Text;
using IW;
using UnityEngine;


public class AbTestDef
{
    public int GroupsCount;
    public string UserGroup;
    public string TestName;
}

public static class AbTestName
{
    public static readonly string DAILY_REWARD = "daily_reward";
}

public class AbTestDataManager : ECSEntity
{
    public static int ComponentGuid = ECSManager.GetNextGuid();
    public override int Guid => ComponentGuid;

    private ECSEntity context;

    private string currentGroup;

    private Dictionary<string, AbTestDef> tests;
    public Dictionary<string, AbTestDef> Tests
    {
        get
        {
            if (tests == null)
            {
                tests = new Dictionary<string, AbTestDef>
                {
                    {AbTestName.DAILY_REWARD, new AbTestDef {GroupsCount = 3}},
                };

                foreach (var pair in tests)
                {
                    pair.Value.TestName = pair.Key;
                    string group = GenerateTestGroup(pair.Value.GroupsCount);
                    
                    // if something went wrong with ids, disable tests!
                    if (string.IsNullOrEmpty(group))
                    {
                        tests.Clear();
                        break;
                    }
                    
                    pair.Value.UserGroup = group;
                }

            }

            return tests;
        }
    }

    public override void OnRegisterEntity(ECSEntity entity)
    {
        context = entity;
    }

    public override void OnUnRegisterEntity(ECSEntity entity)
    {
        context = null;
    }

    private string GetUserId()
    {
#if UNITY_EDITOR
        return ProfileService.Current.ClientID; 
#else 
        return bfgRave.currentRaveId();
#endif
    }
    
    private string GenerateTestGroup(int groupsCount)
    {
        IW.Logger.Log("[AbTestController] => GenerateTestGroup: groupsCount: " + groupsCount);

        const string GROUPS = "abcdefghijklmnop";
        
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