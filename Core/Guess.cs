namespace WordleSolver.Core
{
    public readonly struct Guess
    {
        public string Word { get; }
        public MaskColour[] Mask { get; }

        public int Length => Word.Length;

        public Guess(string word, string mask) : this(word, mask.ToMaskColours())
        {
        }

        public Guess(string word, MaskColour[] mask)
        {
            if (word.Length != mask.Length) throw new ArgumentException("Guess and Mask length must match.");
            Word = word;
            Mask = mask;
        }

        //Pair up the chars of the word with its relevant char from the mask
        public (char letter, MaskColour mask)[] CharMask
        {
            get
            {
                var charMask = new (char letter, MaskColour mask)[Word.Length];
                for (var i = 0; i < Word.Length; i++)
                {
                    charMask[i] = (Word[i], Mask[i]);
                }

                return charMask;
            }
        }

        //Use the supplied mask to blank out any grey letters
        public char[] WordCharsWithoutGreys => CharMask.Select(c => c.mask is MaskColour.Green or MaskColour.Yellow ? c.letter : '-').ToArray();

    }

    public enum MaskColour
    {
        Unset,
        Green,
        Yellow,
        Grey
    }


}
