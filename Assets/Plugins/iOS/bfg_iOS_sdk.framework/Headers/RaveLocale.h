//
//  RaveLocale.h
//  RaveSocial
//
//

/**
 * RaveLocale
 *
 * Provides the identifier code used by Rave for mapping assets based on locale as well as convenience
 * methods for access the language and region values
 */

@protocol RaveLocale <NSObject>
/**
 * Access to the current locale identifier
 *
 * @return The string identifier for the current environment.  Usually will be something along
 * the lines of "en_us" where the first part of the string is the language and the second is the
 * the region
 */
- (NSString *)identifier;

/**
 * Convenience access to the language portion of the identifier
 *
 * @return The string for the language portion of the identifier (e.g. "en" for "en_us")
 */
- (NSString *)language;

/**
 * Convenience access to the region portion of the locale identifier
 *
 * @return The string for the region portion of the identifier (e.g. "us" for "en_us")
 */
- (NSString *)region;
@end
