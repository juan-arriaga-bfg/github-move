//
//  BigFishMergeDecisionScene.h
//  BigFishScenePack
//
//  Copyright © 2016 Rave Social, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AuthFlowScene.h"

@interface BigFishMergeDecisionScene : AuthFlowScene
@property (nonatomic, retain) id<RaveUser> otherUser;
@property (nonatomic, copy) RaveUserMergeDecisionCallback callback;
@end
