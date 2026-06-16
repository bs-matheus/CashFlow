using CashFlow.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography;

public static class PasswordEncrypterBuilder
{
    public static IPasswordEncrypter Build()
    {
        var mock = new Mock<IPasswordEncrypter>();

        mock.Setup(x => x.Encrypt(It.IsAny<string>()))
            .Returns("&fefwe*22vhe");

        return mock.Object;
    }
}
