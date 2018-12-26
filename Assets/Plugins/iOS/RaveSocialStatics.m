#import <bfg_iOS_sdk/RaveSocial.h>
#import <bfg_iOS_sdk/RaveUtilities.h>
#import <bfg_iOS_sdk/RaveScene.h>
#import <bfg_iOS_sdk/RaveUser.h>
#import <bfg_iOS_sdk/RaveUsersManager.h>
#import <bfg_iOS_sdk/RaveLeaderboardManager.h>
#import <bfg_iOS_sdk/RaveAppDataKeyUserPair.h>
#import "RaveSocialStatics.h"

#import <bfg_iOS_sdk/RaveGiftsManager.h>
#import <bfg_iOS_sdk/RaveGift.h>
#import <bfg_iOS_sdk/RaveContactsManager.h>
#import <bfg_iOS_sdk/RaveSharingManager.h>
#import <bfg_iOS_sdk/RaveAchievementsManager.h>
#import <bfg_iOS_sdk/RaveConnectFriendsController.h>
#import <bfg_iOS_sdk/RaveAuthenticationManager.h>
#import <bfg_iOS_sdk/RaveAppDataKeysManager.h>
#import <bfg_iOS_sdk/RaveMergePolicy.h>

@interface NSError (RaveUnity)
- (NSString *)raveUnity_errorString;
@end

@implementation NSError (RaveUnity)
- (NSString *)raveUnity_errorString {
    return ErrorToString(self);
}
@end

@interface UnityCallbackContext : NSObject
+ (instancetype)context:(const char *)callbackModule callbackName:(const char *)callbackName pid:(const char *)pid;
- (instancetype)param:(NSString *)param;
- (instancetype)error:(NSError *)error;
- (void)callback;

@property (nonatomic, copy) NSString * callbackModule;
@property (nonatomic, copy) NSString * callbackName;
@property (nonatomic, copy) NSString * pid;
@property (nonatomic, retain) NSMutableString * parameters;
@end

@implementation UnityCallbackContext
@synthesize callbackModule;
@synthesize callbackName;
@synthesize pid;
@synthesize parameters;

+ (instancetype)context:(const char *)callbackModule callbackName:(const char *)callbackName pid:(const char *)pid {
    UnityCallbackContext * context = [self new];
    #if !__has_feature(objc_arc)
        context = [context autorelease];
    #endif
    context.callbackModule = SafeString(callbackModule);
    context.callbackName = SafeString(callbackName);
    context.pid = SafeString(pid);
    context.parameters = [NSMutableString stringWithString:context.pid];
    return context;
}

#if !__has_feature(objc_arc)
- (void)dealloc {
    [callbackModule release];
    [callbackName release];
    [parameters release];
    [pid release];
    [super dealloc];
}
#endif

- (instancetype)append:(id)param {
    [self.parameters appendString:@"|"];
    if (param != nil) {
        [self.parameters appendString:param];
    }
    return self;
}

- (instancetype)param:(NSString *)param {
    return [self append:param];
}

- (instancetype)error:(NSError *)error {
    return [self append:error.raveUnity_errorString];
}

- (void)callback {
    UnitySendMessage([self.callbackModule UTF8String], [self.callbackName UTF8String], [self.parameters UTF8String]);
    self.parameters = [NSMutableString stringWithString:self.pid];
}
@end

char* MakeStringCopy (NSString* string) {
    if (string == NULL) return NULL;
    unsigned long utf8Length = strlen([string UTF8String]);
    char* res = (char*)malloc(utf8Length + 1);
    strncpy(res, [string UTF8String], utf8Length);
    res[utf8Length] = 0;
    return res;
}

const char* StringForUnity(const char* string)
{
    if (string == NULL)
        return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

NSString* BoolToString(bool b)
{
    return b ? @"True" : @"False";
}

void SafeAppend(NSMutableString* base,NSString* toAppend,NSString* delim)
{
    if(toAppend)
    {
        [base appendString:toAppend];
    }
    else
    {
        [base appendString:@"NULL"];
    }
    [base appendString:delim];
}

NSString* SafeString(const char* s)
{
    if(s)
    {
        return [NSString stringWithUTF8String:s];
    }
    else
    {
        return @"NULL";
    }
}

NSString * ParamString(const char * s) {
    NSString * safeString = SafeString(s);
    if ([safeString isEqualToString:@"NULL"]) {
        return nil;
    }
    return safeString;
}

NSString* StringArrayToString(NSArray* array)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delim = @"<ListRaveString>";
    for (NSString* string in array)
    {
        SafeAppend(s,string,delim);
    }
    return s;
}

NSArray* StringToStringArray(const char* s)
{
    if (strlen(s) == 0) {
        return nil;
    }
    NSMutableArray* ret = [[[NSString stringWithUTF8String:s] componentsSeparatedByString:@"<ListRaveString>"] mutableCopy];
    for (int i=0; i<[ret count]; i++) {
        if([ret[i] isEqualToString:@"NULL"]) {
            ret[i] = nil;
        }
    }
    return ret;
}

NSString* NSDictionaryToString(NSDictionary* dict)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delim = @"<DictRaveString>";
    for (NSString* key in dict) {
        SafeAppend(s,key,delim);
        SafeAppend(s,[dict objectForKey:key],delim);
    }
    return s;
}

NSMutableDictionary* StringToNSDictionary(const char* s)
{
    if (strlen(s) == 0) {
        return nil;
    }
    NSMutableDictionary* d = [NSMutableDictionary dictionary];
    NSArray* data = [[NSString stringWithUTF8String:s] componentsSeparatedByString:@"<DictRaveString>"];
    for (int i=1; i<[data count]; i+=2) {
        if(![data[i] isEqualToString:@"NULL"]) {
            [d setObject:data[i] forKey:data[i-1]];
        }
    }
    return d;
}

NSString* NSNumberToString(NSNumber* number)
{
    if(number)
        return [NSString stringWithFormat:@"%@",number,nil];
    else
        return @"0";
}

static NSDateFormatter* dateFormatter = nil;

NSDateFormatter* getDateFormatter()
{
    if(dateFormatter == nil) {
        dateFormatter = [[NSDateFormatter alloc] init];
        [dateFormatter setDateFormat:@"MM/dd/yyyy"];
    }
    return dateFormatter;
}

NSString* DateToString(NSDate* date)
{
    return [getDateFormatter() stringFromDate:date];
}

NSDate* StringToDate(const char* s)
{
    return [getDateFormatter() dateFromString:[NSString stringWithUTF8String:s]];
}

NSDate* NSStringToDate(NSString* s)
{
    return [getDateFormatter() dateFromString:s];
}

NSMutableString* RaveUserToString(id<RaveUser> obj)
{
    if (obj == nil) {
        return [NSMutableString stringWithString:@"NULL"];
    }
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveUser>";
    SafeAppend(s,BoolToString(obj.isGuest),delimiter);
    SafeAppend(s,obj.displayName,delimiter);
    SafeAppend(s,obj.realName,delimiter);
    SafeAppend(s,obj.username,delimiter);
    SafeAppend(s,obj.email,delimiter);
    SafeAppend(s,DateToString(obj.birthdate),delimiter);
    SafeAppend(s,obj.raveId,delimiter);
    SafeAppend(s,obj.facebookId,delimiter);
    SafeAppend(s,obj.googlePlusId,delimiter);
    SafeAppend(s,obj.thirdPartyId,delimiter);
    SafeAppend(s,obj.gender,delimiter);
    SafeAppend(s,obj.pictureURL,delimiter);
    return s;
}

id<RaveUser> UserIdToRaveUser(NSString* raveId)
{
    return [RaveSocial.usersManager getUserById:raveId];
}

NSString* RaveUserArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveUser>";
    for (id<RaveUser> item in objs) {
        SafeAppend(s,RaveUserToString(item), delimiter);
    }
    return s;
}

NSString* StringOrNil(NSString* string)
{
    if([string isEqualToString:@"NULL"]) {
        return nil;
    }
    return string;
}

NSDate* DateOrNil(NSString* string)
{
    if([string isEqualToString:@"NULL"]) {
        return nil;
    }
    return NSStringToDate(string);
}

