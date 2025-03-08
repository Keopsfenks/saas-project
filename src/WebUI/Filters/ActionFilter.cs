using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebUI.Models;

namespace WebUI.Filters
{
    public class ActionFilter : IAsyncActionFilter
    {
        private readonly IWebHostEnvironment _env;
        public ActionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is Controller controller)
            {
                var menus = System.IO.File.ReadAllText(_env.WebRootPath + "/menu.json");
                var menuList = JsonConvert.DeserializeObject<List<MenuModel>>(menus);
                controller.ViewData["Menu"] = menuList.Where(w => w.IsHiddenMenu == false).OrderBy(o => o.Order).ToList();
            }

            // execute any code before the action executes
            var result = await next();
            // execute any code after the action executes
        }
    }
}
