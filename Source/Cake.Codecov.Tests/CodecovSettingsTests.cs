using System;
using Cake.Core.IO;
using FluentAssertions;
using Xunit;

namespace Cake.Codecov.Tests
{
    public sealed class CodecovSettingsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Remove_Empty_String_Value(string? value)
        {
            // Given
            var settings = new CodecovSettings
            {
                Tag = "v1.1.0"
            };

            // When
            settings.Tag = value!;

            // Then
            settings.Tag.Should().BeNull();
        }

        [Fact]
        public void Should_Set_Branch_Value()
        {
            // Given
            const string expected = "next";
            var settings = new CodecovSettings
            {
                // When
                Branch = expected
            };

            // Then
            settings.Branch.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Build_Value()
        {
            // Given
            const string expected = "543";
            var settings = new CodecovSettings
            {
                // When
                Build = expected
            };

            // Then
            settings.Build.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Clean_Value()
        {
            // Given
            var settings = new CodecovSettings
            {
                // When
                CleanReports = true
            };

            // Then
            settings.CleanReports.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Commit_Value()
        {
            // Given
            const string expected = "02eecc9564a8f89ddf13cf63b615cf08adee23cc";
            var settings = new CodecovSettings
            {
                // When
                Commit = expected
            };

            // Then
            settings.Commit.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_DryRun_Value()
        {
            // Given
            var settings = new CodecovSettings
            {
                // When
                DryRun = true
            };

            // Then
            settings.DryRun.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_EnableGcovSupport_Value()
        {
            // Given
            var settings = new CodecovSettings
            {
                // When
                EnableGcovSupport = true
            };

            // Then
            settings.EnableGcovSupport.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Envs_Value()
        {
            // Given
            var expected = new[] { "APPVEYOR_IMAGE", "TESTING" };
            var settings = new CodecovSettings
            {
                // When
                Envs = expected
            };

            // Then
            settings.Envs.Should().HaveCount(expected.Length).And.BeSameAs(expected);
        }

        [Fact]
        public void Should_Set_Feature_Value()
        {
            // Given
            var expected = new[] { "s3" };
            var settings = new CodecovSettings
            {
                // When
                Features = expected
            };

            // Then
            settings.Features.Should().HaveCount(expected.Length).And.BeSameAs(expected);
        }

        [Fact]
        public void Should_Set_Files_Value()
        {
            // Given
            var expected = new[] { "file1.txt", "file2.xml" };
            var settings = new CodecovSettings
            {
                // When
                Files = expected
            };

            // Then
            settings.Files.Should().HaveCount(expected.Length).And.BeSameAs(expected);
        }

        [Fact]
        public void Should_Set_Flags_Value()
        {
            // Given
            const string expected = "Integration";
            var settings = new CodecovSettings
            {
                // When
                Flags = expected
            };

            // Then
            settings.Flags.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Name_Value()
        {
            // Given
            const string expected = "Some Name";
            var settings = new CodecovSettings
            {
                // When
                Name = expected
            };

            // Then
            settings.Name.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_NonZero_Value()
        {
            // Given
            var settings = new CodecovSettings
            {
                // When
                NonZero = true
            };

            // Then
            settings.NonZero.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_ParentSha_Value()
        {
            // Given
            const string expected = "some-sha";
            var settings = new CodecovSettings
            {
                // When
                ParentSha = expected
            };

            // Then
            settings.ParentSha.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Pr_Value()
        {
            // Given
            const string expected = "512";
            var settings = new CodecovSettings
            {
                // When
                Pr = expected
            };

            // Then
            settings.Pr.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_RootDirectory_Value()
        {
            // Given
            var expected = (DirectoryPath)"root/other";
            var settings = new CodecovSettings
            {
                // When
                RootDirectory = expected
            };

            // Then
            settings.RootDirectory.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Set_SearchDirectory_Value()
        {
            // Given
            var expected = (DirectoryPath)"some-search-path";
            var settings = new CodecovSettings
            {
                // When
                SearchDirectory = expected
            };

            // Then
            settings.SearchDirectory.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Set_ShowChangelog_Value()
        {
            // Given
            var settings = new CodecovSettings
            {
                // When
                ShowChangelog = true
            };

            // Then
            settings.ShowChangelog.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Slug_Value()
        {
            // Given
            const string expected = "my-test-slug";
            var settings = new CodecovSettings
            {
                // When
                Slug = expected
            };

            // Then
            settings.Slug.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Tag_Value()
        {
            // Given
            const string expected = "v1.0.0";
            var settings = new CodecovSettings
            {
                // When
                Tag = expected
            };

            // Then
            settings.Tag.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Token_Value()
        {
            // Given
            var expected = Guid.NewGuid().ToString();
            var settings = new CodecovSettings
            {
                // When
                Token = expected
            };

            // Then
            settings.Token.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Url_Value()
        {
            // Given
            var expected = new Uri("https://localhost.com/test/server");
            var settings = new CodecovSettings
            {
                // When
                Url = expected
            };

            // Then
            settings.Url.Should().Be(expected);
        }

        [Fact]
        public void Should_Set_Verbose_Value()
        {
            // Given
            var settings = new CodecovSettings
            {
                // When
                Verbose = true,
            };

            // Then
            settings.Verbose.Should().BeTrue();
        }
    }
}
