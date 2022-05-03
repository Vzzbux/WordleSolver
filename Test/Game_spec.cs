using System;
using FluentAssertions;
using WordleSolver.Core;
using Xunit;

namespace WordleSolver.Test
{
    public class Game_spec
    {

        private readonly WordList _wordList;
        private readonly Game _game;

        public Game_spec()
        {
            _wordList = new WordList();
            _wordList.Load("wordlewords.txt");
            _game = new Game(_wordList, "testy");
        }

        [Theory()]
        [InlineData("testy", "GGGGG")]
        [InlineData("testa", "GGGG-")]
        [InlineData("yetts", "YGYGY")]
        [InlineData("above", "----Y")]
        [InlineData("keeps", "-G--Y")]
        [InlineData("agree", "---Y-")]
        [InlineData("eerie", "-G---")]
        [InlineData("attic", "-YY--")]
        [InlineData("xylyl", "-Y---")]
        public void Guess_returns_correct_mask(string guess, string expectedMask)
        {
            var mask = new string(_game.Guess(guess).ToChars());
            mask.Should().Be(expectedMask);
        }

        [Fact]
        public void An_Exception_is_thrown_when_a_non_word_is_guessed()
        {
            Assert.Throws<ArgumentException>(() => _game.Guess("abcde"));
        }

        [Fact]
        public void An_Exception_is_thrown_when_a_4_letter_word_is_guessed()
        {
            Assert.Throws<ArgumentException>(() => _game.Guess("test"));
        }

        [Fact]
        public void An_Exception_is_thrown_when_a_6_letter_word_is_guessed()
        {
            Assert.Throws<ArgumentException>(() => _game.Guess("tester"));
        }

        [Fact]
        public void Incorrect_guess_doesnt_set_won()
        {
            _game.Guess("testa");
            _game.Won.Should().BeFalse();
        }

        [Fact]
        public void Correct_guess_sets_won()
        {
            _game.Guess("testy");
            _game.Won.Should().BeTrue();
        }

        [Fact]
        public void Tries_starts_at_zero()
        {
            _game.Tries.Should().Be(0);
        }

        [Fact]
        public void Guesses_increment_tries()
        {
            _game.Guess("testa");
            _game.Guess("teste");
            _game.Guess("tests");
            _game.Guess("testy");
            _game.Tries.Should().Be(4);
        }

    }
}
