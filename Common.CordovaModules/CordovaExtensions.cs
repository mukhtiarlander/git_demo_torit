using System.Web;

namespace Common.CordovaModules
{
    public static class CordovaExtensions
    {
        public static void SetCordovaCookie(this HttpResponseBase response, string platform)
        {
            if (!string.IsNullOrWhiteSpace(platform))
            {
                response.SetCookie(new HttpCookie(CordovaHelper.Instance.PlatformCookieKey, platform));
            }
        }
    }
}
