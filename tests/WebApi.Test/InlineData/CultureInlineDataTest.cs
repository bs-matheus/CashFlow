using System.Collections;

namespace WebApi.Test.InlineData;

public class CultureInlineDataTest : IEnumerable<object[]>
{
    private static readonly string[] s_cultures =
    [
        "en",
        "fr",
        "pt-BR",
        "pt-PT"
    ];

    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (string culture in s_cultures)
        {
            yield return new object[] { culture };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
