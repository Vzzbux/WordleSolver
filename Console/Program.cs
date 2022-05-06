using System.Diagnostics;
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
        Console.Write("Guess: ");
        var guess = Console.ReadLine()?.ToLower().Trim();
        if (guess == null || guess.Length != words.WordLength)
        {
            exit = true;
            continue;
        }

        Console.Write(" Mask: ");
        var mask = Console.ReadLine()?.ToUpper().PadRight(words.WordLength);
        if (mask == null || mask.Length != words.WordLength)
        {
            exit = true;
            continue;
        }

        Console.Clear();
        Console.WriteLine($"Guess: {guess}");
        Console.WriteLine($"Mask : {mask}");

        guesser.AddGuess(new Guess(guess, mask));
        var candidateWords = guesser.GetValidWords(words).ToArray();

        Console.WriteLine("");
        Console.WriteLine("Valid guesses:");

        foreach (var word in candidateWords)
        {
            Console.WriteLine($"{word.Value} {word.Frequency}");
        }

        Console.WriteLine($"{candidateWords.Length} words");
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
                guess = guesses.FirstOrDefault().Value;
            }

            mask = game.Guess(guess);
            for (byte i = 0; i < guess.Length; i++)
            {
                Console.ForegroundColor = mask[i].ToConsoleColor();
                Console.Write(guess[i]);
            }

            Console.WriteLine();
            //Console.WriteLine($"{guess} : {mask.ToChars()}");
        } while (!game.Won);

        Console.ForegroundColor = ConsoleColor.White;
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