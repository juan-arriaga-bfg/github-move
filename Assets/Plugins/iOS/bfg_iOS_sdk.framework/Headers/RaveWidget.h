//
//  RaveWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

/**
 RaveWidget represents a lightweight controller which has its own view and contains a small
 unit of functionality. A RaveWidget automatically updates its UI when the underlying resource
 package changes. Make sure that [RaveUI setBackendHostName:] was called at the same 
 runloop step as the first call to create widget.
*/

#import "RavePGView.h"

@interface RaveWidget : RavePGObject

+ (NSDictionary *)xmlProperties;

- (UIViewController*)parentViewController;

// Designated initializer.
- (id)initWithPGView:(RavePGView*)aPGView cssFiles:(NSArray*)aCssFiles;

- (UIView*)view;

+ (instancetype)widget;
+ (instancetype)widgetWithParentView:(UIView *)parentView;
+ (instancetype)widgetWithContentsOfFile:(NSString*)path;
+ (instancetype)widgetWithContentsOfFile:(NSString*)path cssFiles:(NSArray*)aCssFiles;

- (UIView*)findViewWithID:(NSString *)anId;

- (void)loadView;
- (void)viewDidLoad;
@end
