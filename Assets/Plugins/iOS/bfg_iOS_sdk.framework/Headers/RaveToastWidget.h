//
//  RSToastWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//
#import "RaveWidget.h"

@interface RaveToastWidget : RaveWidget

@property (nonatomic, assign) NSTimeInterval presentationDuration;
@property (nonatomic, assign) NSTimeInterval animationDuration;

- (void)presentWithCompletion:(dispatch_block_t)aCompletion;
@end
