using MediatR;

using Mongo.Passport.Api.Responses;
using Mongo.Passport.Api.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Mongo.Passport.Api.Controllers;
using Mongo.Passport.Domain.Aggregates.Account;
using Microsoft.Extensions.Options;
using Mongo.Passport.Domain.Aggregates.Account.Enums;
using Mongo.Passport.Api.Requests.AccountRequests.Handlers;
using Mongo.Passport.Api.Services.UserAgentParser;
using MongoDB.Bson;
using System.Net;

namespace Mongo.Passport.Api.Requests.Handlers;

public class ActivationRequestHandler : IRequestHandler<ActivationRequest, ActivationResponse>
{
    private readonly JWTOptions _jwtOptions;
    private readonly IAccountRepo _accountRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AccountController> _logger;

    public ActivationRequestHandler(
        IAccountRepo accountRepo,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JWTOptions> jwtOptions,
        ILogger<AccountController> logger)
    {
        _accountRepo = accountRepo;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<ActivationResponse> Handle(ActivationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _accountRepo.FindByActivationCodeAsync(request.Code);

            if (account is null) throw new BadHttpRequestException("Account not found");

            if (account.AccessStatus != AccessStatus.WaitActivate) throw new BadHttpRequestException("Account not wait activate");

            await _accountRepo.ChangeActivationCodeAsync(account.Id, string.Empty);

            await _accountRepo.ChangeAccessStatusAsync(account.Id, AccessStatus.Active);

            var jwtToken = new JwtToken(account.Id, _jwtOptions.SecretKey, _jwtOptions.Issuer, _jwtOptions.ExpiresHours).Value;

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var userAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();

            var operationSystem = UserAgentParser.GetOperatingSystem(userAgent);

            await _accountRepo.AddDeviceAsync(account, new()
            {
                Id = ObjectId.GenerateNewId(),
                Name = operationSystem,
                Version = UserAgentParser.GetOsVersion(userAgent, userAgent),
                Token = encodedJwt,
                OnlineAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });

            return new ActivationResponse(Token: encodedJwt, Message: "Access true");
        }
        catch(Exception ex)
        {
            _logger.LogError($"Error with signin. Error: {ex}");

            throw;
        }

    }
}
