using System.Runtime.Serialization;

namespace ServiceLibraryPexeso
{
    [DataContract]
    public enum Result
    {
        [EnumMember] Win,
        [EnumMember] Lose,
        [EnumMember] Draw
    }
}