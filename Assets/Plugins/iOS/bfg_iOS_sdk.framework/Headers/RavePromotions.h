//
//  RavePromotions.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface RavePromotions : NSObject
+ (NSOrderedSet *)recommendedApps;
+ (void)updateRecommendedAppsWithCallback:(RaveCompletionCallback)callback;
@end
