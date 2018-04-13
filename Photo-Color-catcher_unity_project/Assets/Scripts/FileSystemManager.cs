using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// Used to navigate through directories and get images from a directory.
/// </summary>
public class FileSystemManager : Singleton<FileSystemManager>
{
    protected FileSystemManager()
    {
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
