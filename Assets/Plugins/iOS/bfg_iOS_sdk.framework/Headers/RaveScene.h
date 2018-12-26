//
//  RaveScene.h
//
//  RaveSocial
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/RaveBusyDelegate.h>
/**
 RaveScene represents a view controller whose UI is defined in an XML file.
 It can host widgets and views, and is automatically updated after its
 resource package changes. Make sure that [RaveSocial setBackendHostName:] was called at the same
 runloop step as the first call to create scene.
*/

@class RaveWidget;
@interface RaveScene : UIViewController <RaveBusyDelegate>

// Designated initializer.
- (id)initWithContentsOfFile:(NSString*)aFileName cssFiles:(NSArray*)aCSSFiles;

/**
 Check this flag before presenting the scene. If the flag's value is NO, you should not
 present the controller because the feature has been disabled.
*/
- (BOOL)canBePresented;

+ (instancetype)sceneWithWidget:(RaveWidget*)aWidget;
+ (instancetype)sceneWithContentsOfFile:(NSString*)aFileName;
+ (instancetype)sceneWithContentsOfFile:(NSString*)aFileName cssFiles:(NSArray*)aCSSFiles;
+ (instancetype)sceneWithContentView:(UIView *)contentView;

- (UIView*)findViewWithID:(NSString*)viewID;

- (id)findObjectWithID:(NSString*)objectID;

- (void)styleView:(UIView *)viewToStyle;

/**
 Expects the XML file name to be identical to the class name, but without the 'Rave' prefix.
 For instance, [RaveLoginScene scene] calls [RaveLoginScene sceneWithContentsOfFile:@"LoginScene.xml"]
*/
+ (instancetype)scene;

- (void)show;
- (void)dismiss;
// The dismiss function below is called by the UI and can be considered private
- (void)dismiss:(id)sender;
- (void)dismissed;
- (void)push;
- (void)pop;

@property (nonatomic, assign, getter=isRootScene) BOOL rootScene;
@property (nonatomic, copy) dispatch_block_t sceneCompleteCallback;
@end
