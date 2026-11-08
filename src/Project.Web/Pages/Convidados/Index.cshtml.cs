using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Convidados;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
