using BuberDinner.Application.Authentication.Commands.Register;
using BuberDinner.Application.Authentication.Queries;
using BuberDinner.Application.Common;
using BuberDinner.Contracts.Authentication;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        RegisterCommand registerCommand = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);
        ErrorOr<AuthenticationResult> registerResult = await _mediator.Send(registerCommand);

        return registerResult.Match(
            authResult => Ok(MapAuthResult(authResult)),
            errors => Problem(errors)
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        LoginQuery loginQuery = new LoginQuery(
            request.Email,
            request.Password);
        ErrorOr<AuthenticationResult> loginResult = await _mediator.Send(loginQuery);

        return loginResult.Match(
            authResult => Ok(MapAuthResult(authResult)),
            errors => Problem(errors)
        );
    }

    private static AuthenticationResponse MapAuthResult(AuthenticationResult authResult)
    {
        return new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.FirstName,
            authResult.User.LastName,
            authResult.User.Email,
            authResult.Token);
    }
}