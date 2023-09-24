using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.IdentityModel.Tokens;

using MongoDB.Bson;

namespace Mongo.Passport.Api.Requests.AccountRequests.Handlers;

public class JwtToken
{
    public readonly JwtSecurityToken Value;

    public JwtToken(ObjectId Id, string SecretKey, string Issuer, int ExpiresHours)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var identity = new ClaimsIdentity(new[] { new Claim("_id", Id.ToString())});

        var handler = new JwtSecurityTokenHandler();

        Value = handler.CreateJwtSecurityToken(
            subject: identity,
            issuer: Issuer, 
            expires: DateTime.UtcNow.AddHours(ExpiresHours),
            issuedAt: DateTime.UtcNow,
            signingCredentials: credentials);
    }
}
