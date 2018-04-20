using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Used to create the file system explorer that allows the player to navigate through directories using the UI.
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
    private GameObject canvas;

    /// <summary>
    /// The UI text label used to show the actual directory path.
    /// </summary>
    [SerializeField]
    private Text actualDirectoryText;

    /// <summary>
    /// The file system explorer page list.
    /// </summary>
    private List<GameObject> pageList;

    /// <summary>
    /// The input menu controller list.
    /// </summary>
    private List<GameObject> inputMenuControllerList;

    protected FileSystemManager()
    {
    }

    /// <summary>
    /// Changes the actual directory and load the available directories and images.
    /// </summary>
    /// <param name="i_path">The new directory.</param>
    public void ChangeDirectory(string i_path)
    {
        //Shows the new directory path as actual.
        if (actualDirectoryText != null)
            actualDirectoryText.text = i_path;

        //Get the directories and images that are available in the new directory.
        string[] directories = GetDirectories(i_path);
        string[] images = GetImages(i_path);

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
        

        GameObject aux;
        string[] pathSplit;
        Texture2D auxText;
        byte[] loadingBytes;
        Sprite spr;
        GUIObjectEnable objectEnable;

        //If there is a parent directory available = 1; otherwise 0;
        int parent = (Directory.GetParent(i_path) == null) ? 0 : 1;

        //Creates the input menu controller for the first page of the file system explorer.
        inputMenuControllerList.Add(new GameObject("InputMenuController"));
        inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

        //Creates the first page of the file system explorer.
        pageList.Add((GameObject)Instantiate(fileSystemModelPrefab));
        pageList[pageList.Count - 1].transform.SetParent(canvas.transform, false);

        //Destroys the "previous page" button
        Destroy(pageList[pageList.Count - 1].GetChild("Prev page"));

        //For each element to show in the file system explorer (directories and images).
        for (int i = 0; i < directories.Length + images.Length + parent; i++)
        {
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
                    aux.GetComponentInChildren<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;
                    aux.GetComponentInChildren<Text>().text = "back";
                }
                else
                {
                    //Sets up the directory button.
                    aux.GetComponentInChildren<GUIChangeDirectory>().path = directories[i - parent];
                    pathSplit = directories[i - parent].Split(Path.DirectorySeparatorChar);
                    aux.GetComponentInChildren<Text>().text = pathSplit[pathSplit.Length - 1];
                }

                //If it is the first element of the page.
                if (i % 9 == 0)
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().startingElem = aux;

                //Adds the button to the input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(aux);
            }

            //If it is an image.
            else
            {
                //Destroys the directory button.
                Destroy(pageList[pageList.Count - 1].GetChild("Directory " + i % 9));

                //Gets the image button.
                aux = pageList[pageList.Count - 1].GetChild("Image " + i % 9);

                //Sets up the image button.
                aux.GetComponentInChildren<GUIConfigImage>().path= images[i-directories.Length - 1];
                pathSplit = images[i - directories.Length - 1].Split(Path.DirectorySeparatorChar);
                aux.GetComponentInChildren<Text>().text = pathSplit[pathSplit.Length - 1];
                auxText = new Texture2D(4, 4);
                loadingBytes = File.ReadAllBytes(images[i - directories.Length - 1]);
                auxText.LoadImage(loadingBytes);
                spr = Sprite.Create(auxText, new Rect(0, 0, auxText.width, auxText.height), new Vector2(0, 0));
                aux.GetChild("Image").GetComponent<Image>().sprite = spr;

                //If it is the first element of the page.
                if (i % 9 == 0)
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().startingElem = aux;

                //Adds the button to the input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(aux);
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

                //Maps the elements of the input menu controller.
                //inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().MapElements();
            }

            //If it is the last element of the page.
            else if (i % 9 == 8)
            {
                //Creates the input menu controller for the next page of the file system explorer.
                inputMenuControllerList.Add(new GameObject("InputMenuController"));
                inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

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
                objectEnable.enable = true;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = true;
                objectEnable.target = pageList[pageList.Count - 1];

                //Sets up the "next page" button (deactivate this page and this input menu controller).
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = pageList[pageList.Count - 2];

                //Adds the "next page" button to the input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().uiElements.Add(aux);

                //Maps the elements of the input menu controller.
                //inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().MapElements();

                //Gets the "previous page" button of the next page.
                aux = pageList[pageList.Count - 1].GetChild("Prev page");

                //Sets up the "previous page" button (activate this page and this input menu controller).
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = true;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = true;
                objectEnable.target = pageList[pageList.Count - 2];

                //Sets up the "previous page" button (deactivate the next page and the next input menu controller).
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = pageList[pageList.Count - 1];

                //Adds the "previous page" button to the next input menu controller.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(aux);
            }
        }

        foreach (GameObject page in pageList)
        {
            for (int i = 0; i < 9; i++)
            {
                aux = page.GetChild("Image "+i);

                if (aux != null)
                {
                    foreach (GameObject page2 in pageList)
                    {
                        objectEnable = aux.AddComponent<GUIObjectEnable>();
                        objectEnable.target = page2;
                        objectEnable.enable = false;
                    }

                    foreach (GameObject inputMenuController in inputMenuControllerList)
                    {
                        objectEnable = aux.AddComponent<GUIObjectEnable>();
                        objectEnable.target = inputMenuController;
                        objectEnable.enable = false;
                    }
                }
            }
        }
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
