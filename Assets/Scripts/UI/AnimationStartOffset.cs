using UnityEngine;

public class AnimationStartOffset : MonoBehaviour 
{
    [SerializeField] [Range(0,1)] private float m_offset;
    
    [SerializeField] private bool m_random;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void OnEnable()
    {
        if (animator == null) return;
        
        var animationName = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        animator.Play(animationName, 0, m_random ? Random.Range(0f, 1f) : m_offset);
    }
}