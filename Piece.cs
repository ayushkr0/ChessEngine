namespace ChessEngine
{
    public class Piece
    {
        public string Color { get; }
        public string Type { get; }

        public Piece(string color, string type)
        {
            Color = color;
            Type = type;
        }
    }
}
