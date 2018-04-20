using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ImageConfigurationManager : Singleton<ImageConfigurationManager>
{

    public float deformationMargin;

    [SerializeField]
    private GameObject imageConfigurationPageModel;

    [SerializeField]
    private GameObject pageCanvas;

    [Header("Division Cell Tiles")]

    [SerializeField]
    private GameObject centerDivisionCell;

    [SerializeField]
    private GameObject lowerBorderDivisionCell;

    [SerializeField]
    private GameObject rightBorderDivisionCell;

    [SerializeField]
    private GameObject upperBorderDivisionCell;

    [SerializeField]
    private GameObject leftBorderDivisionCell;

    [SerializeField]
    private GameObject lowerRightVertexDivisionCell;

    [SerializeField]
    private GameObject upperRightVertexDivisionCell;

    [SerializeField]
    private GameObject upperLeftVertexDivisionCell;

    [SerializeField]
    private GameObject lowerLeftVertexDivisionCell;

    [SerializeField]
    private GameObject lowerDecorationDivisionCell;

    [SerializeField]
    private GameObject rightDecorationDivisionCell;

    [SerializeField]
    private GameObject upperDecorationDivisionCell;

    [SerializeField]
    private GameObject leftDecorationDivisionCell;

    [SerializeField]
    private GameObject rightLowerDecorationDivisionCell;

    [SerializeField]
    private GameObject lowerRightDecorationDivisionCell;

    [SerializeField]
    private GameObject upperRightDecorationDivisionCell;

    [SerializeField]
    private GameObject rightUpperDecorationDivisionCell;

    [SerializeField]
    private GameObject leftUpperDecorationDivisionCell;

    [SerializeField]
    private GameObject upperLeftDecorationDivisionCell;

    [SerializeField]
    private GameObject lowerLeftDecorationDivisionCell;

    [SerializeField]
    private GameObject leftLowerDecorationDivisionCell;

    protected ImageConfigurationManager()
    {
    }

    public void GetImageConfigurations(string i_path)
    {
        byte[] loadingBytes = File.ReadAllBytes(i_path);

        Texture2D auxText = new Texture2D(1, 1);

        auxText.LoadImage(loadingBytes);

        List<int[]> imageConfigs = new List<int[]>();

        int cellSize;

        int aux;

        List<GameObject> pageList = new List<GameObject>();

        List<GameObject> inputMenuControllerList = new List<GameObject>();

        GameObject auxGameObject;

        string[] pathSplit;

        GUIObjectEnable objectEnable;

        Sprite spr = null;

        Vector2 anchorMax;

        Vector2 anchorMin;

        for (int i = 4; i > 0; i--)
        {
            if (auxText.width > auxText.height)
            {
                cellSize = auxText.width / i;
                aux = Mathf.RoundToInt(auxText.height / (float)cellSize);
                if (aux > 0 && aux <= 4 && Mathf.Abs(1 - auxText.height / (float)(cellSize * aux)) <= deformationMargin)
                    imageConfigs.Add(new int[]{i,aux});
            }
            else
            {
                cellSize = auxText.height / i;
                aux = Mathf.RoundToInt(auxText.width / (float)cellSize);
                if (aux > 0 && aux <= 4 && Mathf.Abs(1 - auxText.width / (float)(cellSize * aux)) <= deformationMargin)
                    imageConfigs.Add(new int[]{aux,i});
            }
        }

        if (imageConfigs.Count == 0)
        {
            pageList.Add((GameObject)Instantiate(imageConfigurationPageModel));
            pageList[pageList.Count-1].transform.SetParent(pageCanvas.transform, false);
            Destroy(pageList[pageList.Count-1].GetChild("Prev page"));
            Destroy(pageList[pageList.Count-1].GetChild("Next page"));
            Destroy(pageList[pageList.Count-1].GetChild("Accept button"));
            Destroy(pageList[pageList.Count-1].GetChild("Cells config"));
            pathSplit = i_path.Split(Path.DirectorySeparatorChar);
            aux = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
            pageList[pageList.Count-1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - aux, aux);
            inputMenuControllerList.Add(new GameObject("InputMenuController"));
            auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");
            auxGameObject.GetComponent<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().startingElem = auxGameObject;
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements = new List<GameObject>();
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
        }
        else
        {
            pageList.Add((GameObject)Instantiate(imageConfigurationPageModel));
            pageList[pageList.Count-1].transform.SetParent(pageCanvas.transform, false);
            Destroy(pageList[pageList.Count-1].GetChild("Prev page"));
            inputMenuControllerList.Add(new GameObject("InputMenuController"));
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();
            for (int i = 0; i < imageConfigs.Count; i++)
            {
                Destroy(pageList[pageList.Count-1].GetChild("Error text"));
                pathSplit = i_path.Split(Path.DirectorySeparatorChar);
                aux = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
                pageList[pageList.Count-1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - aux, aux);
                pageList[pageList.Count-1].GetChild("Cells config").GetComponent<Text>().text = imageConfigs[i][0]+"x"+imageConfigs[i][1];

                //SPRITE DE FONDO
                if (spr==null)
                    spr = Sprite.Create(auxText, new Rect(0, 0, auxText.width, auxText.height), new Vector2(0, 0));
                auxGameObject = pageList[pageList.Count - 1].GetChild("Background image");
                auxGameObject.GetComponent<Image>().sprite = spr;
                if (imageConfigs[i][0] > imageConfigs[i][1])
                {
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + imageConfigs[i][0] / (float)(imageConfigs[i][0] + 2)/2f, 0.5f + imageConfigs[i][1] / (float)(imageConfigs[i][0] + 2)/2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - imageConfigs[i][0] / (float)(imageConfigs[i][0] + 2)/2f, 0.5f - imageConfigs[i][1] / (float)(imageConfigs[i][0] + 2)/2f);
                }
                else
                {
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + imageConfigs[i][0] / (float)(imageConfigs[i][1] + 2)/2f, 0.5f + imageConfigs[i][1] / (float)(imageConfigs[i][1] + 2)/2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - imageConfigs[i][0] / (float)(imageConfigs[i][1] + 2)/2f, 0.5f - imageConfigs[i][1] / (float)(imageConfigs[i][1] + 2)/2f);
                }

                for (int x = 0; x < imageConfigs[i][0] + 2; x++)
                {
                    for (int y = 0; y < imageConfigs[i][1] + 2; y++)
                    {
                        if (!(x == 0 && y == 0) && !(x == 0 && y == imageConfigs[i][1] + 1) && !(x == imageConfigs[i][0] + 1 && y == 0) && !(x == imageConfigs[i][0]+1 && y == imageConfigs[i][1] + 1))
                        {
                            auxGameObject = new GameObject("DivisionCell("+x+","+y+")",typeof(RectTransform));
                            auxGameObject.transform.SetParent(pageList[pageList.Count-1].GetChild("Margin").transform);
                            (auxGameObject.transform as RectTransform).offsetMin = Vector2.zero;
                            (auxGameObject.transform as RectTransform).offsetMax = Vector2.zero;
                            if (imageConfigs[i][0] > imageConfigs[i][1])
                            {
                                (auxGameObject.transform as RectTransform).anchorMax = new Vector2((x + 1) / (float)(imageConfigs[i][0] + 2), ((imageConfigs[i][0]-imageConfigs[i][1])/2f + y +1) / (float)(imageConfigs[i][0] + 2));
                                (auxGameObject.transform as RectTransform).anchorMin = new Vector2(x / (float)(imageConfigs[i][0] + 2), ((imageConfigs[i][0]-imageConfigs[i][1])/2f + y) / (float)(imageConfigs[i][0] + 2));

                            }
                            else
                            {
                                (auxGameObject.transform as RectTransform).anchorMax = new Vector2(((imageConfigs[i][1]-imageConfigs[i][0])/2f + x +1) / (float)(imageConfigs[i][1] + 2),(y + 1) / (float)(imageConfigs[i][1] + 2));
                                (auxGameObject.transform as RectTransform).anchorMin = new Vector2(((imageConfigs[i][1]-imageConfigs[i][0])/2f + x) / (float)(imageConfigs[i][1] + 2),y / (float)(imageConfigs[i][1] + 2));
                            }
                            if (x == 0)
                            {
                                ((GameObject)Instantiate(leftDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }
                            else if (x == imageConfigs[i][0] + 1)
                            {
                                ((GameObject)Instantiate(rightDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }
                            else if (y == 0)
                            {
                                ((GameObject)Instantiate(lowerDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }
                            else if (y == imageConfigs[i][1] + 1)
                            {
                                ((GameObject)Instantiate(upperDecorationDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }
                            else
                            {
                                ((GameObject)Instantiate(centerDivisionCell)).transform.SetParent(auxGameObject.transform, false);
                            }
                        }
                    }
                }

                auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");
                auxGameObject.GetComponent<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().startingElem = auxGameObject;
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                auxGameObject = pageList[pageList.Count - 1].GetChild("Accept button");
                //ADD FUNC
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                if (i == imageConfigs.Count - 1)
                {
                    Destroy(pageList[pageList.Count - 1].GetChild("Next page"));
                }
                else
                {
                    pageList.Add((GameObject)Instantiate(imageConfigurationPageModel));
                    pageList[pageList.Count-1].transform.SetParent(pageCanvas.transform, false);
                    inputMenuControllerList.Add(new GameObject("InputMenuController"));
                    inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

                    //Deactivates the page and the input menu controller.
                    pageList[pageList.Count - 1].SetActive(false);
                    inputMenuControllerList[pageList.Count - 1].SetActive(false);

                    //Gets the "next page" button of this page.
                    auxGameObject = pageList[pageList.Count - 2].GetChild("Next page");

                    //Sets up the "next page" button (activate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Sets up the "next page" button (deactivate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Adds the "next page" button to the input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                    //Maps the elements of the input menu controller.
                    //inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().MapElements();

                    //Gets the "previous page" button of the next page.
                    auxGameObject = pageList[pageList.Count - 1].GetChild("Prev page");

                    //Sets up the "previous page" button (activate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Sets up the "previous page" button (deactivate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Adds the "previous page" button to the next input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                }
            }
        }

        foreach (GameObject page in pageList)
        {
            auxGameObject = page.GetChild("Back button");
            foreach (GameObject page2 in pageList)
            {
                auxGameObject.AddComponent<GUIDestroy>().target = page2;
            }

            foreach (GameObject inputMenuController in inputMenuControllerList)
            {
                auxGameObject.AddComponent<GUIDestroy>().target = inputMenuController;
            }

            auxGameObject = page.GetChild("Accept button");
            foreach (GameObject page2 in pageList)
            {
                auxGameObject.AddComponent<GUIDestroy>().target = page2;
            }

            foreach (GameObject inputMenuController in inputMenuControllerList)
            {
                auxGameObject.AddComponent<GUIDestroy>().target = inputMenuController;
            }
        }

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
