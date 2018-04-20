using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIDestroy : GUITool {

    public GameObject target;

    public override void Execute()
    {
        Destroy(target);
    }
}
