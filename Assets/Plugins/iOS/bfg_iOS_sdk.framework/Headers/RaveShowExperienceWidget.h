//
//  RSShowExperienceWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RaveWidget.h"

@interface RaveShowExperienceWidget : RaveWidget
- (void)showWithFullScreenSlide:(id)aSender;

+ (RaveShowExperienceWidget*)widget;

- (UIButton*)button;
@end

