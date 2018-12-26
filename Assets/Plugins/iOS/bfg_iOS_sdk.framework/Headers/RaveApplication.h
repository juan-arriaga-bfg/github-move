//
//  RaveApplication.h
//
//  RaveSocial
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RaveApplication <NSObject>
@property (nonatomic, retain, readonly) NSString * name;
@property (nonatomic, retain, readonly) NSString * applicationDescription;
@property (nonatomic, retain, readonly) NSString * imageUrl;
@property (nonatomic, retain, readonly) NSString * facebookObjectId;
@property (nonatomic, retain, readonly) NSString * applicationId;
@property (nonatomic, retain, readonly) NSArray  * sampleImageUrls;
@end
