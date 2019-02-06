using UnityEngine;

public class UIOrderIngredientElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#Mark1")] private GameObject mark;

    public override void OnViewShow(IWUIWindowView context)
    {
        base.OnViewShow(context);

        if (entity == null)
        {
            return;
        }
        
        var contentEntity = entity as UIOrderIngredientElementEntity;
        mark.SetActive(contentEntity.Mark); 
    }
}