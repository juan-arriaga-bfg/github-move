//
//  BfgSettingsWrapper.mm
//  Unity-iPhone
//
//  Created by Thomas Schessler on 11/19/18.
//  Copyright 2018 Big Fish Games, Inc. All rights reserved.
//


#import <bfg_iOS_sdk/bfgSettings.h>
#import "UnityWrapperUtilities.h"

#define STRING_LENGTH 300

extern "C"
{
    
    BOOL __bfgSettings__getBoolean(char* key, BOOL withDefault)
    {
        NSString * nsKey = [NSString stringWithUTF8String:key];
        return [[bfgSettings get:nsKey ofType:[NSNumber class] withDefault:@(FALSE)] boolValue];
    }
    
    double __bfgSettings__getDouble(char* key, double withDefault)
    {
        NSString * nsKey = [NSString stringWithUTF8String:key];
        return [[bfgSettings get:@"foo" ofType:[NSNumber class] withDefault:@(0.0)] doubleValue];
    }
    
    int __bfgSettings__getInteger(char* key, int withDefault)
    {
        NSString * nsKey = [NSString stringWithUTF8String:key];
        return [[bfgSettings get:@"foo" ofType:[NSNumber class] withDefault:@(0)] intValue];
    }
    
    long __bfgSettings__getLong(char* key, long withDefault)
    {
        NSString * nsKey = [NSString stringWithUTF8String:key];
        return [[bfgSettings get:@"foo" ofType:[NSNumber class] withDefault:@(0.0)] longValue];
    }
    
    void __bfgSettings__getString(char* key, char* withDefault, char* returnGetString)
    {
        NSString * nsKey = [NSString stringWithUTF8String:key];
        NSString * _getString = [bfgSettings get:nsKey ofType:[NSString class] withDefault:@("")];
        
        if (_getString == nil)
        {
            return;
        }
        
        NSUInteger length = [_getString lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        if (length <= STRING_LENGTH)
        {
            copyStringToBuffer( _getString, returnGetString, (int)length+1 );
        }
    }
}
