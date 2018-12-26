/// \file bfgPurchase.h
/// \brief bfgPurchase header file.
///
// \author Created by Sean Hummel on 5/12/11.
// \author Updated by Craig Thompson on 10/1/13.
// \author Updated by Craig Thompson on 7/14/15.
// \copyright Copyright 2013 Big Fish Games, Inc. All rights reserved.
///

/*! \file */

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>

#import <bfg_iOS_sdk/bfgPurchaseObject.h>

/// Key for notification userInfo that returns an array of productIds of products for which
/// product information was successfully acquired from the App Store.
///
/// \since 4.2
#define BFG_ACQUIRE_PRODUCT_INFO_SUCCESSES_USER_INFO_KEY    @"BFG_ACQUIRE_PRODUCT_INFO_SUCCESSES_KEY"

/// Key for notification userInfo that returns an array of productIds of products for which
/// product information could not be acquired. Failure is generally due to the ID not being
/// found on the App Store.
///
/// \since 4.2
#define BFG_ACQUIRE_PRODUCT_INFO_FAILURES_USER_INFO_KEY     @"BFG_ACQUIRE_PRODUCT_INFO_FAILURES_KEY"

/// Key for notification userInfo that returns a purchase object.
///
/// \since 4.2
#define BFG_PURCHASE_OBJECT_USER_INFO_KEY                   @"BFG_PURCHASE_OBJECT_USER_INFO_KEY"

/// Key for notification userInfo that returns a product ID.
///
/// \since 4.7
#define BFG_PRODUCT_ID_USER_INFO_KEY                        @"BFG_PRODUCT_ID_USER_INFO_KEY"

/// Key for restore failed notification that indicates whether failure was caused by cancel.
/// If the key is present in the userInfo and has value of true then restore failure was
/// caused by the user canceling the restore and the game does not need to surface a
/// failure message.
///
/// \since 5.10
#define BFG_RESTORE_CANCELLED_USER_INFO_KEY                 @"BFG_RESTORE_CANCELLED_USER_INFO_KEY"

/// The purchase has been completely removed from the queue. Has a product ID for its userInfo.
///
/// \since 4.7
#define NOTIFICATION_FINISH_PURCHASE_COMPLETE               @"NOTIFICATION_FINISH_PURCHASE_COMPLETE"


/// The purchase failed.
///
/// \since 4.2
#define NOTIFICATION_PURCHASE_FAILED                        @"NOTIFICATION_PURCHASE_FAILED"

/// The purchase succeeded.
///
/// \since 4.2
#define NOTIFICATION_PURCHASE_SUCCEEDED                     @"NOTIFICATION_PURCHASE_SUCCEEDED"

/// The purchase was deferred.
///
/// This can occur when family shared purchases and Ask To Buy are enabled. If this
/// notification is received, resume the game with the expectation of the purchase completing
/// sometime in the future (once the purchase has been approved by the parent/card holder).
/// Be aware that when a purchase is deferred, the bfgPurchase sucess flag will be set to NO
/// even though the purchase might eventually succeed.
///
/// \since 6.0
#define NOTIFICATION_PURCHASE_DEFERRED                      @"NOTIFICATION_PURCHASE_DEFERRED"

/// The restore failed.
///
/// \since 4.2
#define NOTIFICATION_RESTORE_FAILED                         @"NOTIFICATION_RESTORE_FAILED"

/// The restore succeeded.
///
/// \since 4.2
#define NOTIFICATION_RESTORE_SUCCEEDED                      @"NOTIFICATION_RESTORE_SUCCEEDED"

/// The product information about a purchase.
///
/// \since 4.0
/// updated 4.2
#define NOTIFICATION_PURCHASE_PRODUCTINFORMATION            @"NOTIFICATION_PURCHASE_PRODUCTINFORMATION"

/// Product information must not contain more than 10K.
///
/// \since 4.2
#define MAX_PRODUCT_INFO_BYTES                              (1024u * 10u)


///
/// \brief Manages Apple's In-App Purchases for Free-to-Play games.
///
/// If you are using these APIs, you should become an observer
/// for the following events:
///
/// - NOTIFICATION_PURCHASE_PRODUCTINFORMATION
/// - NOTIFICATION_PURCHASE_SUCCEEDED
/// - NOTIFICATION_PURCHASE_FAILED
/// - NOTIFICATION_PURCHASE_DEFERRED
/// - NOTIFICATION_RESTORE_SUCCEEDED
/// - NOTIFICATION_RESTORE_FAILED
///
/// It is important to begin observing these events before starting
/// the bfgPurchase service.
///
/// Occasionally a game will need to support functionality beyond what is provided
/// by bfgPurchase. For example, bfgPurchase does not support downloadable content. In
/// this case, you can talk to your producer about setting purchaseExpertMode to "1".
/// With purchaseExpertMode enabled, you will receive SKPaymentTransactions with your
/// bfgPurchaseObject when purchasing and restoring. It is your responsibility to call
/// finishPurchase on both new purchases and restores if you enable expert mode. In
/// non-expert mode, restores will close themselves.
///
/// \since 4.0.0
///
@interface bfgPurchase : NSObject


