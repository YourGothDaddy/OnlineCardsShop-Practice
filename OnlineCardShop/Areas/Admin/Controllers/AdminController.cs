namespace OnlineCardShop.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static AdminConstants;

    [Area(AdminConstants.AreaName)]
    [Authorize(Roles = AdministratorRoleName)]

    public abstract class AdminController : Controller
    {
    }
}
