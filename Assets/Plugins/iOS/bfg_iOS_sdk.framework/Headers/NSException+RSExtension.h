//
//  NSException+RSExtension.h
//  RaveSocial
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSException (RSExtension)
+ (NSException*)rs_exeptionWithReason:(NSString*)aReason;
+ (NSException*)rs_alreadyAuthenticated;
@end
