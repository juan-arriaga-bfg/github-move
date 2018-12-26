//
//  RaveSharing.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface RaveSharing : NSObject
+(void)postToFacebook:(NSString *)wallPost withCallback:(RaveCompletionCallback)callback;
+(void)postToFacebook:(NSString *)wallPost withImage:(UIImage *)image withCallback:(RaveCompletionCallback)callback;
+(void)shareWith:(NSArray *)externalContacts subject:(NSString *)subject message:(NSString *)message withCallback:(RaveShareRequestCallback)callback;
+(void)shareWith:(NSArray *)externalContacts viaPlugin:(NSString *)pluginKeyName subject:(NSString *)subject message:(NSString *)message withCallback:(RaveShareRequestCallback)callback;
+ (NSString *)getExternalIdForShareInstall:(NSString *)appCallUrl viaPlugin:(NSString *)source;

@end
