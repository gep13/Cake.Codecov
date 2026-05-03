using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using Moq;

namespace Cake.Codecov.Tests
{
    internal class CodecovAliasesFixture : CodecovRunnerFixture
    {
        private readonly ICakeContext _context;
        public IEnumerable<string>? Files { get; set; }
        public string? File { get; set; }
        public string? Token { get; set; }

        public CodecovAliasesFixture()
        {
            var argumentsMoq = new Mock<ICakeArguments>();
            var registryMoq = new Mock<IRegistry>();
            var dataService = new Mock<ICakeDataService>();
            _context = new CakeContext(
                FileSystem,
                Environment,
                Globber,
                new FakeLog(),
                argumentsMoq.Object,
                ProcessRunner,
                registryMoq.Object,
                Tools, dataService.Object,
                Configuration);
        }

        protected override void RunTool()
        {
            if (Settings != null)
            {
                _context.Codecov(Settings);
            }
            else if (!string.IsNullOrEmpty(File))
            {
                if (!string.IsNullOrEmpty(Token))
                {
                    _context.Codecov(File, Token);
                }
                else
                {
                    _context.Codecov(File);
                }
            }
            else if (Files?.Any() == true)
            {
                if (!string.IsNullOrEmpty(Token))
                {
                    _context.Codecov(Files, Token);
                }
                else
                {
                    _context.Codecov(Files);
                }
            }
        }
    }
}
