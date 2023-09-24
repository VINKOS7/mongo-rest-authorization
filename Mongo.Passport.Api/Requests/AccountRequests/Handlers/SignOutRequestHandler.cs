using MediatR;

using Mongo.Passport.Api.Responses;
using Mongo.Passport.Api.Options;
using Mongo.Passport.Api.Requests.Handlers;
using Mongo.Passport.Domain.Aggregates.Account;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MongoDB.Bson;
using Mongo.Passport.Api.Services.SignOutTokenService;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mongo.Passport.Api.Requests.AccountRequests.Handlers;

public class SignOutRequestHandler : IRequestHandler<SignOutRequest, SignOutResponse>
{
    private readonly JWTOptions _jwtOptions;
    private readonly IAccountRepo _accountRepo;
    //private readonly ISignOutTokenService _signOutTokenService;
    private readonly ILogger<SignInEmailRequestHandler> _logger;

    public SignOutRequestHandler(
        IAccountRepo accountRepo,
        //ISignOutTokenService signOutTokenService,
        IOptions<JWTOptions> jwtOptions,
        ILogger<SignInEmailRequestHandler> logger)
    {
        _accountRepo = accountRepo;
        //_signOutTokenService = signOutTokenService;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<SignOutResponse> Handle(SignOutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            var dataToken = handler.ReadJwtToken(request.Token);

            var id = dataToken.Payload["_id"];

            var account = await _accountRepo.FindByIdAsync((ObjectId)id);

            if (account is null) throw new BadHttpRequestException("Do not touch token, my junior hacker))");

            await _accountRepo.RemoveDeviceByTokenAsync(request.Token);

            return new SignOutResponse(Message: "signout");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with attempting signout. Ex: {ex}");

            throw new HttpRequestException("Error with attempting signout.", ex, HttpStatusCode.InternalServerError);
        }
    }
}
