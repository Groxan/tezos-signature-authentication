using Netezos.Keys;

namespace TezosSignatureAuth.AspNetCore.Services
{
    public class AuthService
    {
        const int MessageExpiration = 60;
        readonly ILogger Logger;

        public AuthService(ILogger<AuthService> logger)
        {
            Logger = logger;
        }

        public bool TryAuthenticate(AuthMessage message, out string? address, out string? role)
        {
            role = null;
            address = null;

            #region check expiration
            if (message.Timestamp.AddSeconds(MessageExpiration) < DateTime.UtcNow)
            {
                Logger.LogWarning("Authentication message expired");
                return false;
            }
            #endregion

            #region check public key
            PubKey pubKey;
            try
            {
                pubKey = PubKey.FromBase58(message.PublicKey);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Invalid public key");
                return false;
            }
            #endregion

            #region check signature
            try
            {
                if (!pubKey.Verify(message.PayloadBytes, message.Signature))
                {
                    Logger.LogWarning("Invalid signature");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Invalid signature");
                return false;
            }
            #endregion

            address = pubKey.Address;
            role = "user";

            return true;
        }
    }
}
