using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Used to navigate through directories and get images from a directory.
/// </summary>
public class FileSystemManager : Singleton<FileSystemManager>
{
    [SerializeField]
    private GameObject fileSystemModelPrefab;

    [SerializeField]
    private GameObject canvas;

    private List<GameObject> screenList;

    private List<GameObject> inputMenuControllerList;

    protected FileSystemManager()
    {
    }

    public void ChangeDirectory(string i_path)
    {
        string[] directories = GetDirectories(i_path);
        string[] images = GetImages(i_path);

        if (screenList == null)
            screenList = new List<GameObject>();
        else
            for (int i = 0; i < screenList.Count; i++)
                Destroy(screenList[i]);

        if (inputMenuControllerList == null)
            inputMenuControllerList = new List<GameObject>();
        else
            for (int i = 0; i < inputMenuControllerList.Count; i++)
                Destroy(inputMenuControllerList[i]);

        GameObject aux;
        string[] pathSplit;
        Texture2D auxText;
        byte[] loadingBytes;
        Sprite spr;
        GUIObjectEnable objectEnable;
        int parent = 1;
        if (Directory.GetParent(i_path) == null)
            parent = 0;
        /*
        inputMenuController = new GameObject("InputMenuController");
        inputMenuController.AddComponent<InputMenuController>();
        */
        inputMenuControllerList.Add(new GameObject("InputMenuController"));
        inputMenuControllerList[inputMenuControllerList.Count-1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();
        screenList.Add((GameObject)Instantiate(fileSystemModelPrefab));
        screenList[screenList.Count-1].transform.SetParent(canvas.transform, false);
        Destroy(screenList[screenList.Count-1].GetChild("Prev page"));
        for (int i = 0; i < directories.Length + images.Length + parent; i++)
        {
            /*
            if (i % 9 == 0)
            {
                if (i != 0)
                {
                    screenList[screenList.Count-1].SetActive(false);
                    inputMenuControllerList[screenList.Count-1].SetActive(false);
                }
                //SI NO ES LA PRIMERA, REDIRIGIR AL ANTERIOR MENU
            }
            */
            //Directory
            if (i < directories.Length + parent)
            {
                Destroy(screenList[screenList.Count-1].GetChild("Image " + i%9));
                aux = screenList[screenList.Count-1].GetChild("Directory " + i%9);
                //Back directory
                if (i == 0 && parent == 1)
                {
                    aux.GetComponentInChildren<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;
                    aux.GetComponentInChildren<Text>().text = "back";
                }
                else
                {
                    aux.GetComponentInChildren<GUIChangeDirectory>().path= directories[i- parent];
                    pathSplit = directories[i - parent].Split(Path.DirectorySeparatorChar);
                    aux.GetComponentInChildren<Text>().text = pathSplit[pathSplit.Length - 1];
                }
                if (i%9 == 0)
                    inputMenuControllerList[inputMenuControllerList.Count-1].GetComponent<InputMenuController>().startingElem = aux;
                inputMenuControllerList[inputMenuControllerList.Count-1].GetComponent<InputMenuController>().uiElements.Add(aux);
            }
            //Image
            else
            {
                Destroy(screenList[screenList.Count-1].GetChild("Directory " + i%9));
                aux = screenList[screenList.Count-1].GetChild("Image " + i%9);
                //aux.GetComponentInChildren<GUIChangeDirectory>().path= images[i-directories.Length - 1];
                pathSplit = images[i-directories.Length - 1].Split(Path.DirectorySeparatorChar);
                aux.GetComponentInChildren<Text>().text = pathSplit[pathSplit.Length - 1];
                auxText = new Texture2D(4, 4);
                loadingBytes = File.ReadAllBytes(images[i - directories.Length - 1]);
                auxText.LoadImage(loadingBytes);
                spr = Sprite.Create (auxText, new Rect (0, 0, auxText.width, auxText.height), new Vector2 (0, 0));
                aux.GetChild("Image").GetComponent<Image>().sprite = spr;
                if (i%9 == 0)
                    inputMenuControllerList[inputMenuControllerList.Count-1].GetComponent<InputMenuController>().startingElem = aux;
                if (inputMenuControllerList[inputMenuControllerList.Count-1].GetComponent<InputMenuController>().uiElements == null)
                    inputMenuControllerList[inputMenuControllerList.Count-1].GetComponent<InputMenuController>().uiElements = new List<GameObject>();
                inputMenuControllerList[inputMenuControllerList.Count-1].GetComponent<InputMenuController>().uiElements.Add(aux);
            }
            if (i == directories.Length + images.Length + parent -1 )
            {
                if (i % 9 < 8)
                {
                    for (int j = i % 9 + 1; j < 9; j++)
                    {
                        Destroy(screenList[screenList.Count - 1].GetChild("Directory " + j));
                        Destroy(screenList[screenList.Count - 1].GetChild("Image " + j));
                    }  
                }
                Destroy(screenList[screenList.Count - 1].GetChild("Next page"));
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().MapElements();
            }
            else if (i % 9 == 8)
            {
                inputMenuControllerList.Add(new GameObject("InputMenuController"));
                inputMenuControllerList[inputMenuControllerList.Count-1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();
                screenList.Add((GameObject)Instantiate(fileSystemModelPrefab));
                screenList[screenList.Count-1].transform.SetParent(canvas.transform, false);
                screenList[screenList.Count-1].SetActive(false);
                inputMenuControllerList[screenList.Count-1].SetActive(false);
                aux = screenList[screenList.Count-2].GetChild("Next page");
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = true;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = true;
                objectEnable.target = screenList[screenList.Count - 1];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = screenList[screenList.Count - 2];
                inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().uiElements.Add(aux);
                inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().MapElements();

                aux = screenList[screenList.Count-1].GetChild("Prev page");
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = true;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = true;
                objectEnable.target = screenList[screenList.Count - 2];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                objectEnable = aux.AddComponent<GUIObjectEnable>();
                objectEnable.enable = false;
                objectEnable.target = screenList[screenList.Count - 1];
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(aux);
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
