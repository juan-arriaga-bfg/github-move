//
//  RaveConnectController.h
//  RaveSocial
//
//  Copyright (c) 2015 Rave, Inc. All rights reserved.
//

#import <bfg_iOS_sdk/RaveUtilities.h>
/**
 *  The connect controller is meant to encapsulate common logic for typical connect scenarios
 *
 *  To best leverage a controller integrate into your UI and register a connect observer and change your UI based on state changes
 */

typedef NS_ENUM(NSInteger, RaveConnectControllerState)
{
    /**
     *  Controller is disabled, plugin not ready
     */
    RaveConnectControllerStateDisabled,
    /**
     *  Controller connected, plugin ready
     */
    RaveConnectControllerStateConnected,
    /**
     *  Controller connecting asynchronously, plugin not ready
     */
    RaveConnectControllerStateConnecting,
    /**
     *  Controller disconnected, plugin not ready
     */
    RaveConnectControllerStateDisconnected,
    /**
     *  Controller disconnecting asynchronously, consider plugin not ready
     */
    RaveConnectControllerStateDisconnecting,
};

/**
 *  Implement and register an object with this protocol to observe state changes
 */
@protocol RaveConnectStateObserver <NSObject>
@required
/**
 *  Notification that controller state has changed, monitor to keep UI up-to-date
 *
 *  @param value The new controller state
 */
-(void)onConnectStateChanged:(RaveConnectControllerState)value;
@end

/**
 *  Controller to abstract basic connection and readiness for a given plugin
 *
 *  Used to ease integration burden for custom connect scenarios in a UI
 */
@interface RaveConnectController : NSObject
/**
 *  Create an instance of a connect controller for a given plugin
 *
 *  Automatically updates controller state when setting the observer
 *
 *  @param pluginKeyName Key for plugin to use
 *
 *  @return Instance of connect controller
 */
+(instancetype) controllerWithPlugin:(NSString *)pluginKeyName;

/**
 *  Create an instance of a connect controller for a given plugin
 *
 *  @param pluginKeyName Key for plugin to use
 *  @param autoUpdate    Determines whether to automatically update the controller state when an observer is set
 *
 *  @return Instance of connect controller
 */
+(instancetype) controllerWithPlugin:(NSString *)pluginKeyName autoUpdate:(BOOL)autoUpdate;

/**
 *  Attempt to connect with plugin asynchronously
 *
 *  State changes will be notified through the connect state observer
 *
 *  Errors will be reported to completion callback
*/
-(void) attemptConnect;

/**
 *  Attempt to disconnect with plugin asynchronously
 *
 *  State changes will be notified through the connect state observer
 *
 *  Errors will be reported to completion callback
 */
-(void) attemptDisconnect;

/**
 *  Update connect state of plugin asynchronously
 *
 *  State changes will be notified through the connect state observer
 *
 *  Errors will be reported to completion callback
 */
- (void)updateConnectState;

/**
 *  Plugin key name for plugin to be used with this controller
 *
 *  Typically this won't change
 */
@property (nonatomic, retain) NSString* pluginKeyName;

/**
 *  Register a completion callback if you want to be notified of errors during the asynchronous operations
 */
@property (nonatomic, copy) RaveCompletionCallback callback;

/**
 *  Register an observer for changes in the connect state
 *
 *  Monitor state changes and update UI accordingly
 */
@property (nonatomic, assign) id<RaveConnectStateObserver> connectObserver;

@end