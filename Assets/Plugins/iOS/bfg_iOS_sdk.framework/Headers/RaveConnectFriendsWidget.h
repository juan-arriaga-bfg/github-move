//
//  RaveConnectFriendsWidget.h
//  RaveSocial
//
//  Created by Dmitri Salcedo on 5/6/15.
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>
#import "RaveConnectWidget.h"
#import <bfg_iOS_sdk/RaveConnectFriendsController.h>

@interface RaveConnectFriendsWidget : RaveConnectWidget <RaveConnectFriendsStateObserver>
@property (nonatomic, retain) RaveConnectFriendsController* friendsController;
@end
