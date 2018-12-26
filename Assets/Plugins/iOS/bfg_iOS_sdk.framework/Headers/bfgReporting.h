//
/// \file bfgReporting.h
/// \brief Low-level reporting mechanism
/// \see bfgGameReporting.h
//  bfg_iOS_sdk
//
// \author Created by Sean Hummel on 1/21/11.
// \copyright Copyright (c) 2013 Big Fish Games. All rights reserved.
//

#import <UIKit/UIKit.h>

///
/// \brief To report events or errors to remote reporting service.
///
/// \details bfgReporting has the ability to log data on a per session basis.
/// - Application must call beginSession and EndSession.
/// - On EndSession, session data dictionary is put in the queue.
/// - On BeginSession and on EndSession, the queue of session data is sent.
/// - If send is successful, session data is removed from the queue and the next item in the queue is sent.
///
@interface bfgReporting : NSObject


/// \details Log event to be counted on the server.
///
/// \param eventName Event to be counted on the server.
///
/// \note As of 5.10 this method does nothing, but will likely be restored in 6.0.
///
+ (void)logEvent:(NSString * _Nonnull)eventName;

/// \details Log event to be counted on the server with parameters.
///
/// \param eventName Event to be counted on the server.
/// \param parameters Parameters to be tracked with the event.
///
+ (void)logEvent:(NSString * _Nonnull)eventName withParameters:(NSDictionary * _Nullable)parameters;

/// \details Single fire event.
/// Single fire events are tracked on the client.
/// Once an event is logged, it will not be logged again.
///
/// \param eventName Event to track once.
///
+ (void)logSingleFireEvent:(NSString * _Nonnull)eventName;

@end
