//
//  RSEmailRecommendationWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//
#import "RaveWidget.h"

@interface RaveEmailRecommendationWidget : RaveWidget

@property (nonatomic, retain) NSString* htmlBody;
@property (nonatomic, retain) NSString* body;
@property (nonatomic, retain) NSString* subject;
- (void)recommend:(id)aSender;
@end
