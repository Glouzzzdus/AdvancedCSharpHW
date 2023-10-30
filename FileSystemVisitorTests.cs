using AdvancedCSharp;
using NSubstitute;

namespace AdvancedCSharp
{
    [TestClass]
    public class FileSystemVisitorTests
    {
        private IDirectoryRepository _mockRepo;
        private FileSystemVisitor _visitor;
        private FileInfo _mockFile;
        private DirectoryInfo _mockDirectory;

        [TestInitialize]
        public void SetUp()
        {
            _mockRepo = Substitute.For<IDirectoryRepository>();
            _mockFile = new FileInfo("mockFile.txt");
            _mockDirectory = new DirectoryInfo("mockDirectory");

            _mockRepo.GetFileInfos(Arg.Any<string>()).Returns(new List<FileInfo>());
            _mockRepo.GetDirectoryInfos(Arg.Any<string>()).Returns(new List<DirectoryInfo>());

            _visitor = new FileSystemVisitor(f => true, _mockRepo);
        }

        [TestMethod]
        public void Traverse_ShouldTriggerStartEvent()
        {
            bool isStartTriggered = false;
            _visitor.Start += (sender, args) => isStartTriggered = true;

            _visitor.Traverse("SomePath");

            Assert.IsTrue(isStartTriggered);
        }

        [TestMethod]
        public void Traverse_ShouldTriggerFinishEvent()
        {
            bool isFinishTriggered = false;
            _visitor.Finish += (sender, args) => isFinishTriggered = true;

            _visitor.Traverse("SomePath");

            Assert.IsTrue(isFinishTriggered);
        }
        [TestMethod]
        public void Traverse_ShouldFilterOutFilesBasedOnFilter()
        {
            _visitor = new FileSystemVisitor(f => false, _mockRepo); ;

            var results = _visitor.Traverse("SomePath").ToList();

            Assert.IsFalse(results.Contains(_mockFile));
        }


        [TestMethod]
        public void Traverse_ShouldTriggerFileFoundEvent_WhenFileIsFound()
        {
            _mockRepo.GetFileInfos(Arg.Any<string>()).Returns(new List<FileInfo> { _mockFile });

            bool isFileFoundTriggered = false;
            _visitor.FileFound += (sender, args) => isFileFoundTriggered = true;

            _visitor.Traverse("SomePath");

            Assert.IsTrue(isFileFoundTriggered);
        }

        [TestMethod]
        public void Traverse_ShouldTriggerFilteredFileFoundEvent_WhenFilteredFileIsFound()
        {
            _mockRepo.GetFileInfos(Arg.Any<string>()).Returns(new List<FileInfo> { _mockFile });

            bool isFilteredFileFoundTriggered = false;
            _visitor.FilteredFileFound += (sender, args) => isFilteredFileFoundTriggered = true;

            _visitor.Traverse("SomePath");

            Assert.IsTrue(isFilteredFileFoundTriggered);
        }
    }
}