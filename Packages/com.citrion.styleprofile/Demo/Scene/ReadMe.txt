This demo scene showcases the use of a style profile for UGUI.

1. There are multiple 'Style Listener' components attached to the various UI elements in the scene.
   Those essentially react to specific style changes.
2. On the Canvas there is an 'Assign Style Profile To Listeners In Hierarchy' script which assigns
   either a profile or a profile name to every listener in its hierarchy.
3. We use a 'Register Style Profile' script to register a profile at runtime.
   This is required in order to apply the profile when the game starts.
   Any changes to a profile would also trigger a register for it.


The two buttons apply either the first or second profile.

For more details check out the documentations located under:
Packages/CitrioN - Style Profile/Documentation