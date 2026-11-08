using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Admin;

[Authorize(Roles = Roles.Admin)]
public sealed class IndexModel : PageModel
{
}
