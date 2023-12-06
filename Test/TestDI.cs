using GHelpers;

namespace Test
{
    interface ITestDI
    {

    }

    [DIHelper(DIScope.Singleton, typeof(ITestDI), "test")]
    internal class TestDI
    {
    }
}

