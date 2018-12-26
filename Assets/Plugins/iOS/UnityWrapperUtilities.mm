//
//  UnityWrapperUtilities.mm
//
//  Created by John Starin on 4/1/14.
//
//

#import <Foundation/Foundation.h>
#import "UnityWrapperUtilities.h"

// Notification definitions
#import <bfg_iOS_sdk/bfgPurchase.h>
#import <bfg_iOS_sdk/bfgPurchaseObject.h>
#import <bfg_iOS_sdk/bfgManager.h>
#import <bfg_iOS_sdk/bfgBrandingViewController.h>
#import <bfg_iOS_sdk/bfgGameReporting.h>
#import <bfg_iOS_sdk/bfgReporting.h>

typedef NS_ENUM(NSUInteger, Base64EncodingType) {
    Base64EncodingTypeStandard,
    Base64EncodingTypeWebSafe
};


@interface NSData (BFGUtils)

// NOTE: Keep the 'bfg' prefix on the method names!
// We must avoid collisions with other implementations of "dataFromBase64String"

+ (NSData *)bfgDataFromBase64String:(NSString *)aString encodingType:(Base64EncodingType)encodingType;
- (NSString *)bfgBase64EncodedStringWithEncodingType:(Base64EncodingType)encodingType;

+ (NSData *)bfgDataFromBase64String:(NSString *)aString;
- (NSString *)bfgBase64EncodedString;

- (NSData *)gzipDeflate;
- (NSData *)gzipInflate;

@end

//static const NSString *kNotificationNameKey = @"BfgNotificationName";


// TODO: make thread-safe
@implementation UnityWrapper

+ (id)sharedInstance
{
    static UnityWrapper *sharedUnityWrapper;
    static dispatch_once_t onceToken;

    dispatch_once(&onceToken, ^{
        sharedUnityWrapper = [[self alloc] init];
    });

    return sharedUnityWrapper;
}

- (id)init
{
    self = [super init];
	if (self) {
		self.unityMessageHandlerObjectName = @"";
        self.notificationQueue = [[NSMutableArray alloc] init];
	}

    return self;
}

+ (void)setUnityMessageHandlerObjectName:(NSString *)name
{
	[self.sharedInstance setUnityMessageHandlerObjectName:name];
}

+ (void) flushNotificationQueue
{
    [self.sharedInstance flushNotificationQueue];
}

+ (void)addNotification:(NSString *)notificationName
{
    [self.sharedInstance addNotification:notificationName];
}

- (void)addNotification:(NSString *)notificationName
{
//    NSLog(@"addNotification: %@, %@, %@", notificationName, objectName, methodName);

    if (!notificationName)
    {
        return;
    }

    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleNotification:) name:notificationName object:nil];
}

+ (void)removeNotification:(NSString *)notificationName
{
    [self.sharedInstance removeNotification:notificationName];
}

- (void)removeNotification:(NSString *)notificationName
{
    if (!notificationName)
    {
        return;
    }

    [[NSNotificationCenter defaultCenter] removeObserver:self name:notificationName object:nil];
}

- (void)handleNotification:(NSNotification *)notification
{
    if (!notification || !notification.name)
    {
        return;
    }

    NSMutableDictionary *userInfo = [[NSMutableDictionary alloc] init];

    // Add notification name to userInfo
    //userInfo[kNotificationNameKey] = notification.name;

    // Add notification.userInfo data to userInfo
    if (notification.userInfo)
    {
        for (NSString *keyName in notification.userInfo)
        {
            if (![keyName isKindOfClass:[NSString class]])
                continue;

            id object = notification.userInfo[keyName];

            if ([object isKindOfClass:[bfgPurchaseObject class]])
            {
                NSDictionary *purchaseObjectDictionary = dictionaryFromPurchaseObject(object);
                userInfo[keyName] = purchaseObjectDictionary;
            }
            else if ([object isKindOfClass:[NSString class]] || [object isKindOfClass:[NSNumber class]] || [object isKindOfClass:[NSNull class]])
            {
                userInfo[keyName] = object;
            }
            else if ([NSJSONSerialization isValidJSONObject:object])
            {
                userInfo[keyName] = object;
            }
            else
            {
//                NSLog(@"Unable to return userInfo[%@] for notification: %@", keyName, notification.name);
            }
        }
    }

    // Convert userInfo dictionary to JSON
    NSString *notificationName = notification.name;
    NSString *message = convertJSONObjectToString(userInfo);

    NSString *notificationJson = [NSString stringWithFormat:@"{\"name\":\"%@\", \"arg\":%@}", notificationName, message];

	// Add all messages to the notificationQueue. When the UnityMessageHandler has been loaded, the notificationQueue will begin to flush every frame update.
    // If the notificationQueue isn't flushing, please make sure you've attached UnityMessageHandler.cs as a component to a game object that begins (and doesn't get destroyed) at the very start of the game.
    // If the notificationQueue is causing problems, please contact the DIS team immediately.
    [self addToNotificationQueue:notificationJson];
}

