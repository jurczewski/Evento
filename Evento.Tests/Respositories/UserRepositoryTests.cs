using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Evento.Tests.Respositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task When_adding_new_user_it_should_be_added_correcty_to_the_list()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "user", "test", "test@test.com", "secret");
            IUserRepository repository = new UserRepository();
            //Act
            await repository.AddAsync(user);

            //Assert
            var existingUser = await repository.GetAsync(user.Id);
            Assert.Equal(user, existingUser);
        }
    }
}
