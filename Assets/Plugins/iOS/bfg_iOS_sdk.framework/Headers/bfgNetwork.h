/// \file bfgNetwork.h
/// \brief Network connectivity helper.
///
/// \since 4.5.0
///
//  bfgNetwork.h
//  BFGUIKitExampleQA
//
//  Created by Benjamin Flynn on 10/3/13.
//  Updated by Craig Thompson on 10/3/13.
//  Copyright (c) 2013 Big Fish Games, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/bfglibPrefix.h>

// Forward declaration
@class bfgReachability;


///
/// \details This notification is sent when the status for the network has been updated as a result of the change.
/// You should listen for this notification if you want to synchronously query the updated results for the network status.
///
/// \since 4.5.0
///
#define BFGNETWORK_NOTIFICATION_STATUS_UPDATED @"BFGNETWORK_NOTIFICATION_STATUS_UPDATED"


///
/// \details Sent as a key/value pair in the _object_ of the status update notification, the previous network status.
///
/// \since 4.5.0
///
#define BFG_NETWORK_DICTIONARY_PREVIOUS_STATUS @"previous_internet_status"


///
/// \details Sent as a key/value pair in the _object_ of the status update notification, the current network status.
///
/// \since 4.5.0
///
#define BFG_NETWORK_DICTIONARY_CURRENT_STATUS @"current_internet_status"


///
/// \details These statuses are returned wrapped in an NSNumber as the value of status update notification.
///
/// \since 4.5.0
///
typedef enum
{
    bfgNetworkNotReachable = 0,
    bfgNetworkReachableViaWWAN = 1,
    bfgNetworkReachableViaWiFi = 2
} bfgNetworkStatus;


///
/// \brief A helper class for determining whether or not an Internet connection is available.
///
/// \since 4.5.0
///
@interface bfgNetwork : NSObject

///
/// \details A singleton accessor to a bfgNetwork instance.
///
/// \since 4.5.0
///
+ (bfgNetwork *)sharedInstance;

///
/// \details Initializes bfgNetwork. This is called automatically by bfgManager.
///
/// \since 4.5.0
///
+ (void)initialize;


///
/// \details The network is reachable via either WiFi or cellular means.
///
/// \since 4.5.0
///
- (BOOL)isReachable;


///
/// \details The network is reachable via WiFi.
///
/// \since 4.5.0
///
- (BOOL)isWIFI;


///
/// \details Ask for the reachability status of a particular host. Checks for host reachability are asynchronous
/// and this method provides the latest available value.
///
/// \since 4.5.0
///
- (BOOL)isHostReachable:(NSString *)remoteHost;


///
/// \details When interested in the reachability of a particular host, add the name of that host here and
/// the service will asynchronously determine whether or not it can be reached.
///
/// \since 4.5.0
///
- (BOOL)addRemoteHost:(NSString *)remoteHost;

@end
