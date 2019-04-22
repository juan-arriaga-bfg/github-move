//
//  bfgRave.h
//  bfg_iOS_sdk
//
//  Created by Jason Booth on 6/30/16.
//  Copyright © 2016 Big Fish Games, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdocumentation"
#import <bfg_iOS_sdk/RaveSocial.h>
#import <bfg_iOS_sdk/RaveAppDataKeysManager.h>
#pragma clang diagnostic pop


@protocol bfgRaveDelegate;
@protocol bfgRaveAppDataKeyDelegate;


NS_ASSUME_NONNULL_BEGIN


FOUNDATION_EXPORT NSString * const _Nonnull BFGRaveErrorDomain;

typedef NS_ENUM(NSInteger, BFGRaveErrorCode)
{
    BFGRaveErrorCodeInternalError = 1
    , BFGRaveErrorCodeFailedToAcquireRaveId = 2
};

///
/// \brief Completion block for waiting on Rave id.  If raveId is null, then error will be populated.
///
/// \since 6.6
///
typedef void (^bfgRaveReadyCompletionBlock)(NSString * _Nullable raveId, NSError * _Nullable error);

///
/// \brief Completion block for fetching Rave profile picture
///
/// \since 6.3
///
typedef void (^bfgRaveProfilePictureCompletionBlock)(UIImage * _Nullable image);

///
/// \brief Provides methods for getting information about the current user.
///
/// \since 6.0
///
@interface bfgRave : NSObject <RaveCurrentUserObserver, RaveAppDataKeysStateObserver>

///
/// \details Listen for a rave id to become available
///
/// \param completion Completion block to execute when rave id becomes available.  Block will execute on the main thread.
///
/// \since 6.8
///
+ (void)listenForRaveReady:(bfgRaveReadyCompletionBlock)completion;

///
/// \details Set the Rave delegate before initializing bfgManager to receive Rave login status changes.
///
/// \param delegate The delegate for bfgRave.
///
/// \since 6.0
///
+ (void)setDelegate:(id<bfgRaveDelegate> _Nullable)delegate;

///
/// \details Set the Rave AppDataKeys delegate before initializing bfgManager to be informed of app data key changes and conflicts.
///
/// \param delegate The delegate for bfgRaveAppDataKeyDelegate.
///
/// \since 6.2.0
///
+ (void)setRaveAppDataKeyDelegate:(id<bfgRaveAppDataKeyDelegate>)delegate;

///
/// \details Call this to directly observe status changes in Rave.
///
/// \note When using this method to observe Rave's status changes it's best to make this call from within the listenForRaveReady callback.  This will guarantee that the externalCallback is not made until Rave is fully initialized and ready for use.
///
/// \param externalCallback The block of code to be called when there is a change in login status.
///
/// \since 6.0
///
+ (void)setupLoginStatusCallbackWithExternalCallback:(RaveLoginStatusCallback)externalCallback;

///
/// \details Get the Rave ID of the current user.
///
/// \returns The Rave ID of the current user or nil if there isn't one.
///
/// \since 6.0
///
+ (NSString * _Nullable)currentRaveId;

///
/// \details Get the Rave display name of the current user.
///
/// \returns The Rave display name of the current user or nil if there isn't one.
///
/// \since 6.0
///
+ (NSString * _Nullable)currentRaveDisplayName;

///
/// \details Get the Rave email of the current user.
///
/// \returns The Rave email of the current user or nil if there isn't one.
///
/// \since 6.4
///
+ (NSString * _Nullable)currentRaveEmail;

///
/// \details Get the Rave Profile picture of the current user.
///
/// \param completion The block of code to be called when profile picture is fetched.
///
/// \since 6.3
///
+ (void)fetchCurrentRaveProfilePicture:(bfgRaveProfilePictureCompletionBlock)completion;

///
/// \details Get whether the current user is a guest (unauthenticated).
///
/// \returns YES if the current user is a guest, NO if authenticated or they have personalized their profile.
///
/// \since 6.0
/// \deprecated Deprecated since iOS SDK 6.3
///
+ (BOOL)isCurrentGuest __deprecated_msg("Deprecated since iOS SDK 6.3");

///
/// \details Get whether the current user is authenticated.
///
/// \returns YES if the current user is a authenticated, NO if unauthenticated.
///
/// \since 6.3
///
+ (BOOL)isCurrentAuthenticated;

