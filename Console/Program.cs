using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using WordleSolver.Core;

var words = new WordList();
words.Load(@"wordlewords.txt", false);
words.Load(@"words.txt", true, true);

Console.Write("(I)nteractive or (P)lay self?: ");
var key = Console.ReadKey();
Console.WriteLine();

switch (key.Key)
{
    case ConsoleKey.I:
    {
        Interactive(words);
        break;
    }
    case ConsoleKey.P:
    {
        PlaySelf(words);
        break;
    }
}
Environment.Exit(0);

void Interactive(WordList words)
{
    var guesser = new NextWordGuesser();
    var exit = false;
    while (!exit)
    {
        Console.WriteLine("[Enter the 5-letter word guessed on the last turn]");
        Console.Write("Guess: ");
        var guessWord = Console.ReadLine()?.ToLower().Trim();
        if (guessWord == null || guessWord.Length != words.WordLength)
        {
            Console.WriteLine("Must be a 5-letter word.");
            continue;
        }

        Console.WriteLine("[Now enter the mask response. Use Y for yellow, G for green, any other character for grey]");
        Console.SetCursorPosition(7, Console.CursorTop - 2);
        var mask = new MaskColour[guessWord.Length];
        for (var i = 0; i < guessWord.Length; i++)
        {
            var maskKey = Console.ReadKey(true);
            if (maskKey.Key == ConsoleKey.Backspace)
            {
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.ForegroundColor = MaskColour.Unset.ToConsoleColor();
                i--;
                Console.Write(guessWord[i]);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                i--;
                continue;
            }
            mask[i] = maskKey.Key switch
            {
                ConsoleKey.Y => MaskColour.Yellow,
                ConsoleKey.G => MaskColour.Green,
                _ => MaskColour.Grey
            };
            Console.ForegroundColor = mask[i].ToConsoleColor();
            Console.Write(guessWord[i]);
        }

        guesser.AddGuess(new Guess(guessWord, mask));

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"Guesses so far: ");
        foreach (var guess in guesser.Guesses)
        {
            for (var i = 0; i < guess.Length; i++)
            {
                Console.ForegroundColor = guess.Mask[i].ToConsoleColor();
                Console.Write(guess.Word[i]);
            }
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Gray;

        var candidateWords = guesser.GetValidWords(words).ToArray();

        Console.WriteLine("Next valid guesses:");

        foreach (var word in candidateWords)
        {
            Console.WriteLine($"{word.Value} {word.Frequency}");
        }

        Console.WriteLine($"[{candidateWords.Length} words]");
        Console.WriteLine();
    }
}

void PlaySelf(WordList words, int numGames = 100)
{
    var sw = new Stopwatch();
    sw.Start();
    var tries = new List<int>();
    for (var c = 1; c <= numGames; c++)
    {
        var game = new Game(words);
        var guesser = new NextWordGuesser();

        //var guess = words.GetRandomWord().Value;
        var guess = "crane";
        Console.WriteLine($"Game {c}:");
        var mask = new MaskColour[words.WordLength];
        do
        {
            if (mask.All(m => m != MaskColour.Unset))
            {
                guesser.AddGuess(new Guess(guess, mask));
                var guesses = guesser.GetValidWords(words);
                guess = guesses.LastOrDefault().Value;
            }

            mask = game.Guess(guess);
            for (var i = 0; i < guess.Length; i++)
            {
                Console.ForegroundColor = mask[i].ToConsoleColor();
                Console.Write(guess[i]);
            }

            Console.WriteLine();
            //Console.WriteLine($"{guess} : {mask.ToChars()}");
        } while (!game.Won);

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"Guessed {guess} in {game.Tries} tries");
        Console.WriteLine();
        tries.Add(game.Tries);
    }
    sw.Stop();
    Console.WriteLine($"Played {tries.Count} games in {sw.Elapsed.TotalSeconds:F1} seconds");
    Console.WriteLine($"Best   : {tries.Min()} tries");
    Console.WriteLine($"Worst  : {tries.Max()} tries");
    Console.WriteLine($"Average: {tries.Average()} tries");
}