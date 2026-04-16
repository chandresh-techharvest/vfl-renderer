using Microsoft.AspNetCore.Http;
using System;

namespace VFL.Renderer.Common
{
    public static class CookieHelper
    {
        // Set a cookie with optional encryption
        /// <summary>
        /// Sets a cookie with the specified parameters. If encryption is enabled, the value will be encrypted before being stored.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encrypt"></param>
        /// <param name="expireDays"></param>
        /// <param name="httpOnly"></param>
        /// <param name="secure"></param>
        /// <param name="sameSite"></param>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        public static void SetCookie(HttpResponse response, string key, string value, bool encrypt = false, int? expireMins = 10080, bool httpOnly = false, bool secure = false, SameSiteMode sameSite = SameSiteMode.Lax, string domain = null, string path = "/")
        {
            if (encrypt && !string.IsNullOrEmpty(value))
            {
                value = EncryptionHelper.Encrypt(value);
            }

            var options = new CookieOptions
            {
                Expires = expireMins.HasValue ? DateTimeOffset.UtcNow.AddMinutes(expireMins.Value) : (DateTimeOffset?)null,
                HttpOnly = httpOnly,
                Secure = secure,
                SameSite = sameSite,
                Path = path
            };

            if (!string.IsNullOrEmpty(domain))
                options.Domain = domain;

            response.Cookies.Append(key, value, options);
        }

        // Update a cookie (just call SetCookie, as it will overwrite)
        /// <summary>
        /// Updates a cookie by calling SetCookie with the same key. If the cookie already exists, it will be overwritten with the new value and options. If encryption is enabled, the value will be encrypted before being stored.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="encrypt"></param>
        /// <param name="expireDays"></param>
        /// <param name="httpOnly"></param>
        /// <param name="secure"></param>
        /// <param name="sameSite"></param>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        public static void UpdateCookie(HttpResponse response, string key, string value, bool encrypt = false, int? expireDays = 7, bool httpOnly = false, bool secure = false, SameSiteMode sameSite = SameSiteMode.Lax, string domain = null, string path = "/")
        {
            SetCookie(response, key, value, encrypt, expireDays, httpOnly, secure, sameSite, domain, path);
        }

        // Delete a cookie
        /// <summary>
        /// Deletes a cookie by setting its expiration date to a past date. If the cookie has a specific domain or path, those must be specified as well to ensure the correct cookie is deleted.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="key"></param>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        public static void DeleteCookie(HttpResponse response, string key, string domain = null, string path = "/")
        {
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = path
            };

            if (!string.IsNullOrEmpty(domain))
                options.Domain = domain;

            response.Cookies.Delete(key);
        }

        /// <summary>
        /// Gets a cookie value by key. If decryption is enabled, the value will be decrypted before being returned. If the cookie does not exist or decryption fails, null will be returned.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="decrypt"></param>
        /// <returns></returns>
        public static string GetCookie(HttpRequest request, string key, bool decrypt = false)
        {
            if (request.Cookies.TryGetValue(key, out var value))
            {
                if (decrypt && !string.IsNullOrEmpty(value))
                {
                    try
                    {
                        value = EncryptionHelper.Decrypt(value);
                    }
                    catch
                    {
                        // Handle decryption error if needed
                        value = null;
                    }
                }
                return value;
            }
            return null;
        }
    }
}