///
/// \details Get whether the current user is personalized (e.g., some user data has been customized but user is not authenticated).
///
/// \returns YES if the current user is personalized, NO if non-personalized.
///
/// \since 6.3
///
+ (BOOL)isCurrentPersonalized;

///
/// \details Get the Rave ID of the last user that is not the same as the current user.
///
/// \returns The Rave ID of the last user or nil if there isn't one.
///
/// \since 6.0
///
+ (NSString * _Nullable)lastRaveId;

///
/// \details Get whether the last user is a guest (unauthenticated).
///
/// \returns YES if the last user is a guest, NO if authenticated.
///
/// \since 6.0
///
+ (BOOL)isLastGuest;

///
/// \details Logs out the current user if that user is not a guest.
///
/// \since 6.0
///
+ (void)logoutCurrentUser;

///
/// \details Presents the standard sign-in screen if the current user is not authenticated.
///
/// \since 6.0
///
+ (void)presentSignIn;

///
/// \details Presents the standard sign-in screen if the current user is not authenticated.
///
/// \param origin A string denoting the starting point of the auth flow for reporting. Allows us to measure which areas in the game produce the highest authentication conversions. Example: 'level_end'
///
/// \since 6.3
///
+ (void)presentSignInWithOrigin:(NSString * _Nullable)origin;

///
/// \details Presents the Newsletter signup screen if the current user is not authenticated.
///
/// \since 6.3
///
+ (void)presentNewsletterSignup;

///
/// \details Presents the Newsletter signup screen if the current user is not authenticated.
///
/// \param origin A string denoting the starting point of the auth flow for reporting. Allows us to measure which areas in the game produce the highest authentication conversions. Example: 'level_end'
///
/// \since 6.3
///
+ (void)presentNewsletterSignupWithOrigin:(NSString * _Nullable)origin;

///
/// \details Presents the standard profile screen if the current user is authenticated or personalized.
///
/// \since 6.0
///
+ (void)presentProfile;

///
/// \details Presents the standard profile screen if the current user is authenticated or personalized.
///
/// \param origin A string denoting the starting point of the auth flow for reporting. Allows us to measure which areas in the game produce the highest authentication conversions. Example: 'level_end'
///
/// \since 6.3
///
+ (void)presentProfileWithOrigin:(NSString * _Nullable)origin;

///
/// \details Returns whether rave is initialized or not.
///
/// \since 6.0
///
+ (BOOL)isRaveInitialized;

///
/// \details Call this to set the Rave display name.
///
/// \param displayName The name that the Rave display name will be changed to.
///
/// \since 6.0
///
+ (void)changeRaveDisplayName:(NSString * _Nonnull)displayName;

///
/// \details Call this to select the App Data Key.
/// \note Only call this after a key is selected (by the user or the game) from the set of unresolved keys.
///
/// \param key The key that the App Data Key will be changed to.
///
/// \since 6.2.0
///
+ (void)selectRaveAppDataKey:(NSString * _Nonnull)key;

///
/// \details Call this to initiate a fetch of the current Rave App Data Key.
/// This makes an asynchronous call to Rave to retrieve the App Data Key.
/// If the fetch is successful, the delegate method bfgRaveFetchCurrentAppDataKeyDidSucceed: will be called.
/// If the fetch is unsuccessful, the delegate method bfgRaveFetchCurrentAppDataKeyDidFailWithError: will be called.
/// In the case where you are offline and have unresolved keys or logged out while offline, calling this method will call the delegate method bfgRaveFetchCurrentAppDataKeyDidFailWithError: with nil as the error value.
///
/// \since 6.2.0
///
+ (void)fetchCurrentAppDataKey;

@end

///
/// \brief Interface bfgRaveDelegate is used to provide games with a method of receiving RaveSocial SDK callbacks.
///
/// \since 6.0
///
@protocol bfgRaveDelegate <NSObject>

@optional

///
/// \details Informs the delegate when there has been an error with the profile screen.
///
/// \param error The error, if any, that was encountered.
///
/// \since 6.0
///
- (void)bfgRaveProfileFailedWithError:(NSError * _Nullable)error;

///
/// \details Informs the delegate that the profile updated successfully.
///
/// \since 6.0
///
- (void)bfgRaveProfileSucceeded;

