//
//  RaveAppDataKeyUserPair+Private.h
//  RaveSocial
//
//  Created by Brian Heller on 4/25/17.
//  Copyright © 2017 Rave, Inc. All rights reserved.
//

#import "RaveAppDataKeyUserPair.h"

@interface RaveAppDataKeyUserPair ()
@property (nonatomic, retain) NSString * raveId;
@property (nonatomic, retain) NSString * appDataKey;
- (instancetype)initWithRaveId:(NSString *)raveId appDataKey:(NSString *)appDataKey;
@end
