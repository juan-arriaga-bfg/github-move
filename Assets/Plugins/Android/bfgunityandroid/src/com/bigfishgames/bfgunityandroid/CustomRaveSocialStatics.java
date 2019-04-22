package com.bigfishgames.bfgunityandroid;

import android.app.Activity;
import android.util.Log;
import co.ravesocial.sdk.RaveCompletionListener;
import co.ravesocial.sdk.RaveException;
import co.ravesocial.sdk.RaveSettings;
import co.ravesocial.sdk.RaveSocial;
import co.ravesocial.sdk.RaveSettings.Android;
import co.ravesocial.sdk.core.AccessTokenListener;
import co.ravesocial.sdk.core.RaveAchievement;
import co.ravesocial.sdk.core.RaveAppDataKeyUserPair;
import co.ravesocial.sdk.core.RaveAuthentication;
import co.ravesocial.sdk.core.RaveAuthenticationManager;
import co.ravesocial.sdk.core.RaveContact;
import co.ravesocial.sdk.core.RaveGift;
import co.ravesocial.sdk.core.RaveGiftRequest;
import co.ravesocial.sdk.core.RaveGiftType;
import co.ravesocial.sdk.core.RaveLeaderboard;
import co.ravesocial.sdk.core.RaveMergePolicy;
import co.ravesocial.sdk.core.RaveMergeUser;
import co.ravesocial.sdk.core.RaveScore;
import co.ravesocial.sdk.core.RaveUser;
import co.ravesocial.sdk.core.RaveAppDataKeysManager.AppDataKeyListListener;
import co.ravesocial.sdk.core.RaveAppDataKeysManager.AppDataKeyListener;
import co.ravesocial.sdk.core.RaveAppDataKeysManager.AppDataKeySetListener;
import co.ravesocial.sdk.core.RaveAppDataKeysManager.RaveAppDataKeysStateListener;
import co.ravesocial.sdk.core.RaveAppDataKeysManager.RaveAppDataKeysStateObserver;
import co.ravesocial.sdk.core.RaveAuthenticationManager.RaveReadinessListener;
import co.ravesocial.sdk.core.RaveContactsManager.RaveContactsFilter;
import co.ravesocial.sdk.core.RaveContactsManager.RaveContactsListener;
import co.ravesocial.sdk.core.RaveGiftsManager.RaveContactGiftResult;
import co.ravesocial.sdk.core.RaveGiftsManager.RaveGiftResultListener;
import co.ravesocial.sdk.core.RaveMergePolicy.RaveMergeDecisionListener;
import co.ravesocial.sdk.core.RaveSharingManager.RaveShareRequest;
import co.ravesocial.sdk.core.RaveSharingManager.RaveShareRequestsListener;
import co.ravesocial.sdk.core.RaveUsersManager.RaveAccountExistsListener;
import co.ravesocial.sdk.core.RaveUsersManager.RaveCurrentUserObserver;
import co.ravesocial.sdk.core.RaveUsersManager.RaveIdentitiesListener;
import co.ravesocial.sdk.core.RaveUsersManager.RaveUserChanges;
import co.ravesocial.sdk.core.RaveUsersManager.RaveUsersListener;
import co.ravesocial.sdk.internal.core.RaveCoreAuthentication;
import co.ravesocial.sdk.login.RaveLoginStatusListener;
import co.ravesocial.sdk.login.RaveLoginStatusListener.RaveLoginStatus;
import co.ravesocial.sdk.ui.RaveConnectController;
import co.ravesocial.sdk.ui.RaveConnectFriendsController;
import co.ravesocial.sdk.ui.RaveConnectController.ConnectState;
import co.ravesocial.sdk.ui.RaveConnectController.RaveConnectStateObserver;
import co.ravesocial.sdk.ui.RaveConnectFriendsController.RaveConnectFriendsStateObserver;
import co.ravesocial.sdk.ui.RaveConnectFriendsController.RaveFindFriendsState;
import co.ravesocial.util.logger.RaveLog;
import com.unity3d.player.UnityPlayer;
import java.io.File;
import java.net.URI;
import java.net.URISyntaxException;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map.Entry;

public class CustomRaveSocialStatics {
    static String TAG = "CustomRaveSocialStatics";
    static HashMap<String, CustomRaveSocialStatics.StaticConnectControllerHandler> connectHandlers;
    static HashMap<String, CustomRaveSocialStatics.StaticConnectFriendsControllerHandler> friendHandlers;
    static HashMap<String, CustomRaveSocialStatics.StaticCurrentUserObserver> userObservers;

    public CustomRaveSocialStatics() {
    }

    public static String ErrorToString(RaveException e) {
        return e != null?String.format("Error:%d:%s", new Object[]{Integer.valueOf(e.getErrorCode()), e.getLocalizedMessage()}):"";
    }

    public static String BoolToString(boolean b) {
        return b?"True":"False";
    }

    public static String SafeString(String s) {
        return s != null?s:"NULL";
    }

    public static boolean FloatToBool(float f) {
        return f != 0.0F;
    }

    public static void SafeAppend(StringBuilder base, String toAppend, String delim) {
        if(toAppend != null) {
            base.append(toAppend);
        } else {
            base.append("NULL");
        }

        base.append(delim);
    }

    public static String StringListToString(Collection<String> strings) {
        if(strings == null) {
            return "";
        } else {
            StringBuilder s = new StringBuilder();
            String delim = "<ListRaveString>";
            Iterator var3 = strings.iterator();

            while(var3.hasNext()) {
                String string = (String)var3.next();
                SafeAppend(s, string, delim);
            }

            return s.toString();
        }
    }

    public static String DateToString(Date date) {
        if(date == null) {
            return "";
        } else {
            DateFormat df = new SimpleDateFormat("MM/dd/yyyy");
            return df.format(date);
        }
    }

    public static Date StringToDate(String s) throws ParseException {
        DateFormat df = new SimpleDateFormat("MM/dd/yyyy");
        return df.parse(s);
    }

    public static String StringDictionaryToString(HashMap<String, String> map) {
        String delim = "<DictRaveString>";
        StringBuilder s = new StringBuilder();
        Iterator var3 = map.entrySet().iterator();

        while(var3.hasNext()) {
            Entry<String, String> entry = (Entry)var3.next();
            SafeAppend(s, (String)entry.getKey(), delim);
            SafeAppend(s, (String)entry.getValue(), delim);
        }

        return s.toString();
    }

    public static HashMap<String, String> StringToStringDictionary(String s) {
        String delim = "<DictRaveString>";
        HashMap<String, String> output = new HashMap();
        String[] parts = s.split(delim);

        for(int i = 0; i < parts.length - 1; i += 2) {
            if(!"NULL".equals(parts[i + 1]) && !"".equals(parts[i + 1])) {
                output.put(parts[i], parts[i + 1]);
            }
        }

        return output;
    }

    public static String NumberToString(Number number) {
        return number == null?"NULL":String.valueOf(number);
    }

    public static List<String> StringToStringList(String s) {
        String[] strings = s.split("<ListRaveString>");
        return Arrays.asList(strings);
    }

    public static String RaveUserToString(RaveUser obj) {
        if(obj == null) {
            return "NULL";
        } else {
            StringBuilder s = new StringBuilder();
            String delimiter = "<RaveUser>";
            SafeAppend(s, BoolToString(obj.isGuest()), delimiter);
            SafeAppend(s, obj.getDisplayName(), delimiter);
            SafeAppend(s, obj.getRealName(), delimiter);
            SafeAppend(s, obj.getUsername(), delimiter);
            SafeAppend(s, obj.getEmail(), delimiter);
            SafeAppend(s, DateToString(obj.getBirthdate()), delimiter);
            SafeAppend(s, obj.getRaveId(), delimiter);
            SafeAppend(s, obj.getFacebookId(), delimiter);
            SafeAppend(s, obj.getGooglePlusId(), delimiter);
            SafeAppend(s, obj.getThirdPartyId(), delimiter);
            SafeAppend(s, obj.getGender(), delimiter);
            SafeAppend(s, obj.getPictureURL(), delimiter);
            return s.toString();
        }
    }

