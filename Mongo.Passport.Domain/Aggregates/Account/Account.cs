using Mongo.Migration.Documents;
using Dotseed.Domain;
using MongoDB.Bson;

using Mongo.Passport.Domain.Aggregates.Account.Commands;
using Mongo.Passport.Domain.Aggregates.Account.Enums;

using Mongo.Passport.Domain.Aggregates.Account.Values;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mongo.Passport.Domain.Aggregates.Account;

public class Account : DomainAggregate, IDocument, IAggregateRoot
{
    public static Account From(IAddAccountCommand command, string userAgent)
    {
        var now = DateTime.UtcNow;

        var account = new Account()
        {
            AccessStatus = AccessStatus.WaitActivate,
            ActivationCode = $"{new Random().Next(000000, 999999)}",
            Nickname = command.Nickname,
            Password = command.Password,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,

            Devices = new(),

            PasswordAt = now,
            PhoneNumberAt = now,
            EmailAt = now,
        };

        //This logic, from one project, left as an example domain and integration events
        //account.AddDomainEvent(new AddAccountDomainEvent(account.Id, account.Firstname, account.Lastname));

        return account; 
    }

    public AccessStatus AccessStatus { get; set; }
    public string ActivationCode { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public DateTime PasswordAt { get; set; }
    public string Email { get; set; }
    public DateTime EmailAt { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime PhoneNumberAt { get; set; }
    public DocumentVersion Version { get; set; }

    public List<Device> Devices { get; set; }
}
