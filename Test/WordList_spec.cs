using FluentAssertions;
using WordleSolver.Core;
using Xunit;

namespace WordleSolver.Test
{
    public class WordList_spec
    {
        private readonly WordList _wordList;

        public WordList_spec()
        {
            _wordList = new WordList();
        }

        [Fact]
        public void Load_a_file_with_frequencies()
        {
            _wordList.Load(@"words.txt", true);
            _wordList.Should().HaveCount(978);
            _wordList.Should().AllSatisfy(x =>
                {
                    x.Value.Should().HaveLength(5);
                    x.Frequency.Should().BeGreaterOrEqualTo(1);
                }
            );
        }

        [Fact]
        public void Load_a_file_without_frequencies()
        {
            _wordList.Load(@"wordlewords.txt", false);
            _wordList.Should().HaveCount(12974);
            _wordList.Should().AllSatisfy(x =>
                {
                    x.Value.Should().HaveLength(5);
                    x.Frequency.Should().Be(0);
                }
            );
        }

        [Fact]
        public void Words_are_not_duplicated()
        {
            _wordList.Add(new Word("Test", 0));
            _wordList.Add(new Word("Test", 1));
            _wordList.Should().HaveCount(1);
        }
    }
}