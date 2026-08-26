using UnityEngine;

namespace CitrioN.Common.Editor
{
  public static class SupportUtilities
  {
    public static string DISCORD_INVITATION_LINK = "https://discord.gg/3Cx5SB8pNR";
    public static string SUPPORT_EMAIL_ADDRESS = "business.norman.schneider@gmail.com";
    public static string PUBLISHER_PAGE = "https://assetstore.unity.com/publishers/82905";

    public static void JoinDiscord()
    {
      Application.OpenURL(DISCORD_INVITATION_LINK);
    }
  } 
}