using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicDataTypes
{
    /// <summary>
    /// Data type used to store RGB components combinations.
    /// </summary>
    [System.Serializable]
    public class RGBContent
    {
        /// <summary>
        /// Contains R component.
        /// </summary>
        public bool _r;

        /// <summary>
        /// Contains G component.
        /// </summary>
        public bool _g;

        /// <summary>
        /// Contains B component.
        /// </summary>
        public bool _b;

        public RGBContent(bool i_r, bool i_g, bool i_b)
        {
            _r = i_r;
            _g = i_g;
            _b = i_b;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            RGBContent aux = (RGBContent)obj;

            if (_r != aux._r)
                return false;

            if (_g != aux._g)
                return false;

            if (_b != aux._b)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Data type used to store an image fit on an array.
    /// </summary>
    [System.Serializable]
    public class OnArrayImage
    {

        //CHANGE TO TEXTURE2D AND SURROGATE??

        /// <summary>
        /// Pixel array.
        /// </summary>
        public Color[] _pixels;

        /// <summary>
        /// Image width.
        /// </summary>
        public int _width;

        /// <summary>
        /// Image height.
        /// </summary>
        public int _height;

        public OnArrayImage(int i_width, int i_height)
        {
            _width = i_width;
            _height = i_height;
            _pixels = new Color[_width * _height];
        }

        public OnArrayImage(Texture2D i_img)
        {
            _width = i_img.width;
            _height = i_img.height;
            _pixels = i_img.GetPixels();
        }

        public Texture2D ToTexture2D()
        {
            Texture2D aux = new Texture2D(_width, _height);
            aux.SetPixels(_pixels);
            aux.Apply();
            return aux;
        }
    }

    /// <summary>
    /// Data type used to storage information from a level navigable cell.
    /// </summary>
    [System.Serializable]
    public class LevelCell
    {
        /// <summary>
        /// The cell image stored as an array.
        /// </summary>
        public OnArrayImage _img;

        /// <summary>
        /// The cell average color.
        /// </summary>
        public Color _average;

        /// <summary>
        /// The RGB components assigned to the cell.
        /// </summary>
        public RGBContent _rgbComponents;

        /// <summary>
        /// The cell average grayscale value.
        /// </summary>
        public float _grayscale;
    }

    /// <summary>
    /// Data type used to storage information from a level.
    /// </summary>
    [System.Serializable]
    public class Level
    {
        /// <summary>
        /// The level image stored as an array.
        /// </summary>
        public OnArrayImage _img;

        /// <summary>
        /// The level cells map.
        /// </summary>
        public Dictionary<Vector2,LevelCell> _cells;

        /// <summary>
        /// The connected level cells graph.
        /// </summary>
        public GraphType<Vector2> _graph;

        public bool _completed;
    }

    /// <summary>
    /// Data type used to storage information from a world (processed image).
    /// </summary>
    [System.Serializable]
    public class World
    {
        public string _name;

        public int[] _imageConfig;

        /// <summary>
        /// The level image stored as an array.
        /// </summary>
        public OnArrayImage _img;

        /// <summary>
        /// The world levels map.
        /// </summary>
        public Dictionary<Vector2,Level> _levels;
    }
}
