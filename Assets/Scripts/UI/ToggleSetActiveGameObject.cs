using UnityEngine;
using UnityEngine.UI;

public class ToggleSetActiveGameObject : MonoBehaviour
{
    [SerializeField] private Toggle target;
    
    [SerializeField] private bool invert;

    private void Start()
    {
        OnValueChenget(target);
    }

    public void OnValueChenget(Toggle toggle)
    {
        if (invert)
        {
            gameObject.SetActive(!toggle.isOn);
            return;
        }
        
        gameObject.SetActive(toggle.isOn);
    }
}