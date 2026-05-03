using System.Runtime.InteropServices;
using Cake.Codecov.Internals;
using Cake.Testing.Fixtures;

namespace Cake.Codecov.Tests
{
    internal class CodecovRunnerFixture : ToolFixture<CodecovSettings>
    {
        private readonly IPlatformDetector? _platformDetector;

        public CodecovRunnerFixture()
            : base(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "codecov.exe" : "codecov")
        {
        }

        public CodecovRunnerFixture(IPlatformDetector platformDetector, string expectedExecutable)
            : base(expectedExecutable)
        {
            _platformDetector = platformDetector;
        }

        protected override void RunTool()
        {
            var tool = _platformDetector != null
                ? new CodecovRunner(_platformDetector, FileSystem, Environment, ProcessRunner, Tools)
                : new CodecovRunner(FileSystem, Environment, ProcessRunner, Tools);
            tool.Run(Settings);
        }
    }
}
