﻿using Auction.Domain.Common;

namespace Auction.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public Guid RoleId { get; set; }
}
