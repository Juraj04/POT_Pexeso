using System.Windows.Controls;

namespace GuiPexeso
{
    class BoardItem
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public Button Button { get; set; }
        public char DefaultChar { get; set; } = ' ';
        public char CardChar { get; set; }
        public bool Found { get; set; } = false;

        public BoardItem(int rowIndex, int colIndex, Button button, char cardChar)
        {
            RowIndex = rowIndex;
            ColIndex = colIndex;
            Button = button;
            CardChar = cardChar;
        }
    }
}
