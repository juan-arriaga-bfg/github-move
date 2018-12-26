//
//  RSAddGooglePlusContactsWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RaveWidget.h"

@interface RaveAddGooglePlusContactsWidget : RaveWidget

@property (nonatomic, retain) NSString* gplusNotConnectedMessage;
@property (nonatomic, retain) NSString* friendsAddedMessage;

@property (nonatomic, assign) BOOL connectIfNotConnected;
- (void)addGplusFriends:(id)aSender;

@end
