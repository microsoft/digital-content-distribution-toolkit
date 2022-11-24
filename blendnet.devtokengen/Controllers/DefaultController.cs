// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blendnet.devtokengen.Controllers
{
    [AllowAnonymous]
    public class DefaultController : Controller
    {
        [Route(""), HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult RedirectDefaultToSwagger()
        {
            return Redirect("/swagger");
        }
    }
}