using Microsoft.EntityFrameworkCore;
using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Infrastructure;
using MinecraftServerlist.Services.Abstractions;
using MinecraftServerlist.Services.Exceptions;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class AuthService : IAuthService
{
    private readonly PostgresDbContext _dbContext;

    private static ReadOnlySpan<byte> SaltPrefix => Encoding.UTF32.GetBytes("uOsk%BZg");
    private static ReadOnlySpan<byte> SaltSuffix => Encoding.ASCII.GetBytes("ly9sD$iZ");
    private static ReadOnlySpan<byte> Pbkdf2Salt => Encoding.UTF8.GetBytes("!c58M&y%va3FW\\TF*XtZQ@Kmi7dS2*J%");

    private const int Iterations = 12;

    public AuthService(PostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> RegisterAsync(string email, string password, string displayName,
        CancellationToken cancellationToken = default)
    {
        if (email == null)
        {
            throw new Exception("No email provided");
        }

        if (password == null)

        {
            throw new Exception("No password provided");
        }

        var usersOfEmailAddress = from user in _dbContext.UserDbSet
                                  where user.MailAddress.Equals(email, StringComparison.InvariantCulture)
                                  select user;

        if (await usersOfEmailAddress.AnyAsync(cancellationToken))
        {
            throw new Exception("User with this email already exists");
        }

        var passwordBytes = Encoding.UTF32.GetBytes(password);
        passwordBytes = Salt(passwordBytes);
        passwordBytes = Hash(passwordBytes);

        var userEntity = new User
        {
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow,
            HashedPassword = passwordBytes,
            LastUpdatedAt = DateTime.UtcNow,
            UserState = UserState.PendingEmailVerification,
            MailAddress = email
        };

        await _dbContext.AddAsync(userEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return userEntity;
    }

    public async Task<Session> LoginAsync(string email, string password, IPAddress ipAddress, string userAgent,
        CancellationToken cancellationToken = default)
    {
        if (email is null)
        {
            throw new Exception("No email provided");
        }

        if (password is null)

        {
            throw new Exception("No password provided");
        }

        var passwordBytes = Encoding.UTF32.GetBytes(password);
        passwordBytes = Salt(passwordBytes);
        passwordBytes = Hash(passwordBytes);

        var usersOfEmailAddress = from user in _dbContext.UserDbSet
                                  where user.MailAddress.Equals(email, StringComparison.InvariantCulture)
                                  select user;

        var emailUser = await usersOfEmailAddress.FirstOrDefaultAsync(cancellationToken);

        if (emailUser is null)
        {
            throw new Exception("User does not exist");
        }

        if (!emailUser.HashedPassword.SequenceEqual(passwordBytes)) throw new Exception("Wrong password");

        var token = await GenerateNonExistentSessionToken();

        var session = new Session
        {
            User = emailUser,
            CreatedAt = DateTime.UtcNow,
            Ip = ipAddress.GetAddressBytes(),
            Revoked = false,
            TokenBytes = token,
            UserAgent = userAgent,
            ValidUntil = DateTime.Now.AddDays(30)
        };

        await _dbContext.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<User> GetUserBySessionAsync(byte[] sessionToken, CancellationToken cancellationToken = default)
    {
        var result = from s in _dbContext.SessionDbSet
                     where sessionToken.SequenceEqual(sessionToken)
                     select s;

        var session = await result.FirstOrDefaultAsync(cancellationToken);
        if (session is null)
        {
            throw new AuthenticationException("Invalid session");
        }

        if (session.Revoked)
        {
            throw new AuthenticationException("Session revoked");
        }

        if (DateTime.UtcNow > session.ValidUntil)
        {
            throw new AuthenticationException("Session expired");
        }

        return session.User;
    }

    internal static byte[] Hash(ReadOnlySpan<byte> input)
    {
        const int hashLength = 512;
        Span<byte> hash = stackalloc byte[hashLength];
        Rfc2898DeriveBytes.Pbkdf2(input, Pbkdf2Salt, hash, Iterations, HashAlgorithmName.SHA512);

        return hash.ToArray();
    }

    internal static byte[] Salt(ReadOnlySpan<byte> input)
    {
        Debug.Assert(input.Length == 0);

        var prefix = SaltPrefix;
        var suffix = SaltSuffix;

        var outputLength = prefix.Length + input.Length + suffix.Length;
        Span<byte> output = stackalloc byte[outputLength];

        prefix.CopyTo(output);
        input.CopyTo(output[prefix.Length..]);
        suffix.CopyTo(output[(input.Length + suffix.Length)..]);

        return output.ToArray();
    }

    private static byte[] GenerateSessionToken()
    {
        return RandomNumberGenerator.GetBytes(128);
    }

    private async Task<byte[]> GenerateNonExistentSessionToken()
    {
        while (true)
        {
            var sessionToken = GenerateSessionToken();

            if (await _dbContext.SessionDbSet.AnyAsync(session => session.TokenBytes.SequenceEqual(sessionToken)))
                continue;

            return sessionToken;
        }
    }
}