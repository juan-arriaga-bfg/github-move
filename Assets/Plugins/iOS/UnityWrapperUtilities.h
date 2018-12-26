//
//  UnityWrapperUtilities.h
//
//  Created by John Starin on 4/1/14.
//
//

#pragma once
@interface UnityWrapper : NSObject
+ (id)sharedInstance;
- (void)handleNotification:(NSNotification *)notification;
@property (strong, nonatomic) NSString *unityMessageHandlerObjectName;
@property (strong, nonatomic) NSMutableArray * notificationQueue;
@end

extern "C"
{
    /// Copy an NSString to a C-style buffer
    ///
    /// @return YES if copy was successful; NO otherwise
    BOOL copyStringToBuffer(NSString *string, char* outputBuffer, int outputBufferSize);
    
    /// Converts JSON object into an NSString
    ///
    /// @return nil if the object cannot be converted
    NSString* convertJSONObjectToString(NSObject *jsonObject);
    
    /// Converts JSON string into an NSDictionary
    ///
    /// @return nil if the JSON string is invalid or not a dictionary at the root level
    NSDictionary* convertJSONtoDictionary(const char* json);
    
    /// Converts JSON string into an NSArray
    ///
    /// @return nil if the JSON string is invalid or not an array at the root level
    NSArray* convertJSONtoArray(const char* json);
    
    // purchaseObject is of type bfgPurchaseObject
    NSDictionary* dictionaryFromPurchaseObject(NSObject *purchaseObject);
}
