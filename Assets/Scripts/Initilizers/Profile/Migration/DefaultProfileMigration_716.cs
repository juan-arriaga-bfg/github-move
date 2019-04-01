using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public partial class DefaultProfileMigration
{
    private static void Migrate716(int clientVersion, UserProfile profile) 
    {
        var questSave = profile.GetComponent<OrdersSaveComponent>(OrdersSaveComponent.ComponentGuid);

        questSave.Orders = new List<OrderSaveItem>();
    }
}
