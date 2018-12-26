//
//  RaveEditProfilePictureController.h
//  RaveSocial
//
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 *  Returns a valid image when one is chosen otherwise an error will be returned
 *
 *  @param image  UIImage chosen by the user
 *  @param error  indicates whether an error occurred
 */
typedef void (^RaveEditProfilePictureObserver)(UIImage * image, NSError * error);

/**
 *  This controller wraps basic functionality for picking a photo from the library or directly from the camera
 */
@interface RaveEditProfilePictureController : NSObject
/**
 *  Observe when a picture is chosen by the user or an error occurs
 *
 *  @param observer The observer callback
 */
- (void)setObserver:(RaveEditProfilePictureObserver)observer;

/**
 *  Trigger native UI for choosing a picture from the library or camera
 *
 *  @param autoSave   Designate whether the controller should automatically save the chosen picture to the current user
 *  @param anchorView View that anchors the user action that triggered the chooser - used to locate the chooser UI
 */
- (void)choosePicture:(BOOL)autoSave anchorView:(UIView *)anchorView;

/**
 *  Saves an image to the Rave backend for the current user
 *
 *  @param image Image to be saved
 *  @param callback Save completion callback reports an error or none if operation succeeded
 */
- (void)saveImage:(UIImage *)image callback:(RaveCompletionCallback)callback;
@end
