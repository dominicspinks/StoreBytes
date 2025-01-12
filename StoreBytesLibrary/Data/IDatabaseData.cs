﻿using StoreBytesLibrary.Models;

namespace StoreBytesLibrary.Data
{
    public interface IDatabaseData
    {
        UserToken? GetUserTokenByApiKey(string apiKey);
    }
}