using UnityEngine;
using System.Collections;

/// <summary>
/// GUIObject that reacts when is selected.
/// </summary>
public class GUISelectableElement : GUIObject
{

    public override void Selected()
    {
        base.Selected();
        gameObject.SendMessage("Execute", SendMessageOptions.DontRequireReceiver);
    }

}
