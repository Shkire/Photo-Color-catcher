using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;
using UnityEngine.UI;

public class WorldLevelsLoadingManager : Singleton<WorldLevelsLoadingManager>
{
    [SerializeField]
    private GameObject p_levelSelectionModel;

    [SerializeField]
    private GameObject p_parentCanvas;

    [SerializeField]
    private GameObject p_divisionCell;

    [SerializeField]
    private GameObject p_backButtonFX;

    [SerializeField]
    private GameObject p_progress;

    [SerializeField]
    private GameObject p_loadingText;

    [SerializeField]
    private GameObject p_startFX;

    [SerializeField]
    private GameObject p_finishFX;


    private GameObject levelSelectionScreen;

    private GameObject inputMenuController;

    protected WorldLevelsLoadingManager()
    {
    }

    public void StartLoadLevels(string i_path)
    {
        StartCoroutine(LoadLevels(i_path));
    }

    public IEnumerator LoadLevels(string i_path)
    {
        p_startFX.SendMessage("Launch", SendMessageOptions.DontRequireReceiver);

        WorldLoadingManager.Instance.DisableMenuController();

        p_loadingText.GetComponent<Text>().text = "Loading levels...";

        (p_progress.transform as RectTransform).anchorMin = (new Vector2(0,0));

        GameObject auxGameObject;
        Sprite spr = null;
        GUIObjectEnable objectEnable;

        World world = null;
            
        yield return StartCoroutine(PersistenceManager.Instance.LoadLevels(i_path,x=> world=x,p_progress));

        //Creates the level selection screen.
        if (levelSelectionScreen!=null)
            Destroy(levelSelectionScreen);
        levelSelectionScreen = ((GameObject)Instantiate(p_levelSelectionModel));
        levelSelectionScreen.transform.SetParent(p_parentCanvas.transform, false);

        //Creates the InputMenuController.
        if (inputMenuController != null)
            Destroy(inputMenuController);
        inputMenuController = new GameObject("InputMenuController");

        inputMenuController.transform.SetParent(p_parentCanvas.transform);

        inputMenuController.AddComponent<InputMenuController>().uiElements = new List<GameObject>();

        //Shows the image name.
        levelSelectionScreen.GetChild("Image name").GetComponent<Text>().text = world._name;

        //Gets the "Back" button of the page.
        auxGameObject = levelSelectionScreen.GetChild("Back button");

        auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

        //Sets up the "Back" button.
        //auxGameObject.GetComponent<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;

        //Adds the button to the InputMenuController.
        inputMenuController.GetComponent<InputMenuController>().startingElem = auxGameObject;
        inputMenuController.GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

        //From 0 to cells by row - 1.
        for (int x = 0; x < world._imageConfig[0]; x++)
        {
            //From 0 to cells by column - 1.
            for (int y = 0; y < world._imageConfig[1]; y++)
            {
                //Creates and sets up the cell.
                auxGameObject = new GameObject("Level(" + x + "," + y + ")", typeof(RectTransform));
                auxGameObject.transform.SetParent(levelSelectionScreen.GetChild("Margin").transform);
                (auxGameObject.transform as RectTransform).offsetMin = Vector2.zero;
                (auxGameObject.transform as RectTransform).offsetMax = Vector2.zero;

                //If horizontal image.
                if (world._imageConfig[0] > world._imageConfig[1])
                {
                    //Set ups the cell size.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2((x + 1) / (float)(world._imageConfig[0]), ((world._imageConfig[0] - world._imageConfig[1]) / 2f + y + 1) / (float)(world._imageConfig[0]));
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(x / (float)(world._imageConfig[0]), ((world._imageConfig[0] - world._imageConfig[1]) / 2f + y) / (float)(world._imageConfig[0]));

                }

                //If vertical image.
                else
                {
                    //Set ups the cell size.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(((world._imageConfig[1] - world._imageConfig[0]) / 2f + x + 1) / (float)(world._imageConfig[1]), (y + 1) / (float)(world._imageConfig[1]));
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(((world._imageConfig[1] - world._imageConfig[0]) / 2f + x) / (float)(world._imageConfig[1]), y / (float)(world._imageConfig[1]));
                }
                    
                if (!world._levels[new Vector2(x, y)]._completed)
                    //Creates the level sprite.
                    spr = Sprite.Create(world._levels[new Vector2(x, y)]._img.ToGray(), new Rect(0, 0, world._levels[new Vector2(x, y)]._img.width, world._levels[new Vector2(x, y)]._img.height), Vector2.zero);
                else
                    //Creates the level sprite.
                    spr = Sprite.Create(world._levels[new Vector2(x, y)]._img, new Rect(0, 0, world._levels[new Vector2(x, y)]._img.width, world._levels[new Vector2(x, y)]._img.height), Vector2.zero);
                
                //Shows the level image.
                auxGameObject.AddComponent<Image>().sprite = spr;

                //Instantiates the tile.
                ((GameObject)Instantiate(p_divisionCell)).transform.SetParent(auxGameObject.transform, false);

                //Makes the GameObject selectable.
                auxGameObject.AddComponent<GUISelectableElement>();

                //Sets up the level button.
                auxGameObject.AddComponent<GUILoadLevel>()._path = i_path;
                auxGameObject.GetComponent<GUILoadLevel>()._levelPos = new Vector2(x, y);

                //Adds the level image to the InputMenuController.
                inputMenuController.GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                //Creates the focused decoration GameObject.
                auxGameObject = new GameObject("Focused", typeof(RectTransform));

                //Sets the parent transform.
                auxGameObject.transform.SetParent(inputMenuController.GetComponent<InputMenuController>().uiElements[inputMenuController.GetComponent<InputMenuController>().uiElements.Count - 1].transform);

                //Sets up the decoration size.
                (auxGameObject.transform as RectTransform).offsetMin = Vector2.zero;
                (auxGameObject.transform as RectTransform).offsetMax = Vector2.one;
                (auxGameObject.transform as RectTransform).anchorMin = Vector2.zero;
                (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);

                //Adds the GUIDecoration.
                auxGameObject.AddComponent<GUIDecoration>().behaviour = GUIDecoration.DecorationContext.WhenFocused;

                //Adds the disabled image.
                auxGameObject.AddComponent<Image>().enabled = false;
            }
        }
            
        /*

        //Gets the "Back" button.
        auxGameObject = levelSelectionScreen.GetChild("Back button");

        //Adds an GUIDestroy component to the image button.
        auxGameObject.AddComponent<GUIDestroy>().target = levelSelectionScreen;

        //Adds an GUIDestroy component to the image button.
        auxGameObject.AddComponent<GUIDestroy>().target = inputMenuController;
        */

        WorldLoadingManager.Instance.EnableMenuController();

        p_finishFX.SendMessage("Launch", SendMessageOptions.DontRequireReceiver);
    }

    public void DisableMenuController()
    {
        inputMenuController.SetActive(false);
    }

    public void EnableMenuController()
    {
        inputMenuController.SetActive(true);
    }
}
