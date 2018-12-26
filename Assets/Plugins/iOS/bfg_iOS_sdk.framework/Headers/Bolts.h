/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 *
 */

#import <bfg_iOS_sdk/BFCancellationToken.h>
#import <bfg_iOS_sdk/BFCancellationTokenRegistration.h>
#import <bfg_iOS_sdk/BFCancellationTokenSource.h>
#import <bfg_iOS_sdk/BFExecutor.h>
#import <bfg_iOS_sdk/BFTask.h>
#import <bfg_iOS_sdk/BFTask+Exceptions.h>
#import <bfg_iOS_sdk/BFTaskCompletionSource.h>

#if __has_include(<bfg_iOS_sdk/BFAppLink.h>) && TARGET_OS_IPHONE && !TARGET_OS_WATCH && !TARGET_OS_TV
#import <bfg_iOS_sdk/BFAppLink.h>
#import <bfg_iOS_sdk/BFAppLinkNavigation.h>
#import <bfg_iOS_sdk/BFAppLinkResolving.h>
#import <bfg_iOS_sdk/BFAppLinkReturnToRefererController.h>
#import <bfg_iOS_sdk/BFAppLinkReturnToRefererView.h>
#import <bfg_iOS_sdk/BFAppLinkTarget.h>
#import <bfg_iOS_sdk/BFMeasurementEvent.h>
#import <bfg_iOS_sdk/BFURL.h>
#import <bfg_iOS_sdk/BFWebViewAppLinkResolver.h>
#endif


NS_ASSUME_NONNULL_BEGIN

/**
 A string containing the version of the Bolts Framework used by the current application.
 */
extern NSString *const BoltsFrameworkVersionString;

NS_ASSUME_NONNULL_END
