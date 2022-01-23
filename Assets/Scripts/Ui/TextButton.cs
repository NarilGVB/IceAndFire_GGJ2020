using System;
using UnityEngine.UI;
using TMPro;

public class TextButton : Button
{
    private TextMeshProUGUI text;
    
    protected override void Awake()
    {
        base.Awake();
        
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
        
        ColorBlock colorBlock = colors;
        
        switch (state)
        {
            case SelectionState.Normal:
                text.color = colorBlock.normalColor;
                break;
            case SelectionState.Highlighted:
                text.color = colorBlock.highlightedColor;
                break;
            case SelectionState.Pressed:
                text.color = colorBlock.pressedColor;
                break;
            case SelectionState.Disabled:
                text.color = colorBlock.disabledColor;
                break;
            case SelectionState.Selected:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
