using System;
using System.Collections.Generic;

namespace Utconnect.Common.Identity.Models
{
    internal class ClaimUser : IUser
    {
        public ClaimUser(Guid identifier,
            string userName,
            string name,
            bool isAuthenticated,
            List<int> permissions,
            List<int> roles)
        {
            Id = identifier;
            UserName = userName;
            Name = name;
            IsAuthenticated = isAuthenticated;
            Permissions = permissions;
            Roles = roles;
        }

        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Name { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<int> Permissions { get; set; }
        public List<int> Roles { get; set; }
    }
}