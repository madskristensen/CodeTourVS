using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace CodeTourVS
{
    public class CodeTourPeekableItem : IPeekableItem
    {
        private readonly IPeekResultFactory _peekResultFactory;
        private readonly Step _step;
        private readonly Span _span;

        public CodeTourPeekableItem(IPeekResultFactory peekResultFactory, Step step, Span span)
        {
            _peekResultFactory = peekResultFactory;
            _step = step;
            _span = span;
        }
        public string DisplayName => _step.Title;

        public IEnumerable<IPeekRelationship> Relationships
        {
            get
            {
                yield return new CodeTourPeekRelationship();
            }
        }

        public IPeekResultSource GetOrCreateResultSource(string relationshipName)
        {
            if (relationshipName == CodeTourPeekRelationship.RelationshipName)
            {
                return new CodeTourResultSource();
            }

            return null;
        }
    }
}
