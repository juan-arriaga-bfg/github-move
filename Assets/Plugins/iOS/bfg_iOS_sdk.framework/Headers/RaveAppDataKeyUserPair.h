//
//  RaveAppDataKeyUserPair.h
//  RaveSocial
//
//  Created by Brian Heller on 4/25/17.
//  Copyright © 2017 Rave, Inc. All rights reserved.
//

/**
 *  Provides a method of access for pairing an app data key with a specific user.
 */
@interface RaveAppDataKeyUserPair : NSObject
/**
 *  The RaveID of the associated user.
 */
@property (nonatomic, retain, readonly) NSString *raveId;

/**
 *  The app data key of the associated user.
 */
@property (nonatomic, retain, readonly) NSString *appDataKey;
@end
