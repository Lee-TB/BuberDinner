using System.Collections.ObjectModel;
using BuberDinner.Api.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Authorize]
public class ApiController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        HttpContext.Items[HttpContextItemKeys.Errors] = errors;

        var firstError = errors[0];
        return Problem(firstError);
    }

    private IActionResult Problem(Error error)
    {
        var statusCode = error switch
        {
            { Type: ErrorType.Conflict } => StatusCodes.Status409Conflict,
            { Type: ErrorType.NotFound } => StatusCodes.Status404NotFound,
            { Type: ErrorType.Validation } => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: error.Description);
    }

    private IActionResult ValidationProblem(List<Error> errors)
    {
        var modelState = new ModelStateDictionary();
        errors.ForEach(error => modelState.AddModelError(error.Code, error.Description));
        return ValidationProblem(modelState);
    }
}