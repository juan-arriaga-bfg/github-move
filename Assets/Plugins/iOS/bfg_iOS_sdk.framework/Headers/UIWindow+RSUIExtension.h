//
//  UIWindow+RSUIExtension.h
//  RaveUI
//
//  Created by Viktor Iarovyi on 5/20/13.
//  Copyright (c) 2013 Gorilla Graph, Inc. All rights reserved.
//


@interface UIWindow (RSUIExtension)

- (UIInterfaceOrientationMask)rsui_supportedInterfaceOrientations;
- (void)setRsui_supportedInterfaceOrientations:(UIInterfaceOrientationMask)inMask;

@end

@interface UIWindow (RSModelLibraryImplemented)
+ (UIWindow*)rs_createTemporaryWindow;
@end
