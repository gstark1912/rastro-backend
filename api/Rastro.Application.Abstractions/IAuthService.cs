﻿using Rastro.Domain;

namespace Rastro.Application.Abstractions
{
    public interface IAuthService
    {
        Task<string> LoginAsync(User request);
    }
}
