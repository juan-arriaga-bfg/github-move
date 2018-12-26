//
//  RaveAchievement.h
//  RaveSocial
//
//  Created by gwilliams on 1/10/14.
//  Copyright (c) 2014 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RaveAchievement <NSObject>
@property (nonatomic, readonly) BOOL isUnlocked;
@property (nonatomic, retain, readonly) NSString * achievementDescription;
@property (nonatomic, retain, readonly) NSString * name;
@property (nonatomic, retain, readonly) NSString * key;
@property (nonatomic, retain, readonly) NSString * imageURL;
@end

