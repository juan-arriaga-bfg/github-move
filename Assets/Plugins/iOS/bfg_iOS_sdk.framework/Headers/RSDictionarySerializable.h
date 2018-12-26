//
//  RSDictionarySerializable.h
//  RaveUI
//
//  Created by Iaroslav Pavlov on 7/9/13.
//  Copyright (c) 2013 Gorilla Graph, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RSDictionarySerializable <NSObject>
- (void)rs_restoreStateFromDictionary:(NSDictionary*)aState;
- (void)rs_saveStateToDictionary:(NSMutableDictionary*)aState;

+ (NSArray*)rs_serializableKeyPaths;
- (NSString*)rs_serializableUniqueKey;
@end
