using UnityEngine;
using UnityEngine.UI;

public class LockView : UIBoardView
{
    [SerializeField] private NSText label;
    [SerializeField] private Image Image;
    [SerializeField] private Material GrayscaleMaterial;
    
    protected override ViewType Id => ViewType.Lock;

    private string value;
    public string Value
    {
        get { return value;}
        set
        {
            this.value = value;
            label.Text = value;
        }
    }
    
    public override void Init(Piece piece)
    {
        base.Init(piece);

        Priority = defaultPriority = 0;

        Value = "";
        
        Change(true);
    }

    public void SetGrayscale(bool enabled)
    {
        Image.material = enabled ? GrayscaleMaterial : null;
    }
}