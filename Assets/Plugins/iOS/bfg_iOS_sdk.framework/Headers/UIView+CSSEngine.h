//
//  UIView+CSSEngine.h
//  cssengine
//
//

#import <UIKit/UIKit.h>
#import <UIKit/UIAccessibilityIdentification.h>

@class CSSLayout;
@interface UIView (CSSEngine)
- (void) setCssId:(NSString *)cssId;
- (NSString *) cssId;
- (void) setCssClass:(NSString *)cssClass;
- (NSMutableArray *) cssClass;
- (void) setCssElement:(NSString *)cssElement;
- (NSString *)cssElement;
- (CSSLayout *) createLayout;
- (void) setCssLayout:(CSSLayout *)layout;
- (CSSLayout *)cssLayout;

- (void)setCss_backgroundImage:(UIImage*)aBackgroundImage;
- (UIImage*)css_backgroundImage;
@end

@interface UILabel (CSSEngine)
- (CSSLayout *) createLayout;
@end

@interface UIImageView (CSSEngine)
- (CSSLayout *) createLayout;
@end

@interface NSNull (CSSEngine)
@end