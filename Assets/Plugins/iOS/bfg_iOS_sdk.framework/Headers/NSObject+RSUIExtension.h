//
//  NSObject+RSUIExtension.h
//  RaveUI
//
//  Created by Iaroslav Pavlov on 2/19/13.
//  Copyright (c) Gorilla Graph, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "RSDictionarySerializable.h"

#ifndef __NSObjectRSUIExtension__
#define __NSObjectRSUIExtension__

@interface NSObject (RSUIExtension) <RSDictionarySerializable>
+ (id)rsui_instance;
- (id)rsui_autoreleasedCopy;
- (id)rsui_autoreleasedMutableCopy;
+ (id)rsui_nibInstance;
- (id)rsui_retainAutorelease;
@end




#endif
