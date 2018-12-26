//
//  RaveSceneContext.h
//  RaveSocial
//
//  Created by gwilliams on 8/1/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol RaveSceneContainer <NSObject>
+ (instancetype)instance;
- (UIView *)rootView;
- (UIView *)containerView;
- (void)rotate;
@property (nonatomic, retain) UIView * contentView;
@end

@interface RaveDefaultSceneContainer : NSObject<RaveSceneContainer>
@end

@interface RaveModalSceneContainer : NSObject<RaveSceneContainer>
@property (nonatomic, assign) RaveScene * scene;
@end

@class RaveScene;
@interface RaveSceneContext : NSObject
+ (void)setContainerClass:(Class<RaveSceneContainer>)containerClass;
+ (Class<RaveSceneContainer>)containerClass;

+ (instancetype)instance;

+ (void)pushScene:(RaveScene *)scene;
+ (void)popScene;
+ (RaveScene *)topScene;
+ (NSUInteger)sceneDepth;
+ (void)dismiss;
@end
