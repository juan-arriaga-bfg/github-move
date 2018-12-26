//
//  AuthFlowScene.h
//  BigFishScenePack
//
//  Created by Jason Booth on 6/16/17.
//  Copyright © 2017 Rave Social, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveSocial.h>

typedef NS_ENUM(NSInteger, AuthFlowSceneName) {
    AuthFlowSceneNameNone
    , AuthFlowSceneNameAuthHome
    , AuthFlowSceneNameLogin
    , AuthFlowSceneNameSignUp
    , AuthFlowSceneNameNewsletter
    , AuthFlowSceneNameForgot
    , AuthFlowSceneNameWelcome
    , AuthFlowSceneNameProfile
    , AuthFlowSceneNameWelcomeCoppaFailure
    , AuthFlowSceneNameNLSubscribed
    , AuthFlowSceneNameNLCoppaFailure
    , AuthFlowSceneNameNLCASLOptOut
    , AuthFlowSceneNameSignOut
    , AuthFlowSceneNameFBWelcome
};

typedef NS_ENUM(NSInteger, AuthFlowHotSpot) {
    AuthFlowHotSpotDismiss
    , AuthFlowHotSpotBack
    , AuthFlowHotSpotRegisterEmail
    , AuthFlowHotSpotSignInEmail
    , AuthFlowHotSpotSignInFB
    , AuthFlowHotSpotSignIn
    , AuthFlowHotSpotForgot
    , AuthFlowHotSpotRegister
    , AuthFlowHotSpotSignUp
    , AuthFlowHotSpotSendEmail
    , AuthFlowHotSpotConnectFB
    , AuthFlowHotSpotNotNow
    , AuthFlowHotSpotSave
    , AuthFlowHotSpotSignOut
    , AuthFlowHotSpotClose
    , AuthFlowHotSpotDone
    , AuthFlowHotSpotCancelSignOut
    , AuthFlowHotSpotForcedDismiss
    , AuthFlowNewsletterOptInHotSpotRegister
    , AuthFlowNewsletterOptInHotSpotOptOut
};

@interface AuthFlowScene : RaveScene

+ (NSString *)stringFromSceneName:(AuthFlowSceneName)sceneName;
+ (NSString *)stringFromHotSpot:(AuthFlowHotSpot)hotspot;

- (AuthFlowSceneName)sceneName;

- (UIButton *)buttonForViewId:(NSString *)viewId;

- (void)forceDismiss;

- (void)setKeyboardFields:(NSArray<UIView*> *)keyboardFields;
- (void)dismissKeyboard;

- (void)sceneWillAppear;

@end
