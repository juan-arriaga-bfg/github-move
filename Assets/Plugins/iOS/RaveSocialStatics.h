#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveSocial.h>

struct RaveUserStruct {
	bool isGuest;
	char* displayName;
	char* realName;
	char* username;
	char* email;
	char* raveId;
	char* facebookId;
	char* twitterId;
	char* googlePlusId;
	char* thirdPartyId;
	char* gender;
	char* pictureURL;
};

struct RaveUnityError {
    int code;
    char* domain;
};

extern char* MakeStringCopy(NSString* string);
extern NSString* SafeString(const char* s);
extern NSMutableDictionary* StringToNSDictionary(const char* s);
extern NSString* BoolToString(bool b);
extern void SafeAppend(NSMutableString* base,NSString* toAppend,NSString* delim);
extern NSString* ErrorToString(NSError* anError);

extern void UnityCompletionCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSError* anError);
RaveCompletionCallback UnityCompletionCallbackFactory(const char * callbackModule, const char * callbackName, const char * pid);
extern void UnitySceneCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSError* error);
extern void UnityLoginSceneCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, BOOL loggedIn, BOOL accountCreated, NSString* pluginKeyName, NSError * error);
extern void UnityAccountInfoSceneCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, BOOL loggedOut, NSError * error);

extern void RaveSocialPreInit();
extern void RaveSocialInitialize(const char* callbackModule, const char* callbackName, const char* pid);
extern bool RaveSocialIsLoggedIn();
extern bool RaveSocialIsAnonymous();
extern void RaveSocialGetUser(struct RaveUserStruct* data);
extern bool RaveSocialIsPluginReady(const char* pluginKeyName);

