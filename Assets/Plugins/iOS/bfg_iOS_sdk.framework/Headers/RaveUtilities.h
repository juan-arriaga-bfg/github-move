//
//  RaveUtilities.h
//  RaveUI
//
//  Copyright (c) 2013 Rave Social, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>


/**
 *  Rave completion callback triggered when asynchronous method is finished
 *
 *  @param error An error or nil if the operation was successful
 */
typedef void (^RaveCompletionCallback)(NSError * error);

extern NSString * const RaveVersionNumber;

extern NSString * const RaveConnectPluginFacebook;
extern NSString * const RaveConnectPluginGooglePlus;
extern NSString * const RaveConnectPluginPhonebook;
extern NSString * const RaveConnectPluginGameCenter;

typedef NS_OPTIONS(NSUInteger, RaveCallbackResult) {
    RaveResultSuccessful,
    RaveResultCanceled,
    RaveResultError,
};

#ifdef TEST_BUILD
#define WATCH_CALLBACK(a) {a;}
#else
#define WATCH_CALLBACK(a) @try {a;} @catch (NSException *exception) {logCallbackException(exception,__PRETTY_FUNCTION__);} @finally {}
#define SAFE_COMPLETION(real) RaveCompletionCallback safeCallback = ^(NSError * error) {if (real) {WATCH_CALLBACK(real(error))}};
#endif

void logCallbackException(NSException *exception,const char* funcName);

@protocol RaveUser;
@protocol RaveUserFailure <NSObject>
@property (nonatomic, readonly) id<RaveUser> user;
@property (nonatomic, readonly) NSError * error;
@end

typedef void (^RaveReadinessCallback)(BOOL ready, NSError * error);

typedef void (^RaveContactsCallback)(NSArray * contacts, NSError * error);
@class RaveShareRequest;
typedef void (^RaveProviderShareRequestCallback)(RaveShareRequest * request, NSError * error);
// an array of requests
typedef void (^RaveShareRequestCallback)(NSArray * requests, NSError * error);

typedef void (^RaveIdentitiesCallback)(NSArray * identities, NSError * error);