///
/// \details Requests product information of the default product
/// from the App Store. This method *must* be called before the default product can be purchased.
///
/// \see acquireProductInformationForProducts:
///
/// \return
/// \retval YES The request for product information is sent to the App Store.
/// \retval NO The user is offline or the default productId is already being requested.
///
/// \since 4.0.0
///
+ (BOOL)acquireProductInformation;


///
/// \details Requests product information from the App Store.
/// This method *must* be called before the requested product can be purchased.
///
/// If multiple products are being requested, it is much more efficient to use
/// acquireProductInformationForProducts: rather than repeated calls to this method.
///
/// \see acquireProductInformationForProducts:
///
/// \param productId Identifier of a product that can be purchased in game.
///
/// \return
/// \retval YES The request for product information is sent to the App Store.
/// \retval NO The product information is already being requested.
///
/// \since 4.0.0
///
+ (BOOL)acquireProductInformation:(NSString *)productId;


///
/// \details Requests product information from the App Store.
/// This method *must* be called before the requested products can be purchased.
///
/// \param productIds
/// Set of identifiers of products that can be purchased in the game.
///
/// This method will keep trying to acquire product information if connectivity issues prevent
/// the request from succeeding.
///
/// \return
/// \retval YES The request for product information has been sent to the App Store.
/// \retval NO All productIds are already being requested.
///
/// \since 4.2.0
///
+ (BOOL)acquireProductInformationForProducts:(NSSet *)productIds;


/// \details Checks whether it is possible to purchase the default product at this time.
///
/// \see canStartPurchase:
///
/// \return
/// \retval YES Purchase will succeed if started.
/// \retval NO Purchase already in-flight, restore in-flight, or other restriction
/// preventing purchase.
///
/// \since 4.2.0
///
+ (BOOL)canStartPurchase;


/// \details Checks whether it is possible to purchase a product at this time.
///
/// For most applications, this is the API to call. You should specify a productId
/// that was defined in iTunes Connect by your producer.
///
/// This method presents the UI to users as they begin the purchase process.
///
/// In order to provide a quick result, this method does not try to connect to
/// Big Fish Games' purchase services server. This check will occur in startPurchase before
/// a purchase is sent to Apple.
///
/// \return
/// \retval YES Purchase will succeed if started.
/// \retval NO Purchase already in-flight, restore in-flight, or other restriction
/// preventing purchase.
///
/// \since 4.2.0
///
+ (BOOL)canStartPurchase:(NSString *)productId;


/// \details Convenience method for calling startPurchase:details1:details2:details3:additionalDetails
/// with nil details and nil additional details.
///
/// \see startPurchase: details1:details2:details3:additionalDetails:
///
/// \since 4.2.0
///
+ (void)startPurchase;


/// \details Convenience method for calling startPurchase:details1:details2:details3:additionalDetails
/// with the default productId, nil details, and nil additional details.
///
/// \see startPurchase: details1:details2:details3:additionalDetails:
///
/// \since 4.2.0
///
+ (void)startPurchase:(NSString*)productId;


/// \details Attempt an In-App Purchase with the iTunes store.
///
/// \param productId
/// Identifier of product to be purchased. Originates from the iTunes Store.
/// \param details1
///   An alpha-numeric label to categorize the event. May be *nil*.
/// \param details2
///   An alpha-numeric label to categorize the event. May be *nil*.
/// \param
///   details3 An alpha-numeric label to categorize the event. May be *nil*.
/// \param additionalDetails
///   Additional data to be associated with this productId.
///   This value may be *nil*. If not nil, the value *must* be a flat dictionary containing
///   only NSString * as keys and values.
///
/// This method posts one of the following notifications on the main thread:
///
/// - NOTIFICATION_PURCHASE_SUCCEEDED -- Perform the unlock, persist the purchase, and call finishPurchase:
/// - NOTIFICATION_PURCHASE_FAILED -- Inform the user the purchase failed.
///
/// These notifications contain userInfo, with BFG_PURCHASE_OBJECT_USER_INFO_KEY as a key to the (bfgPurchaseObject *)purchaseObject
///
/// \see bfgPurchaseObject
///
/// \since 4.2.0
///
+ (void)startPurchase:(NSString *)productId details1:(NSString *)details1 details2:(NSString *)details2 details3:(NSString *)details3 additionalDetails:(NSDictionary *)additionalDetails;


