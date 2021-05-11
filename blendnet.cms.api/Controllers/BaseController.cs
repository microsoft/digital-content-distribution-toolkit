using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.cms.api.Controllers
{
    public class BaseController: ControllerBase
    {
        private IContentProviderRepository _contentProviderRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentProviderRepository"></param>
        public BaseController(IContentProviderRepository contentProviderRepository)
        {
            _contentProviderRepository = contentProviderRepository;
        }

        /// <summary>
        /// Checks access for the given content provider id
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        protected async Task<ActionResult> CheckAccess(Guid contentProviderId)
        {
            //if the user is not super admin then should be able to see the content for which the user is content admin
            if (!this.User.IsInRole(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin))
            {
                ContentProviderDto contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

                bool validContentAdmin = IsValidContentAdmin(contentProvider);

                if (!validContentAdmin)
                {
                    return Unauthorized("Not a valid administrator for the given content");
                }
            }

            return Ok();
        }


        /// <summary>
        /// Is Valid Content Admin
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        protected bool IsValidContentAdmin(ContentProviderDto contentProvider)
        {
            if (contentProvider == null)
            {
                return false;
            }

            if (contentProvider.ContentAdministrators == null || contentProvider.ContentAdministrators.Count() <= 0)
            {
                return false;
            }

            Guid userId = UserClaimData.GetUserId(this.User.Claims);

            ContentAdministratorDto contentAdministrator = contentProvider.ContentAdministrators.Where(ca => (  !string.IsNullOrEmpty(ca.PhoneNumber) 
                                                                                                                && ca.PhoneNumber.Equals(this.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)
                                                                                                                && ca.UserId.Equals(userId))).FirstOrDefault();

            if (contentAdministrator == null || contentAdministrator == default(ContentAdministratorDto))
            {
                return false;
            }

            return true;
        }

    }
}