RaveUserChanges* StringToRaveUserChanges(NSString* objs)
{
    if ([objs length] == 0) {
        return nil;
    }
    NSString* delimiter = @"<RaveUserChanges>";
    NSArray* parts = [objs componentsSeparatedByString:delimiter];
    RaveUserChanges* changes = [RaveUserChanges userChanges];
    changes.displayName = StringOrNil(parts[0]);
    changes.realName = StringOrNil(parts[1]);
    changes.email = StringOrNil(parts[2]);
    changes.birthdate = DateOrNil(parts[3]);
    changes.gender = StringOrNil(parts[4]);

    return changes;
}


NSString* RaveGiftTypeToString(id<RaveGiftType> obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveGiftType>";
    SafeAppend(s,obj.typeId,delimiter);
    SafeAppend(s,obj.typeKey,delimiter);
    SafeAppend(s,obj.name,delimiter);
    SafeAppend(s,BoolToString(obj.canRequest),delimiter);
    SafeAppend(s,BoolToString(obj.canGift),delimiter);
    return s;
}

id<RaveGiftType> GiftTypeKeyToRaveGiftType(NSString* giftTypeKey)
{
    return [RaveSocial.giftsManager getGiftTypeByKey:giftTypeKey];
}

NSString* RaveGiftTypeArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveGiftType>";
    for (id<RaveGiftType> item in objs) {
        SafeAppend(s,RaveGiftTypeToString(item), delimiter);
    }
    return s;
}


NSString* RaveGiftToString(id<RaveGift> obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveGift>";
    SafeAppend(s,obj.giftId,delimiter);
    SafeAppend(s,obj.giftTypeKey,delimiter);
    SafeAppend(s,obj.source,delimiter);
    SafeAppend(s,BoolToString(obj.isFromGift),delimiter);
    SafeAppend(s,BoolToString(obj.isFromRequest),delimiter);
    SafeAppend(s,DateToString(obj.timeSent),delimiter);
    SafeAppend(s,RaveGiftTypeToString(obj.giftType),delimiter);
    SafeAppend(s,RaveUserToString(obj.sender),delimiter);
    SafeAppend(s,obj.senderRaveId,delimiter);
    return s;
}

id<RaveGift> GiftIdToRaveGift(NSString* giftId)
{
    return [RaveSocial.giftsManager getGiftById:giftId];
}

NSString* RaveGiftArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveGift>";
    for (id<RaveGift> item in objs) {
        SafeAppend(s,RaveGiftToString(item), delimiter);
    }
    return s;
}


NSString* RaveGiftRequestToString(id<RaveGiftRequest> obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveGiftRequest>";
    SafeAppend(s,obj.requestId,delimiter);
    SafeAppend(s,RaveGiftTypeToString(obj.giftType),delimiter);
    SafeAppend(s,obj.giftTypeKey,delimiter);
    SafeAppend(s,RaveUserToString(obj.requester),delimiter);
    SafeAppend(s,DateToString(obj.timeSent),delimiter);
    SafeAppend(s,obj.requesterRaveId,delimiter);
    return s;
}

id<RaveGiftRequest> GiftRequestIdToRaveGiftRequest(NSString* giftRequestId)
{
    return [RaveSocial.giftsManager getGiftRequestById:giftRequestId];
}

NSString* RaveGiftRequestArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveGiftRequest>";
    for (id<RaveGiftRequest> item in objs) {
        SafeAppend(s,RaveGiftRequestToString(item), delimiter);
    }
    return s;
}


NSString* RaveContactGiftResultToString(RaveContactGiftResult* obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveContactGiftResult>";
    SafeAppend(s,obj.pluginKeyName,delimiter);
    SafeAppend(s,StringArrayToString(obj.externalIds),delimiter);
    return s;
}


NSString* RaveContactGiftResultArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveContactGiftResult>";
    for (RaveContactGiftResult* item in objs) {
        SafeAppend(s,RaveContactGiftResultToString(item), delimiter);
    }
    return s;
}


NSString* RaveShareRequestToString(RaveShareRequest* obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveShareRequest>";
    SafeAppend(s,obj.pluginKeyName,delimiter);
    SafeAppend(s,StringArrayToString(obj.requestIds),delimiter);
    SafeAppend(s,StringArrayToString(obj.userIds),delimiter);
    return s;
}

RaveShareRequest* StringToRaveShareRequest(NSString* s)
{
#if !__has_feature(objc_arc)
    RaveShareRequest * shareRequest = [[RaveShareRequest new] autorelease];
#else
    RaveShareRequest * shareRequest = [RaveShareRequest new];
#endif
    NSString* delimiter = @"<RaveShareRequest>";
    NSArray* parts = [s componentsSeparatedByString:delimiter];

    if(![parts[0] isEqualToString:@"NULL"])
        shareRequest.pluginKeyName = parts[0];
    else
        shareRequest.pluginKeyName = nil;

    shareRequest.requestIds = StringToStringArray([parts[1] UTF8String]);
    shareRequest.userIds = StringToStringArray([parts[2] UTF8String]);
    return shareRequest;
}

NSString* RaveShareRequestArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveShareRequest>";
    for (RaveShareRequest* item in objs) {
        SafeAppend(s,RaveShareRequestToString(item), delimiter);
    }
    return s;
}

NSArray* StringToRaveShareRequestArray(const char* s)
{
    if (strlen(s) == 0) {
        return nil;
    }
    NSString* delimiter = @"<ListRaveShareRequest>";
    NSArray* parts = [[NSString stringWithUTF8String:s] componentsSeparatedByString:delimiter];
    NSMutableArray* requests = [NSMutableArray array];
    for (NSString* part in parts) {
        [requests addObject:StringToRaveShareRequest(part)];
    }
    return requests;
}

NSString* RaveContactToString(id<RaveContact> obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveContact>";
    SafeAppend(s,RaveUserToString(obj.user),delimiter);
    SafeAppend(s,BoolToString(obj.isEmail),delimiter);
    SafeAppend(s,BoolToString(obj.isRaveSocial),delimiter);
    SafeAppend(s,BoolToString(obj.isFacebook),delimiter);
    SafeAppend(s,BoolToString(obj.isGooglePlus),delimiter);
    SafeAppend(s,NSDictionaryToString(obj.externalIds),delimiter);
    SafeAppend(s,obj.displayName,delimiter);
    SafeAppend(s,obj.pictureURL,delimiter);
    SafeAppend(s,obj.thirdPartySource,delimiter);
    SafeAppend(s,obj.key,delimiter);
    return s;
}

NSString* RaveAppDataKeyUserPairToString(RaveAppDataKeyUserPair *pair) {
    NSMutableString *s = [NSMutableString string];
    NSString *delimiter = @"<RaveAppDataKeyUserPair>";
    SafeAppend(s,pair.raveId,delimiter);
    SafeAppend(s,pair.appDataKey,delimiter);
    return s;
}

id<RaveContact> UserIdToRaveContact(NSString* raveId)
{
    return [RaveSocial.contactsManager getContactByRaveId:raveId];
}

//  a contact without a user (and therefore a userId) should be mapped with this function
//  such a contact created by us will have one source->externalId mapping in the externalIds dictionary
id<RaveContact> ContactInfoToRaveContact(NSString* source, NSString* externalId, NSString* displayName,NSString* pictureURL)
{
    NSString  * o_pictureURL = nil;  //   nil is a valid value
    if( ![pictureURL isEqualToString:@"NULL"] )
        o_pictureURL = pictureURL;

    NSString  * o_displayName = nil;  //   nil is a valid value
    if( ![displayName isEqualToString:@"NULL"] )
        o_displayName = displayName;

    return [RaveSocial.contactsManager createContactInstance:source externalId:externalId displayName:o_displayName pictureUrl:o_pictureURL];
}

id<RaveContact> StringToRaveContact(NSString* s)
{
    NSString* delimiter = @"<RaveContact>";
    NSArray* parts = [s componentsSeparatedByString:delimiter];
    NSArray* key = [parts[0] componentsSeparatedByString:@":"];
    if (key.count == 1) {
        return UserIdToRaveContact(key[0]);
    }

    return ContactInfoToRaveContact(key[0], key[1], parts[1], parts[2]);
}


NSString* RaveContactArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveContact>";
    for (id<RaveContact> item in objs) {
        SafeAppend(s,RaveContactToString(item), delimiter);
    }
    return s;
}

NSString* RaveAppDataKeyUserPairArrayToString(NSArray *objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveAppDataKeyUserPair>";
    for (RaveAppDataKeyUserPair *pair in objs) {
        SafeAppend(s,RaveAppDataKeyUserPairToString(pair), delimiter);
    }
    return s;
}

