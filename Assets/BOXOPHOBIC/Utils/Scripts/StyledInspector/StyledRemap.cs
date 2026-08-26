using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledRemap : PropertyAttribute
    {
        public string label = "";
        public float min = 0;
        public float max = 0;
        public float top = 0;
        public float down = 0;
        public bool supportInvert = false;

        public bool showAdvancedSettings = false;

        public StyledRemap(string label, float min, float max)
        {
            this.label = label;
            this.min = min;
            this.max = max;
            this.top = 0;
            this.down = 0;
        }

        public StyledRemap(string label, float min, float max, float top, float down, bool supportInvert)
        {
            this.label = label;
            this.min = min;
            this.max = max;
            this.top = top;
            this.down = down;
            this.supportInvert = supportInvert;
        }
    }
}

