using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using MediatR;

using Mongo.Passport.Api.Serviceses;
using Mongo.Passport.Api.Responses;
using Mongo.Passport.Domain.Aggregates.Account;
using Mongo.Passport.Infrastructure.HttpClients.YandexCaptha;
using System.Net;

namespace Mongo.Passport.Api.Requests.Handlers;

public class SignUpEmailRequestHandler : IRequestHandler<SignUpEmailRequest, SignUpResponse>
{
    private readonly IEmailService _emailService;
    private readonly IAccountRepo _accountRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IYandexCaptchaHttpClient _yandexCaptchaHttpClient;
    private readonly ILogger<SignUpEmailRequestHandler> _logger;
    private readonly IMediator _mediator;
    public SignUpEmailRequestHandler(
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        IYandexCaptchaHttpClient yandexCaptchaHttpClient,
        IEmailService emailService,
        IAccountRepo accountRepo,
        ILogger<SignUpEmailRequestHandler> logger)
    {
        _mediator = mediator;
        _emailService = emailService;
        _accountRepo = accountRepo;
        _httpContextAccessor = httpContextAccessor;
        _yandexCaptchaHttpClient = yandexCaptchaHttpClient;
        _logger = logger;
    }

    public async Task<SignUpResponse> Handle(SignUpEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {

            if (!new EmailAddressAttribute().IsValid(request.Email) && request.Email is not null) throw new BadHttpRequestException("Email not valid");

            if (!new Regex(@"^[\w$&_]{5,24}$").IsMatch(request.Nickname)) throw new BadHttpRequestException("Nick not valid");

            if (!new Regex(@"^\S{4,23}$").IsMatch(request.Password)) throw new BadHttpRequestException("Password not valid");

            if (await _accountRepo.FindByEmailAsync(request.Email) is not null) throw new BadHttpRequestException("Email is busy");

            if (await _accountRepo.FindByNickNameAsync(request.Nickname) is not null) throw new BadHttpRequestException("Nick is busy");

            var account = Account.From(request, _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString());

            await _accountRepo.AddAsync(account);

            account.PublishDomainEvents(_mediator);

            await _emailService.SendEmailAsync(
                account.Nickname,
                account.Email,
                "SignIn ChessWood",
                $"Activation link: {account.ActivationCode}");

            return new SignUpResponse(Message: "Code send to you email");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with sign up. Error: {ex}");

            throw;
        }
    }
}