NSArray* StringToRaveContactArray(const char* s)
{
    if (strlen(s) == 0) {
        return nil;
    }
    NSString* delimiter = @"<ListRaveContact>";
    NSArray* parts = [[NSString stringWithUTF8String:s] componentsSeparatedByString:delimiter];
    NSMutableArray* contacts = [NSMutableArray array];
    for (NSString* part in parts) {
        [contacts addObject:StringToRaveContact(part)];
    }
    return contacts;
}

NSString* RaveScoreToString(id<RaveScore> obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveScore>";
    SafeAppend(s,NSNumberToString(obj.score),delimiter);
    SafeAppend(s,NSNumberToString(obj.position),delimiter);
    SafeAppend(s,obj.userDisplayName,delimiter);
    SafeAppend(s,obj.userPictureUrl,delimiter);
    SafeAppend(s,obj.userRaveId,delimiter);
    return s;
}

NSString* RaveScoreArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveScore>";
    for (id<RaveScore> item in objs) {
        SafeAppend(s,RaveScoreToString(item), delimiter);
    }
    return s;
}

NSString* RaveLeaderboardToString(id<RaveLeaderboard> obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveLeaderboard>";
    SafeAppend(s,obj.name,delimiter);
    SafeAppend(s,obj.key,delimiter);
    SafeAppend(s,obj.desc,delimiter);
    SafeAppend(s,NSNumberToString(obj.sorter),delimiter);
    SafeAppend(s,BoolToString(obj.isAscending),delimiter);
    SafeAppend(s,NSNumberToString(obj.highScore),delimiter);
    SafeAppend(s,NSNumberToString(obj.friendsPosition),delimiter);
    SafeAppend(s,NSNumberToString(obj.globalPosition),delimiter);
    return s;
}

id<RaveLeaderboard> LeaderboardKeyToRaveLeaderboard(NSString* leaderboardKey)
{
    return [RaveSocial.leaderboardsManager getLeaderboardByKey:leaderboardKey];
}

NSString* RaveLeaderboardArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveLeaderboard>";
    for (id<RaveLeaderboard> item in objs) {
        SafeAppend(s,RaveLeaderboardToString(item), delimiter);
    }
    return s;
}

NSString* RaveAchievementToString(id<RaveAchievement> obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<RaveAchievement>";
    SafeAppend(s,BoolToString(obj.isUnlocked),delimiter);
    SafeAppend(s,obj.achievementDescription,delimiter);
    SafeAppend(s,obj.name,delimiter);
    SafeAppend(s,obj.key,delimiter);
    SafeAppend(s,obj.imageURL,delimiter);
    return s;
}

id<RaveAchievement> AchievementKeyToRaveAchievement(NSString* achievementKey)
{
    return [RaveSocial.achievementsManager getAchievementByKey:achievementKey];
}

NSString* RaveAchievementArrayToString(NSArray* objs)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<ListRaveAchievement>";
    for (id<RaveAchievement> item in objs) {
        SafeAppend(s,RaveAchievementToString(item), delimiter);
    }
    return s;
}


NSString* ErrorToString(NSError* anError)
{
    if(anError)
    {
        return [NSString stringWithFormat:@"Error:%ld:%@", (long)anError.code, [anError localizedDescription]];
    }
    return @"";
}

RaveCompletionCallback UnityCompletionCallbackFactory(const char * callbackModule, const char * callbackName, const char * pid) {
    NSString * s_callbackModule = SafeString(callbackModule);
    NSString * s_callbackName = SafeString(callbackName);
    NSString * s_pid = SafeString(pid);

    return
#if !__has_feature(objc_arc)
    Block_copy(
#endif
    ^(NSError * error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }
#if !__has_feature(objc_arc)
    )
#endif
    ;
}

void UnityCompletionCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSError* anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@", pid, ErrorToString(anError)]));
}

void UnityStringCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSString* output,NSError* anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, output, ErrorToString(anError)]));
}

void UnityString2Callback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSString* output, NSString* output2, NSError* anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@|%@", pid, output, output2, ErrorToString(anError)]));
}

void UnityLoginStatusCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, enum RaveLoginStatus status, NSError* anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, [NSNumber numberWithInt:status], ErrorToString(anError)]));
}

void UnityUserCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, id<RaveUser> user, NSError* anError)
{
    NSString* userString = RaveUserToString(user);
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, userString, ErrorToString(anError)]));
}

void UnityUsersCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSOrderedSet* users, NSError* anError)
{
    NSString* usersString = RaveUserArrayToString([users array]);
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, usersString, ErrorToString(anError)]));
}

void UnityStringListCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSArray* array, NSError*anError)
{
    NSString* arrayString = StringArrayToString(array);
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, arrayString, ErrorToString(anError)]));
}

void UnityAccountExistsCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, bool accountExists, bool hasPassword, NSError* anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@|%@", pid, BoolToString(accountExists), BoolToString(hasPassword), ErrorToString(anError)]));
}

@protocol GiftFailedResponse <NSObject>
@property (nonatomic, retain) NSString* uuid;
@property (nonatomic, retain) NSString* msg;
@end

void UnityGiftResultCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSArray * succeeded, NSArray * failed, NSError * anError)
{
    // Parse succeeded into a string of uuids
    NSMutableArray * o_succeeded = [NSMutableArray arrayWithArray:succeeded];
    for (int i=0; i<[o_succeeded count]; i++) {
        o_succeeded[i] = [o_succeeded[i] raveId];
    }

    // Parse failed into a string of uuids
    NSMutableArray * o_failed = [NSMutableArray arrayWithArray:failed];
    for (int i=0; i<[o_failed count]; i++) {
        o_failed[i] = [o_failed[i] uuid];
    }

    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@|%@", pid, StringArrayToString(o_succeeded), StringArrayToString(o_failed), ErrorToString(anError)]));
}

void UnityContactGiftResultCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSArray * results, NSError * anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, RaveContactGiftResultArrayToString(results), ErrorToString(anError)]));
}

void UnityGiftAndShareCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSArray * shareRequests, NSArray * contactGiftResults, NSError * anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@|%@", pid, RaveShareRequestArrayToString(shareRequests), RaveContactGiftResultArrayToString(contactGiftResults), ErrorToString(anError)]));
}

void UnityContactListCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSArray * contacts, NSError * anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, RaveContactArrayToString(contacts), ErrorToString(anError)]));
}

void UnityAppDataKeyUserSetCallback(NSString *callbackModule, NSString *callbackName, NSString *pid, NSArray *keySet, NSError *error) {
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, RaveAppDataKeyUserPairArrayToString(keySet), ErrorToString(error)]));
}

void UnityConnectCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, BOOL connectionFlag, NSError * anError)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, BoolToString(connectionFlag), ErrorToString(anError)]));
}

void UnityShareRequestListCallback(NSString* callbackModule, NSString* callbackName, NSString* pid,NSArray * requests, NSError * error)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, RaveShareRequestArrayToString(requests), ErrorToString(error)]));
}

void UnitySceneCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, NSError* error)
{
    UnityCompletionCallback(callbackModule, callbackName, pid, error);
}

void UnityAccountInfoSceneCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, BOOL loggedOut, NSError * error)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@", pid, BoolToString(loggedOut), ErrorToString(error)]));
}

void UnityLoginSceneCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, BOOL loggedIn, BOOL accountCreated, NSString* pluginKeyName, NSError * error)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@|%@|%@", pid, BoolToString(loggedIn), BoolToString(accountCreated), pluginKeyName, ErrorToString(error)]));
}

void RaveSocialUpdateLeaderboards(const char* callbackModule,const char* callbackName,const char* pid)
{
    NSString* module = [NSString stringWithUTF8String:callbackModule];
    NSString* name = [NSString stringWithUTF8String:callbackName];
    NSString* _pid = [NSString stringWithUTF8String:pid];

    [[RaveSocial leaderboardsManager] updateLeaderboards:^(NSError *anError) {
        UnityCompletionCallback(module,name,_pid,anError);
    }];
}


const char* RaveUsersManagerGetUserById(const char* raveId)
{
    NSString* s_raveId = [NSString stringWithUTF8String:raveId];

    id<RaveUser> result = [[RaveSocial usersManager] getUserById:s_raveId];
    return StringForUnity([RaveUserToString(result) UTF8String]);
}

