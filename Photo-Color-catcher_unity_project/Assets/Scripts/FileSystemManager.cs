using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Singleton used to create the file system explorer that allows the player to navigate through directories using the UI.
/// </summary>
public class FileSystemManager : Singleton<FileSystemManager>
{
    /// <summary>
    /// The prefab used to create the file system explorer.
    /// </summary>
    [SerializeField]
    private GameObject fileSystemModelPrefab;

    /// <summary>
    /// The parent canvas of the file system explorer.
    /// </summary>
    [SerializeField]
    private Transform canvas;

    /// <summary>
    /// The UI text label used to show the actual directory path.
    /// </summary>
    [SerializeField]
    private Text actualDirectoryText;

    /// <summary>
    /// The GameObject of the Back button of the file sytem explorer screen.
    /// </summary>
    [SerializeField]
    private GameObject backButton;

    /// <summary>
    /// The FXSequence launched every time an image is selected on the file system explorer.
    /// </summary>
    [SerializeField]
    private GameObject p_selectImageFX;

    /// <summary>
    /// The file system explorer page list.
    /// </summary>
    private List<GameObject> pageList;

    /// <summary>
    /// The InputMenuController list.
    /// </summary>
    private List<GameObject> inputMenuControllerList;

    /// <summary>
    /// The bar that shows the progress of the image processing.
    /// </summary>
    [SerializeField]
    private RectTransform p_progressBar;

    /// <summary>
    /// The Text label of the loading screen.
    /// </summary>
    [SerializeField]
    private Text p_loadingText;

    /// <summary>
    /// The FXSequence launched when the file system explorer starts the loading of a new directory.
    /// </summary>
    [SerializeField]
    private FXSequence p_startFX;

    /// <summary>
    /// The FXSequence launched when the file system explorer finishes the loading of a new directory.
    /// </summary>
    [SerializeField]
    private FXSequence p_finishFX;

    protected FileSystemManager()
    {
    }

    /// <summary>
    /// Launches the coroutine that changes the directory of the file system explorer.
    /// </summary>
    /// <param name="i_path">The new directory path.</param>
    public void StartChangeDirectory(string i_path)
    {
        StartCoroutine(ChangeDirectory(i_path));
    }

    /// <summary>
    /// Changes the current directory and loads all the available directories and images of the new directory and shows it on the file system explorer.
    /// </summary>
    /// <param name="i_path">The new directory path.</param>
    IEnumerator ChangeDirectory(string i_path)
    {
        //Launches the starting FXSequence.
        p_startFX.Launch();

        //If the new path is empty gets the path of the "MyPictures" folder.
        string path = (i_path == null || i_path.Equals(string.Empty)) ? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) : i_path;

        //Sets the text of the loading screen.
        p_loadingText.text = "Loading...";

        //Initializes the progress bar position.
        p_progressBar.anchorMin = (new Vector2(0, 0));

        //Shows the new directory path as actual.
        if (actualDirectoryText != null)
            actualDirectoryText.text = path;

        //Get the directories and images that are available in the new directory.
        string[] directories = GetDirectories(path);
        string[] images = GetImages(path);

        //Destroys the previous page list.
        if (pageList != null)
            for (int i = 0; i < pageList.Count; i++)
                Destroy(pageList[i]);
        
        //Initializes the page list.
        pageList = new List<GameObject>();

        //Destroys the previous input menu controller list.
        if (inputMenuControllerList != null)
            for (int i = 0; i < inputMenuControllerList.Count; i++)
                Destroy(inputMenuControllerList[i]);
        
        //Initializes the input menu controller list.
        inputMenuControllerList = new List<GameObject>();

        //If there is a parent directory available = 1; otherwise 0;
        int parent = (Directory.GetParent(path) == null) ? 0 : 1;

        //Creates the input menu controller for the first page of the file system explorer.
        inputMenuControllerList.Add(new GameObject("InputMenuController"));
        inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(canvas.transform);
        inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._uiElements = new List<GameObject>();
        inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(backButton);
        inputMenuControllerList[inputMenuControllerList.Count - 1].SetActive(false);

        //Creates the first page of the file system explorer.
        pageList.Add((GameObject)Instantiate(fileSystemModelPrefab));
        pageList[pageList.Count - 1].transform.SetParent(canvas.transform, false);

        //Destroys the "previous page" button
        Destroy(pageList[pageList.Count - 1].GetChild("Prev page"));

        yield return null;

