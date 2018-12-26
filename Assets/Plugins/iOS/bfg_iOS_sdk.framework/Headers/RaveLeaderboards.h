//
//  RaveLeaderboards.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Ravesocial/RaveLeaderboard.h>

@interface RaveLeaderboards : NSObject
+ (NSOrderedSet *) leaderboards;
+ (void)updateLeaderboards:(RaveCompletionCallback)callback;

+ (id<RaveLeaderboard>)getLeaderboardByKey:(NSString *)leaderboardkey;

+ (void)submitScoreByKey:(NSString *)leaderboardKey withScore:(NSNumber *)score withCallback:(RaveCompletionCallback)callback;

+ (NSOrderedSet *)getGlobalScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize;
+ (void)updateGlobalScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

+ (NSOrderedSet*)getFriendsScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize;
+ (void)updateFriendsScoresByKey:(NSString *)leaderboardKey withPage:(NSNumber*)page withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

+ (NSOrderedSet*)getMyGlobalScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber *)pageSize;
+ (void)updateMyGlobalScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber*)pageSize withCallback:(RaveCompletionCallback)callback;

+ (NSOrderedSet*)getMyFriendsScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber *)pageSize;
+ (void)updateMyFriendsScoresByKey:(NSString *)leaderboardKey withPageSize:(NSNumber *)pageSize withCallback:(RaveCompletionCallback)callback;

+ (NSOrderedSet*)getMyGlobalScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent;
+ (void)updateMyGlobalScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent withCallback:(RaveCompletionCallback)callback;

+ (NSOrderedSet*)getMyFriendsScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent;
+ (void)updateMyFriendsScoresAdjacentByKey:(NSString *)leaderboardKey withAdjacent:(NSNumber*)adjacent withCallback:(RaveCompletionCallback)callback;

+ (NSNumber *)getHighScoreByKey:(NSString *)leaderboardKey;
+ (NSNumber *)getGlobalPositionByKey:(NSString *)leaderboardKey;
+ (NSNumber *)getFriendsPositionByKey:(NSString *)leaderboardKey;
@end