void RaveUsersManagerUpdateUserById(const char* raveId, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_raveId = [NSString stringWithUTF8String:raveId];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] updateUserById:s_raveId withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveUsersManagerCurrent()
{
    id<RaveUser> result = [[RaveSocial usersManager] current];
    return StringForUnity([RaveUserToString(result) UTF8String]);
}

void RaveUsersManagerUpdateCurrent(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] updateCurrent:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void ApplyKeyValueToUser(id<RaveUser> currentUser, NSString * accessKey, SEL getSelector, SEL setSelector, NSDictionary * changes, NSMutableDictionary * savedValues)
{
    id newValue = [changes valueForKey:accessKey];
    id currentValue = [currentUser performSelector:getSelector];
    [savedValues setValue:currentValue forKey:accessKey];
    [currentUser performSelector:setSelector withObject:newValue];
}

void ApplyDictionaryToCurrentUser(NSDictionary * changes, NSMutableDictionary * savedValues)
{
    id<RaveUser> currentUser = RaveSocial.currentUser;
    ApplyKeyValueToUser(currentUser, @"displayName", @selector(displayName), @selector(setDisplayName:), changes, savedValues);
    ApplyKeyValueToUser(currentUser, @"realName", @selector(realName), @selector(setRealName:), changes, savedValues);
    ApplyKeyValueToUser(currentUser, @"username", @selector(username), @selector(setUsername:), changes, savedValues);
    ApplyKeyValueToUser(currentUser, @"email", @selector(email), @selector(setEmail:), changes, savedValues);
    ApplyKeyValueToUser(currentUser, @"gender", @selector(gender), @selector(setGender:), changes, savedValues);
    ApplyKeyValueToUser(currentUser, @"birthdate", @selector(birthdate), @selector(setBirthdate:), changes, savedValues);
}

void RavePushChangesToCurrentUser(const char* callbackModule, const char* callbackName, const char* pid, const char* changesString)
{
    NSMutableDictionary * changes = StringToNSDictionary(changesString);
    NSMutableDictionary * savedData = [NSMutableDictionary dictionary];

    // Exception for birthdate
    NSString * dateString = changes[@"birthdate"];
    if (dateString.length) {
        changes[@"birthdate"] = NSStringToDate(dateString);
    }

    ApplyDictionaryToCurrentUser(changes, savedData);

    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [RaveSocial.usersManager pushCurrent:^(NSError * error) {
        if (error) {
            ApplyDictionaryToCurrentUser(savedData, nil);
        }
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveUsersManagerCheckAccountExists(const char* email, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_email = [NSString stringWithUTF8String:email];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] checkAccountExists:s_email withCallback:^(BOOL accountExists, BOOL hasPassword, NSError * error) {
        UnityAccountExistsCallback(s_callbackModule, s_callbackName, s_pid, accountExists, hasPassword, error);
    }];
}

void RaveUsersManagerCheckThirdPartyAccountExists(const char* email, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_email = [NSString stringWithUTF8String:email];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] checkThirdPartyAccountExists:s_email withCallback:^(BOOL accountExists, BOOL hasPassword, NSError * error) {
        UnityAccountExistsCallback(s_callbackModule, s_callbackName, s_pid, accountExists, hasPassword, error);
    }];
}

void RaveUsersManagerFetchAccessToken(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] fetchAccessToken:^(NSString* accessToken, NSError *error) {
        UnityStringCallback(s_callbackModule, s_callbackName, s_pid, accessToken, error);
    }];
}

void RaveUsersManagerFetchRandomUsersForApplication(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] fetchRandomUsersForApplication:^(NSOrderedSet * users, NSError * error) {
        UnityUsersCallback(s_callbackModule, s_callbackName, s_pid, users, error);
    }];
}

void RaveUsersManagerFetchRandomUsersForApplication2(const char* appUuid, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_appUuid = [NSString stringWithUTF8String:appUuid];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] fetchRandomUsersForApplication:s_appUuid callback:^(NSOrderedSet * users, NSError * error) {
        UnityUsersCallback(s_callbackModule, s_callbackName, s_pid, users, error);
    }];
}

void RaveUsersManagerFetchRandomUsersForApplication3(const char* appUuid, float excludeContacts, float limit, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_appUuid = [NSString stringWithUTF8String:appUuid];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] fetchRandomUsersForApplication:s_appUuid excludeContacts:[NSNumber numberWithFloat:excludeContacts] withLimit:[NSNumber numberWithFloat:limit] callback:^(NSOrderedSet * users, NSError * error) {
        UnityUsersCallback(s_callbackModule, s_callbackName, s_pid, users, error);
    }];
}

void RaveUsersManagerFetchAllIdentities(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] fetchAllIdentities:^(NSArray * identities, NSError * error) {
        UnityStringListCallback(s_callbackModule, s_callbackName, s_pid, identities, error);
    }];
}

void RaveUsersManagerFetchIdentitiesForApplication(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial usersManager] fetchIdentitiesForApplication:^(NSArray * identities, NSError * error) {
        UnityStringListCallback(s_callbackModule, s_callbackName, s_pid, identities, error);
    }];
}

void RaveUsersManagerPushProfilePicture(const char* callbackModule, const char* callbackName, const char* pid, const char* url) {
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    NSString* s_url = SafeString(url);

    NSURL* urlObject = [NSURL URLWithString:s_url];
    [RaveSocial.usersManager pushProfilePicture:urlObject callback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}


const char* RaveGiftsManagerGetGiftTypeByKey(const char* typeKey)
{
    NSString* s_typeKey = [NSString stringWithUTF8String:typeKey];

    id<RaveGiftType> result = [[RaveSocial giftsManager] getGiftTypeByKey:s_typeKey];
    return StringForUnity([RaveGiftTypeToString(result) UTF8String]);
}

const char* RaveGiftsManagerGiftTypes()
{
    NSOrderedSet* result = [[RaveSocial giftsManager] giftTypes];
    return StringForUnity([RaveGiftTypeArrayToString([result array]) UTF8String]);
}

void RaveGiftsManagerUpdateGiftTypes(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] updateGiftTypes:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveGiftsManagerGetGiftById(const char* giftId)
{
    NSString* s_giftId = [NSString stringWithUTF8String:giftId];

    id<RaveGift> result = [[RaveSocial giftsManager] getGiftById:s_giftId];
    return StringForUnity([RaveGiftToString(result) UTF8String]);
}

const char* RaveGiftsManagerGetGiftRequestById(const char* requestId)
{
    NSString* s_requestId = [NSString stringWithUTF8String:requestId];

    id<RaveGiftRequest> result = [[RaveSocial giftsManager] getGiftRequestById:s_requestId];
    return StringForUnity([RaveGiftRequestToString(result) UTF8String]);
}

const char* RaveGiftsManagerGifts()
{
    NSOrderedSet* result = [[RaveSocial giftsManager] gifts];
    return StringForUnity([RaveGiftArrayToString([result array]) UTF8String]);
}

void RaveGiftsManagerUpdateGifts(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] updateGifts:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveGiftsManagerGiftRequests()
{
    NSOrderedSet* result = [[RaveSocial giftsManager] giftRequests];
    return StringForUnity([RaveGiftRequestArrayToString([result array]) UTF8String]);
}

void RaveGiftsManagerUpdateGiftRequests(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] updateGiftRequests:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveGiftsManagerSendGiftsToUsersWithKey(const char* giftTypeKey, const char* userIds, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_giftTypeKey = [NSString stringWithUTF8String:giftTypeKey];
    NSArray* o_userIds = StringToStringArray(userIds);
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] sendGiftsWithKey:s_giftTypeKey toUsersById:o_userIds withCallback:^(NSArray * succeeded, NSArray * failed, NSError * error) {
        UnityGiftResultCallback(s_callbackModule, s_callbackName, s_pid, succeeded, failed, error);
    }];
}

void RaveGiftsManagerAcceptGiftId(const char* giftId, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_giftId = [NSString stringWithUTF8String:giftId];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] acceptGiftById:s_giftId withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveGiftsManagerRejectGiftById(const char* giftId, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_giftId = [NSString stringWithUTF8String:giftId];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] rejectGiftById:s_giftId withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveGiftsManagerRequestGiftWithKey(const char* giftTypeKey, const char* userIds, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_giftTypeKey = [NSString stringWithUTF8String:giftTypeKey];
    NSArray* o_userIds = StringToStringArray(userIds);
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    //TODO: should be by id?
    [[RaveSocial giftsManager] requestGiftWithKey:s_giftTypeKey fromUsersById:o_userIds withCallback:^(NSArray * succeeded, NSArray * failed, NSError * error) {
        UnityGiftResultCallback(s_callbackModule, s_callbackName, s_pid, succeeded, failed, error);
    }];
}

