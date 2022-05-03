using FluentAssertions;
using System.Linq;
using WordleSolver.Core;
using Xunit;

namespace WordleSolver.Test
{
    public class NextWordGuesser_spec
    {
        private readonly WordList _wordList;
        private readonly NextWordGuesser _guesser;

        public NextWordGuesser_spec()
        {
            _wordList = new WordList();
            _wordList.Load("wordlewords.txt");
            _guesser = new NextWordGuesser();
        }

        [Theory()]
        [InlineData("testy", "-GGGG", new string[]{"zesty","pesty"})]
        [InlineData("would", "-----", new string[]{"their","first","after"})]
        [InlineData("tales", "YYYYY", new string[]{"slate", "stale", "steal", "least" })]
        public void Guess_returns_valid_words(string guess, string mask, string[] validWords)
        {
            _guesser.AddGuess(new Guess(guess, mask.ToMaskColours()));
            var words = _guesser.GetValidWords(_wordList);
            words.Select(w => w.Value).Should().Contain(validWords);
        }

        [Theory()]
        [InlineData("acres", "YGYYY", "scare")]
        [InlineData("there", "GGYYG", "three")]
        public void Guess_only_returns_single_valid_word(string guess, string mask, string onlyValidWord)
        {
            _guesser.AddGuess(new Guess(guess, mask.ToMaskColours()));
            var words = _guesser.GetValidWords(_wordList);
            words.Select(w => w.Value).Should().ContainSingle(onlyValidWord);
        }

        [Fact]
        public void Guesses_stack_up_to_give_single_valid_word()
        {
            _guesser.AddGuess(new Guess("dreck", "  yy ".ToMaskColours()));
            _guesser.GetValidWords(_wordList).Select(w => w.Value).Should().Contain("uncle");
            _guesser.AddGuess(new Guess("uncle", "  g y".ToMaskColours()));
            _guesser.GetValidWords(_wordList).Select(w => w.Value).Should().Contain("facet");
            _guesser.AddGuess(new Guess("facet", "gggg ".ToMaskColours()));
            _guesser.GetValidWords(_wordList).Select(w => w.Value).Should().ContainSingle("faces");
        }
    }
}
