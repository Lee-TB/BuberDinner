using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Application.Common;
using BuberDinner.Domain.Common.Errors;
using BuberDinner.Domain.Entities;
using ErrorOr;
using MediatR;

namespace BuberDinner.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
  private readonly IJwtTokenGenerator _jwtTokenGenerator;
  private readonly IUserRepository _userRepository;
  public LoginQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
  {
    _jwtTokenGenerator = jwtTokenGenerator;
    _userRepository = userRepository;
  }
  public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    // 1. Validate the user exists
    var user = _userRepository.GetUserByEmail(query.Email);
    if (user is not User)
    {
      return Errors.Authentication.InvalidCredentials;
    }

    // 2. Validate the password is correct
    if (user.Password != query.Password)
    {
      return Errors.Authentication.InvalidCredentials;
    }

    // 3. Create Jwt token
    var token = _jwtTokenGenerator.GenerateToken(user);

    return new AuthenticationResult(user, token);
  }
}