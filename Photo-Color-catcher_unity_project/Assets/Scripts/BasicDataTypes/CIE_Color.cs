using System.Collections;
using System.Collections.Generic;

namespace CIE_Color
{

    /// <summary>
    /// Data type used to store colors in CIE XYZ format.
    /// </summary>
    public class CIE_XYZ_Color
    {
        /// <summary>
        /// The X component: linear combination of cone response curves chosen to be nonnegative.
        /// </summary>
        public float x;

        /// <summary>
        /// The Y component: luminosity.
        /// </summary>
        public float y;

        /// <summary>
        /// The Z component: quasi-equal to blue stimulation.
        /// </summary>
        public float z;
    }

    /// <summary>
    /// Data type used to store colors in CIE Lab format.
    /// </summary>
    public class CIE_LabColor
    {
        /// <summary>
        /// The L component: lightness.
        /// </summary>
        public float l;

        /// <summary>
        /// The a component: position between red and green.
        /// </summary>
        public float a;

        /// <summary>
        /// The b component: position between yellow and blue.
        /// </summary>
        public float b;
    }

}
