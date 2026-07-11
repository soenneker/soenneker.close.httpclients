using Soenneker.Close.HttpClients.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Close.HttpClients.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class CloseOpenApiHttpClientTests : HostedUnitTest
{
    private readonly ICloseOpenApiHttpClient _httpclient;

    public CloseOpenApiHttpClientTests(Host host) : base(host)
    {
        _httpclient = Resolve<ICloseOpenApiHttpClient>(true);
    }

    [Test]
    public void Default()
    {

    }
}
