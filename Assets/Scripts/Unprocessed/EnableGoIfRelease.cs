using UnityEngine;

public class EnableGoIfRelease : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Start()
    {
#if !DEBUG // Define should be inside, do not wrap all Start func code
        target.SetActive(true);
#endif
    }
}