using Microsoft.VisualStudio.Language.Intellisense;

namespace CodeTourVS
{
    public class CodeTourPeekRelationship : IPeekRelationship
    {
        public const string RelationshipName = "codetour";
        public string Name => "codetour";

        public string DisplayName => "Code Tour";
    }
}
