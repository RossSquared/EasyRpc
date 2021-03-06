﻿using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace EasyRpc.Tests.Middleware
{
    public static class SerializeMethods
    {
        public static string SerializeToBase64String<T>(this T value)
        {
            MemoryStream returnStream = new MemoryStream();

            using (var text = new StreamWriter(returnStream))
            {
                using (var jsonStream = new JsonTextWriter(text))
                {
                    var serializer = new JsonSerializer();

                    serializer.Serialize(jsonStream, value);
                }
            }
            
            return Convert.ToBase64String(returnStream.ToArray());
        }

        public static T DeserializeFromBase64String<T>(this string value)
        {
            using (var memoryStream = new MemoryStream(Convert.FromBase64String(value)))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var serialized = streamReader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(serialized);
                }
            }
        }

        public static Stream SerializeToStream<T>(this T value, string compression = null)
        {
            MemoryStream returnStream = new MemoryStream();
            Stream compressed = null;
            Stream stream = returnStream;

            if (compression == "gzip")
            {
                stream = compressed = new GZipStream(returnStream, CompressionMode.Compress);
            }
            else if (compression == "br")
            {
                stream = compressed = new BrotliStream(returnStream, CompressionLevel.Fastest);
            }

            using (var text = new StreamWriter(stream))
            {
                using (var jsonStream = new JsonTextWriter(text))
                {
                    var serializer = new JsonSerializer();

                    serializer.Serialize(jsonStream, value);
                }
            }

            compressed?.Dispose();

            return new MemoryStream(returnStream.ToArray());
        }

        public static T Deserialize<T>(this byte[] bytes)
        {
            var newMemoryStream = new MemoryStream(bytes);

            using (var text = new StreamReader(newMemoryStream))
            {
                using (var jsonStream = new JsonTextReader(text))
                {
                    var serializer = new JsonSerializer();

                    return serializer.Deserialize<T>(jsonStream);
                }
            }
        }


        public static T DeserializeGzip<T>(this byte[] bytes)
        {
            var newMemoryStream = new MemoryStream(bytes);

            using (var gzipStream = new GZipStream(newMemoryStream, CompressionMode.Decompress))
            {
                using (var text = new StreamReader(gzipStream))
                {
                    using (var jsonStream = new JsonTextReader(text))
                    {
                        var serializer = new JsonSerializer();

                        return serializer.Deserialize<T>(jsonStream);
                    }
                }
            }
        }

        public static T DeserializeFromMemoryStream<T>(this MemoryStream stream)
        {
            var newMemoryStream = new MemoryStream(stream.ToArray());

            using (var text = new StreamReader(newMemoryStream))
            {
                using (var jsonStream = new JsonTextReader(text))
                {
                    var serializer = new JsonSerializer();

                    return serializer.Deserialize<T>(jsonStream);
                }
            }
        }
    }
}
