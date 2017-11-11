namespace Eriona
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new ErionaGame())
            {
                game.Run();
            }
        }
    }
}