//
//  RaveErrors.h
//
//  RaveSocial
//  Copyright (c) 2013 Rave, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum
{
    kRSUknownError                                  = 0,
    kRSInvalidParametersError                       = 1,
    kRSUserIsNotLoggedInPermissionError             = 2,
    kRSNoInternetConnectionError                    = 3,
    
    kRSNeedConfirmDeviceRegistrationError           = 4,
    kRSEmailIsTakenRegistrationError                = 5,
    kRSUserNameIsTakenError                         = 6,
    kRSOperationCancelledError                      = 7,
    kRSCannotFetchFacebookDataError                 = 8,
    kRSAddressbookAccessHadBeenDeniedError          = 9,
    kRSAddressbookAccessRestrictedError             = 10,
    kRSAddressbookAccessJustDeniedError             = 11,
    kRSGooglePlusClientIdIsNotSpecifiedError        = 12,
    kRSUserExistsError                              = 13,
    kRSUserDoesNotExistError                        = 14,
    kRSFacebookAuthenticationDataInvalidError       = 15,
    kRSGPlusAuthenticationDataInvalidError          = 16,
    kRSFacebookDuplicateMessageError                = 17,     // facebook antispam defence when posting duplicate messages on wall.

    kRSFacebookIsNotLinkedError                     = 18,
    kRSGooglePlusIsNotLinkedError                   = 19,
    kRSEmailAndPasswordCombinationError             = 21,
    kRSIncorrectPasswordError                       = 22,
    kRSUserIsNotConnectedToSocialNetworkError       = 23,
    kRSGraphAPIError                                = 24,
    
    kRSScopeMismatchError                           = 28,
    kRSBackendHostNameIsNotSpecifiedError           = 29,
    kRSOtherSocialUIDConnectedError                 = 30,
    
    kRSSystemFacebookPasswordChangedError           = 31,// when this error is encountered, show message
    kRSDeviceNotRegisteredError                     = 32,
    kRSUserDeniedFriendsPermissionError             = 33,
    kRSUserIsAlreadyLoggedInError                   = 34,
    // for user to change password in device settings.
} RSError;

extern NSString* const RaveErrorDomain;
extern NSString* const RaveCriticalErrorDomain;

//  consider deprecated
extern NSString* const kRSErrorDomain;
