using System.Collections.Generic;

public partial class DefaultProfileMigration
{
    private static void Migrate716(int clientVersion, UserProfile profile)
    {
        profile.OrdersSave.Orders = new List<OrderSaveItem>();
    }
}
