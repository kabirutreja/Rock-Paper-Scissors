// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledMessage : PropertyAttribute
    {
        public string type;
        public string message;

        public float top;
        public float down;

        public StyledMessage(string type, string message)
        {
            this.type = type;
            this.message = message;
            this.top = 0;
            this.down = 0;
        }

        public StyledMessage(string type, string message, float top, float down)
        {
            this.type = type;
            this.message = message;
            this.top = top;
            this.down = down;
        }
    }
}