// test data (remove)
+ (NSDictionary *)fakeUserDictWithPurchaseObject
{
    bfgPurchaseObject *purchaseObject = [[bfgPurchaseObject alloc] init];
    purchaseObject.productInfo = @{ PRODUCT_INFO_PRODUCT_ID : @"com.bigfishgames.AwesomeSauce",
                                    PRODUCT_INFO_CURRENCY : @"usd",
                                    PRODUCT_INFO_PRICE : @(0.99),
                                    PRODUCT_INFO_PRICE_STR : @"$0.99",
                                    PRODUCT_INFO_DESCRIPTION : @"The best sauce!",
                                    PRODUCT_INFO_TITLE : @"Awesome Sauce™" };
    // Deprecated in SDK 6.0
    // purchaseObject.purchaseStart = [NSDate date];
    purchaseObject.canceled = NO;
    purchaseObject.restore = NO;
    purchaseObject.sandbox = NO;
    purchaseObject.success = YES;
    purchaseObject.quantity = 1;
    purchaseObject.paymentTransaction = [SKPaymentTransaction new];

    return (@{ BFG_PURCHASE_OBJECT_USER_INFO_KEY : purchaseObject });
}

// remove the first notification in the notification queue stack once per flushNotificationQueue
- (void) flushNotificationQueue
{
    if (self.notificationQueue.count > 0)
    {
        NSString* msg = [self.notificationQueue objectAtIndex:0];
        UnitySendMessage(self.unityMessageHandlerObjectName.UTF8String, "HandleNativeMessage", msg.UTF8String);
        [self.notificationQueue removeObjectAtIndex:0];
    }
}

- (void) addToNotificationQueue:(NSString *) notificationJson
{
    [self.notificationQueue addObject: notificationJson];
    if (self.notificationQueue.count == 10)
    {
        NSLog(@"WARNING: The notificationQueue size is now 10.\nThe notification queue will begin flushing once the game object with the UnityMessageHandler attached is loaded.\n It will flush one notification in the notificationQueue at a time and will flush every N frames as defined in Notification_Flush_Period_In_Frames in UnityMessageHandler.cs (default is 1)");
    }
    if ([self.notificationQueue count] > 25)
    {
        NSLog(@"WARNING: The notificationQueue has grown to 26 objects, removing the first object in the queue.");
        [self.notificationQueue removeObjectAtIndex:0];
    }
}
@end


