using UnityEngine;
using System.Collections;

/// <summary>
/// Base Component that allows to implement associated actions.
/// </summary>
[RequireComponent(typeof(GUISelectableElement))]
public abstract class GUITool : MonoBehaviour
{

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public virtual void Execute()
    {
    }

}
