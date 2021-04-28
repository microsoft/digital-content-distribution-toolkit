using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blendnet.common.dto.User;

namespace blendnet.api.proxy
{
    public class UserProxy
    {
        private static UserProxy instance = null;

        private UserProxy()
        {

        }

        public static UserProxy Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new UserProxy();
                }

                return instance;
            }
        }


        public User GetUser(Guid userId)
        {
            // correct this later
            return new User();
        }

        public User GetUser(string phoneNumber)
        {
            //correct this later
            return new User();
        }


    }
}