        //For each element to show in the file system explorer (directories and images).
        for (int i = 0; i < directories.Length + images.Length + parent; i++)
        {
            GameObject aux;
            string[] pathSplit;
            Texture2D auxText;
            byte[] loadingBytes;
            Sprite spr;
            GUIObjectEnable objectEnable;

            //If it is a directory.
            if (i < directories.Length + parent)
            {
                //Destroys the image button.
                Destroy(pageList[pageList.Count - 1].GetChild("Image " + i % 9));

                //Gets the directory button.
                aux = pageList[pageList.Count - 1].GetChild("Directory " + i % 9);

                //If it is the parent directory.
                if (i == 0 && parent == 1)
                {
                    //Sets up the directory button.
                    aux.GetComponentInChildren<GUIChangeDirectory>()._path = Directory.GetParent(path).FullName;
                    foreach (Text textLabel in aux.GetComponentsInChildren<Text>())
                        textLabel.text = "back";
                }
                else
                {
                    //Sets up the directory button.
                    aux.GetComponentInChildren<GUIChangeDirectory>()._path = directories[i - parent];
                    pathSplit = directories[i - parent].Split(Path.DirectorySeparatorChar);
                    foreach (Text textLabel in aux.GetComponentsInChildren<Text>())
                        textLabel.text = pathSplit[pathSplit.Length - 1];
                }

                //If it is the first element of the page.
                if (i % 9 == 0)
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._startingElem = aux;

                //Adds the button to the input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(aux);
            }

            //If it is an image.
            else
            {
                //Destroys the directory button.
                Destroy(pageList[pageList.Count - 1].GetChild("Directory " + i % 9));

                //Gets the image button.
                aux = pageList[pageList.Count - 1].GetChild("Image " + i % 9);

                //Sets up the image button.
                aux.AddComponent<GUILaunchFX>()._target = p_selectImageFX;
                aux.GetComponentInChildren<GUIConfigImage>()._path = images[i - directories.Length - 1];
                pathSplit = images[i - directories.Length - 1].Split(Path.DirectorySeparatorChar);
                foreach (Text textLabel in aux.GetComponentsInChildren<Text>())
                    textLabel.text = pathSplit[pathSplit.Length - 1];
                auxText = new Texture2D(4, 4);
                loadingBytes = File.ReadAllBytes(images[i - directories.Length - 1]);
                auxText.LoadImage(loadingBytes);
                spr = Sprite.Create(auxText, new Rect(0, 0, auxText.width, auxText.height), new Vector2(0, 0));
                aux.GetChild("Image").GetComponent<Image>().sprite = spr;

                //If it is the first element of the page.
                if (i % 9 == 0)
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._startingElem = aux;

                //Adds the button to the input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(aux);
            }

            //If it is the last element of the file system explorer.
            if (i == directories.Length + images.Length + parent - 1)
            {
                //If it isn't the last element of the page.
                if (i % 9 < 8)
                {
                    //Destroys the unused buttons.
                    for (int j = i % 9 + 1; j < 9; j++)
                    {
                        Destroy(pageList[pageList.Count - 1].GetChild("Directory " + j));
                        Destroy(pageList[pageList.Count - 1].GetChild("Image " + j));
                    }  
                }

                //Destroys the unused "next page" button.
                Destroy(pageList[pageList.Count - 1].GetChild("Next page"));
            }

            //If it is the last element of the page.
            else if (i % 9 == 8)
            {
                //Creates the input menu controller for the next page of the file system explorer.
                inputMenuControllerList.Add(new GameObject("InputMenuController"));
                inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(canvas.transform);
                inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._uiElements = new List<GameObject>();

                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(backButton);

                //Creates the next page of the file system explorer.
                pageList.Add((GameObject)Instantiate(fileSystemModelPrefab));
                pageList[pageList.Count - 1].transform.SetParent(canvas.transform, false);

                //Deactivates the page and the input menu controller.
                pageList[pageList.Count - 1].SetActive(false);
                inputMenuControllerList[pageList.Count - 1].SetActive(false);

                //Gets the "next page" button of this page.
                aux = pageList[pageList.Count - 2].GetChild("Next page");

                //Sets up the "next page" button (activate the next page and the next input menu controller).
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = true;
                objectEnable._target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = true;
                objectEnable._target = pageList[pageList.Count - 1];

                //Sets up the "next page" button (deactivate this page and this input menu controller).
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = false;
                objectEnable._target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = false;
                objectEnable._target = pageList[pageList.Count - 2];

                //Adds the "next page" button to the input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>()._uiElements.Add(aux);

                //Gets the "previous page" button of the next page.
                aux = pageList[pageList.Count - 1].GetChild("Prev page");

                //Sets up the "previous page" button (activate this page and this input menu controller).
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = true;
                objectEnable._target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = true;
                objectEnable._target = pageList[pageList.Count - 2];

                //Sets up the "previous page" button (deactivate the next page and the next input menu controller).
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = false;
                objectEnable._target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable._enable = false;
                objectEnable._target = pageList[pageList.Count - 1];

                //Adds the "previous page" button to the next input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(aux);
            }

            //Updates the progress bar.
            p_progressBar.anchorMin = (new Vector2(i / (float)(directories.Length + images.Length + parent), 0));

            yield return null;
        }

        //Enables the first InputMenuController.
        inputMenuControllerList[0].SetActive(true);

        //Launches the finishing FXSequence.
        p_finishFX.Launch();
    }

    /// <summary>
    /// Gets all the directories contained in the given path.
    /// </summary>
    /// <returns>Directories contained in the path.</returns>
    /// <param name="i_path">Path.</param>
    public string[] GetDirectories(string i_path)
    {
        if (!Directory.Exists(i_path))
            return null;
        string[] directories = Directory.GetDirectories(i_path);
        List<string> aux = directories.ToList();
        for (int i = 0; i < aux.Count; i++)
        {
            try
            {
                Directory.GetDirectories(aux[i]);
            }
            catch (Exception e)
            {
                aux.RemoveAt(i);
                i--;
            }
        }
        directories = aux.ToArray();
        Array.Sort(directories, StringComparer.InvariantCulture);

        return directories;
    }

    /// <summary>
    /// Gets all the images with .jpg or .png extension contained in the given path.
    /// </summary>
    /// <returns>JPG and PNG images conteined in the path.</returns>
    /// <param name="i_path">Path.</param>
    public string[] GetImages(string i_path)
    {
        if (!Directory.Exists(i_path))
            return null;
        
        string[] validExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
        string[] images = Directory.GetFiles(i_path).Where(x => validExtensions.Contains(Path.GetExtension(x).ToLower())).ToArray();
        Array.Sort(images, StringComparer.InvariantCulture);
        return images;
    }
}
