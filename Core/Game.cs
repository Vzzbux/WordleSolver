namespace WordleSolver.Core
{
    /// <summary>
    /// Class to simulate a Wordle game - pick a word for the player to deduce,
    /// then give correct responses to guesses against it.
    /// </summary>
    public class Game
    {
        private readonly WordList _wordList;
        public string Word { get; }
        public int Tries { get; private set; }
        public bool Won { get; private set; }

        public Game(WordList wordList, string? word = null)
        {
            _wordList = wordList;
            Word = word ?? _wordList.GetRandomWord().Value;
            Tries = 0;
        }

        /// <summary>
        /// Respond to a guess with the correct mask
        /// </summary>
        /// <param name="guess">The word to guess</param>
        /// <returns>The correct mask array for this guess</returns>
        /// <exception cref="ArgumentException">If the guess does not appear in the given wordlist</exception>
        public MaskColour[] Guess(string guess)
        {
            if (guess.Length != Word.Length) throw new ArgumentException($"Guess must be {Word.Length} letters");
            if (!_wordList.Select(w => w.Value).Contains(guess)) throw new ArgumentException($"{guess} is not a known word");
            
            Tries++;
            var mask = new MaskColour[guess.Length];
            var wordCopy = Word.ToCharArray();
            for (byte i = 0; i < guess.Length; i++)
            {
                if (guess[i] == wordCopy[i])
                {
                    mask[i] = MaskColour.Green;
                    wordCopy[i] = '-';
                }
            }
            for (byte i = 0; i < guess.Length; i++)
            {
                if (mask[i] == MaskColour.Green) continue;
                if (wordCopy.Contains(guess[i]))
                {
                    mask[i] = MaskColour.Yellow;
                    wordCopy[Array.IndexOf(wordCopy, guess[i])] = '-';
                }
                else
                {
                    mask[i] = MaskColour.Grey;
                }
            }

            Won = mask.All(m => m == MaskColour.Green);
            return mask;
        }
    }
}
