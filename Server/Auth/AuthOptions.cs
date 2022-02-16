using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Auth
{
    internal static class AuthOptions
    {
        internal const int Lifetime = 1; // время жизни токена - 1 минута
        internal const string Issuer = "MyIssuer";
        internal const string Audience = "MyServer";
        private const string PublicKeyString = "MIIBCgKCAQEA6MX0ILQdATMQoC8Hk1S9Vk/IJ2pMOOKYv4oz2Rrp4ccQJF0VzMRcUUMa/9rlqlyw/0aFFhl90a\u002B7bEE6RWn95p3qRBFk4Y3XP873boJfXs4YMqV1kl7VpdcZidhSdX9BAn5Tdkq8H8hyHcdnKoJR9V1Iw4jif3VseHo6/sTtjizFldxBY2cKSRywl8vbZ5HPWfXzDF/o0v7dqGrGeTaCRrKy3Bh7YyljCIcPi2uFQ1q6Px9mjI1\u002Btebq4\u002Bph0MVBMC85ifwEtm8SobcwfMRaAd0sPFa/lnb9wZ59zWtngHFU5aErZpY4/P1FQoY1rckBZRsGAtzE4WLMaNe5lHeyFQIDAQAB";
        private const string PrivateKeyString = "MIIEpAIBAAKCAQEA6MX0ILQdATMQoC8Hk1S9Vk/IJ2pMOOKYv4oz2Rrp4ccQJF0VzMRcUUMa/9rlqlyw/0aFFhl90a\u002B7bEE6RWn95p3qRBFk4Y3XP873boJfXs4YMqV1kl7VpdcZidhSdX9BAn5Tdkq8H8hyHcdnKoJR9V1Iw4jif3VseHo6/sTtjizFldxBY2cKSRywl8vbZ5HPWfXzDF/o0v7dqGrGeTaCRrKy3Bh7YyljCIcPi2uFQ1q6Px9mjI1\u002Btebq4\u002Bph0MVBMC85ifwEtm8SobcwfMRaAd0sPFa/lnb9wZ59zWtngHFU5aErZpY4/P1FQoY1rckBZRsGAtzE4WLMaNe5lHeyFQIDAQABAoIBAEC7M92YWOyJd9kj2vZ0vUZwX1ferp2vHg098JcybdH0mp\u002BSJ0rPXGDYyXxXMkEDD9GdmcsAgcW7GeLL7Nl61Tm5tnkBYU18NVywQIzBwqXJqF8FNkHjD4TT4W089AlqvvhjivA9bj1kb5\u002BqYR0oKoSSUXbW7S3GyMhkLV3GA83iS6mXoJLPR3vz8ZsE/nmxYuBj8SMJdzzfOjLElUc/JmKtWeBQOmr91AuaJc3QioBsKm\u002BCLMMatBfH8iem8eiO6GMWCzjToMIt4s/dH5IVKH7DrcbyXqFAjyRXybjmupwHsQaY811m4ys8BlMygk9tb\u002BbePPEzdI/J6fee7LFrzk0CgYEA\u002Bvh6NoPWoo3ax/FKKb8nVfxTVXHibAHmqQ8V8KELqfLb2u2hBDhTVeozSSgWr8OS9oR\u002BDtXOshqbK4hReXClbkfO0y2EvQDA6Z8IwEFd3seXSfPTRn4lJQnIn0fCwjO6uMgKQEzqjYbntkyv\u002BCCjUzeervslva5m8u\u002BsyK4rmQcCgYEA7XAe4hb66q9eNHXsx1J7A1XB8j\u002BywzpTUA9pFwXweOzWw/Y/PZZKGqOrVgPzKcslLTznfN0AdupSXUaYnnDicNkk2hCvua/T7oYWr7vnHMPbfLBttOkwyg4GzoqYyz42hIqQTYxYA5aSRUvuWfRdjaQxH3SZUqPl5eWmxZMtIQMCgYEAhdfUHZSr998dKXp2KdNm4AWET9I/hYhyAgEomz7X4JfDRntqXrzHPM988GvvxWbQzqWUMMc3x2ZHEK1PfitkHm8vTxI8cyuetjj5SsbRkAjzlx3ws\u002Bx8VMdl9gQoSreZwgf055vdjwK\u002BOkuloPePAsdr2jfYRdNn1fGaPSAd4isCgYEAktauIsB3eL/vkJ4FStd1gAB/KyO9mALwFykQos5ng0FlKFeXkKzrM0JY6Z8TuJsLaMirmIrMiL4tJcZNCF9jisqfgcSkCD/23yJJ0XBxY1a03I33qtQz\u002BiFmkEEicT8OVDWlPHA41i8\u002BUmC9DSCU4s3SG1Pupi76NI0r8DAuzqUCgYAY\u002BFM7LQIPsj0\u002BcCVk5FWUrcUlHWA1chj/a7V22frDyv4QnYbXP91X6QoORRqGvAD2ExGzu\u002BB8YNpdEVpcKpI0iJcXD/UeETxVrzbYJEiRTVEtCYZGzdJnbY6o5W9kncexnYv\u002BRFG\u002BJbllBWTiPgdsmwQHg4esK5FwXJgI6sFg3Q==";

        public static SecurityKey PublicKey = GetPublicKey();
        public static SecurityKey PrivateKey = GetPrivateKey();

        private static SecurityKey GetPrivateKey()
        {
            var key = RSA.Create();
            key.ImportRSAPrivateKey(source: Convert.FromBase64String(PrivateKeyString), bytesRead: out int _);
            return new RsaSecurityKey(key);
        }

        private static SecurityKey GetPublicKey()
        {
            var key = RSA.Create();
            key.ImportRSAPublicKey(source: Convert.FromBase64String(PublicKeyString), bytesRead: out int _);
            return new RsaSecurityKey(key);
        }
    }
}
