using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mongo.Passport.Api.Responses;

public record SignInResponse(
    [JsonProperty("token")] string Token, 
    [JsonProperty("message")] string Message
);

public record SignUpResponse([JsonProperty("message")] string Message);

public record SignOutResponse([JsonProperty("message")] string Message);

public record ActivationResponse(
    [JsonProperty("token")] string Token,
    [JsonProperty("message")] string Message
);

public record ForgotPasswordResponse([JsonProperty("message")] string Message);

public record ActivateNewPasswordResponse(
    [JsonProperty("message")] string Message,
    [JsonProperty("token")] string Token
);