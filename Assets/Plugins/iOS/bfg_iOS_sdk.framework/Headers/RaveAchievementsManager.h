//
//  RaveAchievementsManager.h
//  RaveSocial
//
//  Created by gwilliams on 12/9/13.
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveAchievement.h>

@protocol RaveAchievementsManager <NSObject>
- (id<RaveAchievement>)getAchievementByKey:(NSString *)achievementKey;

@property (nonatomic, retain, readonly) NSArray * achievements;
- (void) updateAchievements:(RaveCompletionCallback)callback;
- (void) unlockAchievement:(NSString *)achievementKey withCallback:(RaveCompletionCallback)callback;
@end
