//
//  RaveAssetsProvider.h
//  RaveUI
//
//  Created by Iaroslav Pavlov on 5/13/13.
//  Copyright (c) 2013 Gorilla Graph, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RavePGAssetsProvider;
@protocol CSSAssetsProvider;
@interface RaveAssetsProvider : NSObject <RavePGAssetsProvider, CSSAssetsProvider>

@end
