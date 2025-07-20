using Xunit;
using Moq;
using Dotnet.Script.Core.Interactive;
using System.Threading.Tasks;
using System.IO;

namespace Dotnet.Script.Core.Interactive.Tests
{
    public class InteractiveRunnerTests
    {
        private readonly Mock<IScriptRunner> _mockScriptRunner;
        private readonly Mock<IInteractiveCommandProvider> _mockCommandProvider;
        private readonly Mock<TextReader> _