extern void RaveSocialUpdateLeaderboards(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveSocialLoginStatusCallback(const char* callbackModule, const char* callbackName, const char* pid);

extern const char* RaveUsersManagerGetUserById(const char* raveId);
extern void RaveUsersManagerUpdateUserById(const char* raveId, const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveUsersManagerCurrent();
extern void RaveUsersManagerUpdateCurrent(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerPushCurrent(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerCheckAccountExists(const char* email, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerCheckThirdPartyAccountExists(const char* email, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerFetchAccessToken(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerFetchRandomUsersForApplication(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerFetchRandomUsersForApplication2(const char* appUuid, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerFetchRandomUsersForApplication3(const char* appUuid, float excludeContacts, float limit, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerFetchAllIdentities(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerFetchIdentitiesForApplication(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveUsersManagerPushProfilePicture(const char* callbackModule, const char* callbackName, const char* pid, const char* url);

extern const char* RaveGiftsManagerGetGiftTypeByName(const char* name);
extern const char* RaveGiftsManagerGetGiftTypeById(const char* typeId);
extern const char* RaveGiftsManagerGetGiftTypeByKey(const char* typeKey);
extern const char* RaveGiftsManagerGiftTypes();
extern void RaveGiftsManagerUpdateGiftTypes(const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveGiftsManagerGetGiftById(const char* giftId);
extern const char* RaveGiftsManagerGetGiftRequestById(const char* requestId);
extern const char* RaveGiftsManagerGifts();
extern void RaveGiftsManagerUpdateGifts(const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveGiftsManagerGiftRequests();
extern void RaveGiftsManagerUpdateGiftRequests(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerSendGiftsWithKey(const char* giftTypeKey, const char* users, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerSendGiftsWithKey2(const char* giftTypeKey, const char* userIds, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerSendGiftsWithKey3(const char* giftTypeKey, const char* contacts, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerSendGiftsAndShareWithKey(const char* giftTypeKey, const char* contacts, const char* subject, const char* message, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerSendGiftsAndShareWithKey2(const char* giftTypeKey, const char* pluginKeyName, const char* contacts, const char* subject, const char* message, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerAcceptGift(const char* gift, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerAcceptGiftById(const char* giftId, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerRejectGift(const char* gift, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerRejectGiftById(const char* giftId, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerRequestGiftWithKey(const char* giftTypeKey, const char* users, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerRequestGiftWithKey2(const char* giftTypeKey, const char* userIds, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerGrantGiftRequest(const char* request, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerGrantGiftRequestById(const char* requestId, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerIgnoreGiftRequest(const char* request, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerIgnoreGiftRequestById(const char* requestId, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerFetchGiftContentForShareInstall(const char* appCallUrl, const char* source, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerAttachGiftWithKey(const char* giftTypeKey, const char* requests, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerFetchGiftKeyForExternalId(const char* externalId, const char* source, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerDetachGiftKeyForExternalId(const char* externalId, const char* source, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerSendGifts(const char* giftTypeId, const char* users, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerSendGifts2(const char* giftTypeId, const char* userIds, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerRequestGift(const char* giftTypeId, const char* users, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveGiftsManagerRequestGift2(const char* giftTypeId, const char* userIds, const char* callbackModule, const char* callbackName, const char* pid);

extern const char* RaveContactsManagerCreateContactInstance(const char* source, const char* externalId, const char* displayName);
extern const char* RaveContactsManagerCreateContactInstance2(const char* source, const char* externalId, const char* displayName, const char* pictureUrl);
extern const char* RaveContactsManagerPhonebookAutoUpdateEnabled();
extern const char* RaveContactsManagerAll();
extern void RaveContactsManagerUpdateAll(const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveContactsManagerAllUsingThisApplication();
extern void RaveContactsManagerUpdateAllUsingThisApplication(const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveContactsManagerFacebook();
extern void RaveContactsManagerUpdateFacebook(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveContactsManagerAddContactsByUsername(const char* usernames, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveContactsManagerDeleteContact(const char* userUuid, const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveContactsManagerGetAllUsingApplication(const char* appId);
extern void RaveContactsManagerUpdateAllUsingApplication(const char* appId, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveContactsManagerFetchAllExternal(float filter, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveContactsManagerFetchExternalFrom(const char* pluginKeyName, float filter, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveContactsManagerInviteToAppVia(const char* pluginKeyName, const char* contacts, const char* invitation, const char* callbackModule, const char* callbackName, const char* pid);

extern const char* RaveSharingManagerPluginType();
extern const char* RaveSharingManagerRequestIds();
extern const char* RaveSharingManagerUserIds();
extern void RaveSharingManagerTweet(const char* tweet, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveSharingManagerTweet2(const char* tweet, const char* image, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveSharingManagerPostToFacebook(const char* wallPost, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveSharingManagerPostToFacebook2(const char* wallPost, const char* image, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveSharingManagerPostToGooglePlus(const char* postText, const char* imageURL, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveSharingManagerShareWith(const char* externalContacts, const char* subject, const char* message, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveSharingManagerShareWith2(const char* externalContacts, const char* pluginKeyName, const char* subject, const char* message, const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveSharingManagerGetExternalIdForShareInstall(const char* appCallUrl, const char* source);


extern const char* RaveLeaderboardManagerLeaderboards();
extern void RaveLeaderboardManagerUpdateLeaderboards(const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveLeaderboardManagerGetLeaderboardByKey(const char* leaderboardkey);
extern void RaveLeaderboardManagerUpdateLeaderboardByKey(const char* leaderboardKey, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveLeaderboardManagerSubmitScoreByKey(const char* leaderboardKey, float score, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveLeaderboardManagerUpdateGlobalScoresByKey(const char* leaderboardKey, float page, float pageSize, const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveLeaderboardManagerGetGlobalScoresByKey(const char* leaderboardKey, float page, float pageSize);
extern const char* RaveLeaderboardManagerGetFriendsScoresByKey(const char* leaderboardKey, float page, float pageSize);
extern void RaveLeaderboardManagerUpdateFriendsScoresByKey(const char* leaderboardKey, float page, float pageSize, const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveLeaderboardManagerUpdateMyGlobalScoresByKey(const char* leaderboardKey, float pageSize, const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveLeaderboardManagerGetMyGlobalScoresByKey(const char* leaderboardKey, float pageSize);
extern const char* RaveLeaderboardManagerGetMyFriendsScoresByKey(const char* leaderboardKey, float pageSize);
extern void RaveLeaderboardManagerUpdateMyFriendsScoresByKey(const char* leaderboardKey, float pageSize, const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey(const char* leaderboardKey, float adjacent);
extern void RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey(const char* leaderboardKey, float adjacent, const char* callbackModule, const char* callbackName, const char* pid);
extern const char* RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey(const char* leaderboardKey, float adjacent);
extern void RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey(const char* leaderboardKey, float adjacent, const char* callbackModule, const char* callbackName, const char* pid);
extern float RaveLeaderboardManagerGetHighScoreByKey(const char* leaderboardKey);
extern float RaveLeaderboardManagerGetFriendsPositionByKey(const char* leaderboardKey);
extern float RaveLeaderboardManagerGetGlobalPositionByKey(const char* leaderboardKey);

extern const char* RaveAchievementsManagerGetAchievementByKey(const char* key);
extern const char* RaveAchievementsManagerAchievements();
extern void RaveAchievementsManagerUpdateAchievements(const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveAchievementsManagerUnlockAchievement(const char* achievementKey, const char* callbackModule, const char* callbackName, const char* pid);

extern void RaveConnectFriendsControllerControllerWithPlugin(const char* pluginName);
extern void RaveConnectFriendsControllerSetObserver(const char* pluginName,const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveConnectFriendsControllerAttemptGetFriends(const char* pluginName);
extern void RaveConnectFriendsControllerAttemptForgetFriends(const char* pluginName);
extern void RaveConnectFriendsControllerDelete(const char* pluginName);

extern void RaveConnectControllerControllerWithPlugin(const char* pluginName);
extern void RaveConnectControllerSetObserver(const char* pluginName,const char* callbackModule, const char* callbackName, const char* pid);
extern void RaveConnectControllerAttemptConnect(const char* pluginName);
extern void RaveConnectControllerAttemptDisconnect(const char* pluginName);
extern void RaveConnectControllerDelete(const char* pluginName);
