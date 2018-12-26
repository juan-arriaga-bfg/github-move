//
//  RavePGAdapter.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

@class CXMLElement;

@protocol RavePGXMLEntity <NSObject>

- (UIView*)view;
- (NSString*)id;
@property (nonatomic, assign) id parent;

- (UIView*)findViewWithID:(NSString *)anId;
- (id<RavePGXMLEntity>)findObjectWithID:(NSString *)anId;
@end

@protocol RavePGAdapter <NSObject>

+ (id<RavePGXMLEntity>)instanceWithXMLElement:(CXMLElement*)anElement
                               parentView:(UIView*)parentView;

+ (NSString *)xmlTagName;

@end
