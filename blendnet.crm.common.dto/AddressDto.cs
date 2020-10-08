namespace blendnet.crm.common.dto
{
    public class AddressDto
    {
        public string id { get; set; }
        public string streetName { get; set; }
        public string town { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pin { get; set; }
        public MapLocationDto mapLocation { get; set; }
    }
}