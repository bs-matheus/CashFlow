using CashFlow.Domain.Repositories.Users;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UsersReadOnlyRepositoryBuilder
{
    private readonly Mock<IUsersReadOnlyRepository> _repository;

    public UsersReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUsersReadOnlyRepository>();
    }

    public void ExistActiveUserWithEmail(string existentEmail)
    {
        _repository.Setup(x => x.ExistActiveUserWithEmailAsync(existentEmail)).ReturnsAsync(true);
    }

    public IUsersReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}
