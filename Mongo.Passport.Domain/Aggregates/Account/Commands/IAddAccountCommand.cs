using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongo.Passport.Domain.Aggregates.Account.Commands;

public interface IAddAccountCommand
{
    string Nickname { get; }
    string Password { get; }
    string Email { get; }
    string PhoneNumber { get; }
}
