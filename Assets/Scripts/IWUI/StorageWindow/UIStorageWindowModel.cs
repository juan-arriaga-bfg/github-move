public class UIStorageWindowModel : IWWindowModel 
{
    public string Title
    {
        get { return "Storage"; }
    }

    public string Message
    {
        get { return "Click on the object to sell it!"; }
    }
    
    public string Capacity
    {
        get { return string.Format("capacity: {0}/{1}", 10, 100); }
    }
}
