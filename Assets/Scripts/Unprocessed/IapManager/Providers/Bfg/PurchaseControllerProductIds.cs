using System;
using System.Collections.Generic;

/// <summary>
/// PurchaseControllerProductIds
/// This is a class that stores all product identifiers for iOS, Google and Amazon.
/// It is up to the developer to set these values here. 
/// </summary>
public static class PurchaseControllerProductIds
{
    public static void ClearAllLists()
    {
        consumableGoogleProductIds.Clear();
        consumableIOSProductIds.Clear();
        consumableAmazonProductIds.Clear();
        nonConsumableGoogleProductIds.Clear();
        nonConsumableIOSProductIds.Clear();
        nonConsumableAmazonProductIds.Clear();
    }
    
    public static readonly List<string> consumableGoogleProductIds = new List<string>
    {

    };

    public static readonly List<string> nonConsumableGoogleProductIds = new List<string>
    { 

    };

    public static readonly List<string> consumableAmazonProductIds = new List<string>
    {		

    };

    public static readonly List<string> nonConsumableAmazonProductIds = new List<string>
    {

    };

    public static readonly List<string> consumableIOSProductIds = new List<string>
    {

    };

    public static readonly List<string> nonConsumableIOSProductIds = new List<string>
    {

    };
}