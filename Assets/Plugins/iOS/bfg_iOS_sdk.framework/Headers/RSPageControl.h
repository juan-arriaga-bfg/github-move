//
//  RSPageControl.h
//  RaveUI
//
//  Created by Damien DeVille on 1/14/11.
//  Copyright (c) 2013 Gorilla Graph. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIControl.h>
#import <UIKit/UIKitDefines.h>

typedef enum
{
	RSPageControlTypeOnFullOffFull		= 0,
	RSPageControlTypeOnFullOffEmpty		= 1,
	RSPageControlTypeOnEmptyOffFull		= 2,
	RSPageControlTypeOnEmptyOffEmpty	= 3,
}
RSPageControlType ;


@interface RSPageControl : UIControl 
{
	NSInteger numberOfPages ;
	NSInteger currentPage ;
}

// Replicate UIPageControl features
@property(nonatomic) NSInteger numberOfPages ;
@property(nonatomic) NSInteger currentPage ;

@property(nonatomic) BOOL hidesForSinglePage ;

@property(nonatomic) BOOL defersCurrentPageDisplay ;
- (void)updateCurrentPageDisplay ;

- (CGSize)sizeForNumberOfPages:(NSInteger)pageCount ;

/*
	RSPageControl add-ons - all these parameters are optional
	Not using any of these parameters produce a page control identical to Apple's UIPage control
 */
- (id)initWithType:(RSPageControlType)theType ;

@property (nonatomic) RSPageControlType type ;

@property (nonatomic,retain) UIColor *onColor ;
@property (nonatomic,retain) UIColor *offColor ;

@property (nonatomic,retain) UIImage *onImage;
@property (nonatomic,retain) UIImage *offImage;

@property (nonatomic) CGFloat indicatorDiameter ;
@property (nonatomic) CGFloat indicatorSpace ;

@end

