//
//  RaveLeaderboardsManager.h
//  RaveSocial
//
//  Created by gwilliams on 12/9/13.
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveLeaderboard.h>

@protocol RaveLeaderboardManager <NSObject>
@property (nonatomic, retain, readonly) NSOrderedSet * leaderboards;
- (void)updateLeaderboards:(RaveCompletionCallback)callback;

- (id<RaveLeaderboard>)getLeaderboardByKey:(NSString *)leaderboardkey;
- (void)updateLeaderboardByKey:(NSString *)leaderboardKey withCallback:(RaveCompletionCallback)callback;

- (void)submitScoreByKey:(NSString *)leaderboardKey withScore:(NSNumber *)score withCallback:(RaveCompletionCallback)callback;

- (void)updateGlobalScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;
- (NSOrderedSet *)getGlobalScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize;

- (NSOrderedSet*)getFriendsScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize;
- (void)updateFriendsScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

- (void)updateMyGlobalScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;
- (NSOrderedSet*)getMyGlobalScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber*)pageSize;

- (NSOrderedSet*)getMyFriendsScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber*)pageSize;
- (void)updateMyFriendsScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

- (NSOrderedSet*)getMyGlobalScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent;
- (void)updateMyGlobalScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent withCallback:(RaveCompletionCallback)callback;

- (NSOrderedSet*)getMyFriendsScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent;
- (void)updateMyFriendsScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent withCallback:(RaveCompletionCallback)callback;

- (NSNumber *)getHighScoreByKey:(NSString *)leaderboardKey;

- (NSNumber *)getFriendsPositionByKey:(NSString *)leaderboardKey;

- (NSNumber *)getGlobalPositionByKey:(NSString *)leaderboardKey;

@end