void RaveGiftsManagerGrantGiftRequestById(const char* requestId, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_requestId = [NSString stringWithUTF8String:requestId];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] grantGiftRequestById:s_requestId withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveGiftsManagerIgnoreGiftRequestById(const char* requestId, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_requestId = [NSString stringWithUTF8String:requestId];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] ignoreGiftRequestById:s_requestId withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveGiftsManagerSendGifts(const char* giftTypeId, const char* userIds, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_giftTypeId = [NSString stringWithUTF8String:giftTypeId];
    NSArray* o_userIds = StringToStringArray(userIds);
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial giftsManager] sendGifts:s_giftTypeId toUsersById:o_userIds withCallback:^(NSArray * succeeded, NSArray * failed, NSError * error) {
        UnityGiftResultCallback(s_callbackModule, s_callbackName, s_pid, succeeded, failed, error);
    }];
}

const char* RaveContactsManagerAll()
{
    NSOrderedSet* result = [[RaveSocial contactsManager] all];
    return StringForUnity([RaveContactArrayToString([result array]) UTF8String]);
}

void RaveContactsManagerUpdateAll(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial contactsManager] updateAll:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveContactsManagerAllUsingThisApplication()
{
    NSOrderedSet* result = [[RaveSocial contactsManager] allUsingThisApplication];
    return StringForUnity([RaveContactArrayToString([result array]) UTF8String]);
}

void RaveContactsManagerUpdateAllUsingThisApplication(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial contactsManager] updateAllUsingThisApplication:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveContactsManagerFacebook()
{
    NSOrderedSet* result = [[RaveSocial contactsManager] facebook];
    return StringForUnity([RaveContactArrayToString([result array]) UTF8String]);
}

void RaveContactsManagerUpdateFacebook(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial contactsManager] updateFacebook:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveContactsManagerAddContactsByUsername(const char* usernames, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    NSArray* o_usernames = StringToStringArray(usernames);

    [[RaveSocial contactsManager] addContactsByUsername:o_usernames withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveContactsManagerDeleteContact(const char* userUuid, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_userUuid = [NSString stringWithUTF8String:userUuid];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial contactsManager] deleteContact:s_userUuid withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveContactsManagerGetAllUsingApplication(const char* appId)
{
    NSString* s_appId = [NSString stringWithUTF8String:appId];

    NSOrderedSet* result = [[RaveSocial contactsManager] getAllUsingApplication:s_appId];
    return StringForUnity([RaveContactArrayToString([result array]) UTF8String]);
}

void RaveContactsManagerUpdateAllUsingApplication(const char* appId, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_appId = [NSString stringWithUTF8String:appId];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial contactsManager] updateAllUsingApplication:s_appId withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveContactsManagerFetchAllExternal(float filter, const char* callbackModule, const char* callbackName, const char* pid)
{
    //TODO: assure filter is turned into right enum
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial contactsManager] fetchAllExternal:filter withCallback:^(NSArray * contacts, NSError * error) {
        UnityContactListCallback(s_callbackModule, s_callbackName, s_pid, contacts, error);
    }];
}

void RaveContactsManagerFetchExternalFrom(const char* pluginKeyName, float filter, const char* callbackModule, const char* callbackName, const char* pid)
{
    //TODO: assure filter is turned into right enum
    NSString* s_pluginKeyName = [NSString stringWithUTF8String:pluginKeyName];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial contactsManager] fetchExternalFrom:s_pluginKeyName filter:filter withCallback:^(NSArray * contacts, NSError * error) {
        UnityContactListCallback(s_callbackModule, s_callbackName, s_pid, contacts, error);
    }];
}

