using MediatR;

using Mongo.Passport.Api.Responses;
using Mongo.Passport.Api.Controllers;
using Mongo.Passport.Api.Options;
using Mongo.Passport.Domain.Aggregates.Account;
using Microsoft.Extensions.Options;
using Mongo.Passport.Domain.Aggregates.Account.Enums;
using System.IdentityModel.Tokens.Jwt;
using Mongo.Passport.Api.Services.UserAgentParser;
using MongoDB.Bson;
using System;
using System.Text.RegularExpressions;
using System.Net;

namespace Mongo.Passport.Api.Requests.AccountRequests.Handlers;

public class ActivateNewPasswordRequestHandler : IRequestHandler<ActivateNewPasswordRequest, ActivateNewPasswordResponse>
{
    private readonly JWTOptions _jwtOptions;
    private readonly IAccountRepo _accountRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AccountController> _logger;

    public ActivateNewPasswordRequestHandler(
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

    public async Task<ActivateNewPasswordResponse> Handle(ActivateNewPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!new Regex(@"^\S{4,23}$").IsMatch(request.Password)) throw new BadHttpRequestException("Password not valid");

            var account = await _accountRepo.FindByEmailAsync(request.Email);

            if (account is null) return new ActivateNewPasswordResponse(Token: null, Message: "Account not found");

            if (account.AccessStatus != AccessStatus.WaitChangePassword) throw new BadHttpRequestException("Not wait change password");

            await _accountRepo.ChangePasswordAsync(account.Id, request.Password);

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

            return new ActivateNewPasswordResponse(Token: encodedJwt, Message: $"Access true");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with signin. Error: {ex}");

            throw new HttpRequestException($"Error with attemping forgot password. Ex: {ex}", ex, HttpStatusCode.InternalServerError);
        }
    }
}
