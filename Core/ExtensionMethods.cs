namespace WordleSolver.Core
{
    public static class ExtensionMethods
    {
        public static char[] ToChars(this MaskColour[] value) => value.Select(c => c.ToChar()).ToArray();

        public static char ToChar(this MaskColour value)
        {
            return value switch
            {
                MaskColour.Green => 'G',
                MaskColour.Yellow => 'Y',
                MaskColour.Grey => '-',
                _ => default
            };
        }

        public static MaskColour[] ToMaskColours(this string value) => value.ToCharArray().ToMaskColours();
        public static MaskColour[] ToMaskColours(this char[] value) => value.Select(x => x.ToMaskColour()).ToArray();
        

        public static MaskColour ToMaskColour(this char value)
        {
            return value switch
            {
                'G' or 'g' => MaskColour.Green,
                'Y' or 'y' => MaskColour.Yellow,
                _ => MaskColour.Grey,
            };
        }

        public static ConsoleColor ToConsoleColor(this MaskColour value)
        {
            return value switch
            {
                MaskColour.Green => ConsoleColor.DarkGreen,
                MaskColour.Yellow => ConsoleColor.DarkYellow,
                _ => ConsoleColor.DarkGray
            };
        }
    }
}
