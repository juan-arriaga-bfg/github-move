//
//  Bfgutils.mm
//  Unity-iPhone
//
//  Created by Michael Molien on 01/29/15.
//  Copyright 2015 Big Fish Games, Inc. All rights reserved.
//
//

#import <bfg_iOS_sdk/bfgutils.h>
#import "UnityWrapperUtilities.h"

extern "C"
{

    void __bfgutils__bfgUDID(char* returnBFGUDID, int size)
    {
        NSString *_bfgUDID = [bfgutils bfgUDID];
        copyStringToBuffer( _bfgUDID, returnBFGUDID, size );
    }

	// Big Fish iOS SDK 5.8

    int __bfgutils__bfgIDFV(char* returnIDFV, int size)
    {
        NSString *_bfgIDFV = [bfgutils bfgIDFV];
        NSUInteger length = [_bfgIDFV lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
        if (size > length)
        {
            copyStringToBuffer( _bfgIDFV, returnIDFV, size );
        }
        return (int) length;
    }
}
