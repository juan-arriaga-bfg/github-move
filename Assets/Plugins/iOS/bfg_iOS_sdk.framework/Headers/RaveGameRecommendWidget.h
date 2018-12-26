//
//  RSGameRecommendWidget.h
//
//  RaveUI
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//
#import "RaveWidget.h"

@interface RaveGameRecommendWidget : RaveWidget

@property (nonatomic, retain) NSString* connectedMessage;

- (void)selectFacebook:(id)aSender;
- (void)selectGooglePlus:(id)aSender;
- (void)handleRecommend:(id)sender;

@end
