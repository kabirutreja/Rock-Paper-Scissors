using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledCategory : PropertyAttribute
    {
        public string category;
        public bool colapsable;
        public string message;
        public float top;
        public float down;

        public StyledCategory(string category)
        {
            this.category = category;
            this.top = 10;
            this.down = 10;
            this.colapsable = false;
        }

        public StyledCategory(string category, float top, float down)
        {
            this.category = category;
            this.top = top;
            this.down = down;
            this.colapsable = false;
        }

        public StyledCategory(string category, bool colapsable, int top, int down)
        {
            this.category = category;
            this.top = top;
            this.down = down;
            this.colapsable = colapsable;
        }

        public StyledCategory(string category, bool colapsable, string message, int top, int down)
        {
            this.category = category;
            this.top = top;
            this.down = down;
            this.colapsable = colapsable;
            this.message = message;
        }
    }
}

