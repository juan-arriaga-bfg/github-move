public partial class DefaultProfileMigration
{
    private static void Migrate724(int clientVersion, UserProfile profile)
    {
        foreach (var item in profile.OrdersSave.Orders)
        {
            if (item.State == OrderState.Enough) continue;

            item.State -= 1;
        }
    }
}