namespace BuildingBlocks.Primitives.IO;

public static class StreamExtensions
{
    extension(Stream stream)
    {
        public async Task<byte[]> ToByteArrayAsync(CancellationToken ct = default)
        {
            if (stream is MemoryStream ms)
                return ms.ToArray();

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, ct);
            return memoryStream.ToArray();
        }

        public async Task<string> ToStringAsync(System.Text.Encoding? encoding = null, CancellationToken ct = default)
        {
            encoding ??= System.Text.Encoding.UTF8;

            if (stream.CanSeek)
                stream.Position = 0;

            using var reader = new StreamReader(stream, encoding, leaveOpen: true);
            return await reader.ReadToEndAsync(ct);
        }
    }
}
