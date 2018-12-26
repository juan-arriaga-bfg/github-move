//
//  ThirdPartyRaveConnectPlugin.h
//  RaveSocial
//
//  Created by gwilliams on 10/20/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveConnectPlugin.h>
#import <bfg_iOS_sdk/RaveConnectPluginImpl.h>

@interface ThirdPartyRaveConnectPlugin : RaveConnectPluginImpl<RaveConnectPlugin>
- (NSString *)thirdPartySource;
@property (nonatomic, retain) NSDictionary * credentials;
@end
