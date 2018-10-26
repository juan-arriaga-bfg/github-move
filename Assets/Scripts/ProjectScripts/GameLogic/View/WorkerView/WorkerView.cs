using UnityEngine;

public class WorkerView: BoardElementView
{
    [SerializeField] private Animator workAnimation;
    public Animator WorkAnimation => workAnimation;
}