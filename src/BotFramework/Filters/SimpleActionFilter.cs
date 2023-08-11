// using System;
// using Microsoft.AspNetCore.Mvc.Filters;
//
// namespace BotFramework.Filters;
//
// public class SimpleActionFilter : Attribute, IActionFilter
// {
//     public void OnActionExecuting(ActionExecutingContext context)
//     {
//     }
//
//     public void OnActionExecuted(ActionExecutedContext context)
//     {
//         if (context.HttpContext.Response.StatusCode == 302)
//         {
//             context.HttpContext.Response.StatusCode = 200;
//         }
//     }
// }