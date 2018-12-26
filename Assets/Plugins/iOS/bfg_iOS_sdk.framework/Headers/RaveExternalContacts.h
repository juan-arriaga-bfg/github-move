//
//  RaveExternalContacts.h
//  RaveSocial
//
//  Created by gwilliams on 2/19/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveExternalContact.h>

@interface RaveExternalContacts : NSObject
+ (void)fetchFrom:(NSString *)pluginKeyName withCallback:(RaveExternalContactsCallback)callback;
+ (void)fetchFromAll:(RaveExternalContactsCallback)callback;

+ (void)invite:(NSArray *)externalContacts subject:(NSString *)subject message:(NSString *)message interactive:(BOOL)isInteractive;
+ (void)invite:(NSArray *)externalContacts subject:(NSString *)subject message:(NSString *)message interactive:(BOOL)isInteractive withGiftTypeKey:(NSString *)giftTypeKey;
@end
