namespace WordleSolver.Core
{
    /// <summary>
    /// Convenience class to represent a list of words of a constant length
    /// </summary>
    public class WordList : HashSet<Word>
    {
        public byte WordLength { get; }
        private readonly Random _random;

        public WordList(byte wordLength = 5)
        {
            WordLength = wordLength;
            _random = new Random();
        }

        public void Load(string path, bool includeFrequency = false)
        {
            foreach (var line in File.ReadLines(path))
            {
                if (includeFrequency)
                {
                    var split = line.Split('\t', StringSplitOptions.TrimEntries);
                    if (split.Length < 4) continue;
                    var wordValue = split[1].ToLower();
                    if (!int.TryParse(split[3], out var wordFreq)) continue;
                    if (!wordValue.All(char.IsLetter)) continue;

                    var word = new Word(wordValue, wordFreq);
                    if (word.Value.Length == WordLength)
                    {
                        this.Add(word);
                    }
                }
                else
                {
                    var wordValue = line.Trim().ToLower();
                    if (wordValue.Length != WordLength) continue;
                    if (this.Any(a => a.Value == wordValue)) continue;
                    this.Add(new Word(wordValue));
                }
            }
        }

        public Word GetRandomWord()
        {
            var r = _random.Next(0, Count);
            return this.ToList()[r];
        }
    }
    public readonly struct Word : IEquatable<Word>
    {
        public string Value { get; }
        public int Frequency { get; }

        public int UniqueLetters => Value.Distinct().Count();

        public Word(string value, int frequency = 0)
        {
            Value = value;
            Frequency = frequency;
        }

        public bool Equals(Word other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Word other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Word left, Word right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Word left, Word right)
        {
            return !(left == right);
        }
    }
}
