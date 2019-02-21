using UnityEngine;

public class UIOrderIngredientElementViewController : UISimpleScrollElementViewController
{
    [IWUIBinding("#Mark")] private GameObject mark;

    public override void Init()
    {
        base.Init();

        UpdateMark();
    }

    private void UpdateMark()
    {
        if (entity == null)
        {
            return;
        }
        
        var contentEntity = entity as UIOrderIngredientElementEntity;       

        mark.SetActive(contentEntity.Mark);
    }
}