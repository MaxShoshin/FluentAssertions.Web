using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace FluentAssertions.Web.Internal.ContentProcessors
{
    internal class JsonProcessor : ProcessorBase
    {
        private static readonly JavaScriptEncoder JavaScriptEncoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        private readonly HttpContent? _httpContent;
        public JsonProcessor(HttpContent? httpContent)
        {
            _httpContent = httpContent;
        }

        protected override async Task Handle(StringBuilder contentBuilder)
        {
            var content = await _httpContent!.ReadAsStreamAsync();

            JsonDocument? jsonDocument = null;
            try
            {
                if (content != null)
                {
                    jsonDocument = await JsonDocument.ParseAsync(content, new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true
                    });
                }
            }
            catch
            {
                // ignored
            }

            if (jsonDocument != null)
            {
                // write the JsonDocument into a MemoryStream as indented
                using var stream = new MemoryStream();
                var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                {
                    Indented = true,
                    Encoder = JavaScriptEncoder,
                });
                jsonDocument.WriteTo(writer);
                await writer.FlushAsync();

                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                // get the string Json from the MemoryStream
                using var streamReader = new StreamReader(stream, Encoding.UTF8);

                var jsonString = await streamReader.ReadToEndAsync();

                contentBuilder.Append(jsonString);
            }
            else
            {
                // append the content as it is if it cannot be parsed as a json
                contentBuilder.Append(await _httpContent.ReadAsStringAsync());
            }
        }

        protected override bool CanHandle()
        {
            if (_httpContent == null || _httpContent.IsDisposed())
            {
                return false;
            }

            _httpContent.TryGetContentLength(out long length);
            _httpContent.TryGetContentTypeMediaType(out var mediaType);

            return length <= ContentFormatterOptions.MaximumReadableBytes
                   && (mediaType.EqualsCaseInsensitive("application/json") || mediaType.EqualsCaseInsensitive("application/problem+json"));
        }
    }
}
