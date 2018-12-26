//
//  RaveErrorsAlertWidget.h
//
//  RaveUI
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "RaveWidget.h"

@interface RaveErrorsAlertWidget : RaveWidget

- (void)dismiss;

+ (RaveErrorsAlertWidget*)widgetWithError:(NSError *)error;
+ (RaveErrorsAlertWidget*)widgetWithMessages:(NSArray *)messages;

// should return array of NSString objects with errors descriptions.
@property (nonatomic, retain) NSArray* errorMessages;
- (void)setErrorMessage:(NSError *)error;
@end

