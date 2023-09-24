using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;

namespace Mongo.Passport.Api.Services.SignOutTokenService;

public class SignOutTokenService : BackgroundService//, ISignOutTokenService//IHostedService//, IDisposable
{
    private List<Token> _tokens = new();

    public bool IsSignOutToken(string Token)
    {
        if (_tokens.FirstOrDefault(t => t.Value == Token) is not null) return true;

        return false;
    }

    public void SetSignOutToken(string Token)
    {
        var handler = new JwtSecurityTokenHandler();

        var dataToken = handler.ReadJwtToken(Token);

        var expires = (DateTime)dataToken.Payload["exp"];

        if (_tokens.FirstOrDefault(t => t.Value == Token) is null) _tokens.Add(new (Token, expires));
    }

    public void RemoveSignOutToken(string Token)
    {
        _tokens.Remove(_tokens.FirstOrDefault(t => t.Value == Token));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        new Thread(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_tokens.Count() == 0)
                {
                    await Task.Delay(10000, stoppingToken);

                    continue;
                }

                _tokens.RemoveAll(t => t.TimeDeath >= DateTime.UtcNow);

                await Task.Delay(_tokens[0].TimeDeath - DateTime.UtcNow, stoppingToken);
            }
        }).Start();

        return Task.CompletedTask;
    }

    private record Token(string Value, DateTime TimeDeath);
}
