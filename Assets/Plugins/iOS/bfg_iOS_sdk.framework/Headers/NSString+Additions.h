//
//  NSString+Additions.h
//  RaveUI
//
//  Copyright (c) 2013 Gorilla Graph, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface NSString (Additions)

+ (NSString*)rsui_nameFromEmail:(NSString*)email;

- (BOOL)rsui_isValidEmail;

@end

@interface NSString (RSModelLibraryImplemented)
- (NSString*)rs_localizedString;
@end

@interface NSString (NSString_SBJsonParsing)
- (id)JSONValue;
@end

