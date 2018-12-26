//
///  \file bfgBrandingViewController.h
///  \brief Creates branding splash screens.
///
// \author Created by Sean Hummel on 10/25/10.
// \copyright Copyright 2013 Big Fish Games, Inc. All rights reserved.

#import <UIKit/UIKit.h>

// When the branding screen has been completed, this notifies the application.
#define BFGBRANDING_NOTIFICATION_COMPLETED @"BFGBRANDING_NOTIFICATION_COMPLETED"

///
/// \brief Creates branding splash screens. 
///
/// \details To create and display beginning branding splash screens,
/// you need to allocate and initialize bfgBrandingViewController.
///  \code
///  Example:
///  bfgBrandingViewController * brandingViewCtl = [[bfgBrandingViewController alloc] init];
/// [self.activeViewController.view addSubview: brandingViewCtl.view];
/// \endcode
///
/// If you are using these APIs, you should become an observer for the following events:
///
/// - BFGBRANDING_NOTIFICATION_COMPLETED - Sent after branding is complete via last screen or touch event.
/// 

@interface bfgBrandingViewController : UIViewController

/// \details Forces branding to complete.
- (void)complete;

- (void)startBranding;

@end