    RaveUser RaveUserIdToRaveUser(String raveId) {
        return RaveSocial.usersManager.getUserById(raveId);
    }

    public static String RaveUserArrayToString(List<RaveUser> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveUser>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveUser item = (RaveUser)var3.next();
            SafeAppend(s, RaveUserToString(item), delimiter);
        }

        return s.toString();
    }

    static String StringOrNull(String string) {
        return string.contentEquals("NULL")?null:string;
    }

    static Date DateOrNull(String string) {
        if(string.contentEquals("NULL")) {
            return null;
        } else {
            try {
                return StringToDate(string);
            } catch (ParseException var2) {
                RaveLog.e(TAG, "Couldn't parse birthdate " + string);
                return null;
            }
        }
    }

    static RaveUserChanges StringToRaveUserChanges(String objs) {
        if(objs.length() == 0) {
            return null;
        } else {
            String[] parts = objs.split("<RaveUserChanges>");
            RaveUserChanges changes = new RaveUserChanges();
            changes.displayName = StringOrNull(parts[0]);
            changes.realName = StringOrNull(parts[1]);
            changes.email = StringOrNull(parts[2]);
            changes.birthdate = DateOrNull(parts[3]);
            changes.gender = StringOrNull(parts[4]);
            return changes;
        }
    }

    public static String RaveGiftTypeToString(RaveGiftType obj) {
        if(obj == null) {
            return "NULL";
        } else {
            StringBuilder s = new StringBuilder();
            String delimiter = "<RaveGiftType>";
            SafeAppend(s, obj.getId(), delimiter);
            SafeAppend(s, obj.getKey(), delimiter);
            SafeAppend(s, obj.getName(), delimiter);
            SafeAppend(s, BoolToString(obj.canRequest()), delimiter);
            SafeAppend(s, BoolToString(obj.canGift()), delimiter);
            return s.toString();
        }
    }

    RaveGiftType RaveGiftTypeKeyToRaveGiftType(String giftTypeKey) {
        return RaveSocial.giftsManager.getGiftTypeByKey(giftTypeKey);
    }

    public static String RaveGiftTypeArrayToString(List<RaveGiftType> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveGiftType>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveGiftType item = (RaveGiftType)var3.next();
            SafeAppend(s, RaveGiftTypeToString(item), delimiter);
        }

        return s.toString();
    }

    public static String RaveGiftToString(RaveGift obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveGift>";
        SafeAppend(s, obj.getGiftId(), delimiter);
        SafeAppend(s, obj.getGiftTypeKey(), delimiter);
        SafeAppend(s, obj.getSenderRaveId(), delimiter);
        SafeAppend(s, BoolToString(obj.isFromGift()), delimiter);
        SafeAppend(s, BoolToString(obj.isFromRequest()), delimiter);
        SafeAppend(s, DateToString(obj.getTimeSent()), delimiter);
        SafeAppend(s, RaveGiftTypeToString(obj.getGiftType()), delimiter);
        SafeAppend(s, RaveUserToString(obj.getSender()), delimiter);
        SafeAppend(s, obj.getSenderRaveId(), delimiter);
        return s.toString();
    }

    RaveGift RaveGiftIdToRaveGift(String giftId) {
        return RaveSocial.giftsManager.getGiftById(giftId);
    }

    public static String RaveGiftArrayToString(List<RaveGift> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveGift>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveGift item = (RaveGift)var3.next();
            SafeAppend(s, RaveGiftToString(item), delimiter);
        }

        return s.toString();
    }

    public static String RaveGiftRequestToString(RaveGiftRequest obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveGiftRequest>";
        SafeAppend(s, obj.getRequestId(), delimiter);
        SafeAppend(s, RaveGiftTypeToString(obj.getGiftType()), delimiter);
        SafeAppend(s, obj.getGiftTypeKey(), delimiter);
        SafeAppend(s, RaveUserToString(obj.getRequester()), delimiter);
        SafeAppend(s, DateToString(obj.getTimeSent()), delimiter);
        SafeAppend(s, obj.getRequesterRaveId(), delimiter);
        return s.toString();
    }

    RaveGiftRequest RaveGiftRequestIdToRaveGiftRequest(String giftRequestId) {
        return RaveSocial.giftsManager.getGiftRequestById(giftRequestId);
    }

    public static String RaveGiftRequestArrayToString(List<RaveGiftRequest> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveGiftRequest>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveGiftRequest item = (RaveGiftRequest)var3.next();
            SafeAppend(s, RaveGiftRequestToString(item), delimiter);
        }

        return s.toString();
    }

    public static String RaveContactGiftResultToString(RaveContactGiftResult obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveContactGiftResult>";
        SafeAppend(s, obj.getPluginKeyName(), delimiter);
        SafeAppend(s, StringListToString(obj.getExternalIds()), delimiter);
        return s.toString();
    }

    public static String RaveContactGiftResultArrayToString(List<RaveContactGiftResult> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveContactGiftResult>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveContactGiftResult item = (RaveContactGiftResult)var3.next();
            SafeAppend(s, RaveContactGiftResultToString(item), delimiter);
        }

        return s.toString();
    }

    public static String RaveShareRequestToString(RaveShareRequest obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveShareRequest>";
        SafeAppend(s, obj.getPluginKeyName(), delimiter);
        SafeAppend(s, StringListToString(obj.getRequestIds()), delimiter);
        SafeAppend(s, StringListToString(obj.getUserIds()), delimiter);
        return s.toString();
    }

    static RaveShareRequest StringToRaveShareRequest(String s) {
        String delim = "<RaveShareRequest>";
        String[] parts = s.split(delim);
        String pluginType = parts[0];
        List<String> requestIds = StringToStringList(parts[1]);
        List<String> userIds = StringToStringList(parts[2]);
        return new RaveShareRequest(pluginType, requestIds, userIds);
    }

    RaveShareRequest RaveStringToRaveShareRequest(String pluginType, List<String> requestIds, List<String> userIds) {
        return new RaveShareRequest(pluginType, requestIds, userIds);
    }

    public static String RaveShareRequestArrayToString(List<RaveShareRequest> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveShareRequest>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveShareRequest item = (RaveShareRequest)var3.next();
            SafeAppend(s, RaveShareRequestToString(item), delimiter);
        }

        return s.toString();
    }

    public static List<RaveShareRequest> StringToRaveShareRequestArray(String s) {
        String delim = "<ListRaveShareRequest>";
        String[] parts = s.split(delim);
        List<RaveShareRequest> requests = new ArrayList();
        String[] var4 = parts;
        int var5 = parts.length;

        for(int var6 = 0; var6 < var5; ++var6) {
            String part = var4[var6];
            requests.add(StringToRaveShareRequest(part));
        }

        return requests;
    }

    public static String RaveContactToString(RaveContact obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveContact>";
        SafeAppend(s, RaveUserToString(obj.getUser()), delimiter);
        SafeAppend(s, BoolToString(obj.isEmail()), delimiter);
        SafeAppend(s, BoolToString(obj.isRaveSocial()), delimiter);
        SafeAppend(s, BoolToString(obj.isFacebook()), delimiter);
        SafeAppend(s, BoolToString(obj.isGooglePlus()), delimiter);
        SafeAppend(s, StringDictionaryToString(obj.getExternalIds()), delimiter);
        SafeAppend(s, obj.getDisplayName(), delimiter);
        SafeAppend(s, obj.getPictureURL(), delimiter);
        SafeAppend(s, obj.getThirdPartySource(), delimiter);
        SafeAppend(s, obj.getKey(), delimiter);
        return s.toString();
    }

    public static String RaveAppDataKeyUserPairToString(RaveAppDataKeyUserPair obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveAppDataKeyUserPair>";
        SafeAppend(s, obj.getRaveId(), delimiter);
        SafeAppend(s, obj.getAppDataKey(), delimiter);
        return s.toString();
    }

    RaveContact RaveUserIdToRaveContact(String raveId) {
        return RaveSocial.contactsManager.getContactByRaveId(raveId);
    }

    static RaveContact ContactInfoToRaveContact(String source, String externalId, String displayName, String pictureURL) {
        String o_pictureURL = null;
        if(!"NULL".equals(pictureURL)) {
            o_pictureURL = pictureURL;
        }

        return RaveSocial.contactsManager.createContactInstance(source, externalId, displayName, o_pictureURL);
    }

    static RaveContact UserIdToRaveContact(String raveId) {
        return raveId.length() > 0?RaveSocial.contactsManager.getContactByRaveId(raveId):null;
    }

    static RaveContact StringToRaveContact(String s) {
        String delim = "<RaveContact>";
        String[] parts = s.split(delim);
        String[] key = parts[0].split(":");
        return key.length == 1?UserIdToRaveContact(key[0]):ContactInfoToRaveContact(key[0], key[1], parts[1], parts[2]);
    }

    public static String RaveContactArrayToString(List<RaveContact> objs) {
        if(objs == null) {
            return "";
        } else {
            StringBuilder s = new StringBuilder();
            String delimiter = "<ListRaveContact>";
            Iterator var3 = objs.iterator();

            while(var3.hasNext()) {
                RaveContact item = (RaveContact)var3.next();
                SafeAppend(s, RaveContactToString(item), delimiter);
            }

            return s.toString();
        }
    }

    public static String RaveAppDataKeyUserPairArrayToString(List<RaveAppDataKeyUserPair> objs) {
        if(objs == null) {
            return "";
        } else {
            StringBuilder s = new StringBuilder();
            String delimiter = "<ListRaveAppDataKeyUserPair>";
            Iterator var3 = objs.iterator();

            while(var3.hasNext()) {
                RaveAppDataKeyUserPair pair = (RaveAppDataKeyUserPair)var3.next();
                SafeAppend(s, RaveAppDataKeyUserPairToString(pair), delimiter);
            }

            return s.toString();
        }
    }

    public static List<RaveContact> StringToRaveContactArray(String s) {
        String delim = "<ListRaveContact>";
        String[] parts = s.split(delim);
        List<RaveContact> contacts = new ArrayList();
        String[] var4 = parts;
        int var5 = parts.length;

        for(int var6 = 0; var6 < var5; ++var6) {
            String part = var4[var6];
            RaveContact contact = StringToRaveContact(part);
            if(contact != null) {
                contacts.add(contact);
            }
        }

        return contacts;
    }

    public static String RaveScoreToString(RaveScore obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveScore>";
        SafeAppend(s, NumberToString(obj.getScore()), delimiter);
        SafeAppend(s, NumberToString(obj.getPosition()), delimiter);
        SafeAppend(s, obj.getUserDisplayName(), delimiter);
        SafeAppend(s, obj.getUserPictureUrl(), delimiter);
        SafeAppend(s, obj.getUserRaveId(), delimiter);
        return s.toString();
    }

    public static String RaveScoreArrayToString(List<RaveScore> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveScore>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveScore item = (RaveScore)var3.next();
            SafeAppend(s, RaveScoreToString(item), delimiter);
        }

        return s.toString();
    }

    public static String RaveLeaderboardToString(RaveLeaderboard obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveLeaderboard>";
        SafeAppend(s, obj.getName(), delimiter);
        SafeAppend(s, obj.getKey(), delimiter);
        SafeAppend(s, obj.getDesc(), delimiter);
        SafeAppend(s, NumberToString(obj.getSorter()), delimiter);
        SafeAppend(s, BoolToString(obj.isAscending()), delimiter);
        SafeAppend(s, NumberToString(obj.getHighScore()), delimiter);
        SafeAppend(s, NumberToString(obj.getFriendsPosition()), delimiter);
        SafeAppend(s, NumberToString(obj.getGlobalPosition()), delimiter);
        return s.toString();
    }

    RaveLeaderboard RaveLeaderboardKeyToRaveLeaderboard(String leaderboardKey) {
        return RaveSocial.leaderboardsManager.getLeaderboard(leaderboardKey);
    }

    public static String RaveLeaderboardArrayToString(List<RaveLeaderboard> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveLeaderboard>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveLeaderboard item = (RaveLeaderboard)var3.next();
            SafeAppend(s, RaveLeaderboardToString(item), delimiter);
        }

        return s.toString();
    }

    public static String RaveAchievementToString(RaveAchievement obj) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<RaveAchievement>";
        SafeAppend(s, BoolToString(obj.isUnlocked().booleanValue()), delimiter);
        SafeAppend(s, obj.getAchievementDescription(), delimiter);
        SafeAppend(s, obj.getName(), delimiter);
        SafeAppend(s, obj.getKey(), delimiter);
        SafeAppend(s, obj.getImageUrl(), delimiter);
        return s.toString();
    }

    RaveAchievement RaveAchievementKeyToRaveAchievement(String achievementKey) {
        return RaveSocial.achievementsManager.getAchievementByKey(achievementKey);
    }

    public static String RaveAchievementArrayToString(List<RaveAchievement> objs) {
        StringBuilder s = new StringBuilder();
        String delimiter = "<ListRaveAchievement>";
        Iterator var3 = objs.iterator();

        while(var3.hasNext()) {
            RaveAchievement item = (RaveAchievement)var3.next();
            SafeAppend(s, RaveAchievementToString(item), delimiter);
        }

        return s.toString();
    }

    public static RaveCompletionListener CompletionCallback(final String callbackModule, final String callbackName, final String pid) {
        return new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        };
    }

    public static AppDataKeyListener AppDataKeyCallback(final String callbackModule, final String callbackName, final String pid) {
        return new AppDataKeyListener() {
            public void onComplete(String key, RaveException exception) {
                CustomRaveSocialStatics.UnityStringCallback(callbackModule, callbackName, pid, key, exception);
            }
        };
    }

    public static AppDataKeySetListener AppDataKeySetCallback(final String callbackModule, final String callbackName, final String pid) {
        return new AppDataKeySetListener() {
            public void onComplete(List<RaveAppDataKeyUserPair> keys, RaveException exception) {
                CustomRaveSocialStatics.UnityAppDataKeySetCallback(callbackModule, callbackName, pid, keys, exception);
            }
        };
    }

    public static void UnityCompletionCallback(String callbackModule, String callbackName, String pid, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s", new Object[]{pid, ErrorToString(exception)}));
    }

    public static void UnityStringCallback(String callbackModule, String callbackName, String pid, String string, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, string, ErrorToString(exception)}));
    }

    public static void UnityString2Callback(String callbackModule, String callbackName, String pid, String string, String string2, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s|%s", new Object[]{pid, string, string2, ErrorToString(exception)}));
    }

    public static void UnityLoginStatusCallback(String callbackModule, String callbackName, String pid, RaveLoginStatus status, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%d|%s", new Object[]{pid, Integer.valueOf(status.ordinal()), ErrorToString(exception)}));
    }

    public static void UnityUserCallback(String callbackModule, String callbackName, String pid, RaveUser user, RaveException exception) {
        String userString = RaveUserToString(user);
        (new CustomRaveSocialStatics.UnityCallbackContext(callbackModule, callbackName, pid)).param(userString).error(exception).callback();
    }

    public static void UnityUsersCallback(String callbackModule, String callbackName, String pid, List<RaveUser> users, RaveException exception) {
        String usersString = RaveUserArrayToString(users);
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, usersString, ErrorToString(exception)}));
    }

    public static void UnityStringListCallback(String callbackModule, String callbackName, String pid, List<String> strings, RaveException exception) {
        String mergedString = StringListToString(strings);
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, mergedString, ErrorToString(exception)}));
    }

    public static void UnityAccountExistsCallback(String callbackModule, String callbackName, String pid, boolean exists, boolean hasPassword, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s|%s", new Object[]{pid, BoolToString(exists), BoolToString(hasPassword), ErrorToString(exception)}));
    }

    public static void UnityGiftResultCallback(String callbackModule, String callbackName, String pid, List<String> succeeded, List<String> failed, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s|%s", new Object[]{pid, StringListToString(succeeded), StringListToString(failed), ErrorToString(exception)}));
    }

    public static void UnityContactGiftResultCallback(String callbackModule, String callbackName, String pid, List<RaveContactGiftResult> results, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, RaveContactGiftResultArrayToString(results), ErrorToString(exception)}));
    }

    public static void UnityGiftAndShareCallback(String callbackModule, String callbackName, String pid, List<RaveShareRequest> shareRequests, List<RaveContactGiftResult> contactGiftResults, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s|%s", new Object[]{pid, RaveShareRequestArrayToString(shareRequests), RaveContactGiftResultArrayToString(contactGiftResults), ErrorToString(exception)}));
    }

    public static void UnityContactListCallback(String callbackModule, String callbackName, String pid, List<RaveContact> contacts, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, RaveContactArrayToString(contacts), ErrorToString(exception)}));
    }

    public static void UnityAppDataKeySetCallback(String callbackModule, String callbackName, String pid, List<RaveAppDataKeyUserPair> pairs, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, RaveAppDataKeyUserPairArrayToString(pairs), ErrorToString(exception)}));
    }

    public static void UnityReadinessCallback(String callbackModule, String callbackName, String pid, boolean readiness, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, BoolToString(readiness), ErrorToString(exception)}));
    }

    public static void UnityShareRequestListCallback(String callbackModule, String callbackName, String pid, List<RaveShareRequest> requests, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, RaveShareRequestArrayToString(requests), ErrorToString(exception)}));
    }

    public static void UnitySceneCallback(String callbackModule, String callbackName, String pid, RaveException exception) {
        UnityCompletionCallback(callbackModule, callbackName, pid, exception);
    }

    public static void UnityAccountInfoSceneCallback(String callbackModule, String callbackName, String pid, boolean loggedOut, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s", new Object[]{pid, BoolToString(loggedOut), ErrorToString(exception)}));
    }

    public static void UnityLoginSceneCallback(String callbackModule, String callbackName, String pid, boolean loggedIn, boolean accountCreated, String pluginKeyName, RaveException exception) {
        UnityMessageQueueRunner.AddMessageToQueue(callbackModule, callbackName, String.format("%s|%s|%s|%s|%s", new Object[]{pid, BoolToString(loggedIn), BoolToString(accountCreated), pluginKeyName, ErrorToString(exception)}));
    }

    public static String RaveUsersManagerGetUserById(String raveId) {
        RaveUser result = RaveSocial.usersManager.getUserById(raveId);
        return RaveUserToString(result);
    }

    public static void RaveUsersManagerUpdateUserById(String raveId, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.updateUserById(raveId, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveUsersManagerCurrent() {
        RaveUser result = RaveSocial.usersManager.getCurrent();
        return RaveUserToString(result);
    }

    public static void RaveUsersManagerUpdateCurrent(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.updateCurrent(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void applyMapToUser(HashMap<String, String> userData, HashMap<String, String> savedData) {
        RaveUser user = RaveSocial.getCurrentUser();
        String value = (String)userData.get("displayName");
        if(value != null) {
            savedData.put("displayName", user.getDisplayName());
            user.setDisplayName(value);
        }

        value = (String)userData.get("realName");
        if(value != null) {
            savedData.put("realName", user.getRealName());
            user.setRealName(value);
        }

        value = (String)userData.get("username");
        if(value != null) {
            savedData.put("username", user.getUsername());
            user.setUsername(value);
        }

        value = (String)userData.get("email");
        if(value != null) {
            savedData.put("email", user.getEmail());
            user.setEmail(value);
        }

        value = (String)userData.get("gender");
        if(value != null) {
            savedData.put("gender", user.getGender());
            user.setGender(value);
        }

        value = (String)userData.get("birthdate");
        if(value != null) {
            savedData.put("birthdate", DateToString(user.getBirthdate()));

            try {
                if(!"".equals(value)) {
                    user.setBirthdate(StringToDate(value));
                } else {
                    user.setBirthdate((Date)null);
                }
            } catch (ParseException var5) {
                RaveLog.e(TAG, "Couldn't parse birthdate " + value);
            }
        }

    }

    public static void RavePushChangesToCurrentUser(final String callbackModule, final String callbackName, final String pid, String parameterString) {
        RaveUser user = RaveSocial.getCurrentUser();
        if(user == null) {
            RaveLog.e(TAG, "No RaveUser available");
            UnityCompletionCallback(callbackModule, callbackName, pid, new RaveException("No RaveUser available"));
        } else {
            HashMap<String, String> parameters = StringToStringDictionary(parameterString);
            final HashMap<String, String> savedData = new HashMap();
            applyMapToUser(parameters, savedData);
            RaveSocial.usersManager.pushCurrent(new RaveCompletionListener() {
                public void onComplete(RaveException exception) {
                    if(exception != null) {
                        CustomRaveSocialStatics.applyMapToUser(savedData, new HashMap());
                    }

                    CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                }
            });
        }
    }

    public static void RaveUsersManagerCheckAccountExists(String email, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.checkAccountExists(email, new RaveAccountExistsListener() {
            public void onComplete(boolean accountExists, boolean hasPassword, RaveException exception) {
                CustomRaveSocialStatics.UnityAccountExistsCallback(callbackModule, callbackName, pid, accountExists, hasPassword, exception);
            }
        });
    }

    public static void RaveUsersManagerCheckThirdPartyAccountExists(String email, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.checkThirdPartyAccountExists(email, new RaveAccountExistsListener() {
            public void onComplete(boolean exists, boolean hasPassword, RaveException exception) {
                CustomRaveSocialStatics.UnityAccountExistsCallback(callbackModule, callbackName, pid, exists, hasPassword, exception);
            }
        });
    }

    public static void RaveUsersManagerFetchAccessToken(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.fetchAccessToken(new AccessTokenListener() {
            public void onComplete(String accessToken, RaveException exception) {
                CustomRaveSocialStatics.UnityStringCallback(callbackModule, callbackName, pid, accessToken, exception);
            }
        });
    }

    public static void RaveUsersManagerFetchRandomUsersForApplication(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.fetchRandomUsersForApplication(new RaveUsersListener() {
            public void onComplete(List<RaveUser> users, RaveException exception) {
                CustomRaveSocialStatics.UnityUsersCallback(callbackModule, callbackName, pid, users, exception);
            }
        });
    }

    public static void RaveUsersManagerFetchRandomUsersForApplication2(String appUuid, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.fetchRandomUsersForApplication(appUuid, new RaveUsersListener() {
            public void onComplete(List<RaveUser> users, RaveException exception) {
                CustomRaveSocialStatics.UnityUsersCallback(callbackModule, callbackName, pid, users, exception);
            }
        });
    }

    public static void RaveUsersManagerFetchRandomUsersForApplication3(String appUuid, float excludeContacts, float limit, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.fetchRandomUsersForApplication(appUuid, FloatToBool(excludeContacts), (int)limit, new RaveUsersListener() {
            public void onComplete(List<RaveUser> users, RaveException exception) {
                CustomRaveSocialStatics.UnityUsersCallback(callbackModule, callbackName, pid, users, exception);
            }
        });
    }

    public static void RaveUsersManagerFetchAllIdentities(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.fetchIdentities(new RaveIdentitiesListener() {
            public void onComplete(List<String> mergedIdentities, RaveException exception) {
                CustomRaveSocialStatics.UnityStringListCallback(callbackModule, callbackName, pid, mergedIdentities, exception);
            }
        });
    }

    public static void RaveUsersManagerFetchIdentitiesForApplication(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.usersManager.fetchIdentitiesForApplication(new RaveIdentitiesListener() {
            public void onComplete(List<String> mergedIdentities, RaveException exception) {
                CustomRaveSocialStatics.UnityStringListCallback(callbackModule, callbackName, pid, mergedIdentities, exception);
            }
        });
    }

    public static void RaveUsersManagerPushProfilePicture(final String callbackModule, final String callbackName, final String pid, final String url) {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            public void run() {
                URI uri;
                try {
                    uri = new URI(url);
                } catch (URISyntaxException var3) {
                    CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, new RaveException(var3));
                    return;
                }

                File file = new File(uri);
                RaveSocial.usersManager.pushProfilePicture(file.getPath(), new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static String RaveGiftsManagerGetGiftTypeByKey(String typeKey) {
        RaveGiftType result = RaveSocial.giftsManager.getGiftTypeByKey(typeKey);
        return RaveGiftTypeToString(result);
    }

    public static String RaveGiftsManagerGiftTypes() {
        List<RaveGiftType> objs = RaveSocial.giftsManager.getGiftTypes();
        return RaveGiftTypeArrayToString(objs);
    }

    public static void RaveGiftsManagerUpdateGiftTypes(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.giftsManager.updateGiftTypes(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static RaveGift RaveGiftsManagerGetGiftById(String giftId) {
        return RaveSocial.giftsManager.getGiftById(giftId);
    }

    public static RaveGiftRequest RaveGiftsManagerGetGiftRequestById(String giftRequestId) {
        return RaveSocial.giftsManager.getGiftRequestById(giftRequestId);
    }

    public static String RaveGiftsManagerGifts() {
        List<RaveGift> objs = RaveSocial.giftsManager.getGifts();
        return RaveGiftArrayToString(objs);
    }

    public static void RaveGiftsManagerUpdateGifts(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.giftsManager.updateGifts(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveGiftsManagerGiftRequests() {
        List<RaveGiftRequest> objs = RaveSocial.giftsManager.getGiftRequests();
        return RaveGiftRequestArrayToString(objs);
    }

    public static void RaveGiftsManagerUpdateGiftRequests(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.giftsManager.updateGiftRequests(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveGiftsManagerSendGiftsToUsersWithKey(String giftTypeKey, String userIds, final String callbackModule, final String callbackName, final String pid) {
        List<String> o_userIds = StringToStringList(userIds);
        RaveSocial.giftsManager.sendGiftWithKeyToUsers(giftTypeKey, o_userIds, new RaveGiftResultListener() {
            public void onComplete(List<String> succeeded, List<String> failed, List<String> errors, RaveException e) {
                CustomRaveSocialStatics.UnityGiftResultCallback(callbackModule, callbackName, pid, succeeded, failed, e);
            }
        });
    }

    public static void RaveGiftsManagerAcceptGiftId(String giftId, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.giftsManager.acceptGiftById(giftId, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveGiftsManagerRejectGiftById(String giftId, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.giftsManager.rejectGiftById(giftId, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveGiftsManagerRequestGiftWithKey(String giftTypeKey, String userIds, final String callbackModule, final String callbackName, final String pid) {
        List<String> o_userIds = StringToStringList(userIds);
        RaveSocial.giftsManager.requestGiftWithKeyFromUsers(giftTypeKey, o_userIds, new RaveGiftResultListener() {
            public void onComplete(List<String> succeeded, List<String> failed, List<String> errors, RaveException e) {
                CustomRaveSocialStatics.UnityGiftResultCallback(callbackModule, callbackName, pid, succeeded, failed, e);
            }
        });
    }

    public static void RaveGiftsManagerGrantGiftRequestById(String requestId, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.giftsManager.grantGiftRequestById(requestId, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveGiftsManagerIgnoreGiftRequestById(String requestId, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.giftsManager.ignoreGiftRequestById(requestId, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveGiftsManagerSendGifts(String giftTypeKey, String userIds, final String callbackModule, final String callbackName, final String pid) {
        List<String> o_userIds = StringToStringList(userIds);
        RaveSocial.giftsManager.sendGiftWithKeyToUsers(giftTypeKey, o_userIds, new RaveGiftResultListener() {
            public void onComplete(List<String> succeeded, List<String> failed, List<String> errors, RaveException e) {
                CustomRaveSocialStatics.UnityGiftResultCallback(callbackModule, callbackName, pid, succeeded, failed, e);
            }
        });
    }

    public static String RaveContactsManagerAll() {
        List<RaveContact> objs = RaveSocial.contactsManager.getAll();
        return RaveContactArrayToString(objs);
    }

    public static void RaveContactsManagerUpdateAll(final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveSocial.contactsManager.updateAll(new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static String RaveContactsManagerAllUsingThisApplication() {
        List<RaveContact> objs = RaveSocial.contactsManager.getAllUsingThisApplication();
        return RaveContactArrayToString(objs);
    }

    public static void RaveContactsManagerUpdateAllUsingThisApplication(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.contactsManager.updateAllUsingThisApplication(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveContactsManagerFacebook() {
        List<RaveContact> objs = RaveSocial.contactsManager.getFacebook();
        return RaveContactArrayToString(objs);
    }

    public static void RaveContactsManagerUpdateFacebook(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.contactsManager.updateFacebook(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveContactsManagerAddContactsByUsername(String usernames, final String callbackModule, final String callbackName, final String pid) {
        List<String> o_usernames = StringToStringList(usernames);
        RaveSocial.contactsManager.addContactsByUsername(o_usernames, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveContactsManagerDeleteContact(String userUuid, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.contactsManager.deleteContact(userUuid, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveContactsManagerGetAllUsingApplication(String appId) {
        List<RaveContact> objs = RaveSocial.contactsManager.getAllUsingApplication(appId);
        return RaveContactArrayToString(objs);
    }

    public static void RaveContactsManagerUpdateAllUsingApplication(String appId, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.contactsManager.updateAllUsingApplication(appId, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveContactsManagerFetchAllExternal(final float filter, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveContactsFilter o_filter = RaveContactsFilter.values()[(int)filter];
                RaveSocial.contactsManager.fetchExternalFromAll(o_filter, new RaveContactsListener() {
                    public void onComplete(List<RaveContact> contacts, RaveException exception) {
                        CustomRaveSocialStatics.UnityContactListCallback(callbackModule, callbackName, pid, contacts, exception);
                    }
                });
            }
        });
    }

    public static void RaveContactsManagerFetchExternalFrom(final String pluginKeyName, final float filter, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveContactsFilter o_filter = RaveContactsFilter.values()[(int)filter];
                RaveSocial.contactsManager.fetchExternalFrom(pluginKeyName, o_filter, new RaveContactsListener() {
                    public void onComplete(List<RaveContact> contacts, RaveException exception) {
                        CustomRaveSocialStatics.UnityContactListCallback(callbackModule, callbackName, pid, contacts, exception);
                    }
                });
            }
        });
    }

    public static void RaveSharingManagerPostToFacebook(final String wallPost, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveSocial.sharingManager.postToFacebookWithImage(wallPost, "", new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static void RaveSharingManagerPostToFacebookWithImage(final String wallPost, final String imageURL, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveSocial.sharingManager.postToFacebookWithImage(wallPost, imageURL, new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static void RaveSharingManagerPostToGooglePlus(final String postText, final String imageURL, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveSocial.sharingManager.postToGooglePlusWithImage(postText, imageURL, new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static void RaveSharingManagerShareWith(final String externalContacts, final String subject, final String message, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                List<RaveContact> o_externalContacts = CustomRaveSocialStatics.StringToRaveContactArray(externalContacts);
                RaveSocial.sharingManager.share(o_externalContacts, subject, message, new RaveShareRequestsListener() {
                    public void onComplete(List<RaveShareRequest> requests, RaveException exception) {
                        CustomRaveSocialStatics.UnityShareRequestListCallback(callbackModule, callbackName, pid, requests, exception);
                    }
                });
            }
        });
    }

    public static void RaveSharingManagerShareWithViaPlugin(final String externalContacts, final String pluginKeyName, final String subject, final String message, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                List<RaveContact> o_externalContacts = CustomRaveSocialStatics.StringToRaveContactArray(externalContacts);
                RaveSocial.sharingManager.shareVia(pluginKeyName, o_externalContacts, subject, message, new RaveShareRequestsListener() {
                    public void onComplete(List<RaveShareRequest> requests, RaveException exception) {
                        CustomRaveSocialStatics.UnityShareRequestListCallback(callbackModule, callbackName, pid, requests, exception);
                    }
                });
            }
        });
    }

    public static String RaveSharingManagerGetExternalIdForShareInstall(String appCallUrl, String source) {
        return RaveSocial.sharingManager.getExternalIdForShareInstall(appCallUrl);
    }

    public static String RaveLeaderboardManagerLeaderboards() {
        List<RaveLeaderboard> objs = RaveSocial.leaderboardsManager.getLeaderboards();
        return RaveLeaderboardArrayToString(objs);
    }

    public static void RaveLeaderboardManagerUpdateLeaderboards(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateLeaderboards(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveLeaderboardManagerGetLeaderboardByKey(String leaderboardKey) {
        RaveLeaderboard obj = RaveSocial.leaderboardsManager.getLeaderboard(leaderboardKey);
        return RaveLeaderboardToString(obj);
    }

    public static void RaveLeaderboardManagerUpdateLeaderboardByKey(String leaderboardKey, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateLeaderboard(leaderboardKey, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveLeaderboardManagerSubmitScoreByKey(String leaderboardKey, float score, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.submitScore(leaderboardKey, (int)score, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveLeaderboardManagerUpdateGlobalScoresByKey(String leaderboardKey, float page, float pageSize, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateGlobalScores(leaderboardKey, (int)page, (int)pageSize, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveLeaderboardManagerGetGlobalScoresByKey(String leaderboardKey, float page, float pageSize) {
        List<RaveScore> objs = RaveSocial.leaderboardsManager.getGlobalScores(leaderboardKey, (int)page, (int)pageSize);
        return RaveScoreArrayToString(objs);
    }

    public static String RaveLeaderboardManagerGetFriendsScoresByKey(String leaderboardKey, float page, float pageSize) {
        List<RaveScore> objs = RaveSocial.leaderboardsManager.getFriendsScores(leaderboardKey, (int)page, (int)pageSize);
        return RaveScoreArrayToString(objs);
    }

    public static void RaveLeaderboardManagerUpdateFriendsScoresByKey(String leaderboardKey, float page, float pageSize, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateFriendsScores(leaderboardKey, (int)page, (int)pageSize, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveLeaderboardManagerUpdateMyGlobalScoresByKey(String leaderboardKey, float pageSize, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateMyGlobalScores(leaderboardKey, (int)pageSize, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveLeaderboardManagerGetMyGlobalScoresByKey(String leaderboardKey, float pageSize) {
        List<RaveScore> objs = RaveSocial.leaderboardsManager.getMyGlobalScores(leaderboardKey, (int)pageSize);
        return RaveScoreArrayToString(objs);
    }

    public static String RaveLeaderboardManagerGetMyFriendsScoresByKey(String leaderboardKey, float pageSize) {
        List<RaveScore> objs = RaveSocial.leaderboardsManager.getMyFriendsScores(leaderboardKey, (int)pageSize);
        return RaveScoreArrayToString(objs);
    }

    public static void RaveLeaderboardManagerUpdateMyFriendsScoresByKey(String leaderboardKey, float pageSize, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateMyFriendsScores(leaderboardKey, (int)pageSize, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey(String leaderboardKey, float adjacent) {
        List<RaveScore> objs = RaveSocial.leaderboardsManager.getMyGlobalScoresAdjacent(leaderboardKey, (int)adjacent);
        return RaveScoreArrayToString(objs);
    }

    public static void RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey(String leaderboardKey, float adjacent, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateMyGlobalScoresAdjacent(leaderboardKey, (int)adjacent, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey(String leaderboardKey, float adjacent) {
        List<RaveScore> objs = RaveSocial.leaderboardsManager.getMyFriendsScoresAdjacent(leaderboardKey, (int)adjacent);
        return RaveScoreArrayToString(objs);
    }

    public static void RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey(String leaderboardKey, float adjacent, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.leaderboardsManager.updateMyFriendsScoresAdjacent(leaderboardKey, (int)adjacent, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static float RaveLeaderboardManagerGetHighScoreByKey(String leaderboardKey) {
        return (float)RaveSocial.leaderboardsManager.getHighScore(leaderboardKey).intValue();
    }

    public static float RaveLeaderboardManagerGetFriendsPositionByKey(String leaderboardKey) {
        return (float)RaveSocial.leaderboardsManager.getFriendsPosition(leaderboardKey).intValue();
    }

    public static float RaveLeaderboardManagerGetGlobalPositionByKey(String leaderboardKey) {
        return (float)RaveSocial.leaderboardsManager.getGlobalPosition(leaderboardKey).intValue();
    }

    public static String RaveAchievementManagerGetAchievementByKey(String key) {
        RaveAchievement achievement = RaveSocial.achievementsManager.getAchievementByKey(key);
        return RaveAchievementToString(achievement);
    }

    public static String RaveAchievementsManagerAchievements() {
        List<RaveAchievement> objs = RaveSocial.achievementsManager.getAchievements();
        return RaveAchievementArrayToString(objs);
    }

    public static void RaveAchievementsManagerUpdateAchievements(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.achievementsManager.updateAchievements(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveAchievementsManagerUnlockAchievement(String achievementKey, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.achievementsManager.unlockAchievement(achievementKey, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    static boolean VersionAtLeast(String version, int major, int minor, int subminor) {
        String[] versionSections = version.split("\\-");
        String[] parts = versionSections[0].split("\\.");
        return Integer.parseInt(parts[0]) < major?false:(Integer.parseInt(parts[0]) > major?true:(Integer.parseInt(parts[1]) < minor?false:(Integer.parseInt(parts[1]) > minor?true:(parts.length <= 2?subminor == 0:Integer.parseInt(parts[2]) >= subminor))));
    }

    public static void RaveSocialPreInit() throws RaveException {
        if(!VersionAtLeast(RaveSocial.getSDKVersion(), 2, 10, 4)) {
            throw new RaveException("Rave Version too old. Minimum supported Rave Version for integration is 2.10.4");
        } else {
            RaveSettings.set(Android.UseDialogOverlay, true);
        }
    }

    public static void RaveSocialInitialize(final String callbackModule, final String callbackName, final String pid) {
        try {
            RaveSocialPreInit();
        } catch (RaveException var4) {
            UnityCompletionCallback(callbackModule, callbackName, pid, var4);
            return;
        }

        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            public void run() {
                RaveSocial.initializeRave(UnityPlayer.currentActivity, new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static void RaveSocialOnStart() {
        Log.d(TAG, "OnStart");
        RaveSocial.onStart(UnityPlayer.currentActivity);
    }

    public static void RaveSocialOnStop() {
        Log.d(TAG, "OnStop");
        RaveSocial.onStop(UnityPlayer.currentActivity);
    }

    public static boolean RaveSocialIsInitialized() {
        return RaveSocial.isInitialized();
    }

    public static boolean RaveSocialIsLoggedIn() {
        return RaveSocial.isLoggedIn();
    }

    public static boolean RaveSocialIsLoggedInAsGuest() {
        return RaveSocial.isLoggedInAsGuest();
    }

    public static boolean RaveSocialIsPersonalized() {
        return RaveSocial.isPersonalized();
    }

    public static boolean RaveSocialIsAuthenticated() {
        return RaveSocial.isAuthenticated();
    }

    public static void RaveSocialSetLoginStatusCallback(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.setLoginListener(new RaveLoginStatusListener() {
            public void onLoginStatusChanged(RaveLoginStatus loginStatus, RaveException exception) {
                CustomRaveSocialStatics.UnityLoginStatusCallback(callbackModule, callbackName, pid, loginStatus, exception);
            }
        });
    }

    public static void RaveSocialLoginAsGuest(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.loginAsGuest(new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static void RaveSocialLogOut(final String callbackModule, final String callbackName, final String pid) {
        if(pid.equalsIgnoreCase("NO_CALLBACK")) {
            RaveSocial.logOut((RaveCompletionListener)null);
        } else {
            RaveSocial.logOut(new RaveCompletionListener() {
                public void onComplete(RaveException exception) {
                    CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                }
            });
        }
    }

    public static void RaveSocialLoginWith(final String pluginKeyName, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveSocial.loginWith(pluginKeyName, new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        RaveSocial.getProgress().dismiss();
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static void RaveSocialConnectTo(final String pluginKeyName, final String callbackModule, final String callbackName, final String pid) {
        Activity activity = UnityPlayer.currentActivity;
        activity.runOnUiThread(new Runnable() {
            public void run() {
                RaveSocial.connectTo(pluginKeyName, new RaveCompletionListener() {
                    public void onComplete(RaveException exception) {
                        CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
                    }
                });
            }
        });
    }

    public static boolean RaveSocialIsPluginReady(String pluginName) {
        return RaveSocial.isPluginReady(pluginName);
    }

    public static void RaveSocialCheckReadinessOf(String pluginKeyName, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.authenticationManager.checkReadinessOf(pluginKeyName, new RaveReadinessListener() {
            public void onComplete(boolean isReady, RaveException exception) {
                CustomRaveSocialStatics.UnityReadinessCallback(callbackModule, callbackName, pid, isReady, exception);
            }
        });
    }

    public static void RaveSocialDisconnectFrom(String pluginKeyName, final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.disconnectFrom(pluginKeyName, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveSettingsGetSetting(String settingName) {
        return RaveSettings.get(settingName);
    }

    public static void RaveSettingsSetSetting(String settingName, String value) {
        RaveSettings.set(settingName, value);
    }

    public static void RaveConnectControllerControllerWithPlugin(String pluginName) {
        if(connectHandlers == null) {
            connectHandlers = new HashMap();
        }

        connectHandlers.put(pluginName, new CustomRaveSocialStatics.StaticConnectControllerHandler(pluginName));
    }

    public static void RaveConnectControllerSetObserver(String pluginName, String callbackModule, String callbackName, String pid) {
        CustomRaveSocialStatics.StaticConnectControllerHandler handler = (CustomRaveSocialStatics.StaticConnectControllerHandler)connectHandlers.get(pluginName);
        handler.callbackModule = callbackModule;
        handler.callbackName = callbackName;
        handler.pid = pid;
        handler.startController();
    }

    public static void RaveConnectControllerAttemptConnect(String pluginName) {
        final CustomRaveSocialStatics.StaticConnectControllerHandler handler = (CustomRaveSocialStatics.StaticConnectControllerHandler)connectHandlers.get(pluginName);
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            public void run() {
                if(handler != null && handler.controller != null) {
                    handler.controller.attemptConnect();
                }

            }
        });
    }

    public static void RaveConnectControllerAttemptDisconnect(String pluginName) {
        CustomRaveSocialStatics.StaticConnectControllerHandler handler = (CustomRaveSocialStatics.StaticConnectControllerHandler)connectHandlers.get(pluginName);
        if(handler.controller != null) {
            handler.controller.attemptDisconnect();
        }

    }

    public static void RaveConnectControllerDelete(String pluginName) {
        connectHandlers.remove(pluginName);
    }

    public static void RaveConnectFriendsControllerControllerWithPlugin(String pluginName) {
        if(friendHandlers == null) {
            friendHandlers = new HashMap();
        }

        friendHandlers.put(pluginName, new CustomRaveSocialStatics.StaticConnectFriendsControllerHandler(pluginName));
    }

    public static void RaveConnectFriendsControllerSetObserver(String pluginName, String callbackModule, String callbackName, String pid) {
        CustomRaveSocialStatics.StaticConnectFriendsControllerHandler handler = (CustomRaveSocialStatics.StaticConnectFriendsControllerHandler)friendHandlers.get(pluginName);
        handler.callbackModule = callbackModule;
        handler.callbackName = callbackName;
        handler.pid = pid;
        handler.startController();
    }

    public static void RaveConnectFriendsControllerAttemptGetFriends(String pluginName) {
        final CustomRaveSocialStatics.StaticConnectFriendsControllerHandler handler = (CustomRaveSocialStatics.StaticConnectFriendsControllerHandler)friendHandlers.get(pluginName);
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            public void run() {
                if(handler != null && handler.controller != null) {
                    handler.controller.attemptGetFriends();
                }

            }
        });
    }

    public static void RaveConnectFriendsControllerAttemptForgetFriends(String pluginName) {
        CustomRaveSocialStatics.StaticConnectFriendsControllerHandler handler = (CustomRaveSocialStatics.StaticConnectFriendsControllerHandler)friendHandlers.get(pluginName);
        if(handler.controller != null) {
            handler.controller.attemptForgetFriends();
        }

    }

    public static void RaveConnectFriendsControllerDelete(String pluginName) {
        friendHandlers.remove(pluginName);
    }

    public static void RaveUsersManagerAddCurrentUserObserver(String callbackModule, String callbackName, String pid) {
        if(userObservers == null) {
            userObservers = new HashMap();
        }

        CustomRaveSocialStatics.StaticCurrentUserObserver observer = new CustomRaveSocialStatics.StaticCurrentUserObserver();
        observer.callbackModule = callbackModule;
        observer.callbackName = callbackName;
        observer.pid = pid;
        userObservers.put(pid, observer);
        RaveSocial.usersManager.addCurrentUserObserver(observer);
    }

    public static void RaveUsersManagerRemoveCurrentUserObserver(String pid) {
        CustomRaveSocialStatics.StaticCurrentUserObserver observer = (CustomRaveSocialStatics.StaticCurrentUserObserver)userObservers.get(pid);
        if(observer != null) {
            RaveSocial.usersManager.removeCurrentUserObserver(observer);
            userObservers.remove(pid);
        }

    }

    public static void RaveUsersManagerPushUserChanges(final String callbackModule, final String callbackName, final String pid, String userChanges) {
        RaveUserChanges changes = StringToRaveUserChanges(userChanges);
        RaveSocial.usersManager.pushUserChanges(changes, new RaveCompletionListener() {
            public void onComplete(RaveException exception) {
                CustomRaveSocialStatics.UnityCompletionCallback(callbackModule, callbackName, pid, exception);
            }
        });
    }

    public static String RaveSocialGetCALState() {
        switch(RaveAuthentication.getCALState()) {
            case STATE_ENABLED:
                return "Enabled";
            case STATE_NO_PERMISSION:
                return "NoPermission";
            case STATE_OPTED_OUT:
                return "OptedOut";
            default:
                return "None";
        }
    }

    public static void RaveAppDataKeysManagerSetStateObserver(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.appDataKeysManager.setStateObserver(new RaveAppDataKeysStateObserver() {
            public void appDataKeyStateChanged(String selectedKey, List<String> unresolvedKeys) {
                String keyCollectionString = CustomRaveSocialStatics.StringListToString(unresolvedKeys);
                (new CustomRaveSocialStatics.UnityCallbackContext(callbackModule, callbackName, pid)).param(selectedKey).param(keyCollectionString).callback();
            }
        });
    }

    public static String RaveAppDataKeysManagerLastSelectedKey() {
        return RaveSocial.appDataKeysManager.getLastSelectedKey();
    }

    public static void RaveAppDataKeysManagerFetchCurrentState(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.appDataKeysManager.fetchCurrentState(new RaveAppDataKeysStateListener() {
            public void onComplete(String selectedKey, List<String> rejectedKeys, List<String> unresolvedKeys, RaveException exception) {
                String rejectedKeyCollectionString = CustomRaveSocialStatics.StringListToString(rejectedKeys);
                String unresolvedKeyCollectionString = CustomRaveSocialStatics.StringListToString(unresolvedKeys);
                (new CustomRaveSocialStatics.UnityCallbackContext(callbackModule, callbackName, pid)).param(selectedKey).param(rejectedKeyCollectionString).param(unresolvedKeyCollectionString).error(exception).callback();
            }
        });
    }

    public static void RaveAppDataKeysManagerFetchAvailable(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.appDataKeysManager.fetchAvailable(new AppDataKeyListListener() {
            public void onComplete(List<String> availableKeys, RaveException exception) {
                String keyCollectionString = CustomRaveSocialStatics.StringListToString(availableKeys);
                (new CustomRaveSocialStatics.UnityCallbackContext(callbackModule, callbackName, pid)).param(keyCollectionString).error(exception).callback();
            }
        });
    }

    public static void RaveAppDataKeysManagerFetchSelected(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.appDataKeysManager.fetchSelected(new AppDataKeyListener() {
            public void onComplete(String selectedKey, RaveException exception) {
                (new CustomRaveSocialStatics.UnityCallbackContext(callbackModule, callbackName, pid)).param(selectedKey).error(exception).callback();
            }
        });
    }

    public static void RaveAppDataKeysManagerFetchUnresolved(final String callbackModule, final String callbackName, final String pid) {
        RaveSocial.appDataKeysManager.fetchUnresolved(new AppDataKeyListListener() {
            public void onComplete(List<String> availableKeys, RaveException exception) {
                String keyCollectionString = CustomRaveSocialStatics.StringListToString(availableKeys);
                (new CustomRaveSocialStatics.UnityCallbackContext(callbackModule, callbackName, pid)).param(keyCollectionString).error(exception).callback();
            }
        });
    }

    public static void RaveAppDataKeysManagerSelectKey(String callbackModule, String callbackName, String pid, String key) {
        RaveSocial.appDataKeysManager.selectKey(key, CompletionCallback(callbackModule, callbackName, pid));
    }

    public static void RaveAppDataKeysManagerDeactivateKey(String callbackModule, String callbackName, String pid, String key) {
        RaveSocial.appDataKeysManager.deactivateKey(key, CompletionCallback(callbackModule, callbackName, pid));
    }

    public static void RaveAppDataKeysManagerFetchUserKey(String callbackModule, String callbackName, String pid, String raveId) {
        RaveSocial.appDataKeysManager.fetchUserKey(raveId, AppDataKeyCallback(callbackModule, callbackName, pid));
    }

    public static void RaveAppDataKeysManagerFetchUserKeySet(String callbackModule, String callbackName, String pid, String raveIds) {
        List<String> idList = StringToStringList(raveIds);
        RaveSocial.appDataKeysManager.fetchUserKeySet(idList, AppDataKeySetCallback(callbackModule, callbackName, pid));
    }

    public static void RaveAppDataKeysManagerFetchUserKeySetForContacts(String callbackModule, String callbackName, String pid) {
        RaveSocial.appDataKeysManager.fetchUserKeySetForContacts(AppDataKeySetCallback(callbackModule, callbackName, pid));
    }

    public static void RaveSocialSetMergePolicy(String callbackModule, String callbackName, String pid) {
        RaveSocial.setMergePolicy(new CustomRaveSocialStatics.MergePolicy(callbackModule, callbackName, pid));
    }

    private static String MergeUserToString(RaveMergeUser targetUser) {
        StringBuilder builder = new StringBuilder(RaveUserToString(targetUser));
        SafeAppend(builder, targetUser.getSelectedAppDataKey(), "<RaveUser>");
        return builder.toString();
    }

    public static void RaveSocialMergeDecision(String shouldMerge) {
        RaveMergePolicy policy = ((RaveCoreAuthentication)RaveSocial.authenticationManager).getMergePolicy();
        if(policy instanceof CustomRaveSocialStatics.MergePolicy) {
            CustomRaveSocialStatics.MergePolicy unityMergePolicy = (CustomRaveSocialStatics.MergePolicy)policy;
            if(unityMergePolicy.decisionListener != null) {
                unityMergePolicy.decisionListener.mergeDecision(shouldMerge.compareToIgnoreCase("true") == 0);
            }
        }

    }

    public static String RaveSocialVolatileRaveId() {
        final String[] volatileRaveId = new String[1];
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            public void run() {
                String[] var1 = volatileRaveId;
                synchronized(volatileRaveId) {
                    volatileRaveId[0] = RaveSocial.getVolatileRaveId();
                    volatileRaveId.notify();
                }
            }
        });

        try {
            synchronized(volatileRaveId) {
                volatileRaveId.wait();
                return volatileRaveId[0];
            }
        } catch (Exception var4) {
            Log.e("UnityBindings", "Exception getting volatileRaveId", var4);
            return null;
        }
    }

    static class MergePolicy extends CustomRaveSocialStatics.UnityCallbackContext implements RaveMergePolicy {
        public RaveMergeDecisionListener decisionListener;

        MergePolicy(String callbackModule, String callbackName, String pid) {
            super(callbackModule, callbackName, pid);
        }

        public void makeUserMergeDecision(RaveMergeUser targetUser, RaveMergeDecisionListener listener) {
            this.decisionListener = listener;
            this.param(CustomRaveSocialStatics.MergeUserToString(targetUser)).callback();
        }
    }

    public static class StaticCurrentUserObserver implements RaveCurrentUserObserver {
        public String callbackModule;
        public String callbackName;
        public String pid;

        public StaticCurrentUserObserver() {
        }

        public void userChanged(Collection<String> changedKeys) {
            String keyCollectionString = CustomRaveSocialStatics.StringListToString(changedKeys);
            UnityMessageQueueRunner.AddMessageToQueue(this.callbackModule, this.callbackName, String.format("%s|%s", new Object[]{this.pid, keyCollectionString}));
        }
    }

    public static class StaticConnectFriendsControllerHandler implements RaveConnectFriendsStateObserver {
        public String callbackModule;
        public String callbackName;
        public String pid;
        public String pluginName;
        public RaveConnectFriendsController controller;

        public StaticConnectFriendsControllerHandler(String plugin) {
            this.pluginName = plugin;
        }

        public void onFindFriendsStateChanged(RaveFindFriendsState value) {
            UnityMessageQueueRunner.AddMessageToQueue(this.callbackModule, this.callbackName, String.format("%s|%d", new Object[]{this.pid, Integer.valueOf(value.ordinal())}));
        }

        public void startController() {
            this.controller = new RaveConnectFriendsController(this.pluginName);
            this.controller.setFriendsObserver(this);
        }
    }

    public static class StaticConnectControllerHandler implements RaveConnectStateObserver {
        public String callbackModule;
        public String callbackName;
        public String pid;
        public String pluginName;
        public RaveConnectController controller;

        public StaticConnectControllerHandler(String plugin) {
            this.pluginName = plugin;
        }

        public void onConnectStateChanged(ConnectState value) {
            UnityMessageQueueRunner.AddMessageToQueue(this.callbackModule, this.callbackName, String.format("%s|%d", new Object[]{this.pid, Integer.valueOf(value.ordinal())}));
        }

        public void startController() {
            this.controller = new RaveConnectController(this.pluginName);
            this.controller.setObserver(this);
        }
    }

    public static class UnityCallbackContext {
        private StringBuilder parameters;
        private final String callbackName;
        private final String callbackModule;
        private final String pid;

        UnityCallbackContext(String callbackModule, String callbackName, String pid) {
            this.callbackModule = callbackModule;
            this.callbackName = callbackName;
            this.pid = pid;
            this.parameters = new StringBuilder(pid);
        }

        public CustomRaveSocialStatics.UnityCallbackContext param(String param) {
            this.parameters.append("|" + param);
            return this;
        }

        public CustomRaveSocialStatics.UnityCallbackContext error(RaveException error) {
            this.parameters.append("|" + CustomRaveSocialStatics.ErrorToString(error));
            return this;
        }

        public void callback() {
            UnityMessageQueueRunner.AddMessageToQueue(this.callbackModule, this.callbackName, this.parameters.toString());
            this.parameters = new StringBuilder(this.pid);
        }
    }
}
