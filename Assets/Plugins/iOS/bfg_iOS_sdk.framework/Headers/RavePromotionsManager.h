//
//  RavePromotionsManager.h
//  RaveSocial
//
//  Created by dsalcedo on 12/10/13.
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RaveSocial.h"

@protocol RavePromotionsManager
@required
@property (nonatomic, readonly) NSOrderedSet* recommendedApps;
- (void)updateRecommendedApps:(RaveCompletionCallback)callback;
@end
