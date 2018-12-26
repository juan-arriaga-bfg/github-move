///
/// \file bfgPurchaseObject.h
/// \brief bfgPurchaseObject header file.
///
// \author Created by Benjamin Flynn on 3/14/13.
// \copyright Copyright (c) 2013 Big Fish Games, Inc. All rights reserved.
///

#import <Foundation/Foundation.h>

// Product info dictionary keys
#define PRODUCT_INFO_PRODUCT_ID                             @"productID"
/// Gets the price as a NSDecimalNumber.
#define PRODUCT_INFO_PRICE                                  @"price"
/// Gets the price as a NSString.
#define PRODUCT_INFO_PRICE_STR                              @"priceString"
#define PRODUCT_INFO_TITLE                                  @"title"
#define PRODUCT_INFO_DESCRIPTION                            @"description"
#define PRODUCT_INFO_CURRENCY                               @"currency"
#define MORE_INFO_EVENTDETAILS1                             @"eventDetails1"
#define MORE_INFO_EVENTDETAILS2                             @"eventDetails2"
#define MORE_INFO_EVENTDETAILS3                             @"eventDetails3"
#define MORE_INFO_GAMEDATA                                  @"gameData"


@class SKPaymentTransaction;

///
/// \brief Data for a particular In-App Purchase.
///
/// Contains definitions for the following constants, which are keys
/// to productInfo dictionaries:
/// - PRODUCT_INFO_PRODUCT_ID
/// - PRODUCT_INFO_PRICE
/// - PRODUCT_INFO_PRICE_STR
/// - PRODUCT_INFO_TITLE
/// - PRODUCT_INFO_DESCRIPTION
/// - PRODUCT_INFO_CURRENCY
/// - MORE_INFO_EVENTDETAILS1
/// - MORE_INFO_EVENTDETAILS2
/// - MORE_INFO_EVENTDETAILS3
/// - MORE_INFO_GAMEDATA
///
@interface bfgPurchaseObject : NSObject <NSSecureCoding, NSCopying>

/// \details Contains meta-data on the purchase. Much of this data
/// is populated when the product information is acquired (using
/// <tt>[bfgPurchase acquireProductInfoForProducts:(NSSet *)productIds]</tt>).
@property (nonatomic, strong) NSDictionary          *productInfo;

/// \return YES if purchase was actively canceled by the user.
@property (nonatomic, assign) BOOL                  canceled;

/// \return The SKPaymentTransaction object associated with the current
/// purchase / restore.
///
///  \since 4.7
@property (nonatomic, strong) SKPaymentTransaction  *paymentTransaction;

/// \details Always set to 1 for the current implementation of
/// the SDK.
@property (nonatomic, assign) NSInteger             quantity;

/// \details YES when a purchase is being re-downloaded (either due to
/// a restore action or the purchase of a non-consumable that has previously
/// been purchased).
@property (nonatomic, assign) BOOL                  restore;

/// \details YES when a purchase / restore has been made with an iTunes
/// test user.
@property (nonatomic, assign) BOOL                  sandbox;

/// \details YES when a purchase / restore has been verified successfully.
@property (nonatomic, assign) BOOL                  success;

/// \details Specific error associated with a failed purchase.
@property (nonatomic, strong) NSError               *error;

/// \details Holds the Apple iOS7+ receipt for this transaction.
/// \note We hold on to this receipt in case we are dealing with the purchase of a
/// consumable because the value could be purged from the receipt during a subsequent
/// purchase.
/// \since 5.2
@property (nonatomic, copy) NSData                  *iOS7Receipt;

/// \return YES if all fields are of equal value.
- (BOOL)isEqualToPurchaseObject:(bfgPurchaseObject *)other;

/// \return productId referenced by this purchaseObject. Derived from productInfo.
- (NSString *)productId;


@end
