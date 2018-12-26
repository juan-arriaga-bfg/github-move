//
//  RaveContact.h
//  RaveSocial
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 *  Interface for RaveContact
 */
@protocol RaveContact <RaveObject>
/**
 *  RaveUser for this contact, will be nil if the contact is an external (non-Rave) conact
 */
@property (nonatomic, readonly) id<RaveUser> user;

/**
 *  YES if the source is of this contact is email, otherwise NO
 */
@property (nonatomic, readonly) BOOL isEmail;

/**
 *  YES if the source of this contact is RaveSocial, otherwise NO
 */
@property (nonatomic, readonly) BOOL isRaveSocial;

/**
 *  YES if the source of this contact is Facebook, otherwise NO
 */
@property (nonatomic, readonly) BOOL isFacebook;

/**
 *  YES if the source of this contact is Google+, otherwise NO
 */
@property (nonatomic, readonly) BOOL isGooglePlus;

/**
 *  A map keyed by source to external id (e.g. { "Facebook" : "12390012312" }
 */
@property (nonatomic, readonly) NSDictionary* externalIds;

/**
 *  User display name or nil
 */
@property (nonatomic, readonly) NSString* displayName;

/**
 *  User profile picture URL or nil
 */
@property (nonatomic, readonly) NSString* pictureURL;

/**
 *  Name of third-party authentication source, may be used to key external ids
 */
@property (nonatomic, readonly) NSString* thirdPartySource;

/**
 *  If the contact represents a RaveUser the key will be their raveId
 *  Otherwise, as an external contact it will be "source:externalId"
 *
 *  This field is typically only useful for creating a non-native binding for RaveContacts
 */
@property (nonatomic, readonly) NSString* key;
@end
