namespace WordleSolver.Core
{
    /// <summary>
    /// Class to calculate a list of valid words that satisfy the criteria set
    /// by a set of guesses to a Wordle game (and the game's mask responses)
    /// </summary>
    public class NextWordGuesser
    {
        private ISet<(char, byte)> KnownLettersInKnownPositions { get; }
        private ISet<(char, byte)> KnownLettersInInvalidPositions { get; }
        private IDictionary<char, byte> KnownLetterExactCounts { get; }
        private IDictionary<char, byte> KnownLetterAtLeastCounts { get; }

        public IList<Guess> Guesses { get; }

        public NextWordGuesser()
        {
            KnownLettersInKnownPositions = new HashSet<(char, byte)>();
            KnownLettersInInvalidPositions = new HashSet<(char, byte)>();
            KnownLetterExactCounts = new Dictionary<char, byte>();
            KnownLetterAtLeastCounts = new Dictionary<char, byte>();
            Guesses = new List<Guess>();
        }

        /// <summary>
        /// Add a guess to the criteria set, to further reduce the set of valid words that could solve the game
        /// </summary>
        /// <param name="guess">The guess: consisting of the guessed word and the mask response</param>
        /// <exception cref="ArgumentOutOfRangeException">If the masked response contains any unhandled mask values</exception>
        public void AddGuess(Guess guess)
        {
            Guesses.Add(guess);

            //This will be useful for checking duplicate letters
            var guessCpy = guess.WordCharsWithoutGreys;

            for (byte i = 0; i < guess.Length; i++)
            {
                var c = guess.Word[i];
                KnownLetterAtLeastCounts[c] = (byte) guessCpy.Count(x => x == c);
                switch (guess.Mask[i])
                {
                    case MaskColour.Green:
                        KnownLettersInKnownPositions.Add((c, i));
                        break;
                    case MaskColour.Yellow:
                        KnownLettersInInvalidPositions.Add((c, i));
                        break;
                    case MaskColour.Grey:
                        if (guessCpy.Contains(c))
                        {
                            KnownLettersInInvalidPositions.Add((c, i));
                        }

                        //If this was grey but the letter appears elsewhere in the word as 
                        //yellow or green, then we know how many times the letter appears
                        //in the word
                        KnownLetterExactCounts[c] = (byte) guessCpy.Count(x => x == c);
                        KnownLetterAtLeastCounts.Remove(c);
                        break;
                    case MaskColour.Unset:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(guess));
                }
            }
        }

        /// <summary>
        /// Return the current set of valid words that could solve the target game,
        /// based on the guesses entered so far
        /// </summary>
        /// <param name="words">The list of words to start from, e.g. a dictionary</param>
        /// <returns>Those words that fit the criteria set by the guesses entered so far</returns>
        public IEnumerable<Word> GetValidWords(WordList words)
        {
            IEnumerable<Word> candidateWords = words;

            //Filter down to words that match our known letter counts
            // 0 = grey
            // >0 = grey, but the letter is known to be elsewhere in the word
            foreach (var (letter, count) in KnownLetterExactCounts)
            {
                candidateWords = candidateWords
                    .Where(w => w.Value.Count(x => x == letter) == count);
            }

            //Yellow/Green letters where we know they appear at least this many times
            foreach (var (letter, count) in KnownLetterAtLeastCounts)
            {
                candidateWords = candidateWords
                    .Where(w => w.Value.Count(x => x == letter) >= count);
            }

            //We only want words that have known letters in known positions (green)
            foreach (var (letter, pos) in KnownLettersInKnownPositions)
            {
                candidateWords = candidateWords
                    .Where(w => w.Value[pos] == letter);
            }

            //We know the letter but not the position:
            // - yellow, or
            // - grey, but the letter is known to be elsewhere in the word
            foreach (var (letter, pos) in KnownLettersInInvalidPositions)
            {
                //Make sure the letter is present,
                candidateWords = candidateWords.Where(w => w.Value.Contains(letter));
                //but not in the invalid position
                candidateWords = candidateWords
                    .Where(w => w.Value[pos] != letter);
            }

            //Prefer words with more unique letters, to maximise search space,
            //then prefer more common words
            return candidateWords
                .OrderBy(w => w.UniqueLetters)
                .ThenBy(w => w.Frequency)
                .ThenByDescending(w => w.Value);
        }
    }
}
