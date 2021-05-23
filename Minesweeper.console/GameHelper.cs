namespace Minesweeper.console
{
    public static class GameHelper
    {
        //Get a corresponding capital letter value for corresponding int. Only supporting up to 26 values. 
        //others ways of doing this with add range, ascii vals etc
        private static char[] GameLetters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public static char CurrentXPositionLetter(int xPosition)
        {
            return GameLetters[xPosition];
        }

        //pluralisation here as difficult to place inside ternaries inside string interpolalation
        public static string LivesText(int lives)
        {
            return lives > 1 ? "lives" : "life";
        }
    }
}
