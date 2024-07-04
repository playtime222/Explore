using Explore;

namespace ExploreTests;

public class ContractedTests
{
    [Fact]
    public void Test1()
    {
        Assert.Throws<ArgumentNullException>(() => new Contracted().DoSomething(null));
    }

    [Fact]
    public void Test2()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Contracted().DoSomething(""));
    }

    [Fact]
    public void Test3()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Contracted().DoSomething(" "));
    }

    [Fact]
    public void Test4()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Contracted().DoSomething(1));
    }

    [Fact]
    public void Test5()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Contracted().DoSomething(6));
    }

    [Fact]
    public void Test6()
    {
        new Contracted().DoSomething(3);
    }
}