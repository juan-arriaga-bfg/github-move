//
//  RaveProfilePictureWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/RaveWidget.h>

@interface RaveProfilePictureWidget : RaveWidget

@property (nonatomic, assign) CGFloat cornerRadius;
@property (nonatomic, assign) CGFloat borderWidth;
@property (nonatomic, assign) UIImageView* primaryImageView;

- (void)updateUI;
@end

@interface RaveUserProfilePictureWidget : RaveProfilePictureWidget
@property (nonatomic, retain) id<RaveUser> user;
@end
