using BuildingBlocks.Primitives.IO;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.IO;

public class FileHelperTests
{
    [Theory]
    [InlineData("normal.txt", "normal.txt")]
    [InlineData("my file.txt", "my_file.txt")]
    [InlineData("hello@world!.pdf", "hello_world.pdf")]
    [InlineData("résumé.docx", "r_sum.docx")]
    [InlineData("...hidden", "file.hidden")]
    [InlineData("___leading.txt", "leading.txt")]
    public void SanitizeFileName_CleansInvalidCharacters(string input, string expected)
    {
        FileHelper.SanitizeFileName(input).Should().Be(expected);
    }

    [Fact]
    public void SanitizeFileName_AllInvalidChars_ReturnsFallback()
    {
        FileHelper.SanitizeFileName("@#$.txt").Should().Be("file.txt");
    }

    [Fact]
    public void SanitizeFileName_PreservesExtension()
    {
        FileHelper.SanitizeFileName("my file.tar.gz").Should().EndWith(".gz");
    }

    [Theory]
    [InlineData(".pdf", "application/pdf")]
    [InlineData(".json", "application/json")]
    [InlineData(".png", "image/png")]
    [InlineData(".jpg", "image/jpeg")]
    [InlineData(".html", "text/html")]
    [InlineData(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public void GetMimeType_KnownExtension_ReturnsCorrectType(string extension, string expected)
    {
        FileHelper.GetMimeType(extension).Should().Be(expected);
    }

    [Fact]
    public void GetMimeType_FromFileName_ReturnsCorrectType()
    {
        FileHelper.GetMimeType("document.pdf").Should().Be("application/pdf");
    }

    [Fact]
    public void GetMimeType_UnknownExtension_ReturnsOctetStream()
    {
        FileHelper.GetMimeType(".xyz").Should().Be("application/octet-stream");
    }

    [Fact]
    public void GetMimeType_CaseInsensitive()
    {
        FileHelper.GetMimeType(".PDF").Should().Be("application/pdf");
    }

    [Theory]
    [InlineData(0, "0 B")]
    [InlineData(500, "500 B")]
    [InlineData(1024, "1 KB")]
    [InlineData(1536, "1.5 KB")]
    [InlineData(1_048_576, "1 MB")]
    [InlineData(2_621_440, "2.5 MB")]
    [InlineData(1_073_741_824, "1 GB")]
    [InlineData(1_099_511_627_776, "1 TB")]
    public void FormatFileSize_ReturnsHumanReadable(long bytes, string expected)
    {
        FileHelper.FormatFileSize(bytes).Should().Be(expected);
    }

    [Fact]
    public void FormatFileSize_NegativeBytes_ThrowsArgumentOutOfRange()
    {
        var act = () => FileHelper.FormatFileSize(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
