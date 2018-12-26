//
//  RaveMergePolicy.h
//  RaveSocial
//
//  Copyright © 2016 Rave, Inc. All rights reserved.
//

/**
 *  A RaveMergePolicy must be implemented and set to enable authenticated merging
 *  
 *  There are several important decision points that can occur during an authenticated merge that need to be handled
 *  carefully for a successful integration.  As a result, authenticated merging is not enabled unless an integrator sets
 *  an instance of a RaveMergePolicy.  A demonstration policy will be provided in the accompanying scene pack and demo
 *
 */

/**
 *  Callback to be fired after user has made a merge decision
 *
 *  @param shouldMerge YES if user indicated a merge should take place, otherwise NO
 */
typedef void (^RaveUserMergeDecisionCallback)(BOOL shouldMerge);

@protocol RaveMergeUser <RaveUser>
/**
 *  Makes a selected app data key for the other user available (may be nil)
 *
 *  If this value is non-nil and RaveAppDataKeys.lastSelectedKey is also non-nil then the user will have to
 *  choose between app data keys after merging.
 */
@property (nonatomic, readonly) NSString * selectedAppDataKey;
@end

@protocol RaveMergePolicy <NSObject>
@required
/**
 *  This method will be called when it is time to prompt the user for a decision regarding a merge
 *
 *  Authenticated merging should only be approved with the user's consent. It is a permanent change to two accounts and
 *  can have implications for multiple titles.  It is strongly encouraged to show UI prompting this decision from the user
 *  before responding with the callback.
 *
 *  @param targetUser The user that would be merged with the current user. Display information from this user so customer can make informed decision
 *  @param callback   The callback to inform Rave of the user's merge decision: YES for proceed with merge, NO to cancel.
 */
- (void)makeUserMergeDecision:(id<RaveMergeUser>)targetUser callback:(RaveUserMergeDecisionCallback)callback;

@end