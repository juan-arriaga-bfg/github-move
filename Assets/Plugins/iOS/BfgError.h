//
//  BfgError.h
//  BFG Unity Wrapper
//
//  Created by Alex Bowns on 12/13/17.
//
#import <Foundation/Foundation.h>
#ifndef BfgError_h
#define BfgError_h


#endif /* BfgError_h */

@interface BfgError : NSObject
+ (void) logErrorToCrashlytics:(NSError *) error;
@end

