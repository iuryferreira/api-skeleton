using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Notie.Contracts;
using Notie.Models;

namespace Blog.Api.Filters;

public class ExceptionFilter : IActionFilter, IOrderedFilter
{
    public void OnActionExecuting (ActionExecutingContext context) { }

    public void OnActionExecuted (ActionExecutedContext context)
    {
        var notifier = context.HttpContext.RequestServices.GetService<INotifier>();
        if (context.Exception is null)
        {
            return;
        }

        if (notifier != null)
        {
            notifier.AddNotification(new Notification(context.Exception.InnerException?.GetType().FullName,
                context.Exception.Message));
            var result = new ObjectResult(notifier.All()) {StatusCode = 500};
            context.Result = result;
        }

        context.ExceptionHandled = true;
    }

    public int Order => int.MaxValue - 10;
}
