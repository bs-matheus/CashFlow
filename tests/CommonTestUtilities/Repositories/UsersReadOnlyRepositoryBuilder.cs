using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Users;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UsersReadOnlyRepositoryBuilder
{
    private readonly Mock<IUsersReadOnlyRepository> _mock;

    public UsersReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IUsersReadOnlyRepository>();
    }

    public void ExistActiveUserWithEmail(string existentEmail)
    {
        _mock.Setup(repository => repository.ExistActiveUserWithEmailAsync(existentEmail)).ReturnsAsync(true);
    }

    public UsersReadOnlyRepositoryBuilder GetUserByEmail(User user)
    {
        _mock.Setup(repository => repository.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
        return this;
    }

    public IUsersReadOnlyRepository Build()
    {
        return _mock.Object;
    }
}
