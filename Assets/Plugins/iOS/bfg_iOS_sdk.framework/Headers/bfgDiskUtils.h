/// \file bfgDiskUtils.h
/// \brief BFG Disk Utilities
///
// \author Big Fish Games
// \copyright Copyright 2018 Big Fish Games. All rights reserved.
/// \details Helpful utility methods for retrieving information about the device's hard disk.

#import <Foundation/Foundation.h>

/// \brief bfgDiskUtils class
@interface bfgDiskUtils : NSObject

/// \return The device's available disk space in bytes.
/// \since 6.8
+ (NSUInteger)availableDiskSpace;

@end
