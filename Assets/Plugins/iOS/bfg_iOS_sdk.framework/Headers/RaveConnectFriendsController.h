//
//  RaveFindFriendsController.h
//  RaveSocial
//
//  Created by Dmitri Salcedo on 5/11/15.
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>
#import "RaveConnectController.h"

typedef NS_ENUM(NSInteger, RaveFriendsControllerState)
{
    RaveFriendsControllerStateNotDownloaded,
    RaveFriendsControllerStateDownloading,
    RaveFriendsControllerStateDownloaded,
};

@protocol RaveConnectFriendsStateObserver <NSObject>
@required
-(void)onFindFriendsStateChanged:(RaveFriendsControllerState)value;
@end

@interface RaveConnectFriendsController : RaveConnectController <RaveConnectStateObserver>

@property (nonatomic, assign) id<RaveConnectFriendsStateObserver> friendsObserver;

+(RaveConnectFriendsController*) controllerWithPlugin:(NSString *)pluginKeyName;
-(void) attemptGetFriends;
-(void) attemptForgetFriends;

@end
