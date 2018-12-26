//
//  RSModalView.h
//  RSModalView
//
//  Created by Viktor Iarovyi on 2/14/13.
//  Copyright (c) 2013 Gorilla Graph, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

/*
 Shows UI blocking global message with activity indication.
 */

typedef void (^RSModalViewBlock)(NSInteger selectedButtonIndex);

@interface RSModalView : UIView

@property (nonatomic, retain)  UIView *contentView;
- (void)showModal: (RSModalViewBlock) block;
- (void)dissmiss;

@end