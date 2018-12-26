//
//  RaveExternalContact.h
//  RaveSocial
//
//  Created by gwilliams on 2/19/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

extern NSString * const RaveExternalSourceFacebook;
extern NSString * const RaveExternalSourceGooglePlus;
extern NSString * const RaveExternalSourcePhonebook;

typedef void(^RaveExternalContactsCallback)(NSArray * externalContacts, NSError * error);

@interface RaveExternalContact : NSObject
@property (nonatomic, retain, readonly) NSString * displayName;
@property (nonatomic, retain, readonly) NSString * externalId;
@property (nonatomic, retain, readonly) NSString * source;
@end
