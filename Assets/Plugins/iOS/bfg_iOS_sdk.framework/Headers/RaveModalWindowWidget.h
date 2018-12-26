//
//  RaveModalWindowWidget.h
//
//  RaveUI
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//
#import "RaveWidget.h"
#import "RaveSceneContext.h"

@interface UIViewController (RSUIExtention)
/*
 Returns YES if it allowed to dissmiss the controller UI at current point of time.
 */
- (BOOL)allowedToDismiss;

@end

@interface RaveModalWindowWidget : RaveWidget
- (void)dismiss:(id)sender;
- (void)dismiss;

- (void)setWidget:(RaveWidget*)widget;

- (UIView *)contentView;
- (void)rotate;

/*
 Called after dissmissal. This callback is not called it you call dismiss programatically.
 */
@property (nonatomic, copy) dispatch_block_t didDismissedCallback;

@end
