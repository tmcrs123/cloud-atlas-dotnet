using Cloud_Atlas_Dotnet.Application.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud_Atlas_Dotnet_Tests
{
    public class CorrelationIdMiddlewareTests
    {

        [Fact]
        public async Task It_AddsCorrelationIdToHeaders()
        {
            //arrange
            var context = new DefaultHttpContext();
            
            RequestDelegate next = (context) =>
            {
                return Task.CompletedTask;
            };

            var middleware = new CorrelationIdMiddleware(next);

            //act
            await middleware.InvokeAsync(context);

            var correlationId = context.Request.Headers["CorrelationId"];

            Assert.False(string.IsNullOrEmpty(correlationId));
        }
    }
}
