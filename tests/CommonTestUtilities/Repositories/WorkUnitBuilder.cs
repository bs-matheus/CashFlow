using CashFlow.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;

public static class WorkUnitBuilder
{
    public static IWorkUnit Build()
    {
        var mock = new Mock<IWorkUnit>();

        return mock.Object;
    }
}
