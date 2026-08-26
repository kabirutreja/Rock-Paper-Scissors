using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "SupportContentController_",
                   menuName = "CitrioN/Common/ScriptableObjects/VisualTreeAsset/Controller/Support")]
  public class SupportContentController : ScriptableVisualTreeAssetController
  {
    public string groupName;

    protected static string SUPPORT_FORUM_LINK = "https://forum.unity.com/threads/1537825/";

    protected static string JOIN_DISCORD_BUTTON_CLASS = "button__join-discord";
    protected static string OPEN_SUPPORT_FORUM_BUTTON_CLASS = "button__open-support-forum";
    protected static string COPY_EMAIL_ADDRESS_BUTTON_CLASS = "button__copy-email-address";
    protected static string WRITE_REVIEW_BUTTON_CLASS = "button__write-review";

    public override void Setup(VisualElement root)
    {
      UIToolkitUtilities.SetupButton(root, null, SupportUtilities.JoinDiscord, null, JOIN_DISCORD_BUTTON_CLASS);
      UIToolkitUtilities.SetupButton(root, null, CopyEmailAddressToClipboard, null, COPY_EMAIL_ADDRESS_BUTTON_CLASS);
      UIToolkitUtilities.SetupButton(root, null, () => OpenLink(SUPPORT_FORUM_LINK), null, OPEN_SUPPORT_FORUM_BUTTON_CLASS);
      UIToolkitUtilities.SetupButton(root, null, () => OpenLink(SupportUtilities.PUBLISHER_PAGE), null, WRITE_REVIEW_BUTTON_CLASS);
    }

    public void OpenLink(string url)
    {
      Application.OpenURL(url);
    }

    public static void CopyEmailAddressToClipboard()
    {
      EditorUtilities.CopyStringToClipboard(SupportUtilities.SUPPORT_EMAIL_ADDRESS);
    }
  }
}