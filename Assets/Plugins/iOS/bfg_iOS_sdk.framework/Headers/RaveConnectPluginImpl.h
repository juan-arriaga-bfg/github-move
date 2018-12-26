//
//  RaveConnectPlugin.h
//  RaveSocial
//

#import <UIKit/UIKit.h>
#import <bfg_iOS_sdk/RaveConnectPlugin.h>

@interface RaveConnectPluginImpl : NSObject<RaveConnectPlugin>
- (id)valueForPrivateKey:(NSString *)subKey;
- (void)setValue:(id)value forPrivateKey:(NSString *)subKey;

- (void)setCurrentToken:(NSString *)token;
- (void)setCurrentToken:(NSString *)token byType:(RaveConnectPluginTokenType)tokenType;

@property (nonatomic, copy) NSString * preExistingToken;
@property (nonatomic, assign) BOOL lastKnownReadiness;
@property (nonatomic, assign) BOOL nonAuthenticatedMode;
@property (nonatomic, copy) NSDate * lastFriendsSyncTime;
@end
