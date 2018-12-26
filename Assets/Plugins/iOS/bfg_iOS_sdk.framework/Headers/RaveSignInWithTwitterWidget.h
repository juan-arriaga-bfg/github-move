//
//  RSSignInWithTwitterWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveUtilities.h>
#import "RaveWidget.h"

@interface RaveSignInWithTwitterWidget : RaveWidget

- (void)signInWithTwitter:(id)aSender;

@property (nonatomic, copy) RaveCompletionCallback didFinishCallback;
@end
