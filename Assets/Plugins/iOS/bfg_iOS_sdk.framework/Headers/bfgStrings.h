///
/// \file bfgStrings.h
/// \brief Big Fish String class
///
/// \details bfgStrings header file. JSON based localized string table resource reader.
/// Use instead of the string tables that come with the game.
///
// \author Created by Sean Hummel on 6/28/10.
// \author Updated by Craig Thompson on 10/1/13.
// \copyright Copyright 2013 Big Fish Games. All rights reserved.
///

#import <bfg_iOS_sdk/bfglibPrefix.h>

///
/// \brief Big Fish String class.
///
/// A localizable string class that enables a single file
/// to store strings for all the languages supported by the game.
///
@interface bfgStrings : NSObject

///
/// \param key A key in the bfgstrings file.
/// \return String for the key in the local language.
///
+ (NSString *)stringFromKey:(NSString *)key;

///
/// \param key A key in the bfgstrings file.
/// \param language Two letter language code.
/// \return String for the key in the specified language.
///
+ (NSString *)stringFromKey:(NSString *)key forLanguage:(NSString *)language;

///
/// \param filename A bfgstrings file.
///
/// \return
/// \retval YES if the file loaded without error.
/// \retval NO if the file did not load.
///
+ (BOOL)loadStringFile:(NSString *)filename;

///
/// \param name A namespace for the string values.
/// \param jsonText A text JSON representation of a strings file.
/// \return YES if jsonText deserializes properly into dictionary.
///
/// \return
/// \retval YES if jsonText deserializes properly into the dictionary.
/// \retval NO if jsonText does not deserialize into the dictionary.
///
+ (BOOL)loadStringFile:(NSString *)name fromString:(NSString *)jsonText;

@end
