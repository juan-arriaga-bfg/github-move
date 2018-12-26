//
//  BfgSimplePurchaseWrapper.mm
//

#import <bfg_iOS_sdk/bfgSimplePurchase.h>

#define NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEDIDSUCCEED         @"NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEDIDSUCCEED"
#define NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEDIDFAIL            @"NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEDIDFAIL"
#define NOTIFICATION_SIMPLEPURCHASE_SIMPLERESTOREDIDSUCCEED          @"NOTIFICATION_SIMPLEPURCHASE_SIMPLERESTOREDIDSUCCEED"
#define NOTIFICATION_SIMPLEPURCHASE_SIMPLERESTOREDIDFAIL             @"NOTIFICATION_SIMPLEPURCHASE_SIMPLERESTOREDIDFAIL"
#define NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEORRESTOREDIDCANCEL @"NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEORRESTOREDIDCANCEL"
#define NOTIFICATION_SIMPLEPURCHASE_STARTUP_COMPLETE                 @"NOTIFICATION_SIMPLEPURCHASE_STARTUP_COMPLETE"

@interface BfgSimplePurchaseDelegateWrapper : NSObject <bfgSimplePurchaseDelegate>
    +(instancetype) sharedInstance;
@end

@implementation BfgSimplePurchaseDelegateWrapper

+(instancetype)sharedInstance
{
    static BfgSimplePurchaseDelegateWrapper *_sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{ _sharedInstance = [[BfgSimplePurchaseDelegateWrapper alloc] init]; } );
    return _sharedInstance;
}

- (void)dismissPaywallAndUnlock:(BOOL)unlock
{
    if (unlock)
    {
        [self simplePurchaseDidSucceed];
    }
    else{
        [self simplePurchaseDidFail];
    }
}

- (void)simplePurchaseDidSucceed
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEDIDSUCCEED object:self];
}

- (void)simplePurchaseDidFail
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEDIDFAIL object:self];
}

- (void)simpleRestoreDidSucceed
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_SIMPLEPURCHASE_SIMPLERESTOREDIDSUCCEED object:self];
}

- (void)simpleRestoreDidFail
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_SIMPLEPURCHASE_SIMPLERESTOREDIDFAIL object:self];
}

- (void)simplePurchaseOrRestoreDidCancel
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_SIMPLEPURCHASE_SIMPLEPURCHASEORRESTOREDIDCANCEL object:self];
}

- (void)simplePurchaseStartUpComplete
{
    [[NSNotificationCenter defaultCenter] postNotificationName:NOTIFICATION_SIMPLEPURCHASE_STARTUP_COMPLETE object:self];
}

@end


extern "C"
{
    BOOL __bfgSimplePurchase__isPurchased()
    {
        return [bfgSimplePurchase isPurchased];
    }

    void __bfgSimplePurchase__startServiceWithDelegate()
    {
        [[NSNotificationCenter defaultCenter] addObserver:[BfgSimplePurchaseDelegateWrapper sharedInstance] selector:@selector(simplePurchaseStartUpComplete:) name:kNotificationSimplePurchaseStartupComplete object:nil];
        [bfgSimplePurchase startServiceWithDelegate:[BfgSimplePurchaseDelegateWrapper sharedInstance]];
    }

    void __bfgSimplePurchase__showPaywallUIFrom(int origin)
    {
        switch (origin) {
            case 0:
                [bfgSimplePurchase showPaywallUIFrom:BFGPaywallOriginMainMenu];
                break;
            case 1:
                [bfgSimplePurchase showPaywallUIFrom:BFGPaywallOriginEndOfGameplay];
                break;
            default:
                [bfgSimplePurchase showPaywallUIFrom:BFGPaywallOriginMainMenu];
                break;
        }
    }
    
    void __bfgSimplePurchase__showsubscriptionProfile()
    {
        [bfgSimplePurchase showSubscriptionProfile];
    }
    
    void __bfgSimplePurchase__subscriptionProfileButton()
    {
        [bfgSimplePurchase subscriptionProfileButton];
    }
    
    void __bfgSimplePurchase__subscriptionProfileButtonImageNormal ()
    {
        [bfgSimplePurchase subscriptionProfileButtonImageNormal];
    }
    
    void __bfgSimplePurchase__subscriptionProfileButtonImageHighlighted ()
    {
        [bfgSimplePurchase subscriptionProfileButtonImageHighlighted];
    }
    
}

