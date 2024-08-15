using BuberDinner.Application.Authentication.Commands.Register;
using BuberDinner.Application.Authentication.Queries.Login;
using BuberDinner.Application.Common;
using BuberDinner.Contracts.Authentication;
using ErrorOr;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("auth")]
[AllowAnonymous]
public class AuthenticationController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public AuthenticationController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        RegisterCommand registerCommand = _mapper.Map<RegisterCommand>(request);
        ErrorOr<AuthenticationResult> registerResult = await _mediator.Send(registerCommand);

        return registerResult.Match(
            authResult =>
            {
                var response = _mapper.Map<AuthenticationResponse>(authResult);
                return Ok(response);
            },
            errors => Problem(errors)
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        LoginQuery loginQuery = _mapper.Map<LoginQuery>(request);
        ErrorOr<AuthenticationResult> loginResult = await _mediator.Send(loginQuery);

        return loginResult.Match(
            authResult =>
            {
                var response = _mapper.Map<AuthenticationResponse>(authResult);
                return Ok(response);
            },
            errors => Problem(errors)
        );
    }
}