//
//  bfgCrashlytics.h
//  BFGUIKitExample
//
//  Created by Rajesh Sabale on 8/1/17.
//  Copyright © 2017 Big Fish Games, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

///
/// \brief Crashlytics allows you to associate arbitrary key/value pairs with your crash reports, which are viewable right from the Crashlytics dashboard.
/// This class facilitates the setting of these key/value pairs for informative crash report on the dashboard.
///
/// \note Re-setting the same key will update the value.
///
/// \details Crashlytics supports a maximum of 64 key/value pairs. Once you reach this threshold,
/// additional values are not saved. Each key/value pair can be up to 1 KB in size; anything past
/// that will be truncated.
///
/// \since 6.3
///
@interface bfgCrashlytics : NSObject

///
/// \details Set an object value in Crashlytics to be reported with any crashes from the game.
/// When setting an object value, the object is converted to a string. This is typically done by calling -[NSObject description].
///
/// \param value Object value of the key/value pair.
/// \param key String key of the key/value pair.
/// \since 6.3
///
+ (void)setObjectValue:(id)value forKey:(NSString *)key;

///
/// \details Set int value in Crashlytics to be reported with any crashes from the game.
///
/// \param value int value of the key/value pair.
/// \param key String key of the key/value pair.
/// \since 6.3
///
+ (void)setIntValue:(int)value forKey:(NSString *)key;

///
/// \details Set bool value in Crashlytics to be reported with any crashes from the game.
///
/// \param value bool value of the key/value pair.
/// \param key String key of the key/value pair.
/// \since 6.3
///
+ (void)setBoolValue:(BOOL)value forKey:(NSString *)key;

///
/// \details Set float value in Crashlytics to be reported with any crashes from the game.
///
/// \param value float value of the key/value pair.
/// \param key String key of the key/value pair.
/// \since 6.3
///
+ (void)setFloatValue:(float)value forKey:(NSString *)key;

@end
