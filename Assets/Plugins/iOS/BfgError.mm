//
//  BfgError.mm
//  BFG Unity Wrapper
//
//  Created by Alex Bowns on 12/13/17.
//
#import "BfgError.h"

@implementation BfgError
+ (void) logErrorToCrashlytics:(NSError *) error
{
    NSLog([error localizedDescription]);
}

@end

