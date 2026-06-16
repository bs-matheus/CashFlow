using CashFlow.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography;

public class PasswordEncrypterBuilder
{
    private readonly Mock<IPasswordEncrypter> _mock;

    public PasswordEncrypterBuilder()
    {
        _mock = new Mock<IPasswordEncrypter>();

        _mock.Setup(encrypter => encrypter.Encrypt(It.IsAny<string>())).Returns("&fefwe*22vhe");
    }

    public PasswordEncrypterBuilder Verify(string? password)
    {
        if (!string.IsNullOrEmpty(password))
        {
            _mock.Setup(encrypter => encrypter.Verify(password, It.IsAny<string>())).Returns(true);
        }
        return this;
    }

    public IPasswordEncrypter Build()
    {
        return _mock.Object;
    }
}
