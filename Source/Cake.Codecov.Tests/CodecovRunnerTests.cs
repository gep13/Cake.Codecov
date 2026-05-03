using System;
using Cake.Codecov.Internals;
using Cake.Codecov.Tests.Attributes;
using Cake.Core;
using Cake.Testing;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cake.Codecov.Tests
{
    public sealed class CodecovRunnerTests
    {
        [Fact]
        public void Should_Throw_If_Settings_Are_Null()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = null! };

            // When
            Action result = () => fixture.Run();

            // Then
            result.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("settings");
        }

        [Fact]
        public void Should_Throw_If_Codecov_Executable_Was_Not_Found()
        {
            // Given
            var fixture = new CodecovRunnerFixture();
            fixture.GivenDefaultToolDoNotExist();

            // When
            Action result = () => fixture.Run();

            // Then
            result.Should().Throw<CakeException>().WithMessage("Codecov: Could not locate executable.");
        }

        [Theory]
        [InlineData("/bin/tools/Codecov/codecov.exe", "/bin/tools/Codecov/codecov.exe")]
        [InlineData("./tools/Codecov/codecov.exe", "/Working/tools/Codecov/codecov.exe")]
        public void Should_Use_Codecov_Runner_From_Tool_Path_If_Provided(string toolPath, string expected)
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { ToolPath = toolPath } };
            fixture.GivenSettingsToolPathExist();

            // When
            var result = fixture.Run();

            // Then
            result.Path.FullPath.Should().Be(expected);
        }

        [WindowsTheory]
        [InlineData("C:/Codecov/codecov.exe", "C:/Codecov/codecov.exe")]
        [InlineData("C:\\Codecov\\codecov.exe", "C:/Codecov/codecov.exe")]
        public void Should_Use_Codecov_Runner_From_Tool_Path_If_Provided_On_Windows(string toolPath, string expected)
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { ToolPath = toolPath } };
            fixture.GivenSettingsToolPathExist();

            // When
            var result = fixture.Run();

            // Then
            result.Path.FullPath.Should().Be(expected);
        }

        [UnixTheory]
        [InlineData("/usr/bin/codecov", "/usr/bin/codecov")]
        public void Should_Use_Codecov_Runner_From_Tool_Path_If_Provided_On_Unix(string toolPath, string expected)
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { ToolPath = toolPath } };
            fixture.GivenSettingsToolPathExist();

            // When
            var result = fixture.Run();

            // Then
            result.Path.FullPath.Should().Be(expected);
        }

        [Fact]
        public void Should_Find_Codecov_Runner_If_Tool_Path_Not_Provided_On_Windows()
        {
            // Given
            var mock = new Mock<IPlatformDetector>(MockBehavior.Strict);
            mock.Setup(m => m.IsLinuxPlatform()).Returns(false);
            mock.Setup(m => m.IsOsxPlatform()).Returns(false);

            var fixture = new CodecovRunnerFixture(mock.Object, "codecov.exe");

            // When
            var result = fixture.Run();

            // Then
            result.Path.FullPath.Should().Be("/Working/tools/codecov.exe");

            mock.Verify(m => m.IsLinuxPlatform(), Times.Once());
            mock.Verify(m => m.IsOsxPlatform(), Times.Once());
        }

        [Theory]
        [InlineData("linux-x64/codecov")]
        [InlineData("codecov-linux")]
        [InlineData("codecov")]
        public void Should_Find_Codecov_Runner_If_Tool_Path_Not_Provided_On_Linux(string path)
        {
            // Given
            var mock = new Mock<IPlatformDetector>(MockBehavior.Strict);
            mock.Setup(m => m.IsLinuxPlatform()).Returns(true);
            mock.Setup(m => m.IsOsxPlatform()).Returns(false);

            var fixture = new CodecovRunnerFixture(mock.Object, path);

            // When
            var result = fixture.Run();

            // Then
            result.Path.FullPath.Should().Be("/Working/tools/" + path);

            mock.Verify(m => m.IsLinuxPlatform(), Times.Once());
            mock.Verify(m => m.IsOsxPlatform(), Times.Never());
        }

        [Theory]
        [InlineData("linux-x64/codecov")]
        [InlineData("codecov-macos")]
        [InlineData("codecov")]
        public void Should_Find_Codecov_Runner_If_Tool_Path_Not_Provided_On_osx(string path)
        {
            // Given
            var mock = new Mock<IPlatformDetector>(MockBehavior.Strict);
            mock.Setup(m => m.IsLinuxPlatform()).Returns(false);
            mock.Setup(m => m.IsOsxPlatform()).Returns(true);

            var fixture = new CodecovRunnerFixture(mock.Object, path);

            // When
            var result = fixture.Run();

            // Then
            result.Path.FullPath.Should().Be("/Working/tools/" + path);

            mock.Verify(m => m.IsLinuxPlatform(), Times.Once());
            mock.Verify(m => m.IsOsxPlatform(), Times.Once());
        }

        [Fact]
        public void Should_Set_Working_Directory()
        {
            // Given
            var fixture = new CodecovRunnerFixture();

            // When
            var result = fixture.Run();

            // Then
            result.Process.WorkingDirectory.FullPath.Should().Be("/Working");
        }

        [Fact]
        public void Should_Throw_If_Process_Was_Not_Started()
        {
            // Given
            var fixture = new CodecovRunnerFixture();
            fixture.GivenProcessCannotStart();

            // When
            Action result = () => fixture.Run();

            // Then
            result.Should().Throw<CakeException>().WithMessage("Codecov: Process was not started.");
        }

        [Fact]
        public void Should_Throw_If_Process_Has_A_Non_Zero_Exit_Code()
        {
            // Given
            var fixture = new CodecovRunnerFixture();
            fixture.GivenProcessExitsWithCode(1);

            // When
            Action result = () => fixture.Run();

            // Then
            result.Should().Throw<CakeException>().WithMessage("Codecov: Process returned an error (exit code 1).");
        }

        [Fact]
        public void Should_Set_Branch()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Branch = "develop" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--branch ""develop""");
        }

        [Fact]
        public void Should_Set_Build()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Build = "1" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--build ""1""");
        }

        [Fact]
        public void Should_EnableClean()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { CleanReports = true } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--clean");
        }

        [Fact]
        public void Should_Not_Enable_Clean()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { CleanReports = false } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Should_Set_Commit()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Commit = "603e02d40093d0649cfa787d846ae4ccc038085c" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--sha ""603e02d40093d0649cfa787d846ae4ccc038085c""");
        }

        [Fact]
        public void Should_Enable_DryRun()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { DryRun = true } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be("--dryRun");
        }

        [Fact]
        public void Should_Set_Envs()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Envs = new[] { "env1", "env2" } } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--env ""env1"" ""env2""");
        }

        [Fact]
        public void Should_Set_Features()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Features = new[] { "s3" } } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be("--feature \"s3\"");
        }

        [Fact]
        public void Should_Set_Files()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Files = new[] { "file1.xml", "file2.xml" } } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--file ""file1.xml"",""file2.xml""");
        }

        [Fact]
        public void Should_Normalize_File_Paths()
        {
            var fixture = new CodecovRunnerFixture
            {
                Settings =
                {
                    Files = new[]
                    {
                        string.Format(".{0}path{0}to{0}file.txt", System.IO.Path.AltDirectorySeparatorChar)
                    }
                }
            };

            var expectedPaths = System.IO.Path.Combine(
                ".",
                "path",
                "to",
                "file.txt");

            var result = fixture.Run();

            result.Args.Should().Be($"--file \"{expectedPaths}\"");
        }

        [Fact]
        public void Should_Set_Flags()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Flags = "ut,chrome" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--flag ""ut,chrome""");
        }

        [Fact]
        public void Should_Enable_GCov()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { EnableGcovSupport = true } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--gcov");
        }

        [Fact]
        public void Should_Not_Enable_GCov()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { EnableGcovSupport = false } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Should_Set_Name()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Name = "custom name" } };

            // When
            var result = fixture.Run();

            // ThenRæ
            result.Args.Should().Be(@"--name ""custom name""");
        }

        [Fact]
        public void Should_Set_Pr()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Pr = "1" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--pr ""1""");
        }

        [Fact]
        public void Should_Enable_NonZero()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { NonZero = true } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be("--nonZero");
        }

        [Fact]
        public void Should_Sete_ParentSha()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { ParentSha = "some-kind-of-sha" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--parent ""some-kind-of-sha""");
        }

        [Fact]
        public void Should_Set_RootDirectory()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { RootDirectory = @".\working" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--rootDir ""working""");
        }

        [Fact]
        public void Should_Set_SearchDirectory()
        {
            var directory = Environment.CurrentDirectory.Replace("\\", "/");

            // Given
            var fixture = new CodecovRunnerFixture { Settings = { SearchDirectory = Environment.CurrentDirectory } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@$"--dir ""{directory}""");
        }

        [Fact]
        public void Should_Set_Slug()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Slug = "owner/repo" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--slug ""owner/repo""");
        }

        [Fact]
        public void Should_Enable_Changelog()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { ShowChangelog = true } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be("--changelog");
        }

        [Fact]
        public void Should_Not_Enable_Changelog()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { ShowChangelog = false } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Should_Set_Tag()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Tag = "v1.0.0" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--tag ""v1.0.0""");
        }

        [Fact]
        public void Should_Set_Token()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Token = "00000000-0000-0000-0000-000000000000" } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--token ""00000000-0000-0000-0000-000000000000""");
        }

        [Fact]
        public void Should_Set_Url()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Url = new Uri("https://my-hosted-codecov.com") } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be(@"--url ""https://my-hosted-codecov.com/""");
        }

        [Fact]
        public void Should_Enable_Verbose()
        {
            // Given
            var fixture = new CodecovRunnerFixture { Settings = { Verbose = true } };

            // When
            var result = fixture.Run();

            // Then
            result.Args.Should().Be("--verbose");
        }
    }
}
