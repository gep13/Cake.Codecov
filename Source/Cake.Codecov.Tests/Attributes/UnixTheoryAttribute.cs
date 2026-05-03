using System.Runtime.InteropServices;
using Xunit;

namespace Cake.Codecov.Tests.Attributes
{
    public sealed class UnixTheoryAttribute : TheoryAttribute
    {
        public UnixTheoryAttribute(string? reason = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Skip = reason ?? "non-windows test";
            }
        }
    }
}
