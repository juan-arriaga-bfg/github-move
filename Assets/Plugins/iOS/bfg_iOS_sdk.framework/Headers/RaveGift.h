//
//  RaveGift.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface RaveContactGiftResult : NSObject
@property (nonatomic, copy) NSString * pluginKeyName;
@property (nonatomic, retain) NSArray * externalIds;
@end

typedef void (^RaveGiftKeyCallback)(NSString * giftKey, NSError * error);
typedef void (^RaveGiftResultCallback)(NSArray * succeeded, NSArray * failed, NSError * error);
typedef void (^RaveContactGiftResultsCallback)(NSArray * results, NSError * error);
typedef void (^RaveGiftAndShareResultsCallback)(NSArray * shareRequests, NSArray * contactGiftResults, NSError * error);
typedef void (^RaveGiftContentCallback)(NSString * key, NSString * requestId, NSError * error);

@protocol RaveGiftType <NSObject>
@property (nonatomic, readonly) NSString * typeId;
@property (nonatomic, readonly) NSString * typeKey;
@property (nonatomic, readonly) NSString * name;
@property (nonatomic, readonly) BOOL canRequest;
@property (nonatomic, readonly) BOOL canGift;
@end

@protocol RaveUser;

@protocol RaveGift <NSObject>
@property (nonatomic, readonly) NSString * giftId;
@property (nonatomic, readonly) NSString * giftTypeKey;
@property (nonatomic, readonly) NSString * source;
@property (nonatomic, readonly) BOOL isFromGift;
@property (nonatomic, readonly) BOOL isFromRequest;
@property (nonatomic, readonly) NSDate * timeSent;
@property (nonatomic, readonly) id<RaveGiftType> giftType;
@property (nonatomic, readonly) id<RaveUser> sender;
@property (nonatomic, readonly) NSString* senderRaveId;
@end

@protocol RaveGiftRequest <NSObject>
@property (nonatomic, readonly) NSString * requestId;
@property (nonatomic, readonly) id<RaveGiftType> giftType;
@property (nonatomic, readonly) NSString * giftTypeKey;
@property (nonatomic, readonly) id<RaveUser> requester;
@property (nonatomic, readonly) NSDate * timeSent;
@property (nonatomic, readonly) NSString* requesterRaveId;
@end

