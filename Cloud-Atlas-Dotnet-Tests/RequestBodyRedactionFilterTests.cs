using Castle.Core.Logging;
using Cloud_Atlas_Dotnet.Application.Filters;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Cloud_Atlas_Dotnet_Tests
{
    public class RequestBodyRedactionFilterTests
    {
        [Fact]
        public async void RequestBodyRedactionFilter_MasksSensitiveData()
        {
            var loggerMock = new Mock<ILogger<RequestBodyRedactionFilter>>();
            
            var webhostMock = new Mock<IWebHostEnvironment>();
            webhostMock.Setup(e => e.EnvironmentName).Returns(() => Environments.Production);

            var context = new DefaultHttpContext();
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();

            var actionContext = new ActionContext(context,routeData, actionDescriptor);

            var actionArguments = new Dictionary<string, object?>()
            {
                ["request"] = new User() { Password = "something secret" }
            };

            var filterList = new List<IFilterMetadata>();

            var actionExecutingContext = new ActionExecutingContext(actionContext, filterList, actionArguments, null);

            RequestBodyRedactionFilter filter = new RequestBodyRedactionFilter(loggerMock.Object, webhostMock.Object);

            filter.OnActionExecuting(actionExecutingContext);

            //lock at the logger mock log information

            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                0,
                It.Is<string>(v => v.ToString().Contains("*****")),
                null,
                null
                ));
        }
    }
}
