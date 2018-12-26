//
//  RaveKeyboardManager.h
//  RaveUI
//
//  Created by Iaroslav Pavlov on 5/23/12.
//  Copyright (c) 2013 RaveSocial. All rights reserved.
//

#import <Foundation/Foundation.h>


#ifndef __RSKeyboardManager__
#define __RSKeyboardManager__

@interface RaveKeyboardManager : NSObject

+ (RaveKeyboardManager*)sharedInstance;

@property (nonatomic, readonly) BOOL isKeyboardShown;
@property (nonatomic, assign, readonly) CGRect keyboardRect;

@property (nonatomic, retain) NSDictionary* notificationInfo;

/*
 * Hides keyboard on the first found window.
 */
- (void)hideKeyboard;

- (CGRect)keyboardFrameInView:(UIView*)aView;



@end

#endif // __RSKeyboardManager__

