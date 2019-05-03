using UnityEngine;
using UnityEngine.UI;

public class UIAbTestCheatSheetElementViewController : UIContainerElementViewController
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    [IWUIBinding("#ElementButton")] private GameObject buttonPrefab;
    [IWUIBinding("#ElementTitle")] private NSText title;
    [IWUIBinding("#ElementButtons")] private Transform buttonsHost;
    
    private UIAbTestCheatSheetWindowModel model;
    private UIAbTestCheatSheetElementEntity targetEntity;

    public override void Init()
    {
        base.Init();

        targetEntity = entity as UIAbTestCheatSheetElementEntity;
        
        title.Text = $"Test: {targetEntity.Data.TestName}";

        CreateButtons();
    }

    public void CreateButtons()
    {
        buttonPrefab.gameObject.SetActive(true);
        
        int count = buttonsHost.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            Transform child = buttonsHost.GetChild(i);
            if (child != buttonPrefab.transform)
            {
                Destroy(child.gameObject);
            }
        }

        int groupsCount = targetEntity.Data.GroupsCount;

        for (int i = 0; i < groupsCount; i++)
        {
            string currentGroup = AbTestDataManager.GROUPS[i].ToString();
            
            GameObject buttonGo = Instantiate(buttonPrefab);
            buttonGo.GetComponentInChildren<NSText>().Text = currentGroup.ToUpper();
            buttonGo.GetComponentInChildren<Image>().sprite = targetEntity.Data.UserGroup == currentGroup ? selectedSprite : normalSprite;

            var btn = buttonGo.GetComponentInChildren<Button>();
            btn.onClick.AddListener(() =>
            {
                GameDataService.Current.AbTestManager.ForceSetGroup(targetEntity.Data.TestName, currentGroup);
                CreateButtons();
            });
            
            buttonGo.transform.SetParent(buttonsHost, false);
        }
        
        buttonPrefab.gameObject.SetActive(false);
    }
}