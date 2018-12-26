//
//  RSTwitterConnectWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RaveWidget.h"

@class ACAccount;
@interface RaveTwitterConnectWidget : RaveWidget

@property (nonatomic, retain) NSString* connectedMessage;
@property (nonatomic, copy) RaveCompletionCallback didConnect;
@property (nonatomic, copy) RaveCompletionCallback didLogout;

- (void)toggleTwitter:(id)aSender;

- (void)updateUI;

@end
