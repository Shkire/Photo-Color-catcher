using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mapped GameObjects for menu navigation.
/// </summary>
public class GUIElementMap
{
    public GameObject _up;
    public GameObject _down;
    public GameObject _right;
    public GameObject _left;
}

/// <summary>
/// Unity Component used to allow navigation through a menu of GUISelectableElements.
/// </summary>
public class InputMenuController : MonoBehaviour
{

    /// <summary>
    /// List of elements of the menu.
    /// </summary>
    public List<GameObject> _uiElements;

    /// <summary>
    /// The starting element of the menu.
    /// </summary>
    public GameObject _startingElem;

    /// <summary>
    /// The elements of the menu after being mapped.
    /// </summary>
    private Dictionary<GameObject,GUIElementMap> p_uiMap;

    /// <summary>
    /// The element of the menu that is active now (where the cursor is placed).
    /// </summary>
    private GameObject p_actualElem;

    /// <summary>
    /// The minimun response time between menu inputs.
    /// </summary>
    [SerializeField]
    private float p_uiResponseTime = 0.2f;

    /// <summary>
    /// The last time when a menu input has been detected.
    /// </summary>
    private float p_lastResponseTime;

    void Start()
    {
        MapElements();
    }

    void Update()
    {
        
        //If time between menu inputs has passed.
        if (Time.realtimeSinceStartup - p_lastResponseTime >= p_uiResponseTime)
        {
            if (p_actualElem != null)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (p_uiMap[p_actualElem]._up != null)
                    {
                        p_actualElem.SendMessage("NonFocused");
                        p_actualElem = p_uiMap[p_actualElem]._up;
                        p_actualElem.SendMessage("Focused");
                        p_lastResponseTime = Time.realtimeSinceStartup;
                    }
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (p_uiMap[p_actualElem]._down != null)
                    {
                        p_actualElem.SendMessage("NonFocused");
                        p_actualElem = p_uiMap[p_actualElem]._down;
                        p_actualElem.SendMessage("Focused");
                        p_lastResponseTime = Time.realtimeSinceStartup;
                    }
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    if (p_uiMap[p_actualElem]._right != null)
                    {
                        p_actualElem.SendMessage("NonFocused");
                        p_actualElem = p_uiMap[p_actualElem]._right;
                        p_actualElem.SendMessage("Focused");
                        p_lastResponseTime = Time.realtimeSinceStartup;
                    }
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    if (p_uiMap[p_actualElem]._left != null)
                    {
                        p_actualElem.SendMessage("NonFocused");
                        p_actualElem = p_uiMap[p_actualElem]._left;
                        p_actualElem.SendMessage("Focused");
                        p_lastResponseTime = Time.realtimeSinceStartup;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    p_actualElem.SendMessage("Selected");
                    p_lastResponseTime = Time.realtimeSinceStartup;
                }
            }
        }
    }

    /// <summary>
    /// Maps the elements of the menu.
    /// </summary>
    public void MapElements()
    {
        p_uiMap = new Dictionary<GameObject, GUIElementMap>();

        foreach (GameObject go in _uiElements)
        {
            GUIElementMap elem = new GUIElementMap();
            foreach (GameObject go2 in _uiElements)
            {
                if (go2 != go)
                {
                    float xVal = go2.transform.position.x - go.transform.position.x;
                    float yVal = go2.transform.position.y - go.transform.position.y;
                    if (Mathf.Abs(xVal) > Mathf.Abs(yVal))
                    {
                        if (xVal > 0)
                        {
                            if (elem._right == null || (Vector3.Distance(go.transform.position, go2.transform.position) < (Vector3.Distance(go.transform.position, elem._right.transform.position))))
                                elem._right = go2;
                        }
                        else
                        {
                            if (elem._left == null || (Vector3.Distance(go.transform.position, go2.transform.position) < (Vector3.Distance(go.transform.position, elem._left.transform.position))))
                                elem._left = go2;
                        }
                    }
                    else
                    {
                        if (yVal > 0)
                        {
                            if (elem._up == null || (Vector3.Distance(go.transform.position, go2.transform.position) < (Vector3.Distance(go.transform.position, elem._up.transform.position))))
                                elem._up = go2;
                        }
                        else
                        {
                            if (elem._down == null || (Vector3.Distance(go.transform.position, go2.transform.position) < (Vector3.Distance(go.transform.position, elem._down.transform.position))))
                                elem._down = go2;
                        }
                    }
                }
            }
            p_uiMap.Add(go, elem);
        }
        p_actualElem = _startingElem;
        foreach (GameObject go in _uiElements)
        {
            if (go == p_actualElem)
                go.SendMessage("Focused", SendMessageOptions.DontRequireReceiver);
            else
                go.SendMessage("NonFocused", SendMessageOptions.DontRequireReceiver);
        }
    }
}
