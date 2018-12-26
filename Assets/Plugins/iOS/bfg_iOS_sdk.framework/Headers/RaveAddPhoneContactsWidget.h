//
//  RSAddPhoneContactsWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RaveWidget.h"

@interface RaveAddPhoneContactsWidget : RaveWidget

@property (nonatomic, retain) NSString* contactsAddedMessage;
@property (nonatomic, retain) NSString* userNotLoggedInErrorMessage;

- (void)addPhoneContacts:(id)aSender;
@end
