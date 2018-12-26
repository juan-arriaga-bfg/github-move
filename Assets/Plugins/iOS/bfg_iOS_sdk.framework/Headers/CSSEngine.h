//
//  CSSEngine.h
//  cssengine
//
//  Created by gwilliams on 3/14/13.
//  Copyright (c) 2013 gwilliams. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

void CSSLog(NSString *format, ...);

#define CSSLOG(format, ...) CSSLog(format, ##__VA_ARGS__)

@protocol CSSAssetsProvider <NSObject>
- (UIImage*)imageForKey:(NSString*)aKey;
- (NSString*)pathForResource:(NSString*)aResource ofType:(NSString*)aType;
@end

@class CSSStyleBlock;
@protocol CSSAssetsProvider;
@interface CSSEngine : NSObject <NSCopying>
- (id)init;
- (void)loadFile:(NSString *) filename;
- (void)loadText:(NSString *) filename;
- (void)applyToRoot:(UIView *) rootView;
- (void)addBlockWithInitializer:(void (^)(CSSStyleBlock * newBlock))block;

+ (void)setAssetsProvider:(id<CSSAssetsProvider>)aProvider;
+ (id<CSSAssetsProvider>)assetsProvider;
+ (void)setLoggingEnabled:(BOOL)aLoggingEnabled;
@end



