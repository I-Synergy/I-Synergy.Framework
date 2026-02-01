// <copyright file="DocumentExtensions.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Tests.Support
{
    public static class DocumentExtensions
    {
        public static async Task FillWithAsync(this IDocument document, string text, CancellationToken ct)
        {
            using (var stream = await document.CreateAsync(ct))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(text);
                }
            }
        }

        public static async Task<string> ReadAllAsync(this IDocument document, CancellationToken ct)
        {
            using (var stream = await document.OpenReadAsync(ct))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
