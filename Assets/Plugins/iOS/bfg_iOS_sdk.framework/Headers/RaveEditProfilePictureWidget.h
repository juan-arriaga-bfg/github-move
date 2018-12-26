//
//  RaveEditProfilePictureWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//
#import <bfg_iOS_sdk/RaveWidget.h>
#import "RaveProfilePictureWidget.h"

typedef void(^RaveEditPictureWidgetCallback)(BOOL pickedImage);

@interface RaveEditProfilePictureWidget : RaveProfilePictureWidget

@property (nonatomic, assign) BOOL autoSave;
@property (nonatomic, copy) RaveEditPictureWidgetCallback callback;

- (void)changePicture:(id)aSender;
- (void)saveImageToUser:(RaveCompletionCallback)callback;
- (void)restore;

@end
