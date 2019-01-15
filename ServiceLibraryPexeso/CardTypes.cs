using System.Runtime.Serialization;

namespace ServiceLibraryPexeso
{
    [DataContract]
    public class CardTypes
    {
        [DataMember]
        public int Row { get; set; }
        [DataMember]
        public int Column { get; set; }
        
        public int Count => Row * Column;

        public CardTypes(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"row: {Row}, column: {Column}, count: {Count}";
        }
    }
}
