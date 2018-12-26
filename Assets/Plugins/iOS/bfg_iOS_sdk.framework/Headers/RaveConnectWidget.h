//
//  RaveConnectWidget.h
//  RaveSocial
//
//  Created by gwilliams on 6/6/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveWidget.h>
#import <bfg_iOS_sdk/RaveConnectController.h>

@protocol RaveConnectUIDelegate <NSObject>
- (void)setOn:(BOOL)isOn;
@end

typedef BOOL(^RaveConnectWidgetConfirmDisconnect)();

@interface RaveConnectWidget : RaveWidget <RaveConnectStateObserver>

@property (nonatomic, retain) id<RaveConnectUIDelegate> uiDelegate;
@property (nonatomic, copy) RaveCompletionCallback connectCallback;
@property (nonatomic, copy) NSString * pluginKeyName;
@property (nonatomic, retain) RaveConnectController* controller;

- (void)toggle:(id)aSender;
- (void)cancelWidget;
- (void)updateUI;

@end
