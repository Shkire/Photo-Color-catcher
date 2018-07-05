using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Singleton used to configurate the image for processing and level generation.
/// </summary>
public class ImageConfigurationManager : Singleton<ImageConfigurationManager>
{
    /// <summary>
    /// The deformation margin (max deformation: |1 - originalSize/newSize|).
    /// </summary>
    public float _deformationMargin;

    /// <summary>
    /// The prefab used to create the pages image configuration selector.
    /// </summary>
    [SerializeField]
    private GameObject p_imageConfigurationPageModel;

    /// <summary>
    /// The parent canvas of the image configuration selector.
    /// </summary>
    [SerializeField]
    private GameObject p_pageCanvas;

    /// <summary>
    /// The GameObject of the "Back" button of the image configuration selector.
    /// </summary>
    [SerializeField]
    private GameObject p_backButtonFX;

    [Header("Division Cell Tiles")]

    /// <summary>
    /// The prefab used as a tile inside the divided image grid.
    /// </summary>
    [SerializeField]
    private GameObject p_centerDivisionCell;

    /// <summary>
    /// The prefab used as a tile on the outside of the lower side of the divided image grid.
    /// </summary>
    [SerializeField]
    private GameObject p_lowerDecorationDivisionCell;

    /// <summary>
    /// The prefab used as a tile on the outside of the right side of the divided image grid.
    /// </summary>
    [SerializeField]
    private GameObject p_rightDecorationDivisionCell;

    /// <summary>
    /// The prefab used as a tile on the outside of the upper side of the divided image grid.
    /// </summary>
    [SerializeField]
    private GameObject p_upperDecorationDivisionCell;

    /// <summary>
    /// The prefab used as a tile on the outside of the left side of the divided image grid.
    /// </summary>
    [SerializeField]
    private GameObject p_leftDecorationDivisionCell;

    /// <summary>
    /// The file system explorer page list.
    /// </summary>
    private List<GameObject> p_pageList;

    /// <summary>
    /// The input menu controller list.
    /// </summary>
    private List<GameObject> p_inputMenuControllerList;

    /// <summary>
    /// The InputMenuController that was active and has been disabled.
    /// </summary>
    private GameObject p_activeMenuController;

    protected ImageConfigurationManager()
    {
    }

