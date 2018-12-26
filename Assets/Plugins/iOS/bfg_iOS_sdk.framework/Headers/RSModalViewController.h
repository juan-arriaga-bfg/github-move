//
//  RSModalViewController.h
//  RSModalViewController
//
//  Created by Viktor Iarovyi on 2/14/13.
//  Copyright (c) 2013 RaveSocial. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RSNibViewController.h"
#import "RSModalView.h"

@interface RSModalViewController : RSNibViewController

@property (retain, nonatomic) IBOutlet RSModalView *modalView;

- (void)present;
- (void)dismiss;

+ (void)present;
+ (void)dismiss;

@property (nonatomic, assign, readonly) BOOL presented;

+ (id)sharedInstance;
@end
