using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace RAVESDK
{
    public enum RaveConnectFriendsControllerState
    {
        RaveFriendsControllerStateNotDownloaded,
        RaveFriendsControllerStateDownloading,
        RaveFriendsControllerStateDownloaded,
    };

    public enum RaveConnectControllerState
    {
        RaveConnectControllerStateDisabled,
        RaveConnectControllerStateConnected,
        RaveConnectControllerStateConnecting,
        RaveConnectControllerStateDisconnected,
        RaveConnectControllerStateDisconnecting,
    };

    public class RaveObject
    {
        public static string[] delimiter = new string[] { "<RaveObject>" };
        private static string[] stringListDelimiter = new string[] { "<ListRaveString>" };
        private static string[] stringDictDelimiter = new string[] { "<DictRaveString>" };
        private const float ERROR_FLOAT = Mathf.NegativeInfinity;

        // Serialize and deserialize virtual methods
        public virtual string Serialize()
        {
            return "";
        }

        public virtual void Deserialize(string item)
        {
        }

        // Serialize list generic for RaveObject types
        public static string SerializeList<T>(List<T> list) where T : RaveObject
        {
            if (list == null)
                return "NULL";

            string seperator = RaveObject.delimiter[0].Replace("RaveObject", "List" + typeof(T).Name);
            string data = "";

            foreach (T item in list)
            {
                data = SafeAppend(data, item.Serialize(), seperator);
            }
            // Strip off final separator if non-empty list
            if (data.Length > 0)
            {
                return data.Substring(0, data.Length - seperator.Length);
            }
            else
            {
                return data;
            }
        }

        // Serialize list of raw strings
        public static string SerializeList(List<string> list)
        {
            if (list == null)
                return "NULL";

            string seperator = stringListDelimiter[0];
            string data = "";

            foreach (string item in list)
            {
                data = SafeAppend(data, item, seperator);
            }
            // Strip off final separator if non-empty list
            if (data.Length > 0)
            {
                return data.Substring(0, data.Length - seperator.Length);
            }
            else
            {
                return data;
            }
        }

        // Serialize a string,string dictionary
        public static string SerializeDict(Dictionary<string, string> dict)
        {
            string seperator = stringDictDelimiter[0];
            string data = "";

            foreach (KeyValuePair<string, string> item in dict)
            {
                data = SafeAppend(data, item.Key, seperator);
                data = SafeAppend(data, item.Value, seperator);
            }
            // Strip off final separator if non-empty list
            if (data.Length > 0)
            {
                return data.Substring(0, data.Length - seperator.Length);
            }
            else
            {
                return data;
            }
        }

        // Deserialize list generic for RaveObject types
        public static List<T> DeserializeList<T>(string listString) where T : RaveObject, new()
        {
            List<T> list = new List<T>();
            if (listString == "NULL" || listString == null || listString == "")
            {
                return list;
            }
            string[] seperator = { RaveObject.delimiter[0].Replace("RaveObject", "List" + typeof(T).Name) };
            // Object Delimiter <RaveObject> becomes list delimtere <List RaveObject>
            string[] data;
            data = listString.Split(seperator, StringSplitOptions.None);
            foreach (string item in data)
            {
                if (item.Length > 0)
                {
                    T obj = new T();
                    obj.Deserialize(item);
                    list.Add(obj);
                }
            }
            return list;
        }

        // Deserialize list for raw strings
        public static List<string> DeserializeList(string listString)
        {
            List<string> list = new List<string>();
            string[] seperator = stringListDelimiter;
            string[] data;

            data = SafeSplit(listString, seperator);
            foreach (string item in data)
            {
                if (item.Length > 0)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        // Deserialize string,string dictionaries
        public static Dictionary<string, string> DeserializeDict(string dictString)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] seperator = stringDictDelimiter;
            string[] data;

            data = SafeSplit(dictString, seperator);
            for (int i = 1; i < data.Length; i += 2)
            {
                dict[data[i - 1]] = data[i];
            }
            return dict;
        }

        public static string SafeAppend(string s, string toAppend, string delim)
        {
            if (toAppend != null)
            {
                s = s + (toAppend + delim);
            }
            else
            {
                s = s + ("NULL" + delim);
            }
            return s;
        }

        public static string[] SafeSplit(string s, string[] delim)
        {
            string[] res = s.Split(delim, StringSplitOptions.None);
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] == "NULL")
                {
                    res[i] = null;
                }
            }
            return res;
        }

        public static float SafeConvertFloat(string s)
        {
            if (s == "NULL" || s == "")
                return ERROR_FLOAT;
            else
                return Convert.ToSingle(s);
        }

        public static bool StringToBool(string s)
        {
            return s == "True" || s == "true";
        }

        public static string BoolToString(bool b)
        {
            if (b)
                return "True";
            else
                return "False";
        }

        public static string StringListToString(List<string> strings, string delim)
        {
            String output = "";
            foreach (string s in strings)
            {
                output += s + delim;
            }
            return output;
        }
    }

    public class RaveUser : RaveObject
    {
        new public static string[] delimiter = { "<RaveUser>" };
        public bool isGuest;
        public string displayName;
        public string realName;
        public string username;
        public string email;
        public string birthdate;
        public string raveId;
        public string facebookId;
        public string googlePlusId;
        public string thirdPartyId;
        public string gender;
        public string pictureURL;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, BoolToString(isGuest), delimiter[0]);
            s = SafeAppend(s, displayName, delimiter[0]);
            s = SafeAppend(s, realName, delimiter[0]);
            s = SafeAppend(s, username, delimiter[0]);
            s = SafeAppend(s, email, delimiter[0]);
            s = SafeAppend(s, birthdate, delimiter[0]);
            s = SafeAppend(s, raveId, delimiter[0]);
            s = SafeAppend(s, facebookId, delimiter[0]);
            s = SafeAppend(s, googlePlusId, delimiter[0]);
            s = SafeAppend(s, thirdPartyId, delimiter[0]);
            s = SafeAppend(s, gender, delimiter[0]);
            s = SafeAppend(s, pictureURL, delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            isGuest = StringToBool(parts[0]);
            displayName = parts[1];
            realName = parts[2];
            username = parts[3];
            email = parts[4];
            birthdate = parts[5];
            raveId = parts[6];
            facebookId = parts[7];
            googlePlusId = parts[8];
            thirdPartyId = parts[9];
            gender = parts[10];
            pictureURL = parts[11];
        }
    }

    public class RaveMergeUser : RaveUser
    {
        public string selectedAppDataKey;

        override public void Deserialize(string s)
        {
            base.Deserialize(s);

            string[] parts = SafeSplit(s, delimiter);
            selectedAppDataKey = parts[parts.Length - 1];
        }
    }

    public class RaveUserChanges : RaveObject
    {
        new public static string[] delimiter = { "<RaveUserChanges>" };
        public string displayName;
        public string realName;
        public string email;
        public string birthdate;
        public string gender;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, displayName, delimiter[0]);
            s = SafeAppend(s, realName, delimiter[0]);
            s = SafeAppend(s, email, delimiter[0]);
            s = SafeAppend(s, birthdate, delimiter[0]);
            s = SafeAppend(s, gender, delimiter[0]);

            return s;
        }
    }

    public class RaveGiftType : RaveObject
    {
        new public static string[] delimiter = { "<RaveGiftType>" };
        public string typeId;
        public string typeKey;
        public string name;
        public bool canRequest;
        public bool canGift;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, typeId, delimiter[0]);
            s = SafeAppend(s, typeKey, delimiter[0]);
            s = SafeAppend(s, name, delimiter[0]);
            s = SafeAppend(s, BoolToString(canRequest), delimiter[0]);
            s = SafeAppend(s, BoolToString(canGift), delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            typeId = parts[0];
            typeKey = parts[1];
            name = parts[2];
            canRequest = StringToBool(parts[3]);
            canGift = StringToBool(parts[4]);

        }
    }

    public class RaveGift : RaveObject
    {
        new public static string[] delimiter = { "<RaveGift>" };
        public string giftId;
        public string giftTypeKey;
        public string source;
        public bool isFromGift;
        public bool isFromRequest;
        public string timeSent;
        public RaveGiftType giftType;
        public RaveUser sender;
        public string senderRaveId;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, giftId, delimiter[0]);
            s = SafeAppend(s, giftTypeKey, delimiter[0]);
            s = SafeAppend(s, source, delimiter[0]);
            s = SafeAppend(s, BoolToString(isFromGift), delimiter[0]);
            s = SafeAppend(s, BoolToString(isFromRequest), delimiter[0]);
            s = SafeAppend(s, timeSent, delimiter[0]);
            s = SafeAppend(s, giftType.Serialize(), delimiter[0]);
            s = SafeAppend(s, sender.Serialize(), delimiter[0]);
            s = SafeAppend(s, senderRaveId, delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            giftId = parts[0];
            giftTypeKey = parts[1];
            source = parts[2];
            isFromGift = StringToBool(parts[3]);
            isFromRequest = StringToBool(parts[4]);
            timeSent = parts[5];
            giftType = new RaveGiftType();
            giftType.Deserialize(parts[6]);
            if (parts[7] != null)
            {
                sender = new RaveUser();
                sender.Deserialize(parts[7]);
            }
            else
            {
                sender = null;
            }
            senderRaveId = parts[8];

        }
    }

    public class RaveGiftRequest : RaveObject
    {
        new public static string[] delimiter = { "<RaveGiftRequest>" };
        public string requestId;
        public RaveGiftType giftType;
        public string giftTypeKey;
        public RaveUser requester;
        public string timeSent;
        public string requesterRaveId;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, requestId, delimiter[0]);
            s = SafeAppend(s, giftType.Serialize(), delimiter[0]);
            s = SafeAppend(s, giftTypeKey, delimiter[0]);
            s = SafeAppend(s, requester.Serialize(), delimiter[0]);
            s = SafeAppend(s, timeSent, delimiter[0]);
            s = SafeAppend(s, requesterRaveId, delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            requestId = parts[0];

            if (parts[1] != null)
            {
                giftType = new RaveGiftType();
                giftType.Deserialize(parts[1]);
            }
            else
            {
                giftType = null;
            }

            giftTypeKey = parts[2];

            if (parts[3] != null)
            {
                requester = new RaveUser();
                requester.Deserialize(parts[3]);
            }
            else
            {
                requester = null;
            }
            timeSent = parts[4];
            requesterRaveId = parts[5];

        }
    }

    public class RaveContactGiftResult : RaveObject
    {
        new public static string[] delimiter = { "<RaveContactGiftResult>" };
        public string pluginKeyName;
        public List<String> externalIds;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, pluginKeyName, delimiter[0]);
            s = SafeAppend(s, RaveObject.SerializeList(externalIds), delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            pluginKeyName = parts[0];
            externalIds = RaveObject.DeserializeList(parts[1]);

        }
    }

    public class RaveShareRequest : RaveObject
    {
        new public static string[] delimiter = { "<RaveShareRequest>" };
        public string pluginKeyName;
        public List<String> requestIds;
        public List<String> userIds;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, pluginKeyName, delimiter[0]);
            s = SafeAppend(s, RaveObject.SerializeList(requestIds), delimiter[0]);
            s = SafeAppend(s, RaveObject.SerializeList(userIds), delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            pluginKeyName = parts[0];
            requestIds = RaveObject.DeserializeList(parts[1]);
            userIds = RaveObject.DeserializeList(parts[2]);

        }
    }

    public class RaveContact : RaveObject
    {
        new public static string[] delimiter = { "<RaveContact>" };
        public RaveUser user;
        public bool isEmail;
        public bool isRaveSocial;
        public bool isFacebook;
        public bool isGooglePlus;
        public Dictionary<string, string> externalIds;
        public string displayName;
        public string pictureURL;
        public string thirdPartySource;
        public string key;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, key, delimiter[0]);
            s = SafeAppend(s, displayName, delimiter[0]);
            s = SafeAppend(s, pictureURL, delimiter[0]);
            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            if (parts[0] != null)
            {
                user = new RaveUser();
                user.Deserialize(parts[0]);
            }
            else
            {
                user = null;
            }
            isEmail = StringToBool(parts[1]);
            isRaveSocial = StringToBool(parts[2]);
            isFacebook = StringToBool(parts[3]);
            isGooglePlus = StringToBool(parts[4]);
            externalIds = RaveObject.DeserializeDict(parts[5]);
            displayName = parts[6];
            pictureURL = parts[7];
            thirdPartySource = parts[8];
            key = parts[9];
        }
    }

    public class RaveAppDataKeyUserPair : RaveObject
    {
        new public static string[] delimiter = { "<RaveAppDataKeyUserPair>" };
        public string raveId;
        public string appDataKey;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, raveId, delimiter[0]);
            s = SafeAppend(s, appDataKey, delimiter[0]);
            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            raveId = parts[0];
            appDataKey = parts[1];
        }
    }

    public class RaveLeaderboardView : RaveObject
    {
        new public static string[] delimiter = { "<RaveLeaderboardView>" };
        public string leaderboardKey;
        public string qUsername;
        public float qAdjacent;
        public float qLimit;
        public float qOffset;
        public float qIsFriends;
        public List<RaveScore> scores;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, leaderboardKey, delimiter[0]);
            s = SafeAppend(s, qUsername, delimiter[0]);
            s = SafeAppend(s, qAdjacent.ToString(), delimiter[0]);
            s = SafeAppend(s, qLimit.ToString(), delimiter[0]);
            s = SafeAppend(s, qOffset.ToString(), delimiter[0]);
            s = SafeAppend(s, qIsFriends.ToString(), delimiter[0]);
            s = SafeAppend(s, RaveObject.SerializeList(scores), delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            leaderboardKey = parts[0];
            qUsername = parts[1];
            qAdjacent = Convert.ToSingle(parts[2]);
            qLimit = Convert.ToSingle(parts[3]);
            qOffset = Convert.ToSingle(parts[4]);
            qIsFriends = Convert.ToSingle(parts[5]);
            scores = RaveScore.DeserializeList<RaveScore>(parts[6]);
        }
    }

    public class RaveScore : RaveObject
    {
        new public static string[] delimiter = { "<RaveScore>" };
        public float score;
        public float position;
        public string userDisplayName;
        public string userPictureUrl;
        public string userRaveId;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, score.ToString(), delimiter[0]);
            s = SafeAppend(s, position.ToString(), delimiter[0]);
            s = SafeAppend(s, userDisplayName, delimiter[0]);
            s = SafeAppend(s, userPictureUrl, delimiter[0]);
            s = SafeAppend(s, userRaveId, delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            score = Convert.ToSingle(parts[0]);
            position = Convert.ToSingle(parts[1]);
            userDisplayName = parts[2];
            userPictureUrl = parts[3];
            userRaveId = parts[4];

        }
    }

    public class RaveLeaderboard : RaveObject
    {
        new public static string[] delimiter = { "<RaveLeaderboard>" };
        public string name;
        public string key;
        public string desc;
        public float sorter;
        public bool isAscending;
        public float highScore;
        public float friendsPosition;
        public float globalPosition;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, name, delimiter[0]);
            s = SafeAppend(s, key, delimiter[0]);
            s = SafeAppend(s, desc, delimiter[0]);
            s = SafeAppend(s, sorter.ToString(), delimiter[0]);
            s = SafeAppend(s, BoolToString(isAscending), delimiter[0]);
            s = SafeAppend(s, highScore.ToString(), delimiter[0]);
            s = SafeAppend(s, friendsPosition.ToString(), delimiter[0]);
            s = SafeAppend(s, globalPosition.ToString(), delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            name = parts[0];
            key = parts[1];
            desc = parts[2];
            sorter = SafeConvertFloat(parts[3]);
            isAscending = StringToBool(parts[4]);
            highScore = SafeConvertFloat(parts[5]);
            friendsPosition = SafeConvertFloat(parts[6]);
            globalPosition = SafeConvertFloat(parts[7]);

        }
    }

    public class RaveAchievement : RaveObject
    {
        new public static string[] delimiter = { "<RaveAchievement>" };
        public bool isUnlocked;
        public string achievementDescription;
        public string name;
        public string key;
        public string imageURL;

        override public string Serialize()
        {
            string s = "";
            s = SafeAppend(s, BoolToString(isUnlocked), delimiter[0]);
            s = SafeAppend(s, achievementDescription, delimiter[0]);
            s = SafeAppend(s, name, delimiter[0]);
            s = SafeAppend(s, key, delimiter[0]);
            s = SafeAppend(s, imageURL, delimiter[0]);

            return s;
        }

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            isUnlocked = StringToBool(parts[0]);
            achievementDescription = parts[1];
            name = parts[2];
            key = parts[3];
            imageURL = parts[4];

        }
    }

    public delegate void RaveCompletionCallback(string anError);
    public delegate void RaveAccessTokenCallback(string accessToken, string error);
    public delegate void RaveAccountExistsCallback(bool accountExists, bool hasPassword, string error);
    public delegate void RaveUserCallback(RaveUser user, string error);
    public delegate void RaveUsersCallback(List<RaveUser> users, string error);
    public delegate void RaveLoginCallback(bool loggedIn, string error);
    public delegate void RaveLoginStatusCallback(RaveLoginStatus status, string error);
    public delegate void RaveConnectCallback(bool connected, string error);
    public delegate void RaveDisconnectCallback(bool stillLoggedIn, string error);
    public delegate void RaveReadinessCallback(bool ready, string anError);
    public delegate void RaveContactsCallback(List<RaveContact> contacts, string error);
    public delegate void RavePluginShareRequestCallback(RaveShareRequest request, string error);
    public delegate void RaveShareRequestCallback(List<RaveShareRequest> requests, string error);
    public delegate void RaveIdentitiesCallback(List<string> identities, string error);
    public delegate void RaveGiftKeyCallback(string giftKey, string error);
    public delegate void RaveGiftResultCallback(List<string> succeeded, List<string> failed, string error);
    public delegate void RaveContactGiftResultsCallback(List<RaveGiftRequest> results, string error);
    public delegate void RaveGiftAndShareResultsCallback(List<RaveShareRequest> shareRequests, List<RaveContactGiftResult> contactGiftResults, string error);
    public delegate void RaveThirdPartyCallback(bool isConnected, string thirdPartyId, string error);
    public delegate void RaveGiftContentCallback(string key, string requestId, string error);
    public delegate void RaveConnectFriendsControllerCallback(RaveConnectFriendsControllerState state);
    public delegate void RaveConnectControllerCallback(RaveConnectControllerState state);
    public delegate void RaveSceneCallback(string error);
    public delegate void RaveUserChangedCallback(List<string> changedKeys);

    public enum RaveCallbackResult
    {
        RaveResultSuccessful,
        RaveResultCanceled,
        RaveResultError,
    }

    public class BigFishSignUpData : RaveObject
    {
        new public static string[] delimiter = { "<BigFishSignUpData>" };
        public bool acceptedNewsletter;
        public bool passedCoppaCheck;
        public string birthYear;

        override public void Deserialize(string s)
        {
            string[] parts = SafeSplit(s, delimiter);
            acceptedNewsletter = StringToBool(parts[0]);
            passedCoppaCheck = StringToBool(parts[1]);
            birthYear = parts[2];
        }
    }

    public delegate void BigFishSignUpSceneCallback(RaveCallbackResult result, BigFishSignUpData signUpData, string error);

    public delegate void BigFishCALListener(string eventKey);
}