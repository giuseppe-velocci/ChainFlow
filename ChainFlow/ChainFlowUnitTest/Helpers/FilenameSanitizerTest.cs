using ChainFlow.Helpers;
using FluentAssertions;

namespace ChainFlowUnitTest.Helpers
{
    public class FilenameSanitizerTest
    {
        [Theory]
        [InlineData("!@#$%^&*()=+/\\[]{}:;',.<>?`~\"", "_")]
        [InlineData("Almost.Valid", "Almost_Valid")]
        [InlineData("../../secretfile.txt", "secretfile_txt")]
        [InlineData("A-regular-Filename", "A-regular-Filename")]
        public void Sanitize_WhenInput_HasNonAllowedChars_ReturnsSanitizedString(string filename, string expected)
        {
            FilenameSanitizer.Sanitize(filename).Should().Be(expected);
        }

        [Theory]
        [InlineData("LoremipsumdolorsitametconsecteturadipiscingelitNullampulvinarpellentesquemivelvolutpatUtsitameturnaloremSuspendissecursusveliteuvelitposuerenecbibendumdoloriaculisEtiamacsapienacnuncfaucibusullamcorperAeneanelementumligulautsemblanditmalesuadaCrasegetvelitegetvelittincidunttempuseuvelnequeVivamusvelmiquisfelisvariusultriciesInhachabitasseplateadictumstMaecenasquisfermentumodioMauriseuluctusodioeusodalesnullaVestibulumidmietenimdapibusfacilisisQuisqueegetpurusacelitlaciniacongueIntegerfacilisisvestibulumtortorutelementumtellusultricieseu", "LoremipsumdolorsitametconsecteturadipiscingelitNullampulvinarpel")]
        [InlineData("LoremipsumdolorsitametconsecteturadipiscingelitNullampulvinarpellentesquemivelvo.exe", "LoremipsumdolorsitametconsecteturadipiscingelitNullampulvinarpel")]
        public void Sanitize_WhenInputIsTooLong_ReturnsTruncatedString(string filename, string expected)
        {
            string result = FilenameSanitizer.Sanitize(filename);
            result.Should().Be(expected);
            result.Length.Should().Be(expected.Length);
        }
    }
}
