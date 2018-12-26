//
//  RaveSharingManager.h
//  RaveSocial
//
//  Created by dsalcedo on 12/19/13.
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <bfg_iOS_sdk/RaveSocial.h>
#import <bfg_iOS_sdk/RaveUtilities.h>

@interface RaveShareRequest : NSObject
@property (nonatomic, copy) NSString * pluginKeyName;
@property (nonatomic, retain) NSArray * requestIds;
@property (nonatomic, retain) NSArray * userIds;
@end

@protocol RaveSharingManager <RaveObject>
-(void)postToFacebook:(NSString *)wallPost withCallback:(RaveCompletionCallback)callback;
-(void)postToFacebook:(NSString *)wallPost withImage:(UIImage *)image withCallback:(RaveCompletionCallback)callback;
-(void)postToGooglePlus:(NSString *)postText withImageURL:(NSString *)imageURL withCallback:(RaveCompletionCallback)callback;

- (void)shareWith:(NSArray *)externalContacts subject:(NSString *)subject message:(NSString *)message withCallback:(RaveShareRequestCallback)callback;
- (void)shareWith:(NSArray *)externalContacts viaPlugin:(NSString *)pluginKeyName subject:(NSString *)subject message:(NSString *)message withCallback:(RaveShareRequestCallback)callback;

- (NSString *)getExternalIdForShareInstall:(NSString *)appCallUrl viaPlugin:(NSString *)source;
@end