void RaveSharingManagerPostToFacebook(const char* wallPost, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_wallPost = [NSString stringWithUTF8String:wallPost];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial sharingManager] postToFacebook:s_wallPost withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveSharingManagerPostToFacebookWithImage(const char* wallPost, const char* imageURL, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_wallPost = [NSString stringWithUTF8String:wallPost];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    UIImage * image = [UIImage imageWithContentsOfFile:[NSString stringWithUTF8String:imageURL]];

    [[RaveSocial sharingManager] postToFacebook:s_wallPost withImage:image withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveSharingManagerPostToGooglePlus(const char* postText, const char* imageURL, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_postText = [NSString stringWithUTF8String:postText];
    NSString* s_imageURL = [NSString stringWithUTF8String:imageURL];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial sharingManager] postToGooglePlus:s_postText withImageURL:s_imageURL withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveSharingManagerShareWith(const char* externalContacts, const char* subject, const char* message, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_subject = [NSString stringWithUTF8String:subject];
    NSString* s_message = [NSString stringWithUTF8String:message];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    NSArray* o_externalContacts = StringToRaveContactArray(externalContacts);


    [[RaveSocial sharingManager] shareWith:o_externalContacts subject:s_subject message:s_message withCallback:^(NSArray * requests, NSError * error) {
        UnityShareRequestListCallback(s_callbackModule, s_callbackName, s_pid, requests, error);
    }];
}

void RaveSharingManagerShareWithViaPlugin(const char* externalContacts, const char* pluginKeyName, const char* subject, const char* message, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_pluginKeyName = [NSString stringWithUTF8String:pluginKeyName];
    NSString* s_subject = [NSString stringWithUTF8String:subject];
    NSString* s_message = [NSString stringWithUTF8String:message];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    NSArray* o_externalContacts = StringToRaveContactArray(externalContacts);

    [[RaveSocial sharingManager] shareWith:o_externalContacts viaPlugin:s_pluginKeyName subject:s_subject message:s_message withCallback:^(NSArray * requests, NSError * error) {
        UnityShareRequestListCallback(s_callbackModule, s_callbackName, s_pid, requests, error);
    }];
}

const char* RaveSharingManagerGetExternalIdForShareInstall(const char* appCallUrl, const char* source)
{
    NSString* s_appCallUrl = [NSString stringWithUTF8String:appCallUrl];
    NSString* s_source = [NSString stringWithUTF8String:source];

    return StringForUnity([[[RaveSocial sharingManager] getExternalIdForShareInstall:s_appCallUrl viaPlugin:s_source] UTF8String]);
}

const char* RaveLeaderboardManagerLeaderboards()
{
    NSOrderedSet* result = [[RaveSocial leaderboardsManager] leaderboards];
    return StringForUnity([RaveLeaderboardArrayToString([result array]) UTF8String]);
}

void RaveLeaderboardManagerUpdateLeaderboards(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateLeaderboards:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveLeaderboardManagerGetLeaderboardByKey(const char* leaderboardkey)
{
    NSString* s_leaderboardkey = [NSString stringWithUTF8String:leaderboardkey];

    id<RaveLeaderboard> result = [[RaveSocial leaderboardsManager] getLeaderboardByKey:s_leaderboardkey];
    return StringForUnity([RaveLeaderboardToString(result) UTF8String]);
}

void RaveLeaderboardManagerUpdateLeaderboardByKey(const char* leaderboardKey, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateLeaderboardByKey:s_leaderboardKey withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveLeaderboardManagerSubmitScoreByKey(const char* leaderboardKey, float score, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] submitScoreByKey:s_leaderboardKey withScore:[NSNumber numberWithFloat:score] withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveLeaderboardManagerUpdateGlobalScoresByKey(const char* leaderboardKey, float page, float pageSize, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateGlobalScoresByKey:s_leaderboardKey withPage:[NSNumber numberWithFloat:page] withPageSize:[NSNumber numberWithFloat:pageSize] withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveLeaderboardManagerGetGlobalScoresByKey(const char* leaderboardKey, float page, float pageSize)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    NSOrderedSet* result = [[RaveSocial leaderboardsManager] getGlobalScoresByKey:s_leaderboardKey withPage:[NSNumber numberWithFloat:page] withPageSize:[NSNumber numberWithFloat:pageSize]];
    return StringForUnity([RaveScoreArrayToString([result array]) UTF8String]);
}

const char* RaveLeaderboardManagerGetFriendsScoresByKey(const char* leaderboardKey, float page, float pageSize)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    NSOrderedSet* result = [[RaveSocial leaderboardsManager] getFriendsScoresByKey:s_leaderboardKey withPage:[NSNumber numberWithFloat:page] withPageSize:[NSNumber numberWithFloat:pageSize]];
    return StringForUnity([RaveScoreArrayToString([result array]) UTF8String]);
}

void RaveLeaderboardManagerUpdateFriendsScoresByKey(const char* leaderboardKey, float page, float pageSize, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateFriendsScoresByKey:s_leaderboardKey withPage:[NSNumber numberWithFloat:page] withPageSize:[NSNumber numberWithFloat:pageSize] withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveLeaderboardManagerUpdateMyGlobalScoresByKey(const char* leaderboardKey, float pageSize, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateMyGlobalScoresByKey:s_leaderboardKey withPageSize:[NSNumber numberWithFloat:pageSize] withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveLeaderboardManagerGetMyGlobalScoresByKey(const char* leaderboardKey, float pageSize)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    NSOrderedSet* result = [[RaveSocial leaderboardsManager] getMyGlobalScoresByKey:s_leaderboardKey withPageSize:[NSNumber numberWithFloat:pageSize]];
    return StringForUnity([RaveScoreArrayToString([result array]) UTF8String]);
}

const char* RaveLeaderboardManagerGetMyFriendsScoresByKey(const char* leaderboardKey, float pageSize)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    NSOrderedSet* result = [[RaveSocial leaderboardsManager] getMyFriendsScoresByKey:s_leaderboardKey withPageSize:[NSNumber numberWithFloat:pageSize]];
    return StringForUnity([RaveScoreArrayToString([result array]) UTF8String]);
}

void RaveLeaderboardManagerUpdateMyFriendsScoresByKey(const char* leaderboardKey, float pageSize, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateMyFriendsScoresByKey:s_leaderboardKey withPageSize:[NSNumber numberWithFloat:pageSize] withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveLeaderboardManagerGetMyGlobalScoresAdjacentByKey(const char* leaderboardKey, float adjacent)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    NSOrderedSet* result = [[RaveSocial leaderboardsManager] getMyGlobalScoresAdjacentByKey:s_leaderboardKey withAdjacent:[NSNumber numberWithFloat:adjacent]];
    return StringForUnity([RaveScoreArrayToString([result array]) UTF8String]);
}

void RaveLeaderboardManagerUpdateMyGlobalScoresAdjacentByKey(const char* leaderboardKey, float adjacent, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateMyGlobalScoresAdjacentByKey:s_leaderboardKey withAdjacent:[NSNumber numberWithFloat:adjacent] withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

const char* RaveLeaderboardManagerGetMyFriendsScoresAdjacentByKey(const char* leaderboardKey, float adjacent)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    NSOrderedSet* result = [[RaveSocial leaderboardsManager] getMyFriendsScoresAdjacentByKey:s_leaderboardKey withAdjacent:[NSNumber numberWithFloat:adjacent]];
    return StringForUnity([RaveScoreArrayToString([result array]) UTF8String]);
}

void RaveLeaderboardManagerUpdateMyFriendsScoresAdjacentByKey(const char* leaderboardKey, float adjacent, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial leaderboardsManager] updateMyFriendsScoresAdjacentByKey:s_leaderboardKey withAdjacent:[NSNumber numberWithFloat:adjacent] withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

float RaveLeaderboardManagerGetHighScoreByKey(const char* leaderboardKey)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    return [[[RaveSocial leaderboardsManager] getHighScoreByKey:s_leaderboardKey] floatValue];
}

float RaveLeaderboardManagerGetFriendsPositionByKey(const char* leaderboardKey)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    return [[[RaveSocial leaderboardsManager] getFriendsPositionByKey:s_leaderboardKey] floatValue];
}

float RaveLeaderboardManagerGetGlobalPositionByKey(const char* leaderboardKey)
{
    NSString* s_leaderboardKey = [NSString stringWithUTF8String:leaderboardKey];

    return [[[RaveSocial leaderboardsManager] getGlobalPositionByKey:s_leaderboardKey] floatValue];
}


const char* RaveAchievementsManagerGetAchievementByKey(const char* key)
{
    NSString* s_key = SafeString(key);
    return StringForUnity([RaveAchievementToString([[RaveSocial achievementsManager] getAchievementByKey:s_key]) UTF8String]);
}

const char* RaveAchievementsManagerAchievements()
{
    return StringForUnity([RaveAchievementArrayToString([[RaveSocial achievementsManager] achievements]) UTF8String]);
}

void RaveAchievementsManagerUpdateAchievements(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial achievementsManager] updateAchievements:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveAchievementsManagerUnlockAchievement(const char* achievementKey, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_achievementKey = [NSString stringWithUTF8String:achievementKey];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [[RaveSocial achievementsManager] unlockAchievement:s_achievementKey withCallback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

bool VersionAtLeast(NSString* versionString,int major,int minor,int subminor)
{
    NSArray* versionSections = [versionString componentsSeparatedByString:@"-"];

    NSArray* parts = [versionSections[0] componentsSeparatedByString:@"."];
    if( [parts[0] intValue] < major ) return false;
    if( [parts[0] intValue] > major ) return true;
    if( [parts[1] intValue] < minor ) return false;
    if( [parts[1] intValue] > minor ) return true;

    if( parts.count <= 2 )
    {
        return( subminor == 0);
    }

    return( [parts[2] intValue] >= subminor );
}

@interface StaticURLForwarder : NSObject
@end
@implementation StaticURLForwarder

-(void)gotUrl:(NSNotification*)aNotification {
    NSDictionary* info = [aNotification userInfo];
    [RaveSocial handleURL:[info objectForKey:@"url"] sourceApplication:[info objectForKey:@"sourceApplication"] annotation:[info objectForKey:@"annotation"]];
}

@end

// notification key from unity's AppDelegateListener
extern NSString* const kUnityOnOpenURL;

StaticURLForwarder* urlForwarder;

void RaveSocialPreInit() {
    if( !VersionAtLeast(RaveVersionNumber,2,10,4) )
    {
        [NSException raise:@"Rave Version too old" format:@"Minimum supported Rave Version for integration is 2.10.4"];
    }
    urlForwarder = [StaticURLForwarder new];
    [[NSNotificationCenter defaultCenter] addObserver:urlForwarder selector:@selector(gotUrl:)
                                                 name:kUnityOnOpenURL
                                               object:nil];

}

void RaveSocialInitialize(const char* callbackModule, const char* callbackName, const char* pid)
{
    void RaveSocialPreInit();

    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [RaveSocial initializeRaveWithReadyCallback:^(NSError *anError) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, anError);
    }];
}

bool RaveSocialIsInitialized()
{
    return RaveSocial.isInitialized;
}

bool RaveSocialIsLoggedIn()
{
    return RaveSocial.isLoggedIn;
}

bool RaveSocialIsLoggedInAsGuest()
{
    return RaveSocial.isLoggedInAsGuest;
}

bool RaveSocialIsPersonalized()
{
    return RaveSocial.isPersonalized;
}

bool RaveSocialIsAuthenticated()
{
    return RaveSocial.isAuthenticated;
}

void RaveSocialLoginAsGuest(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [RaveSocial loginAsGuest:^(NSError *anError) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, anError);
    }];
}

void RaveSocialLogOut(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    if([s_pid isEqualToString:@"NO_CALLBACK"]) {
        [RaveSocial logOut:nil];
        return;
    }

    [RaveSocial logOut:^(NSError *anError) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, anError);
    }];
}