    /// <summary>
    /// Gets the available image configurations and creates a selector for chosing one of them.
    /// </summary>
    /// <param name="i_path">The path of the image.</param>
    public void GetImageConfigurations(string i_path)
    {
        List<int[]> imageConfigs = new List<int[]>();
        int auxInt;

        //Loads the image.
        byte[] loadingBytes = File.ReadAllBytes(i_path);
        Texture2D auxText = new Texture2D(1, 1);
        auxText.LoadImage(loadingBytes);

        //Destroys the previous page list.
        if (p_pageList != null)
            for (int i = 0; i < p_pageList.Count; i++)
                Destroy(p_pageList[i]);
        p_pageList = new List<GameObject>();

        //Destroys the previous input menu controller list.
        if (p_inputMenuControllerList != null)
            for (int i = 0; i < p_inputMenuControllerList.Count; i++)
                Destroy(p_inputMenuControllerList[i]);
        p_inputMenuControllerList = new List<GameObject>();

        //From max Levels per side to min Levels per side.
        for (int i = 4; i > 0; i--)
        {
            int cellSize;

            //If horizontal image.
            if (auxText.width > auxText.height)
            {
                //Gets the size of the Level image.
                cellSize = auxText.width / i;

                //Gets the number of Levels by column.
                auxInt = Mathf.RoundToInt(auxText.height / (float)cellSize);

                //If the number of Levels by column is between min and max and the image height deformation is under the margin.
                if (auxInt > 0 && auxInt <= 4 && Mathf.Abs(1 - auxText.height / (float)(cellSize * auxInt)) <= _deformationMargin)

                    //Adds the configuration.
                    imageConfigs.Add(new int[]{ i, auxInt });
            }

            //If vertical image
            else
            {
                //Gets the size of the Level image.
                cellSize = auxText.height / i;

                //Gets the number of Levels by row.
                auxInt = Mathf.RoundToInt(auxText.width / (float)cellSize);

                //If the number of Levels by row is between min and max and the image height deformation is under the margin.
                if (auxInt > 0 && auxInt <= 4 && Mathf.Abs(1 - auxText.width / (float)(cellSize * auxInt)) <= _deformationMargin)

                    //Adds the configuration.
                    imageConfigs.Add(new int[]{ auxInt, i });
            }
        }

        string[] pathSplit;
        GameObject auxGameObject;

        //If there aren't any valid configuration.
        if (imageConfigs.Count == 0)
        {
            //Creates the first page of the configuration selector.
            p_pageList.Add((GameObject)Instantiate(p_imageConfigurationPageModel));
            p_pageList[p_pageList.Count - 1].transform.SetParent(p_pageCanvas.transform, false);

            //Destroys all the useless elements of the page.
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Prev page"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Next page"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Accept button"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Cells config"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Background image"));

            //Shows the image name.
            pathSplit = i_path.Split(Path.DirectorySeparatorChar);
            auxInt = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
            p_pageList[p_pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - auxInt, auxInt);

            //Creates the first InputMenuController.
            p_inputMenuControllerList.Add(new GameObject("InputMenuController"));
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].transform.SetParent(p_pageCanvas.transform);

            //Gets the "Back" button of the page.
            auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Back button");

            //Sets up the "Back" button.
            auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

            //Adds the button to the InputMenuController.
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._startingElem = auxGameObject;
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements = new List<GameObject>();
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);
        }

        //If there are valid configurations.
        else
        {
            //Creates the first page of the configuration selector.
            p_pageList.Add((GameObject)Instantiate(p_imageConfigurationPageModel));
            p_pageList[p_pageList.Count - 1].transform.SetParent(p_pageCanvas.transform, false);

            //Destroys the "Previous page" button.
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Prev page"));

            //Creates the first InputMenuController.
            p_inputMenuControllerList.Add(new GameObject("InputMenuController"));
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].transform.SetParent(p_pageCanvas.transform);
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._uiElements = new List<GameObject>();

            Sprite spr = null;

            //For each image configuration.
            for (int i = 0; i < imageConfigs.Count; i++)
            {
                //Destroys the error text label.
                Destroy(p_pageList[p_pageList.Count - 1].GetChild("Error text"));

                //Shows the image name.
                pathSplit = i_path.Split(Path.DirectorySeparatorChar);
                auxInt = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
                p_pageList[p_pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - auxInt, auxInt) + " - " + imageConfigs[i][0] + "x" + imageConfigs[i][1];
               
                //Shows the columns x rows configuration.
                p_pageList[p_pageList.Count - 1].GetChild("Cells config").GetComponent<Text>().text = imageConfigs[i][0] + "x" + imageConfigs[i][1];

                //If the sprite is not created.
                if (spr == null)
                    spr = Sprite.Create(auxText, new Rect(0, 0, auxText.width, auxText.height), new Vector2(0, 0));

                //Shows the image.
                auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Background image");
                auxGameObject.GetComponent<Image>().sprite = spr;

                //If horizontal image.
                if (imageConfigs[i][0] > imageConfigs[i][1])
                {
                    //Resizes the image.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + imageConfigs[i][0] / (float)(imageConfigs[i][0] + 2) / 2f, 0.5f + imageConfigs[i][1] / (float)(imageConfigs[i][0] + 2) / 2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - imageConfigs[i][0] / (float)(imageConfigs[i][0] + 2) / 2f, 0.5f - imageConfigs[i][1] / (float)(imageConfigs[i][0] + 2) / 2f);
                }

                //If vertical image.
                else
                {
                    //Resizes the image.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + imageConfigs[i][0] / (float)(imageConfigs[i][1] + 2) / 2f, 0.5f + imageConfigs[i][1] / (float)(imageConfigs[i][1] + 2) / 2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - imageConfigs[i][0] / (float)(imageConfigs[i][1] + 2) / 2f, 0.5f - imageConfigs[i][1] / (float)(imageConfigs[i][1] + 2) / 2f);
                }

                //From 0 to cells by row + 1.
                for (int x = 0; x < imageConfigs[i][0] + 2; x++)
                {
                    //From 0 to cells by column + 1.
                    for (int y = 0; y < imageConfigs[i][1] + 2; y++)
                    {
                        //If the cell is not (0,0), (0,cells by column + 1), (cells by row + 1, 0) or (cells by row + 1, cells by column + 1).
                        if (!(x == 0 && y == 0) && !(x == 0 && y == imageConfigs[i][1] + 1) && !(x == imageConfigs[i][0] + 1 && y == 0) && !(x == imageConfigs[i][0] + 1 && y == imageConfigs[i][1] + 1))
                        {
                            //Creates and sets up the cell.
                            auxGameObject = new GameObject("DivisionCell(" + x + "," + y + ")", typeof(RectTransform));
                            auxGameObject.transform.SetParent(p_pageList[p_pageList.Count - 1].GetChild("Margin").transform);
                            (auxGameObject.transform as RectTransform).offsetMin = Vector2.zero;
                            (auxGameObject.transform as RectTransform).offsetMax = Vector2.zero;

                            //If horizontal image.
                            if (imageConfigs[i][0] > imageConfigs[i][1])
                            {
                                //Set ups the cell size.
                                (auxGameObject.transform as RectTransform).anchorMax = new Vector2((x + 1) / (float)(imageConfigs[i][0] + 2), ((imageConfigs[i][0] - imageConfigs[i][1]) / 2f + y + 1) / (float)(imageConfigs[i][0] + 2));
                                (auxGameObject.transform as RectTransform).anchorMin = new Vector2(x / (float)(imageConfigs[i][0] + 2), ((imageConfigs[i][0] - imageConfigs[i][1]) / 2f + y) / (float)(imageConfigs[i][0] + 2));
                            }

                            //If vertical image.
                            else
                            {
                                //Set ups the cell size.
                                (auxGameObject.transform as RectTransform).anchorMax = new Vector2(((imageConfigs[i][1] - imageConfigs[i][0]) / 2f + x + 1) / (float)(imageConfigs[i][1] + 2), (y + 1) / (float)(imageConfigs[i][1] + 2));
                                (auxGameObject.transform as RectTransform).anchorMin = new Vector2(((imageConfigs[i][1] - imageConfigs[i][0]) / 2f + x) / (float)(imageConfigs[i][1] + 2), y / (float)(imageConfigs[i][1] + 2));
                            }

                            //If the cell is (0,?): outside the left side of the grid.
                            if (x == 0)
                            {
                                //Instantiates the tile.
                                ((GameObject)Instantiate(p_leftDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }

                            //If the cell is (cells by row + 1,?): outside the right side of the grid.
                            else if (x == imageConfigs[i][0] + 1)
                            {
                                //Instantiates the tile.
                                ((GameObject)Instantiate(p_rightDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }

                            //If the cell is (?,0): outside the lower side of the grid.
                            else if (y == 0)
                            {
                                //Instantiates the tile.
                                ((GameObject)Instantiate(p_lowerDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }

                            //If the cell is (?,cells by column + 1): outside the upper side of the grid.
                            else if (y == imageConfigs[i][1] + 1)
                            {
                                //Instantiates the tile.
                                ((GameObject)Instantiate(p_upperDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }

                            //If the cell is on the grid.
                            else
                            {
                                //Instantiates the tile.
                                ((GameObject)Instantiate(p_centerDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }
                        }
                    }
                }

                //Gets the "Back" button of the page.
                auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Back button");

                //Sets up the "Back" button.
                auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

                //Adds the button to the InputMenuController.
                p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._startingElem = auxGameObject;
                p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

                //Gets the "Accept" button of the page.
                auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Accept button");

                //Sets up the "Accept" button.
                auxGameObject.GetComponent<GUIGenerateWorld>()._img = auxText;
                auxGameObject.GetComponent<GUIGenerateWorld>()._imageDivisionConfig = imageConfigs[i];
                auxGameObject.GetComponent<GUIGenerateWorld>()._name = p_pageList[p_pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text;

                //Adds the button to the InputMenuController.
                p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

                //If it is the last configuration.
                if (i == imageConfigs.Count - 1)
                {
                    //Destroys the "Next page" button.
                    Destroy(p_pageList[p_pageList.Count - 1].GetChild("Next page"));
                }

                //Otherwise.
                else
                {
                    //Creates the next page of the configuration selector.
                    p_pageList.Add((GameObject)Instantiate(p_imageConfigurationPageModel));
                    p_pageList[p_pageList.Count - 1].transform.SetParent(p_pageCanvas.transform, false);

                    //Creates the next InputMenuController.
                    p_inputMenuControllerList.Add(new GameObject("InputMenuController"));
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].transform.SetParent(p_pageCanvas.transform);
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._uiElements = new List<GameObject>();

                    //Deactivates the page and the InputMenuController.
                    p_pageList[p_pageList.Count - 1].SetActive(false);
                    p_inputMenuControllerList[p_pageList.Count - 1].SetActive(false);

                    //Gets the "Next page" button of this page.
                    auxGameObject = p_pageList[p_pageList.Count - 2].GetChild("Next page");

                    //Sets up the "Next page" button (activate the next page and the next input menu controller).
                    GUIObjectEnable objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_pageList[p_pageList.Count - 1];

                    //Sets up the "Next page" button (deactivate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_pageList[p_pageList.Count - 2];

                    //Adds the "Next page" button to the input menu controller.
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 2].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

                    //Gets the "Previous page" button of the next page.
                    auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Prev page");

                    //Sets up the "Previous page" button (activate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_pageList[p_pageList.Count - 2];

                    //Sets up the "Previous page" button (deactivate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_pageList[p_pageList.Count - 1];

                    //Adds the "Previous page" button to the next input menu controller.
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);
                }
            }
        }
    }

    /// <summary>
    /// Disables the active InputMenuController.
    /// </summary>
    public void DisableMenuController()
    {
        foreach (GameObject inputMenuController in p_inputMenuControllerList)
            if (inputMenuController.activeSelf)
            {
                p_activeMenuController = inputMenuController;
                inputMenuController.SetActive(false);
                break;
            }
    }

    /// <summary>
    /// Enables the InputMenuController that was active.
    /// </summary>
    public void EnableMenuController()
    {
        p_activeMenuController.SetActive(true);
    }
}
