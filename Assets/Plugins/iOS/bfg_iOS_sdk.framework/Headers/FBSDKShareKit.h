// Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
//
// You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
// copy, modify, and distribute this software in source code or binary form for use
// in connection with the web services and APIs provided by Facebook.
//
// As with any software that integrates with the Facebook platform, your use of
// this software is subject to the Facebook Developer Principles and Policies
// [http://developers.facebook.com/policy/]. This copyright notice shall be
// included in all copies or substantial portions of the software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#import <UIKit/UIKit.h>

#import <bfg_iOS_sdk/FBSDKHashtag.h>
#import <bfg_iOS_sdk/FBSDKShareAPI.h>
#import <bfg_iOS_sdk/FBSDKShareConstants.h>
#import <bfg_iOS_sdk/FBSDKShareLinkContent.h>
#import <bfg_iOS_sdk/FBSDKShareOpenGraphAction.h>
#import <bfg_iOS_sdk/FBSDKShareOpenGraphContent.h>
#import <bfg_iOS_sdk/FBSDKShareOpenGraphObject.h>
#import <bfg_iOS_sdk/FBSDKSharePhoto.h>
#import <bfg_iOS_sdk/FBSDKSharePhotoContent.h>
#import <bfg_iOS_sdk/FBSDKShareVideo.h>
#import <bfg_iOS_sdk/FBSDKShareVideoContent.h>
#import <bfg_iOS_sdk/FBSDKSharing.h>
#import <bfg_iOS_sdk/FBSDKSharingContent.h>

#if !TARGET_OS_TV
#import <bfg_iOS_sdk/FBSDKAppGroupAddDialog.h>
#import <bfg_iOS_sdk/FBSDKAppGroupContent.h>
#import <bfg_iOS_sdk/FBSDKAppGroupJoinDialog.h>
#import <bfg_iOS_sdk/FBSDKAppInviteContent.h>
#import <bfg_iOS_sdk/FBSDKAppInviteDialog.h>
#import <bfg_iOS_sdk/FBSDKGameRequestContent.h>
#import <bfg_iOS_sdk/FBSDKGameRequestDialog.h>
#import <bfg_iOS_sdk/FBSDKLikeButton.h>
#import <bfg_iOS_sdk/FBSDKLikeControl.h>
#import <bfg_iOS_sdk/FBSDKLikeObjectType.h>
#import <bfg_iOS_sdk/FBSDKMessageDialog.h>
#import <bfg_iOS_sdk/FBSDKShareButton.h>
#import <bfg_iOS_sdk/FBSDKShareDialog.h>
#import <bfg_iOS_sdk/FBSDKShareDialogMode.h>
#import <bfg_iOS_sdk/FBSDKShareMediaContent.h>
#import <bfg_iOS_sdk/FBSDKSendButton.h>
#else
#import <bfg_iOS_sdk/FBSDKDeviceShareViewController.h>
#import <bfg_iOS_sdk/FBSDKDeviceShareButton.h>
#endif