///
/// \details Informs the delegate that the user cancelled the profile screen.
///
/// \since 6.0
///
- (void)bfgRaveProfileCancelled;

///
/// \details Informs the delegate of the COPPA status after a successful signin.
///
/// \param passedCOPPA YES if the user passed the COPPA check, NO if the user failed the COPPA check.
///
/// \since 6.0
///
- (void)bfgRaveSignInCOPPAResult:(BOOL)passedCOPPA;

///
/// \details Informs the delegate that the signin succeeded.
///
/// \since 6.0
///
- (void)bfgRaveSignInSucceeded;

///
/// \details Informs the delegate that the signin was cancelled by the user.
///
/// \since 6.0
///
- (void)bfgRaveSignInCancelled;

///
/// \details Informs the delegate when there has been an error with the signin screen.
///
/// \param error The error, if any, that was encountered.
///
/// \since 6.0
///
- (void)bfgRaveSignInFailedWithError:(NSError * _Nullable)error;

///
/// \details Informs the delegate when a user has logged in.
///
/// \since 6.0
///
- (void)bfgRaveUserDidLogin;

///
/// \details Informs the delegate when a user has logged out.
///
/// \since 6.0
///
- (void)bfgRaveUserDidLogout;

///
/// \details Informs the delegate when there has been an error in the login process.
///
/// \param loginError The error, if any, that was encountered.
///
/// \since 6.0
///
- (void)bfgRaveUserLoginError:(NSError * _Nullable)loginError;

///
/// \details Informs the delegate when the Rave display name has been changed successfully.
///
/// \since 6.0
///
- (void)bfgRaveChangeDisplayNameDidSucceed;

///
/// \details Informs the delegate when the Rave display name change was unsuccessful.
///
/// \param error The error, if any, that was encountered.
///
/// \since 6.0
///
- (void)bfgRaveChangeDisplayNameDidFailWithError:(NSError * _Nullable)error;

@end

///
/// \brief To use Rave App Data Keys you must register to be a bfgRaveAppDataKeyDelegate.
///
/// \since 6.2.0
///
/// \note There are some offline cases when the app data key will not be current.  Going online will resolve this.
///
@protocol bfgRaveAppDataKeyDelegate <NSObject>

@required

///
/// \details Informs the delegate that the App Data Key changed.
///
/// \param currentAppDataKey The current app data key selected for this application and user.
/// \param previousKey The previous app data key before the change.
///
/// \since 6.2.0
///
- (void)bfgRaveAppDataKeyDidChange:(NSString * _Nonnull)currentAppDataKey previousKey:(NSString * _Nullable)previousKey;

///
/// \details Informs the delegate that there are unresolved keys.
///
/// \param unresolvedKeys The keys that need to be resolved.  There will always be zero or two or more.
/// \param currentAppDataKey The last known selected app data key before conflict.
///
/// \since 6.2.0
///
- (void)bfgRaveAppDataKeyDidReturnUnresolvedKeys:(NSArray<NSString *> * _Nonnull)unresolvedKeys currentAppDataKey:(NSString * _Nullable)currentAppDataKey;

///
/// \details Informs the delegate that selectRaveAppDataKey: succeeded.
///
/// \since 6.2.0
///
- (void)bfgRaveSelectAppDataKeyDidSucceed;

///
/// \details Informs the delegate that selectRaveAppDataKey: failed.
///
/// \param error The error that was encountered.
///
/// \since 6.2.0
///
- (void)bfgRaveSelectAppDataKeyDidFailWithError:(NSError * _Nonnull)error;

@optional

///
/// \details Informs the delegate that fetchCurrentAppDataKey succeeded.
///
/// \param currentAppDataKey The current app data key returned by Rave.
///
/// \since 6.2.0
///
- (void)bfgRaveFetchCurrentAppDataKeyDidSucceed:(NSString * _Nonnull)currentAppDataKey;

///
/// \details Informs the delegate that fetchCurrentAppDataKey failed.
///
/// \note In the case where you are offline and have unresolved keys or logged out while offline, calling fetchCurrentAppDataKey will call this method with nil as the error value.
///
/// \param error The error that was encountered.
///
/// \since 6.2.0
///
- (void)bfgRaveFetchCurrentAppDataKeyDidFailWithError:(NSError * _Nullable)error;

@end


NS_ASSUME_NONNULL_END
