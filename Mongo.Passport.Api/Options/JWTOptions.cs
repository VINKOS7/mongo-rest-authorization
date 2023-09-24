namespace Mongo.Passport.Api.Options;

public class JWTOptions 
{
    public string SecretKey { get; set; }
    public int ExpiresHours { get; set; }
    public string Issuer { get; set; }
}