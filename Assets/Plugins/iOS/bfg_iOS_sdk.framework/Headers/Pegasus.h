//
//  Pegasus.h
//  Pegasus
//
//  Copyright 2012 Jonathan Ellis
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

// Vendor
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

// Third Party
#import "TouchXML.h"

// Categories
#import "NSObject+Invocation.h"
#import "UIColor+HexString.h"

// Data Structures
#import "Tuple.h"

// Framework
#import "RavePGAssetsProvider.h"
#import "RavePGLocalizer.h"
#import "RavePGAdapter.h"
#import "RavePGTranslators.h"

// Views
#import "RavePGView.h"
#import "RavePGLabel.h"
#import "RavePGImageView.h"
#import "RavePGTextField.h"
#import "RavePGButton.h"
#import "RavePGScrollView.h"
#import "RavePGProgressView.h"
#import "RavePGSwitch.h"
#import "RavePGTableView.h"
#import "RavePGTableViewCell.h"
#import "RavePGToolbar.h"
#import "RavePGBarButtonItem.h"

// Layouts
#import "RavePGLayout.h"
#import "RavePGLinearLayout.h"
#import "RavePGGridLayout.h"
#import "RavePGCenterLayout.h"

@interface RavePGPegasus : NSObject

+ (RavePGPegasus*)shared;

@property (nonatomic, retain) id<RavePGAssetsProvider> assetsProvider;
@property (nonatomic, retain) id<RavePGLocalizer> localizer;

@end
