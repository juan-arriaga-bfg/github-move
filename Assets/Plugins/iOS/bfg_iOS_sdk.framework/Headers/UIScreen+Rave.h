//
//  UIScreen+Rave.h
//  RaveSocial
//
//  Created by gwilliams on 7/30/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol OrientationAccess <NSObject>
- (UIInterfaceOrientation) approachingOrientation;
@end

@interface OrientationAccess : NSObject
+(id)instance;
@end

@interface UIScreen (Rave)
+ (CGRect)orientedScreenBounds;

// This is the reference size of whichever side of the screen is smallest, allowing us to scale content
+ (float)referenceScreenDimension;

// Will scale given view according to reference dimensions of platform
+ (void)scaleViewToReference:(UIView*)view;
@end
