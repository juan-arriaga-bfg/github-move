//
//  RaveUI.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/RavePGView.h>

//  used by UI classes to automatically determine values based on class prefix length
extern NSString * const RaveUIPrefix;

@class CSSEngine;
@interface RaveUI : NSObject

/**
 Access the shared instance of RaveUI. The first call will initialize Rave UI subsystem.
*/
+ (void)initializeRave;

/**
 Register a custom XML tag, and the view class that it maps to.
 For instance, if you wanted to expose a custom view class called
 XYZChatView, you would call:
    [[RaveUI shared] registerXMLAdapter:[XYZChatView class]]
*/
+ (void)registerXMLAdapter:(Class<RavePGAdapter>)anAdapter;

/**
 */
+ (CSSEngine *)cssEngine;

/**
 Load CSS files into the global namespace. The CSS loaded here will be visible
 to all widgets and scenes.
*/
+ (void)loadCSSFile:(NSString*)fileName;

@end
