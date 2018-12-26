//
//  BfgDiskUtilsWrapper.mm
//  Unity-iPhone
//
//  Created by Thomas Schessler on 10/19/18.
//  Copyright 2018 Big Fish Games, Inc. All rights reserved.
//
//

#import <bfg_iOS_sdk/bfgDiskUtils.h>

extern "C"
{
    long __bfgDiskUtils__availableDiskSpace()
    {
        return [bfgDiskUtils availableDiskSpace];
    }
}
