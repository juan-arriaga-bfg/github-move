//
//  RaveBusyDelegate.h
//  RaveSocial
//
//  Created by gwilliams on 6/9/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RaveBusyDelegate <NSObject>
- (void)startBusy;
- (void)stopBusy;
- (BOOL)isBusy;
@end
