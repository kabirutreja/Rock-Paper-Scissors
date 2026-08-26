// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledDisplay : PropertyAttribute
    {
        public string displayName = "";

        public StyledDisplay(string displayName)
        {
            this.displayName = displayName;
        }
    }
}

