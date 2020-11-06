namespace blendnet.crm.common.dto
{
    public class AddressDto
    {
        public string StreetName { get; set; }
        
        public string Town { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        public string Pin { get; set; }

        public MapLocationDto MapLocation { get; set; }
    }
}