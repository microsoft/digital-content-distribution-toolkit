using System;
using Newtonsoft.Json;

namespace blendnet.common.dto.User
{
    public class PersonDto : BaseDto
    {
        /// <summary>
        /// Id of the person
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Phone number of the person
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string UserName { get; set; }

        public PersonDto()
        {
        }

        public PersonDto(PersonDto person)
        {
            Id = person.Id;
            PhoneNumber = person.PhoneNumber;
            UserName = person.UserName;
        }

        public PersonDto(Guid id, string phoneNumber, string userName)
        {
            Id = id;
            PhoneNumber = phoneNumber;
            UserName = userName;
        }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber != null
                    && phoneNumber.Length == 10
                    && !phoneNumber.StartsWith("+")
                    && int.TryParse(phoneNumber, out _);
        }
    }
}
