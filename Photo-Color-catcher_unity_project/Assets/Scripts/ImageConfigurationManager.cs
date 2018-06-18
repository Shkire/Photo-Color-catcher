using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Used to configurate the image for processing and level generation.
/// </summary>
public class ImageConfigurationManager : Singleton<ImageConfigurationManager>
{
    /// <summary>
    /// The deformation margin (max deformation: |1 - originalSize/newSize|).
    /// </summary>
    public float _deformationMargin;

    /// <summary>
    /// The prefab used to create the configuration selector.
    /// </summary>
    [SerializeField]
    private GameObject p_imageConfigurationPageModel;

    /// <summary>
    /// The parent canvas of the file system explorer.
    /// </summary>
    [SerializeField]
    private GameObject p_pageCanvas;

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
    private List<GameObject> pageList;

    /// <summary>
    /// The input menu controller list.
    /// </summary>
    private List<GameObject> inputMenuControllerList;

    private GameObject activeMenuController;

    protected ImageConfigurationManager()
    {
    }

    /// <summary>
    /// Gets the available image configurations and creates a selector for chosing one of them.
    /// </summary>
    /// <param name="i_path">The path of the image.</param>
    public void GetImageConfigurations(string i_path)
    {
        byte[] loadingBytes = File.ReadAllBytes(i_path);
        Texture2D auxText = new Texture2D(1, 1);
        auxText.LoadImage(loadingBytes);
        List<int[]> imageConfigs = new List<int[]>();
        int cellSize;
        int aux;

        //Destroys the previous page list.
        if (pageList != null)
            for (int i = 0; i < pageList.Count; i++)
                Destroy(pageList[i]);

        pageList = new List<GameObject>();

        //Destroys the previous input menu controller list.
        if (inputMenuControllerList != null)
            for (int i = 0; i < inputMenuControllerList.Count; i++)
                Destroy(inputMenuControllerList[i]);
        inputMenuControllerList = new List<GameObject>();
        GameObject auxGameObject;
        string[] pathSplit;
        GUIObjectEnable objectEnable;
        Sprite spr = null;

        //From max cells per size to min cells per size.
        for (int i = 4; i > 0; i--)
        {
            //If horizontal image.
            if (auxText.width > auxText.height)
            {
                //Gets the size of the cell.
                cellSize = auxText.width / i;

                //Gets the number of cells by column.
                aux = Mathf.RoundToInt(auxText.height / (float)cellSize);

                //If the number of cells by column is between min and max and the image height deformation is under the margin.
                if (aux > 0 && aux <= 4 && Mathf.Abs(1 - auxText.height / (float)(cellSize * aux)) <= _deformationMargin)

                    //Adds the configuration.
                    imageConfigs.Add(new int[]{ i, aux });
            }

            //If vertical image
            else
            {
                //Gets the size of the cell.
                cellSize = auxText.height / i;

                //Gets the number of cells by row.
                aux = Mathf.RoundToInt(auxText.width / (float)cellSize);

                //If the number of cells by row is between min and max and the image height deformation is under the margin.
                if (aux > 0 && aux <= 4 && Mathf.Abs(1 - auxText.width / (float)(cellSize * aux)) <= _deformationMargin)

                    //Adds the configuration.
                    imageConfigs.Add(new int[]{ aux, i });
            }
        }



        //If there aren't any valid configuration.
        if (imageConfigs.Count == 0)
        {
            //Creates the first page of the configuration selector.
            pageList.Add((GameObject)Instantiate(p_imageConfigurationPageModel));
            pageList[pageList.Count - 1].transform.SetParent(p_pageCanvas.transform, false);

            //Destroys all the useless elements of the page.
            Destroy(pageList[pageList.Count - 1].GetChild("Prev page"));
            Destroy(pageList[pageList.Count - 1].GetChild("Next page"));
            Destroy(pageList[pageList.Count - 1].GetChild("Accept button"));
            Destroy(pageList[pageList.Count - 1].GetChild("Cells config"));
            Destroy(pageList[pageList.Count - 1].GetChild("Background image"));

            //Shows the image name.
            pathSplit = i_path.Split(Path.DirectorySeparatorChar);
            aux = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
            pageList[pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - aux, aux);

            //Creates the first InputMenuController.
            inputMenuControllerList.Add(new GameObject("InputMenuController"));

            inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(p_pageCanvas.transform);

            //Gets the "Back" button of the page.
            auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");

            //Sets up the "Back" button.
            auxGameObject.GetComponent<GUILaunchFX>()._target=p_backButtonFX;

            //Adds the button to the InputMenuController.
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().startingElem = auxGameObject;
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements = new List<GameObject>();
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
        }

        //If there are valid configurations.
        else
        {
            //Creates the first page of the configuration selector.
            pageList.Add((GameObject)Instantiate(p_imageConfigurationPageModel));
            pageList[pageList.Count - 1].transform.SetParent(p_pageCanvas.transform, false);

            //Destroys the "Previous page" button.
            Destroy(pageList[pageList.Count - 1].GetChild("Prev page"));

            //Creates the first InputMenuController.
            inputMenuControllerList.Add(new GameObject("InputMenuController"));
            inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(p_pageCanvas.transform);
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

            //For each image configuration.
            for (int i = 0; i < imageConfigs.Count; i++)
            {
                //Destroys the error text label.
                Destroy(pageList[pageList.Count - 1].GetChild("Error text"));


                //Shows the image name.
                pathSplit = i_path.Split(Path.DirectorySeparatorChar);
                aux = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
                pageList[pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - aux, aux);
               
                //Shows the columns x rows configuration.
                pageList[pageList.Count - 1].GetChild("Cells config").GetComponent<Text>().text = imageConfigs[i][0] + "x" + imageConfigs[i][1];

                //If the sprite is not created.
                if (spr == null)
                    spr = Sprite.Create(auxText, new Rect(0, 0, auxText.width, auxText.height), new Vector2(0, 0));

                //Shows the image.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Background image");
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
                            auxGameObject.transform.SetParent(pageList[pageList.Count - 1].GetChild("Margin").transform);
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
                auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");

                //Sets up the "Back" button.
                auxGameObject.GetComponent<GUILaunchFX>()._target=p_backButtonFX;

                //Adds the button to the InputMenuController.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().startingElem = auxGameObject;
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                //Gets the "Accept" button of the page.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Accept button");

                //Sets up the "Accept" button.
                auxGameObject.GetComponent<GUIGenerateWorld>()._img = auxText;
                auxGameObject.GetComponent<GUIGenerateWorld>()._imageConfig = imageConfigs[i];
                auxGameObject.GetComponent<GUIGenerateWorld>()._name = pageList[pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text;

                //Adds the button to the InputMenuController.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                //If it is the last configuration.
                if (i == imageConfigs.Count - 1)
                {
                    //Destroys the "Next page" button.
                    Destroy(pageList[pageList.Count - 1].GetChild("Next page"));
                }

                //Otherwise.
                else
                {
                    //Creates the next page of the configuration selector.
                    pageList.Add((GameObject)Instantiate(p_imageConfigurationPageModel));
                    pageList[pageList.Count - 1].transform.SetParent(p_pageCanvas.transform, false);

                    //Creates the next InputMenuController.
                    inputMenuControllerList.Add(new GameObject("InputMenuController"));
                    inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(p_pageCanvas.transform);
                    inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

                    //Deactivates the page and the InputMenuController.
                    pageList[pageList.Count - 1].SetActive(false);
                    inputMenuControllerList[pageList.Count - 1].SetActive(false);

                    //Gets the "Next page" button of this page.
                    auxGameObject = pageList[pageList.Count - 2].GetChild("Next page");

                    //Sets up the "Next page" button (activate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Sets up the "Next page" button (deactivate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Adds the "Next page" button to the input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                    //Gets the "Previous page" button of the next page.
                    auxGameObject = pageList[pageList.Count - 1].GetChild("Prev page");

                    //Sets up the "Previous page" button (activate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Sets up the "Previous page" button (deactivate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Adds the "Previous page" button to the next input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                }
            }
        }


        /*
        //For each page.
        foreach (GameObject page in pageList)
        {
            //Gets the "Back" button.
            auxGameObject = page.GetChild("Back button");

            //For each page.
            foreach (GameObject page2 in pageList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = page2;
            }

            //For each InputMenuController.
            foreach (GameObject inputMenuController in inputMenuControllerList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = inputMenuController;
            }

            //Gets the "Accept" button.
            auxGameObject = page.GetChild("Accept button");

            //For each page.
            foreach (GameObject page2 in pageList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = page2;
            }

            //For each InputMenuController.
            foreach (GameObject inputMenuController in inputMenuControllerList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = inputMenuController;
            }
        }
        */
    }

    public void DisableMenuController()
    {
        foreach (GameObject inputMenuController in inputMenuControllerList)
            if (inputMenuController.activeSelf)
            {
                activeMenuController = inputMenuController;
                inputMenuController.SetActive(false);
                break;
            }
    }

    public void EnableMenuController()
    {
        activeMenuController.SetActive(true);
    }
}
