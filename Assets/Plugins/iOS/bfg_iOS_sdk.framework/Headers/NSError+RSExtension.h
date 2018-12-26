//
//  NSError+RSExtension.h
//  RaveSocial
//
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSError (RSExtension)
+ (NSError*)rssdk_criticalErrorWithCode:(NSInteger)code reason:(NSString *)reason userInfo:(NSDictionary *)userInfo;
- (NSError*)rssdk_criticalError;
- (NSError*)rssdk_criticalErrorWithCode:(NSInteger)code;
- (NSError*)rssdk_criticalErrorWithCode:(NSInteger)code reason:(NSString *)reason;
- (BOOL)rssdk_isCriticalError;
- (NSError *)rssdk_originatingError;

+ (NSError*)rs_errorWithReason:(NSString*)aReason;
+ (NSError*)rs_method:(NSString *)method notImplementedForTarget:(NSString *)target;
+ (NSError*)rs_parametersErrorWithReason:(NSString*)aReason;

+ (NSError*)rs_noNetworkError;
+ (NSError*)rs_noLoggedInUserError;
+ (NSError*)rs_needValidateDeviceError;

+ (NSError*)rs_addressBookErrorJustRequestedAuth:(BOOL)aFlag;

+ (NSError*)rs_userExistsError;
+ (NSError*)rs_userExistsErrorWithReason:(NSString*)aReason;
+ (NSError*)rs_gplusIsNotLinkedError;
+ (NSError*)rs_userDoesNotExistError;

+ (NSError*)rs_facebookAuthenticationDataInvalidError;
+ (NSError*)rs_gplusAuthenticationDataInvalidError;
+ (NSError*)rs_restErrorWithJSON:(NSDictionary*)aJSON;
+ (NSError*)rs_userAlreadyLoggedInError;

- (BOOL)rs_isNoNetworkError;
- (BOOL)rs_isGenericNetworkError;
- (BOOL)rs_isParamValidationError;
+ (NSError*)rs_errorWithGPError:(NSError*)aGooglePlusError;

- (NSError *)rs_log;
- (NSError *)rs_logWithContext:(NSString *)context;

+ (NSError *)rs_cancelError;
@end
