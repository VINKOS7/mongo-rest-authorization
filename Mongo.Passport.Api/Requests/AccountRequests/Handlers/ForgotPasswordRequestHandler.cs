
using MediatR;

using Mongo.Passport.Api.Responses;
using Mongo.Passport.Api.Options;
using Mongo.Passport.Api.Requests.Handlers;
using Mongo.Passport.Domain.Aggregates.Account;
using Microsoft.Extensions.Options;
using Mongo.Passport.Api.Serviceses;
using Mongo.Passport.Domain.Aggregates.Account.Enums;
using System.Text.RegularExpressions;
using System.Net;

namespace Mongo.Passport.Api.Requests.AccountRequests.Handlers;

public class ForgotPasswordRequestHandler : IRequestHandler<ForgotPasswordRequest, ForgotPasswordResponse>
{
    private readonly JWTOptions _jwtOptions;
    private readonly IEmailService _emailService;
    private readonly IAccountRepo _accountRepo;
    private readonly ILogger<SignInEmailRequestHandler> _logger;

    public ForgotPasswordRequestHandler(
        IAccountRepo accountRepo,
        IEmailService emailService,
        IOptions<JWTOptions> jwtOptions,
        ILogger<SignInEmailRequestHandler> logger)
    {
        _accountRepo = accountRepo;
        _emailService = emailService;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {      
            var account = await _accountRepo.FindByEmailAsync(request.Email);

            if (account is null) throw new BadHttpRequestException("Account not found");

            await _accountRepo.ChangeActivationCodeAsync(account.Id, $"{new Random().Next(000000, 999999)}");

            await _accountRepo.ChangeAccessStatusAsync(account.Id, AccessStatus.WaitChangePassword);

            var isSendToEmail = await _emailService.SendEmailAsync(
                  account.Nickname,
                  account.Email,
                  "Recovery access in ChessWood", 
                  $"Activation link: {account.ActivationCode}");

            if(!isSendToEmail) throw new BadHttpRequestException("Not send code send to email");

            return new ForgotPasswordResponse(Message: "Code send to email");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with signin. Error: {ex}");

            throw;
        }
    }
}
