//
//  RSSDKRestManager.h
//  RSSDKRestManager
//
//  Copyright © 2015 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef void (^RSSDKCompletionCallback)(NSError * _Nullable error);
/**
 *  Validation helper - instantiate with a failure callback then continue by validating parameters, terminating the call with the passed callback. If one of hte parameters doesn't pass the check the failure callback will be fired, otherwise the passedCallback will be fired.
 */
@interface RSSDKParamValidation : NSObject

/**
 *  Validates an email
 *
 *  @param email Address string to validate
 *
 *  @return YES if email is valid
 */
+ (BOOL)isValidEmail:(nullable NSString *)email;

/**
 *  Validates a uuid
 *
 *  @param uuid Uuid string to validate
 *
 *  @return YES if uuid is valid
 */
+ (BOOL)isValidUuid:(nullable NSString *)uuid;

/**
 *  Validate a uuid
 *
 *  @param uuid      Expected values are nil or a 32 digit hex value
 *  @param paramName The name of the uuid parameter in the calling context
 *
 *  @return The instance called or nil if validation has failed
 */
- (nonnull instancetype)validateUuidParam:(nullable NSString *)uuid paramName:(nonnull NSString *)paramName;

/**
 *  Validate a non-empty uuid
 *
 *  @param uuid      Expects a 32 digit hex value
 *  @param paramName The name of the uuid parameter in the calling context
 *
 *  @return The instance called or nil if validation has failed
 */
- (nonnull instancetype)validateNonEmptyUuidParam:(nonnull NSString *)uuid paramName:(nonnull NSString *)paramName;

/**
 *  Validate that a value is non-nil
 *
 *  @param value     Expects a non-nil pointer to a value
 *  @param paramName The name of the uuid parameter in the calling context
 *
 *  @return The instance called or nil if validation has failed
 */
- (nonnull instancetype)validateNonNilValueParam:(nullable id)value paramName:(nonnull NSString *)paramName;

/**
 *  Validate that a value is non-nil
 *
 *  @param value Expects a non-nil pointer to a value
 *  @param error The error to return in the case that validation fails
 *
 *  @return The instance called or nil if validation has failed
 */
- (nonnull instancetype)validateNonNilValueParam:(nullable id)value error:(nonnull NSError *)error;

/**
 *  Validate a non-empty string
 *
 *  @param string    Expects a non-nil string of 1 or more characters
 *  @param paramName The name of the uuid parameter in the calling context
 *
 *  @return The instance called or nil if validation has failed
 */
- (nonnull instancetype)validateNonEmptyStringParam:(nullable NSString *)string paramName:(nonnull NSString *)paramName;

/**
 *  Validates a non-empty string
 *
 *  @param string Expects a non-nil string of length 1 or more characters
 *  @param error  The error to return in the case that validation fails
 *
 *  @return The instance called or nil if validation has failed
 */
- (nonnull instancetype)validateNonEmptyStringParam:(nullable NSString *)string error:(nonnull NSError *)error;

/**
 *  Calls failure callback and returns nil to break validation chain
 *
 *  @param error Error to pass to failure callback
 *  @return Always returns nil as convenience to break validation chain
 */
- (nullable instancetype)failed:(nonnull NSError *)error;

@end

@interface RSSDKRestValidation : RSSDKParamValidation

/**
 *  Creates a RSSDRestValidation instance - used for common parameter checks performed when preparing a REST request
 *
 *  @param validationContext Description of the calling context, usually the name of the calling method
 *  @param failureCallback   Callback that will be fired if validation of a parameter fails - providing a descriptive error
 *
 *  @return The validation instance
 */
+ (nonnull instancetype)validationContext:(nonnull NSString *)validationContext failureCallback:(nonnull RSSDKCompletionCallback)failureCallback;

/**
 *  Terminates a validation chain.  Will not be called if validation has failed
 *
 *  @param callback Called when validation for the current instance has passed
 */
- (void)passedCallback:(nonnull dispatch_block_t)callback;
@end

typedef void (^RSSDKRestCallback)(id _Nullable responseObject);

@protocol RSSDKRestMethods <NSObject>

/**
 *  Invoke HTTP GET endpoint
 *
 *  @param URLString  path to endpoint, relative paths will be appended
 *  @param parameters typically query string parameters or nil
 *  @param callback   Callback for success or failure
 */
- (void)GET:(nullable NSString *)URLString parameters:(nullable id)parameters callback:(nullable RSSDKRestCallback)callback;

/**
 *  Invoke HTTP POST endpoint
 *
 *  @param URLString  path to endpoint, relative paths will be appended
 *  @param parameters typically JSON parameters or nil
 *  @param callback   Callback for success or failure
 */
- (void)POST:(nullable NSString *)URLString parameters:(nullable id)parameters callback:(nullable RSSDKRestCallback)callback;

/**
 *  Invoke HTTP PUT endpoint
 *
 *  @param URLString  path to endpoint, relative paths will be appended
 *  @param parameters typically JSON parameters or nil
 *  @param callback   Callback for success or failure
 */
- (void)PUT:(nullable NSString *)URLString parameters:(nullable id)parameters callback:(nullable RSSDKRestCallback)callback;

/**
 *  Invoke HTTP DELETE endpoint
 *
 *  @param URLString  path to endpoint, relative paths will be appended
 *  @param parameters typically JSON parameters or nil
 *  @param callback   Callback for success or failure
 */
- (void)DELETE:(nullable NSString *)URLString parameters:(nullable id)parameters callback:(nullable RSSDKRestCallback)callback;

@end

@interface RSSDKRestManager : RSSDKParamValidation<RSSDKRestMethods>
/**
 *  Factory method returning an instance of the REST manager with the given base URL and timeout interval
 *
 *  @param baseURL         Base URL for relative scoped URL strings
 *  @param timeoutInterval Timeout interval for requests
 *  @param locale          Locale
 *
 *  @return Instance of REST manager
 */
+ (nonnull instancetype)managerWithBaseURL:(nullable NSString *)baseURL timeoutInterval:(NSTimeInterval)timeoutInterval locale:(nonnull NSString *)locale;

/**
 *  Enable debug REST logging, typically called by the SDK
 */
@property (nonatomic, assign) BOOL enableDebugLogging;

/**
 *  Refers to a RSSDKParamValidation instance - used for common parameter checks performed when preparing a REST request
 *
 *  @param restContext       Description of the calling context, usually the name of the calling method
 *  @param failureCallback   Callback that will be fired if validation of a parameter fails - providing a descriptive error
 *
 *  @return The validation instance
 */
- (nonnull instancetype)restContext:(nonnull NSString *)restContext failureCallback:(nonnull RSSDKCompletionCallback)failureCallback;

/**
 *  Method to start download task for given URL
 *
 *  @param URLString   URLString to resource to be downloaded
 *  @param destination URL for file destination
 *  @param progress    Pointer to NSProgress pointer to track download progress
 *  @param callback    Completion callback for download task
 *
 *  @return Download task ready to be resumed
 */
- (nonnull NSURLSessionDownloadTask *)downloadTask:(nonnull NSString *)URLString destination:(nonnull NSURL *)destination progress:(NSProgress * __nullable __autoreleasing * __nullable)progress callback:(nullable RSSDKCompletionCallback)callback;

@end
