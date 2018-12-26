//
//  RaveGifts.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveGift.h>

@interface RaveGifts : NSObject
+ (id<RaveGiftType>)getGiftTypeByName:(NSString *)name;
+ (id<RaveGiftType>)getGiftTypeById:(NSString *)typeId;
+ (id<RaveGiftType>)getGiftTypeByKey:(NSString *)typeKey;

+ (NSOrderedSet *) giftTypes;
+ (void) updateGiftTypes:(RaveCompletionCallback)callback;

+ (id<RaveGift>)getGiftById:(NSString*)giftId;
+ (NSOrderedSet *) gifts;
+ (void) updateGifts:(RaveCompletionCallback)callback;

+ (id<RaveGiftRequest>)getGiftRequestById:(NSString *)requestId;
+ (NSOrderedSet *) giftRequests;
+ (void) updateGiftRequests:(RaveCompletionCallback)callback;

+ (void) sendGiftsWithKey:(NSString *)giftTypeKey toUsers:(NSArray *)users withCallback:(RaveGiftResultCallback)callback;
+ (void) sendGiftsWithKey:(NSString *)giftTypeKey toUsersById:(NSArray *)userIds withCallback:(RaveGiftResultCallback)callback;
+ (void) sendGiftsWithKey:(NSString *)giftTypeKey toContacts:(NSArray *)contacts withCallback:(RaveContactGiftResultsCallback)callback;
+ (void) sendGiftsAndShareWithKey:(NSString *)giftTypeKey toContacts:(NSArray *)contacts subject:(NSString *)subject message:(NSString *)message withCallback:(RaveGiftAndShareResultsCallback)callback;
+ (void) sendGiftsAndShareWithKey:(NSString *)giftTypeKey viaPlugin:(NSString *)pluginKeyName toContacts:(NSArray *)contacts subject:(NSString *)subject message:(NSString *)message withCallback:(RaveGiftAndShareResultsCallback)callback;

+ (void) acceptGift:(id<RaveGift>)gift withCallback:(RaveCompletionCallback)callback;
+ (void) acceptGiftById:(NSString*)giftId withCallback:(RaveCompletionCallback)callback;

+ (void) rejectGift:(id<RaveGift>)gift withCallback:(RaveCompletionCallback)callback;
+ (void) rejectGiftById:(NSString*)giftId withCallback:(RaveCompletionCallback)callback;

+ (void) requestGiftWithKey:(NSString *)giftTypeKey fromUsers:(NSArray *)users withCallback:(RaveGiftResultCallback)callback;
+ (void) requestGiftWithKey:(NSString *)giftTypeKey fromUsersById:(NSArray *)userIds withCallback:(RaveGiftResultCallback)callback;

+ (void) grantGiftRequest:(id<RaveGiftRequest>)request withCallback:(RaveCompletionCallback)callback;
+ (void) grantGiftRequestById:(NSString*)requestId withCallback:(RaveCompletionCallback)callback;

+ (void) ignoreGiftRequest:(id<RaveGiftRequest>)request withCallback:(RaveCompletionCallback)callback;
+ (void) ignoreGiftRequestById:(NSString*)requestId withCallback:(RaveCompletionCallback)callback;

+ (void) fetchGiftContentForShareInstall:(NSString *)appCallUrl viaPlugin:(NSString *)source withCallback:(RaveGiftContentCallback)callback;

+ (void) attachGiftWithKey:(NSString *)giftTypeKey toShareRequests:(NSArray *)requests withCallback:(RaveCompletionCallback)callback;
+ (void) fetchGiftKeyForExternalId:(NSString *)externalId forSource:(NSString *)source withCallback:(RaveGiftKeyCallback)callback;
+ (void) detachGiftTypeForExternalId:(NSString *)externalId forSource:(NSString *)source withCallback:(RaveCompletionCallback)callback;

//  deprecated
+ (void) sendGifts:(NSString *)giftTypeId toUsers:(NSArray*)users withCallback:(RaveGiftResultCallback)callback;
+ (void) sendGifts:(NSString *)giftTypeId toUsersById:(NSArray*)ids withCallback:(RaveGiftResultCallback)callback;

+ (void) requestGift:(NSString *)giftTypeId fromUsers:(NSArray*)users withCallback:(RaveGiftResultCallback)callback;
+ (void) requestGift:(NSString *)giftTypeId fromUsersById:(NSArray*)ids withCallback:(RaveGiftResultCallback)callback;

@end
