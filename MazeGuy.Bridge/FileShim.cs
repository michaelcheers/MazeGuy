namespace System.IO
{
    public static class FileShim
    {
        public static bool Exists(string path)
        {
            int? level = ParseLevelFromPath(path);
            return level.HasValue && MazeGuy.Game1.Levels.ContainsKey(level.Value);
        }

        public static string[] ReadAllLines(string path)
        {
            int? level = ParseLevelFromPath(path);
            if (level.HasValue && MazeGuy.Game1.Levels.ContainsKey(level.Value))
            {
                return MazeGuy.Game1.Levels[level.Value];
            }
            return new string[0];
        }

        private static int? ParseLevelFromPath(string path)
        {
            // Handle "../Maze.txt" -> level 2 (original level 1 was broken - no exit)
            if (path.Contains("Maze.txt") && !path.Contains("Maze "))
                return 2;

            // Handle "../Maze 2.txt" -> level 2, etc.
            int start = path.IndexOf("Maze ") + 5;
            if (start < 5) return null;

            int end = path.IndexOf(".txt");
            if (end < 0) return null;

            string numStr = path.Substring(start, end - start);
            if (int.TryParse(numStr, out int level))
                return level;

            return null;
        }
    }
}
