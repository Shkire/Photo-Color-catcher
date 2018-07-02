using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicDataTypes
{
    /// <summary>
    /// Data type used to store combinations of RGB components.
    /// </summary>
    [System.Serializable]
    public class RGBContent
    {
        /// <summary>
        /// If the combination contains R component.
        /// </summary>
        public bool _r;

        /// <summary>
        /// If the combination contains G component.
        /// </summary>
        public bool _g;

        /// <summary>
        /// If the combination contains B component.
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
    /// Data type used to store information from a level navigable cell.
    /// </summary>
    [System.Serializable]
    public class LevelCell
    {
        /// <summary>
        /// The cell image.
        /// </summary>
        public Texture2D _img;

        /// <summary>
        /// The cell average color.
        /// </summary>
        public Color _average;

        /// <summary>
        /// The cell average grayscale value.
        /// </summary>
        public float _grayscale;

        /// <summary>
        /// The RGB components assigned to the cell.
        /// </summary>
        public RGBContent _rgbComponents;
    }

    /// <summary>
    /// Data type used to store information from a level.
    /// </summary>
    [System.Serializable]
    public class Level
    {
        /// <summary>
        /// The level image.
        /// </summary>
        public Texture2D _img;

        /// <summary>
        /// The level cell map.
        /// </summary>
        public Dictionary<Vector2,LevelCell> _cells;

        /// <summary>
        /// The graph containing level cells connection.
        /// </summary>
        public GraphType<Vector2> _graph;

        /// <summary>
        /// If the level is completed.
        /// </summary>
        public bool _completed;
    }

    /// <summary>
    /// Data type used to storage information from a world (processed image).
    /// </summary>
    [System.Serializable]
    public class World
    {
        /// <summary>
        /// The world name.
        /// </summary>
        public string _name;

        /// <summary>
        /// The image division configuration ([columns,rows]).
        /// </summary>
        public int[] _imageDivisionConfig;

        /// <summary>
        /// The world image.
        /// </summary>
        public Texture2D _img;

        /// <summary>
        /// The world level map.
        /// </summary>
        public Dictionary<Vector2,Level> _levels;
    }
}
