//
//  RavePGView.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import "RavePGObject.h"
#import "RavePGAdapter.h"

@interface RavePGView : RavePGObject <RavePGAdapter, RavePGXMLEntity, NSCopying>

- (void)addChild:(id<RavePGXMLEntity>)aChild;
- (void)addSubview:(UIView*)aView;

- (void)setValue:(NSString*)string forVirtualProperty:(NSString *)propertyName;
+ (NSDictionary*)xmlProperties;

+ (UIView*)internalViewWithAttributes:(NSDictionary *)attributes;
+ (Class)implementerWithAttributes:(NSDictionary*)anAttributtes;

@end
