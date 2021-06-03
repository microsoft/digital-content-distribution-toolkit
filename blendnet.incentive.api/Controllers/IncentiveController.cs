using blendnet.common.dto.Incentive;
using blendnet.incentive.repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blendnet.incentive.api.Controllers
{
    public class IncentiveController : ControllerBase
    {
        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        public IncentiveController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveController> logger,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;
        }

        #region Incentive management methods


        #endregion
    }
}
