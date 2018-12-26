//
//  RSAddFacebookFriendsWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import "RaveWidget.h"

@interface RaveAddFacebookFriendsWidget : RaveWidget

@property (nonatomic, retain) NSString* facebookNotConnectedMessage;
@property (nonatomic, retain) NSString* friendsAddedMessage;

@property (nonatomic, assign) BOOL connectIfNotConnected;
- (void)addFacebookFriends:(id)aSender;
@end
