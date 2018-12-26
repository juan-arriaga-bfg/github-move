//
//  RaveAchievements.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveAchievement.h>

@interface RaveAchievements : NSObject
+ (id<RaveAchievement>)getAchievementByKey:(NSString *)key;
+ (NSArray *)achievements;
+ (void)updateAchievements:(RaveCompletionCallback)callback;
+ (void)unlockAchievement:(NSString *)achievementKey withCallback:(RaveCompletionCallback)callback;
@end
