using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.User;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
}
