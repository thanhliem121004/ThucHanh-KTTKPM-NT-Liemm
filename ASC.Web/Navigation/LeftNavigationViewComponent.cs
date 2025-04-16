using ASC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.Design;

namespace ASC.Web.Navigation
{
    [ViewComponent(Name = "ASC.Web.Navigation.LeftNavigation")]
    public class LeftNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(NavigationMenu menu)
        {
            menu.MenuItems = menu.MenuItems.OrderBy(p => p.Sequence).ToList();
            return View(menu); 
        }
    }
}
