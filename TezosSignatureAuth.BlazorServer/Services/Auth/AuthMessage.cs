using System.ComponentModel.DataAnnotations;
using Netezos.Encoding;
using Netezos.Forging;

namespace TezosSignatureAuth.BlazorServer.Services
{
    public class AuthMessage
    {
        [Required]
        public string PublicKey { get; set; } = null!;
        
        [Required]
        public string Signature { get; set; } = null!;
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Payload
            => $"Tezos Signed Message: Hi there, {Timestamp:yyyy-MM-dd HH:mm:ss}!";

        public byte[] PayloadBytes
            => new byte[] { 5, 1 }.Concat(LocalForge.ForgeString(Payload)).ToArray();

        public string PayloadHex
            => Hex.Convert(new byte[] { 5, 1 }.Concat(LocalForge.ForgeString(Payload)));
    }
}
