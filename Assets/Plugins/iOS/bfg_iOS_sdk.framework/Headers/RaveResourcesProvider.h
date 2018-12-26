//
//  RaveResourcesProvider.h
//
//  RaveSocial
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
@class UIImage;

extern NSString* const kRSNewResourcesAvailableNotification;
extern NSString* const kRSNewResourcesAvailableToDownloadNotification;


@protocol RaveResourcesProvider;

@interface RaveResourcesProvider : NSObject

/*
 Current resources provider.
 */
+ (id<RaveResourcesProvider>)currentProvider;

/**
 Sets the default resources path and computes an MD5 hash from the zip file.
 Must be set to enable resource loading features. Must be realtive to [[NSBundle mainBundle] resourcePath]
 path.
 */
+ (void)setDefaultResourcesPath:(NSString*)aDefaultResourcesPath;
+ (NSString*)defaultResourcesPath;

@end

@protocol RaveResourcesProvider <RaveObject>

/**
 Observe this property with KVO to know when a new skin is available.
*/
@property (nonatomic, readonly) BOOL isNewResourcesAvailable;

/**
 Observe this property with KVO to know when a new skin is available to download.
*/
@property (nonatomic, readonly) BOOL isNewResourcesAvailableToDownload;

/**
 Observe this property with KVO to know about download progress.
 Reported in percents.
*/
@property (assign) double downloadProgress;
@property (nonatomic, retain) NSProgress * progress;

- (void)downloadResourcesWithCompletion:(RaveCompletionCallback)aCompletion;
- (void)installResourcesWithCompletion:(RaveCompletionCallback)aCompletion;

- (UIImage*)imageForKey:(NSString*)aKey;

@property (nonatomic, assign) UIInterfaceOrientation targetOrientation;

- (NSString*)pathForResource:(NSString*)aName ofType:(NSString*)aType;

- (NSString*)pathForResource:(NSString*)aName ofType:(NSString*)aType
                forOrientation:(UIInterfaceOrientation)anOrintaiton;

/**
 @brief Returns orientation specific name for resource, for exampele /tmp/foo.png
 will be /tmp/foo_landscape.png if approaching interface orientation is landscape.
 
 @param aResource Name of a resource.
 */
- (NSString*)orientaionSpecificNameForResource:(NSString*)aResource;


- (void)skipNewVersion;


@end
