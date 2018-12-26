//
//  RSReportInappropriatePictureWidget.h
//  RaveUI
//
//  Created by Iaroslav Pavlov on 9/6/13.
//  Copyright (c) 2013 Rave Social, Inc. All rights reserved.
//

/**
 Button with "report-inappropriate-picutre-button" id should be defined in widget's
 xml.
 */

#import "RaveWidget.h"

@interface RaveReportInappropriatePictureWidget : RaveWidget

- (void)report:(id)aSender;

@property (nonatomic, retain) id<RSReadOnlyUserProfile> userProfile;
@end
