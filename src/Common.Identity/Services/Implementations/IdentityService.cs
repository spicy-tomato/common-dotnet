using System.Security.Claims;
using Common.Extensions;
using Common.Identity.Exceptions;
using Common.Identity.Models;
using Common.Identity.Services.Abstractions;
using Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.Identity.Services.Implementations;

internal class IdentityService(IHttpContextAccessor httpContextAccessor, ILogger<IdentityService> logger)
    : IIdentityService
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public IUser? GetCurrent()
    {
        if (_user == null)
        {
            return null;
        }

        IUser user;
        try
        {
            user = MapCurrentUser(_user.Claims);
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
            throw;
        }

        return user;
    }

    private static ClaimUser MapCurrentUser(IEnumerable<Claim> claims)
    {
        List<Claim> claimsList = claims.ToList();
        if (claimsList.Count == 0)
        {
            throw new NoClaimException();
        }

        string identifier = claimsList.Find(x => x.Type == ClaimTypes.NameIdentifier)?.Value
            ?? Guid.Empty.ToString();
        string userName = claimsList.Find(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        string name = claimsList.Find(x => x.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;

        Claim? permission = claimsList.Find(x => x.Type == "permissions");
        List<int> permissions = permission == null
            ? []
            : JsonConvert.DeserializeObject<List<int>>(permission.Value) ?? [];

        DateTime loginDate = (claimsList.Find(x => x.Type == "nbf")?.Value ?? string.Empty).ToUnixDateTime()
            ?? throw new InvalidClaimUnixDateTimeException();
        DateTime expirationDate =
            (claimsList.Find(x => x.Type == ClaimTypes.Expiration)?.Value ?? string.Empty).ToUnixDateTime()
            ?? throw new InvalidClaimUnixDateTimeException();

        Claim? roleIdClaims = claimsList.Find(x => x.Type == "role_ids");
        List<int> roles = roleIdClaims == null
            ? []
            : JsonConvert.DeserializeObject<List<int>>(roleIdClaims.Value) ?? [];

        return new ClaimUser(Guid.Parse(identifier), userName, name, IsAuthenticated(loginDate, expirationDate),
            permissions, roles);
    }

    private static bool IsAuthenticated(DateTime loginTime, DateTime expirationDate)
    {
        DateTime currentTime = DateTime.Now;
        return loginTime < currentTime && currentTime < expirationDate;
    }
}