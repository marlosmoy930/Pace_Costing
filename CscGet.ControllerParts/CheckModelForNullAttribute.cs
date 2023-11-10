using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CscGet.ControllerParts
{
    /// <summary>
    /// Provides checking inside response/request model on null objects.
    /// That was design instead a lot of checks that we had in our Api's Controllers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly Func<IDictionary<string, object>, bool> _validate;

        public CheckModelForNullAttribute() : this(arguments => arguments.Values.Contains(null))
        { }

        private CheckModelForNullAttribute(Func<IDictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (_validate(actionContext.ActionArguments))
            {
                actionContext.Result = new BadRequestObjectResult("The argument cannot be null");
            }
        }
    }
}