void RaveSocialLoginWith(const char* pluginKeyName, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_pluginKeyName = [NSString stringWithUTF8String:pluginKeyName];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [RaveSocial loginWith:s_pluginKeyName callback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

void RaveSocialConnectTo(const char* pluginKeyName, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_pluginKeyName = [NSString stringWithUTF8String:pluginKeyName];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    [RaveSocial connectTo:s_pluginKeyName callback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

bool RaveSocialIsPluginReady(const char* pluginKeyName)
{
    NSString* s_pluginKeyName = [NSString stringWithUTF8String:pluginKeyName];
    return [RaveSocial isPluginReady:s_pluginKeyName];
}

void RaveSocialCheckReadinessOf(const char* pluginKeyName, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_pluginKeyName = [NSString stringWithUTF8String:pluginKeyName];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    [RaveSocial checkReadinessOf:s_pluginKeyName callback:^(BOOL ready, NSError *anError) {
        UnityConnectCallback(s_callbackModule, s_callbackName, s_pid, ready, anError);
    }];
}

void RaveSocialDisconnectFrom(const char* pluginKeyName, const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_pluginKeyName = [NSString stringWithUTF8String:pluginKeyName];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);
    [RaveSocial disconnectFrom:s_pluginKeyName callback:^(NSError *error) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, error);
    }];
}

@protocol StaticRaveFacebookConnectPlugin
@property (nonatomic, assign) BOOL ignoreUrls;
@end

void RaveSocialSetIgnoreFacebookUrls(const char* flag)
{
    NSString* s_flag = [NSString stringWithUTF8String:flag];
    bool flagValue = [s_flag isEqualToString:@"True"];
    id<StaticRaveFacebookConnectPlugin> plugin = [RaveSocial.authenticationManager getConnectPlugin:RaveConnectPluginFacebook];
    if (plugin) {
        plugin.ignoreUrls = flagValue;
    }
}

const char* RaveSettingsGetSetting(const char* settingsName)
{
    NSString* s_settingsName = [NSString stringWithUTF8String:settingsName];
    return StringForUnity([[[RaveSettings sharedInstance] get:s_settingsName] UTF8String]);
}

void RaveSettingsSetSetting(const char* settingsName, const char* value)
{
    NSString* s_settingsName = [NSString stringWithUTF8String:settingsName];
    NSString* s_value = [NSString stringWithUTF8String:value];
    [[RaveSettings sharedInstance] set:s_settingsName toString:s_value];
}

NSMutableDictionary* connectHandlers = nil;

@interface StaticConnectControllerHandler : NSObject <RaveConnectStateObserver>
@property (nonatomic, retain) NSString* callbackModule;
@property (nonatomic, retain) NSString* callbackName;
@property (nonatomic, retain) NSString* pid;
@property (nonatomic, retain) NSString* pluginName;
@property (nonatomic, retain) RaveConnectController* controller;
@end

@implementation StaticConnectControllerHandler
@synthesize callbackModule;
@synthesize callbackName;
@synthesize pid;
@synthesize pluginName;
@synthesize controller;

+(StaticConnectControllerHandler*)makeHandlerForPlugin:(NSString*)plugin {
    StaticConnectControllerHandler* handler = [StaticConnectControllerHandler new];
    handler.pluginName = plugin;
    [connectHandlers setObject:handler forKey:plugin];
#if !__has_feature(objc_arc)
    return [handler autorelease];
#else
    return handler;
#endif
}

-(void)startController {
    self.controller = [RaveConnectController controllerWithPlugin:pluginName];
    [self.controller setConnectObserver:self];
}
-(void)onConnectStateChanged:(RaveConnectControllerState)value {
    UnitySendMessage([self.callbackModule UTF8String], [self.callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%ld", self.pid, (long)value]));
}

#if !__has_feature(objc_arc)
-(void)dealloc {
    [callbackModule release];
    [callbackName release];
    [pid release];
    [pluginName release];
    [super dealloc];
}
#endif

@end

void RaveConnectControllerControllerWithPlugin(const char* pluginName) {
    if(connectHandlers == nil) {
        connectHandlers = [NSMutableDictionary new];
    }
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    [StaticConnectControllerHandler makeHandlerForPlugin:s_pluginName];
}

void RaveConnectControllerSetObserver(const char* pluginName,const char* callbackModule, const char* callbackName, const char* pid) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    StaticConnectControllerHandler* handler = [connectHandlers objectForKey:s_pluginName];
    handler.callbackModule = s_callbackModule;
    handler.callbackName = s_callbackName;
    handler.pid = s_pid;
    [handler startController];
}

void RaveConnectControllerAttemptConnect(const char* pluginName) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    StaticConnectControllerHandler* handler = [connectHandlers objectForKey:s_pluginName];
    [handler.controller attemptConnect];
}
void RaveConnectControllerAttemptDisconnect(const char* pluginName) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    StaticConnectControllerHandler* handler = [connectHandlers objectForKey:s_pluginName];
    [handler.controller attemptDisconnect];
}
void RaveConnectControllerDelete(const char* pluginName) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    [connectHandlers removeObjectForKey:s_pluginName];
}

NSMutableDictionary* friendHandlers = nil;

@interface StaticConnectFriendsControllerHandler : NSObject <RaveConnectFriendsStateObserver>
@property (nonatomic, retain) NSString* callbackModule;
@property (nonatomic, retain) NSString* callbackName;
@property (nonatomic, retain) NSString* pid;
@property (nonatomic, retain) NSString* pluginName;
@property (nonatomic, retain) RaveConnectFriendsController* controller;
@end

@implementation StaticConnectFriendsControllerHandler
@synthesize callbackModule;
@synthesize callbackName;
@synthesize pid;
@synthesize pluginName;
@synthesize controller;

+(StaticConnectFriendsControllerHandler*)makeHandlerForPlugin:(NSString*)plugin {
    StaticConnectFriendsControllerHandler* handler = [StaticConnectFriendsControllerHandler new];
    handler.pluginName = plugin;
    [friendHandlers setObject:handler forKey:plugin];
#if !__has_feature(objc_arc)
    return [handler autorelease];
#else
    return handler;
#endif
}

-(void)startController {
    self.controller = [RaveConnectFriendsController controllerWithPlugin:pluginName];
    [self.controller setFriendsObserver:self];
}
-(void)onFindFriendsStateChanged:(RaveFriendsControllerState)value {
    UnitySendMessage([self.callbackModule UTF8String], [self.callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%ld", self.pid, (long)value]));
}

#if !__has_feature(objc_arc)
-(void)dealloc {
    [callbackModule release];
    [callbackName release];
    [pid release];
    [pluginName release];
    [super dealloc];
}
#endif

@end

void RaveConnectFriendsControllerControllerWithPlugin(const char* pluginName) {
    if(friendHandlers == nil) {
        friendHandlers = [NSMutableDictionary new];
    }
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    [StaticConnectFriendsControllerHandler makeHandlerForPlugin:s_pluginName];
}

void RaveConnectFriendsControllerSetObserver(const char* pluginName,const char* callbackModule, const char* callbackName, const char* pid) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    StaticConnectFriendsControllerHandler* handler = [friendHandlers objectForKey:s_pluginName];
    handler.callbackModule = s_callbackModule;
    handler.callbackName = s_callbackName;
    handler.pid = s_pid;
    [handler startController];
}

void RaveConnectFriendsControllerAttemptGetFriends(const char* pluginName) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    StaticConnectFriendsControllerHandler* handler = [friendHandlers objectForKey:s_pluginName];
    [handler.controller attemptGetFriends];
}
void RaveConnectFriendsControllerAttemptForgetFriends(const char* pluginName) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    StaticConnectFriendsControllerHandler* handler = [friendHandlers objectForKey:s_pluginName];
    [handler.controller attemptForgetFriends];
}
void RaveConnectFriendsControllerDelete(const char* pluginName) {
    NSString* s_pluginName = [NSString stringWithUTF8String:pluginName];
    [friendHandlers removeObjectForKey:s_pluginName];
}

NSMutableDictionary* userObservers = nil;

@interface StaticCurrentUserObserver : NSObject <RaveCurrentUserObserver>
@property (nonatomic, retain) NSString* callbackModule;
@property (nonatomic, retain) NSString* callbackName;
@property (nonatomic, retain) NSString* pid;
@end

@implementation StaticCurrentUserObserver
@synthesize callbackModule;
@synthesize callbackName;
@synthesize pid;

