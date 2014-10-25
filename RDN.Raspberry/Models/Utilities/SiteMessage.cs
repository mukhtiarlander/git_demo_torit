namespace RDN.Raspberry.Models.Utilities
{
    public class SiteMessage
    {
        public SiteMessageType MessageType { get; set; }
        public string Message { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public string LinkText { get; set; }        
    }

    public enum SiteMessageType
    {
        Warning,
        Info,
        Success,
        Error
    }
}