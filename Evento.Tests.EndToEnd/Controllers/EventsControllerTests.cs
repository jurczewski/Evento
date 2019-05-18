using Evento.Infrastructure.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Evento.Tests.EndToEnd.Controllers
{
    public class EventsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public EventsControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        //[InlineData("/events")]
        public async Task Fetching_events_should_return_not_empty_collection()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("events");
            var content = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<IEnumerable<EventDTO>>(content);

            // Assert
            response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.OK);
            events.Should().NotBeEmpty();
        }       
    }
}