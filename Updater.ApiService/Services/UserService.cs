using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.Postgres;
using System.Security.Cryptography;
using System.Text;
using Updater.ApiService.Cache;
using Updater.ApiService.Database;
using Updater.ApiService.Database.Models;

namespace Updater.ApiService.Services;

public class UserService(Context context, UserCache cache)
{
    public async Task<string> GetTokenAsync(string nid, CancellationToken ct = default)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(x => x.Nid == nid,ct);

        if (existingUser is not null)
        {
            if(existingUser.Token != string.Empty)
            {
                return existingUser.Token;
            }
            else
            {
                var token = Helpers.GenerateToken();
                existingUser.Token = token;

                await context.SaveChangesAsync(ct);
                return token;
            }
        }
        else
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Nid = nid,
                Token = Helpers.GenerateToken(),
            };

            await context.Users.AddAsync(user,ct);
            await context.SaveChangesAsync(ct);
            return user.Token;
        }
    }

    public async Task<User?> GetUserByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<User?> GetUserByNidAsync(string nid, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Nid == nid, cancellationToken);
    }

    public async Task<bool> CheckUserStatus(string token, CancellationToken cancellationToken)
    {
        if (cache.ExistUser(token))
        {
            return true;
        }

        var user = await GetUserByTokenAsync(token, cancellationToken);

        if (user == null)
        {
            return false;
        }

        cache.AddToCache(user.Token, user.Nid);

        return true;
    }
}
