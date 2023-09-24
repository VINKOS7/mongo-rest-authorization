using MediatR;
using Newtonsoft.Json;

using Mongo.Passport.Api.Responses;
using Mongo.Passport.Domain.Aggregates.Account.Commands;

namespace Mongo.Passport.Api.Requests;

public record SignInEmailRequest(
    [JsonProperty("email")] string Email,
    [JsonProperty("password")] string Password
) 
: IRequest<SignInResponse>;

public record SignUpEmailRequest(
    [JsonProperty("nickname")] string Nickname,
    [JsonProperty("email")] string Email,
    [JsonProperty("phoneNumber")] string PhoneNumber,
    [JsonProperty("password")] string Password
) 
: IAddAccountCommand, IRequest<SignUpResponse>;

public record SignInPhoneNumberRequest(
    [JsonProperty("phone-number")] string PhoneNumber,
    [JsonProperty("password")] string Password,
    [JsonProperty("captha")] string Captha
) 
: IRequest<string>;

public record ActivationRequest(
    [JsonProperty("code")] string Code
) 
: IRequest<ActivationResponse>;

public record SignOutRequest(
    [JsonProperty("token")] string Token
)
: IRequest<SignOutResponse>;

public record ForgotPasswordRequest(
    [JsonProperty("email")] string Email
)
: IRequest<ForgotPasswordResponse>;

public record ActivateNewPasswordRequest(
    [JsonProperty("password")] string Password,
    [JsonProperty("email")] string Email
)
: IRequest<ActivateNewPasswordResponse>;