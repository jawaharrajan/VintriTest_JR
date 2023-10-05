using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Text.RegularExpressions;
using VintriTest_JR.Models;

namespace VintriTest_JR.ActionFilters
{
    public class UsernameValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var model = context.ActionArguments.FirstOrDefault(x => x.Key == "ratingModel").Value as Ratings;
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            string username = model.Username;
            Match match = regex.Match(input: username);
            if (!match.Success)
            {
                context.Result = new BadRequestObjectResult("The Username field is not a valid e-mail address.");
            }
            return;
        }
            public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