extern "C"
{

#pragma mark - Public Unity Wrapper Utilities

    BOOL copyStringToBuffer(NSString *string, char* outputBuffer, int outputBufferSize)
    {
        if (string && outputBuffer && (outputBufferSize >= ([string length] + 1)))
        {
            strcpy(outputBuffer, [string UTF8String]);
            return YES;
        }

        return NO;
    }

    NSString* convertJSONObjectToString(NSObject *jsonObject)
    {
        NSString *jsonString = @"{}";
        
        if (jsonObject == nil)
        {
            return jsonString;
        }
        
        if ([NSJSONSerialization isValidJSONObject:jsonObject])
        {
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:jsonObject options:0 error:nil];
            jsonString = [[NSString alloc] initWithBytes:[jsonData bytes] length:[jsonData length] encoding:NSUTF8StringEncoding];
        } else if ([jsonObject isKindOfClass:[NSDictionary class]])
        {
            // jsonObject is an NSDictionary, it must become an NSMutableDictionary to change any internal keys/values
            NSMutableDictionary * jsonObjectMutable = [jsonObject mutableCopy];
            id productInfoLocale = [jsonObjectMutable valueForKeyPath:@"BFG_PURCHASE_OBJECT_USER_INFO_KEY.productInfo.priceLocale"];
            if (productInfoLocale !=nil && [productInfoLocale isKindOfClass:[NSLocale class]])
            {
                NSString * localeIdentifier = [productInfoLocale localeIdentifier];
                // productInfo is an NSDictionary, it must become an NSMutableDictionary to change it's priceLocale key
                NSMutableDictionary *productInfoDictionary =  [[[jsonObjectMutable valueForKey:@"BFG_PURCHASE_OBJECT_USER_INFO_KEY"] valueForKey:@"productInfo"] mutableCopy];
                [productInfoDictionary setValue:localeIdentifier forKey:@"priceLocale"];
                [[jsonObjectMutable valueForKey:@"BFG_PURCHASE_OBJECT_USER_INFO_KEY"] setValue:productInfoDictionary  forKey:@"productInfo"];
                
                if ([NSJSONSerialization isValidJSONObject:jsonObjectMutable])
                {
                    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:jsonObjectMutable options:0 error:nil];
                    jsonString = [[NSString alloc] initWithBytes:[jsonData bytes] length:[jsonData length] encoding:NSUTF8StringEncoding];
                }
            }
        }
        
        return jsonString;
    }

    // Internal
    id convertJSONtoObject(const char* json)
    {
        if (!json)
        {
            return nil;
        }

        NSString *jsonString = [NSString stringWithUTF8String:json];
        NSData *jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error;
        NSObject *object = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];

        // Note: 'error' is being populated even when the conversion is successful; ignoring for now

        return object;
    }

    NSDictionary* convertJSONtoDictionary(const char* json)
    {
        NSDictionary *dictionary = convertJSONtoObject(json);

        // check class
        if (![dictionary isKindOfClass:[NSDictionary class]])
        {
            return nil;
        }

        return dictionary;
    }

    NSArray* convertJSONtoArray(const char* json)
    {
        NSArray* array = convertJSONtoObject(json);

        // check class
        if (![array isKindOfClass:[NSArray class]])
        {
            return nil;
        }

        return array;
    }

    NSDictionary* dictionaryFromPurchaseObject(NSObject *purchaseObj)
    {
        // Remove purchaseStart (NSDate) and paymentTransaction (SKPaymentTransaction) since they aren't JSON objects

        bfgPurchaseObject *purchaseObject = (bfgPurchaseObject*) purchaseObj;
        NSMutableDictionary *d = [[NSMutableDictionary alloc] init];
        d[@"productInfo"] = purchaseObject.productInfo;
        //    d[@"purchaseStart"] = purchaseObject.purchaseStart;
        d[@"canceled"] = [NSNumber numberWithBool:purchaseObject.canceled];
        d[@"restore"] = [NSNumber numberWithBool:purchaseObject.restore];
        d[@"sandbox"] = [NSNumber numberWithBool:purchaseObject.sandbox];
        d[@"success"] = [NSNumber numberWithBool:purchaseObject.success];
        d[@"quantity"] = [NSNumber numberWithInteger:purchaseObject.quantity];
        //    d[@"paymentTransaction"] = purchaseObject.paymentTransaction;

        // Receipt handling from Bagelcode:
        // By Bagelcode
        // For extracting receipt
        // Written by 4/21/15
        // jykim
        //
        if (purchaseObject.paymentTransaction != nil)
        {
            d[@"transactionId"] = purchaseObject.paymentTransaction.transactionIdentifier;
        }
        else
        {
            d[@"transactionId"] = @"";
        }

        if (purchaseObject.iOS7Receipt)
        {
            d[@"receipt"] = [purchaseObject.iOS7Receipt bfgBase64EncodedString];
            d[@"receiptType"] = @"iOS7Receipt";
        }
        /*
        //Deprecated in SDK 6.0
        else if (purchaseObject.iOS6Receipt)
        {
            d[@"receipt"] = [purchaseObject.iOS6Receipt bfgBase64EncodedString];
            d[@"receiptType"] = @"iOS6Receipt";
        }
        */
        else
        {
            d[@"receipt"] = @"";
            d[@"receiptType"] = @"none";
        }

        return d;
    }


#pragma mark - External notification functions

    void __BfgUtilities__setUnityMessageHandlerObjectName(const char *name)
    {
        NSString *_name = [NSString stringWithUTF8String:name];

        [UnityWrapper setUnityMessageHandlerObjectName:_name];
    }
    
    void __BfgUtilities__flushNotificationQueue()
    {
        [UnityWrapper flushNotificationQueue];
    }

    void __BfgUtilities__addNotificationObserver(const char *notificationName)
    {
        NSString *_notificationName = [NSString stringWithUTF8String:notificationName];

        [UnityWrapper addNotification:_notificationName];
    }

    void __BfgUtilities__removeNotificationObserver(const char *notificationName)
    {
        NSString *_notificationName = [NSString stringWithUTF8String:notificationName];

        [UnityWrapper removeNotification:_notificationName];
    }
}
