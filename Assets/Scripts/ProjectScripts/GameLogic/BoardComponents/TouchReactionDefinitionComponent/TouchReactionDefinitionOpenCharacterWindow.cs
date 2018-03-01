using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchReactionDefinitionOpenCharacterWindow : TouchReactionDefinitionComponent
{
    public override bool Make(BoardPosition position, Piece piece)
    {
        UIService.Get.ShowWindow(UIWindowType.CharacterWindow);
        return true;
    }
}