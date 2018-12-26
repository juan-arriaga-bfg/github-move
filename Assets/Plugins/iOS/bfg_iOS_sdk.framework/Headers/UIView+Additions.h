//
//  UIView+Additions.h
//  Ferrari
//
//  Created by Iaroslav Pavlov on 8/17/11.
//  Copyright 2011 Lohika, Inc. All rights reserved.
//


#if TARGET_OS_IPHONE

@interface UIView (RSUIAdditions)
- (UIResponder*)rs_firstResponder;

@property (nonatomic, assign) CGSize frameSize;
@property (nonatomic, assign) CGPoint frameOrigin;
@property (nonatomic, assign) CGFloat frameWidth;
@property (nonatomic, assign) CGFloat frameHeight;
@property (nonatomic, assign) CGFloat frameOriginX;
@property (nonatomic, assign) CGFloat frameOriginY;
@property (nonatomic, assign) CGFloat frameCenterX;
@property (nonatomic, assign) CGFloat frameCenterY;

@property (nonatomic, assign) CGPoint frameCenter;
@property (nonatomic, assign) CGPoint boundsCenter;
@property (nonatomic, assign) CGFloat boundsCenterX;
@property (nonatomic, assign) CGFloat boundsCenterY;

- (void)rs_updateCSS;
- (void)rs_attachPegasusEventHandlersToTarget:(id)aTarget;
- (void)rs_addTapSelector:(SEL)selector forTarget:(id)target;

- (UIView*)rs_findViewWithID:(NSString*)anId;
- (UIViewController *)rs_viewController;
- (id)rs_searchForUIViewController;
@end

@interface UIButton (Additions)
+ (NSArray *)rs_serializableKeyPaths;
@end


@interface UIImageView (Additions)
+ (NSArray *)rs_serializableKeyPaths;
@end
#endif
