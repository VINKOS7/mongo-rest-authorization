namespace Mongo.Passport.Api.Services.SignOutTokenService;

public interface ISignOutTokenService
{
    public bool IsSignOutToken(string Token);

    public void SetSignOutToken(string Token);

    public void RemoveSignOutToken(string Token);
}
