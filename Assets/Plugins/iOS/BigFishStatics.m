#import "RaveSocialStatics.h"

#import <bfg_iOS_sdk/BigFishScenePack.h>
#import <bfg_iOS_sdk/RaveLoginScene.h>
#import <bfg_iOS_sdk/RaveAccountInfoScene.h>
#import <bfg_iOS_sdk/RaveFindFriendsScene.h>
#import <bfg_iOS_sdk/RaveSignUpEmailScene.h>

NSString* BigFishSignUpDataToString(BigFishSignUpData* obj)
{
    NSMutableString *s = [NSMutableString string];
    NSString* delimiter = @"<BigFishSignUpData>";
    SafeAppend(s,BoolToString(obj.acceptedNewsletter),delimiter);
    SafeAppend(s,BoolToString(obj.passedCoppaCheck),delimiter);
    SafeAppend(s,obj.birthYear,delimiter);
    return s;
}

void UnitySignUpCallback(NSString* callbackModule, NSString* callbackName, NSString* pid, enum RaveCallbackResult result, BigFishSignUpData *signUpData, NSError *error)
{
    UnitySendMessage([callbackModule UTF8String], [callbackName UTF8String], MakeStringCopy([NSString stringWithFormat:@"%@|%@|%@|%@", pid,
        [NSNumber numberWithInt:result], BigFishSignUpDataToString(signUpData), ErrorToString(error)]));
}

void BigFishInitialize(const char* callbackModule, const char* callbackName, const char* pid)
{
    RaveSocialPreInit();

    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    [BigFishScenePack initializeScenePackWithReadyCallback:^(NSError *anError) {
        UnityCompletionCallback(s_callbackModule, s_callbackName, s_pid, anError);
    }];
}

void BigFishShowLoginScene(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    RaveLoginScene * loginScene = [RaveLoginScene scene];
    loginScene.signUpCallback = ^(RaveCallbackResult result, BigFishSignUpData *signUpData, NSError *error) {
        UnitySignUpCallback(s_callbackModule, s_callbackName, s_pid, result, signUpData, error);
    };
    [loginScene show];
}

void BigFishShowAccountInfoScene(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    RaveAccountInfoScene * accountInfoScene = [RaveAccountInfoScene scene];
    accountInfoScene.signUpCallback = ^(RaveCallbackResult result, BigFishSignUpData *signUpData, NSError *error) {
        UnitySignUpCallback(s_callbackModule, s_callbackName, s_pid, result, signUpData, error);
    };
    [accountInfoScene show];
}


void BigFishShowFindFriendsScene(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    RaveFindFriendsScene* findFriendsScene = [RaveFindFriendsScene scene];
    findFriendsScene.sceneCompleteCallback = ^{
        UnitySceneCallback(s_callbackModule, s_callbackName, s_pid, nil);
    };
    [findFriendsScene show];
}

void BigFishShowSignUpEmailScene(const char* callbackModule, const char* callbackName, const char* pid)
{
    NSString* s_callbackModule = SafeString(callbackModule);
    NSString* s_callbackName = SafeString(callbackName);
    NSString* s_pid = SafeString(pid);

    RaveSignUpEmailScene* signUpScene = [RaveSignUpEmailScene scene];
    signUpScene.signUpCallback = ^(RaveCallbackResult result, BigFishSignUpData *signUpData, NSError *error) {
        UnitySignUpCallback(s_callbackModule, s_callbackName, s_pid, result, signUpData, error);
    };
    [signUpScene show];
}

void EnableBigFishMergePolicy() {
    [RaveSocial setMergePolicy:[BigFishMergePolicy new]];
}
