using UnityEngine;

public class ChestButton : MonoBehaviour
{
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject exclamationMark;

    public void UpdateState(bool value)
    {
        shine.SetActive(value);
        exclamationMark.SetActive(value);
    }
}