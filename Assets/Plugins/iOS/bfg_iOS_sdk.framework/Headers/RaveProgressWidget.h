//
//  RSProgressWidget.h
//
//  RaveUI
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RaveWidget.h"

@interface RaveProgressWidget : RaveWidget

@property (nonatomic, assign, readonly) UILabel* statusLabel;
@property (nonatomic, copy) dispatch_block_t onCancel;
@property (nonatomic, assign, readonly) BOOL canceled;
@property (nonatomic, assign, readonly) BOOL shown;

- (void)showInView:(UIView*)aView withText:(NSString*)aText stopHandler:(dispatch_block_t)doStop;
- (void)dismiss;
- (void)doCancel;
@end
