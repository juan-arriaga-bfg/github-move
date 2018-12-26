//
//  RaveToast.h
//  RaveSocial
//
//  Created by gwilliams on 7/11/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//



@interface RaveToast : NSObject
+ (void)showWelcomeToast:(BOOL)isNewUser;
+ (void)showToast:(NSString *)message;
@end
