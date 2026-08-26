// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledButton : PropertyAttribute
    {
        public string text = "";
        public float top = 0;
        public float down = 0;

        public StyledButton(string text)
        {
            this.text = text;
            this.top = 0;
            this.down = 0;
        }

        public StyledButton(string text, float top, float down)
        {
            this.text = text;
            this.top = top;
            this.down = down;
        }
    }
}

