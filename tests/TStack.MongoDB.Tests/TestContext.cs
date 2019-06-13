using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TStack.MongoDB.Tests
{
    public class TestContext
    {
        private readonly TestServer _server;
        public HttpClient Client { get; private set; }

        public TestContext()
        {
            new ReadEnvironment();
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            Client = _server.CreateClient();
        }
    }
}