/// \details Convenience method for calling finishPurchase: with the default productId.
///
/// \see finishPurchase:
///
/// \since 4.2.0
///
+ (void)finishPurchase;


/// \details A call to this method is *necessary* for all successful purchases. Failed purchases and
/// restores do not need to be finished.
///
/// After performing the unlock and PERSISTING IT, this method must be called to
/// take the purchase off the queue. If this method is not called, the app will be notified
/// of the purchase every time bfgPurchase is restarted. The product cannot be purchased
/// again until the previous purchase has been finished.
///
/// Calling this method on a productId that is not currently being purchased does nothing.
///
/// \param productId
/// The ID of the product to take off the queue.
///
/// \since 4.2.0
///
+ (void)finishPurchase:(NSString *)productId;


/// \return
/// \retval YES The default product is currently being processed for purchase.
/// \retval NO Purchase already in-flight, restore in-flight, or other restriction
/// preventing purchase.
///
/// \since 4.0.0
///
+ (BOOL)isPurchaseActive;


/// \param productId
/// ID of product to query if purchase is active.
///
/// \return
/// \retval YES Purchase is being processed.
/// \retval NO Purchase already in-flight, restore in-flight, or other restriction
/// preventing purchase.
///
/// \since 4.0.0
///
+ (BOOL)isPurchaseActive:(NSString*)productId;


/// \return
/// \retval YES Restore is being processed or verified.
/// \retval NO Restore in-flight or other restriction preventing restore.
///
/// \since 4.2.0
///
+ (BOOL)isRestoreActive;

/// \return Product information related to the default product.
///
/// \see productInformation:
///
/// \since 4.0.0
///
+ (NSDictionary *)productInformation;


/// \details Product information is obtained from Apple during
/// acquireProductInformationForProducts: and held in a transient
/// dictionary.
///
/// **Games should not persist this data but should acquire it on each
/// launch from the iTunes Store.**
///
/// \return A dictionary of information regarding the product with the
/// following keys (pre-processor defines):
/// \retval PRODUCT_INFO_PRODUCT_ID Apple ID of the product.
/// \retval PRODUCT_INFO_PRICE Price as a number.
/// \retval PRODUCT_INFO_PRICE_STR Price as localized string.
/// \retval PRODUCT_INFO_TITLE Title of product.
/// \retval PRODUCT_INFO_DESCRIPTION Description of product.
/// \retval PRODUCT_INFO_CURRENCY Currency price is denoted in.
///
/// \since 4.0.0
///
+ (NSDictionary *)productInformation:(NSString *)productId;


/// \details Restores previous purchases. Receives the same NOTIFICATION_PURCHASE_SUCCEEDED
/// notification as the original purchase.
///
/// \since 4.2.0
///
+ (void)restorePurchases;


/// \details Calling startService on bfgPurchases enables its functionality and causes it to
/// resume processing any operations that were already in progress. All notification
/// listeners should be set up prior to calling start.
///
/// \return
/// \retval YES Purchase has started.
/// \retval NO Purchase has previously started.
///
/// \since 4.2.0
///
+ (BOOL)startService;


/// \details Calling startService on bfgPurchases enables its functionality and causes it to
/// resume processing any operations that were already in progress. All notification
/// listeners should be set up prior to calling start.
///
/// \param error
/// If service fails to start, error will contain the reason. Otherwise error is nil.
///
/// \return
/// \retval YES Purchase has started.
/// \retval NO Purchase has previously started.
///
/// \since 4.7.0
///
+ (BOOL)startService:(NSError * __autoreleasing *)error;


/// \brief Completed purchases that have not been granted to the user
///
/// \details When a purchase completes, you will receive a notification of its success
/// or failure. At this time, award the purchase to the user. This method gives you a
/// secondary way of accessing purchases that should be awarded after the notification
/// has fired.
///
/// \note You must start the bfgPurchase service before calling this method.
///
/// \return Purchase objects that were successfully purchased but have not had
/// "finishPurchase" called on them.
///
/// \since 5.9
///
+ (NSArray<bfgPurchaseObject *> *)deliverablePurchases;

/// \details Checks if a purchase / restore is in progress.
/// Should not be displayed.
///
/// \return
/// \retval YES Purchase or restore in progress.
/// \retval NO Purchase already in-flight, restore in-flight, or other restriction
/// preventing purchase.
///
/// \since 4.2.0
///
+ (BOOL)purchaseActivityInProgress;

@end
