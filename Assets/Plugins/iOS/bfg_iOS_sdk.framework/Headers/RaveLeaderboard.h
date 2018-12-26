//
//  RaveLeaderboard.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RaveLeaderboard <NSObject>
@property (nonatomic, retain, readonly) NSString * name;
@property (nonatomic, retain, readonly) NSString * key;
@property (nonatomic, retain, readonly) NSString * desc;
@property (nonatomic, retain, readonly) NSNumber * sorter;
@property (nonatomic, assign, readonly) BOOL isAscending;

@property (nonatomic, retain, readonly) NSNumber * highScore;
@property (nonatomic, retain, readonly) NSNumber * friendsPosition;
@property (nonatomic, retain, readonly) NSNumber * globalPosition;

- (NSOrderedSet*)getGlobalScoresWithPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize;
- (void)updateGlobalScoresWithPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

- (NSOrderedSet*)getFriendsScoresWithPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize;
- (void)updateFriendsScoresWithPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

- (NSOrderedSet*)getMyGlobalScoresWithPageSize:(NSNumber*)pageSize;
- (void)updateMyGlobalScoresWithPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

- (NSOrderedSet*)getMyFriendsScoresWithPageSize:(NSNumber*)pageSize;
- (void)updateMyFriendsScoresWithPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

- (NSOrderedSet*)getMyGlobalScoresAdjacent:(NSNumber*)adjacent;
- (void)updateMyGlobalScoresAdjacent:(NSNumber*)adjacent withCallback:(RaveCompletionCallback)callback;

- (NSOrderedSet*)getMyFriendsScoresAdjacent:(NSNumber*)adjacent;
- (void)updateMyFriendsScoresAdjacent:(NSNumber*)adjacent withCallback:(RaveCompletionCallback)callback;

- (void)submitScore:(NSNumber *)score withCallback:(RaveCompletionCallback)callback;
@end

@protocol RaveUser;
@protocol RaveScore <NSObject>
@property (nonatomic, retain, readonly) NSNumber * score;
@property (nonatomic, retain, readonly) NSNumber * position;
@property (nonatomic, retain, readonly) NSString * userDisplayName;
@property (nonatomic, retain, readonly) NSString * userPictureUrl;
@property (nonatomic, retain, readonly) NSString * userRaveId;
@end

