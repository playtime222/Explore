using Explore;
using Explore.Deeper;

namespace ExploreTests;

public class UncontractedTests
{
    [Fact]
    public void Test1()
    {
        Assert.Throws<ArgumentNullException>(() => new Uncontracted().DoSomething(null));
    }

    [Fact]
    public void Test2()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Uncontracted().DoSomething(""));
    }

    [Fact]
    public void Test3()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Uncontracted().DoSomething(" "));
    }
}