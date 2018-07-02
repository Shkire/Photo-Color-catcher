using System.Collections;
using System.Collections.Generic;

namespace CIEColor
{

    /// <summary>
    /// Data type used to store colors in CIE XYZ format.
    /// </summary>
    public class CIE_XYZColor
    {
        /// <summary>
        /// The X component: linear combination of cone response curves chosen to be nonnegative.
        /// </summary>
        public float _x;

        /// <summary>
        /// The Y component: luminosity.
        /// </summary>
        public float _y;

        /// <summary>
        /// The Z component: quasi-equal to blue stimulation.
        /// </summary>
        public float _z;
    }

    /// <summary>
    /// Data type used to store colors in CIE Lab format.
    /// </summary>
    public class CIELabColor
    {
        /// <summary>
        /// The L component: lightness.
        /// </summary>
        public float _l;

        /// <summary>
        /// The a component: position between red and green.
        /// </summary>
        public float _a;

        /// <summary>
        /// The b component: position between yellow and blue.
        /// </summary>
        public float _b;
    }

}
