namespace blendnet.crm.common.dto{
    public class ContentAdministratorDto{
        public string id { get; set; }
        public ContentProviderDto  ContentProvider { get; set; }
        public bool isActive { get; set; }
        public string activationDate { get; set; }
        public string deactivationDate { get; set; }
    }
}