using AdvancedCSharp;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AdvancedCSharpHW.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var repository = Substitute.For<IDirectoryRepository>();

        repository.GetDirectoryInfos("dir1").Returns(new DirectoryInfo[] { new("dir1_1"), new("dir1_2") });
        repository.GetDirectoryInfos("dir1_1").Returns(Array.Empty<DirectoryInfo>());
        repository.GetDirectoryInfos("dir1_2").Returns(Array.Empty<DirectoryInfo>());
        repository.GetFileInfos("dir1").Returns(new FileInfo[] { new("file1_1.csv") });
        repository.GetFileInfos("dir1_2").Returns(new FileInfo[] { new("file1_2_1.csv"), new("file1_2_2.json") });
        repository.GetFileInfos("dir1_1").Returns(Array.Empty<FileInfo>());

        var visitor = new FileSystemVisitor(x => x.Name.EndsWith(".csv"), repository);

        // Act
        var result = visitor.Traverse("dir1");

        // Asert
        result.Count().Should().Be(2);
    }

    interface IFoo { }
}