- (void)userChanged:(NSArray *)changedKeys {
    NSString* keyArrayString = StringArrayToString(changedKeys);
    UnitySendMessage([self.callbackModule UTF8String], [self.callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@", self.pid, keyArrayString]));
}

#if !__has_feature(objc_arc)
-(void)dealloc {
    [callbackModule release];
    [callbackName release];
    [pid release];
    [super dealloc];
}
#endif

@end

void RaveUsersManagerAddCurrentUserObserver(const char* callbackModule, const char* callbackName, const char* pid) {
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    if(userObservers == nil) {
        userObservers = [NSMutableDictionary new];
    }

    StaticCurrentUserObserver* observer = [StaticCurrentUserObserver new];
    observer.callbackModule = s_callbackModule;
    observer.callbackName = s_callbackName;
    observer.pid = s_pid;
    [userObservers setObject:observer forKey:s_pid];
    [RaveSocial.usersManager addCurrentUserObserver:observer];
}

void RaveUsersManagerRemoveCurrentUserObserver(const char* pid) {
    NSString* s_pid = SafeString(pid);
    StaticCurrentUserObserver* observer = [userObservers objectForKey:s_pid];
    [RaveSocial.usersManager removeCurrentUserObserver:observer];
    [userObservers removeObjectForKey:s_pid];
#if !__has_feature(objc_arc)
    [observer release];
#endif
}

void RaveUsersManagerPushUserChanges(const char* callbackModule, const char* callbackName, const char* pid, const char* userChanges) {
    NSString* s_userChanges = SafeString(userChanges);

    RaveUserChanges* changes = StringToRaveUserChanges(s_userChanges);
    [RaveSocial.usersManager pushUserChanges:changes callback:UnityCompletionCallbackFactory(callbackModule, callbackName, pid)];
}

@interface UnityAppDataKeysObserver : UnityCallbackContext<RaveAppDataKeysStateObserver>
@end

@implementation UnityAppDataKeysObserver
- (void)appDataKeyStateChanged:(NSString *)selectedKey unresolvedKeys:(NSArray *)unresolvedKeys {
    NSString* unresolvedArrayString = StringArrayToString(unresolvedKeys);
    [[[self param:selectedKey] param:unresolvedArrayString] callback];
}
@end

void RaveAppDataKeysManagerSetStateObserver(const char * callbackModule, const char * callbackName, const char * pid) {
    UnityAppDataKeysObserver * observer = [UnityAppDataKeysObserver context:callbackModule callbackName:callbackName pid:pid];
    [RaveSocial.appDataKeysManager setStateObserver:observer];
}

const char * RaveAppDataKeysManagerLastSelectedKey() {
    return MakeStringCopy(RaveSocial.appDataKeysManager.lastSelectedKey);
}

void RaveAppDataKeysManagerFetchCurrentState(const char * callbackModule, const char * callbackName, const char * pid) {
    UnityCallbackContext * context = [UnityCallbackContext context:callbackModule callbackName:callbackName pid:pid];

    [RaveSocial.appDataKeysManager fetchCurrentState:^(NSString * selectedKey, NSArray * rejectedKeys, NSArray * unresolvedKeys, NSError * error) {
        NSString* rejectedArrayString = StringArrayToString(rejectedKeys);
        NSString* unresolvedArrayString = StringArrayToString(unresolvedKeys);
        [[[[[context param:selectedKey] param:rejectedArrayString] param:unresolvedArrayString] error:error] callback];
    }];
}

void RaveAppDataKeysManagerFetchAvailable(const char * callbackModule, const char * callbackName, const char * pid) {
    UnityCallbackContext * context = [UnityCallbackContext context:callbackModule callbackName:callbackName pid:pid];

    [RaveSocial.appDataKeysManager fetchAvailable:^(NSArray * availableKeys, NSError * error) {
        NSString* availableArrayString = StringArrayToString(availableKeys);
        [[[context param:availableArrayString] error:error] callback];
    }];
}

void RaveAppDataKeysManagerFetchSelected(const char * callbackModule, const char * callbackName, const char * pid) {
    UnityCallbackContext * context = [UnityCallbackContext context:callbackModule callbackName:callbackName pid:pid];

    [RaveSocial.appDataKeysManager fetchSelected:^(NSString * selectedKey, NSError * error) {
        [[[context param:selectedKey] error:error] callback];
    }];
}

void RaveAppDataKeysManagerFetchUnresolved(const char * callbackModule, const char * callbackName, const char * pid) {
    UnityCallbackContext * context = [UnityCallbackContext context:callbackModule callbackName:callbackName pid:pid];

    [RaveSocial.appDataKeysManager fetchUnresolved:^(NSArray * unresolvedKeys, NSError * error) {
        NSString* unresolvedArrayString = StringArrayToString(unresolvedKeys);
        [[[context param:unresolvedArrayString] error:error] callback];
    }];
}

void RaveAppDataKeysManagerSelectKey(const char * callbackModule, const char * callbackName, const char * pid, const char * key) {
    NSString* s_key = SafeString(key);

    [RaveSocial.appDataKeysManager selectKey:s_key callback:UnityCompletionCallbackFactory(callbackModule, callbackName, pid)];
}

void RaveAppDataKeysManagerDeactivateKey(const char * callbackModule, const char * callbackName, const char * pid, const char * key) {
    NSString* s_key = SafeString(key);

    [RaveSocial.appDataKeysManager deactivateKey:s_key callback:UnityCompletionCallbackFactory(callbackModule, callbackName, pid)];
}

void RaveAppDataKeysManagerFetchUserKey(const char * callbackModule, const char * callbackName, const char * pid, const char * raveId) {
    UnityCallbackContext * context = [UnityCallbackContext context:callbackModule callbackName:callbackName pid:pid];
    NSString *s_raveId = SafeString(raveId);

    [RaveSocial.appDataKeysManager fetchUserKey:s_raveId callback:^(NSString *selectedKey, NSError *error) {
        [[[context param:selectedKey] error:error] callback];
    }];
}

void RaveAppDataKeysManagerFetchUserKeySet(const char * callbackModule, const char * callbackName, const char *pid, const char * raveIds) {
    NSString *s_callbackModule = SafeString(callbackModule);
    NSString *s_callbackName = SafeString(callbackName);
    NSString *s_pid = SafeString(pid);
    NSArray *s_raveIds = StringToStringArray(raveIds);

    [RaveSocial.appDataKeysManager fetchUserKeySet:s_raveIds callback:^(NSArray <RaveAppDataKeyUserPair *> *userKeyPairs, NSError * error) {
        UnityAppDataKeyUserSetCallback(s_callbackModule, s_callbackName, s_pid, userKeyPairs, error);
    }];
}

void RaveAppDataKeysManagerFetchUserKeySetForContacts(const char * callbackModule, const char * callbackName, const char *pid) {
    NSString *s_callbackModule = SafeString(callbackModule);
    NSString *s_callbackName = SafeString(callbackName);
    NSString *s_pid = SafeString(pid);

    [RaveSocial.appDataKeysManager fetchUserKeySetForContacts:^(NSArray <RaveAppDataKeyUserPair *> *userKeyPairs, NSError * error) {
        UnityAppDataKeyUserSetCallback(s_callbackModule, s_callbackName, s_pid, userKeyPairs, error);

    }];
}

@interface RaveUnityMergePolicyBinding : UnityCallbackContext<RaveMergePolicy>
@property (nonatomic, copy) RaveUserMergeDecisionCallback decisionCallback;
@end

NSMutableString* RaveMergeUserToString(id<RaveMergeUser> obj)
{
    NSMutableString *s = RaveUserToString(obj);
    NSString* delimiter = @"<RaveUser>";
    SafeAppend(s,obj.selectedAppDataKey,delimiter);
    return s;
 }

@implementation RaveUnityMergePolicyBinding
@synthesize decisionCallback;

- (void)makeUserMergeDecision:(id<RaveMergeUser>)targetUser callback:(RaveUserMergeDecisionCallback)callback {
    self.decisionCallback = callback;
    [[self param:RaveMergeUserToString(targetUser)] callback];
}
@end

void RaveSocialSetMergePolicy(const char * callbackModule, const char * callbackName, const char * pid) {
    [RaveSocial setMergePolicy:[RaveUnityMergePolicyBinding context:callbackModule callbackName:callbackName pid:pid]];
}

@protocol MergeWorkaround
- (id<RaveMergePolicy>)mergePolicy;
@end

void RaveSocialMergeDecision(const char * shouldMerge) {
    id<RaveMergePolicy> policy = [(id)RaveSocial.authenticationManager mergePolicy];
    if ([policy isKindOfClass:RaveUnityMergePolicyBinding.class]) {
        RaveUnityMergePolicyBinding * mergePolicy = (RaveUnityMergePolicyBinding *)policy;
        if (mergePolicy.decisionCallback != nil) {
            mergePolicy.decisionCallback([SafeString(shouldMerge) boolValue]);
        }
    }
}

const char * RaveSocialVolatileRaveId() {
    return MakeStringCopy(RaveSocial.volatileRaveId);
}
