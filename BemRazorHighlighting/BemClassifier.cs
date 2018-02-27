using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace BemRazorHighlighting
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "EditorClassifier1" classification type.
    /// </summary>
    internal class BemClassifier : IClassifier
    {
        public const string BEM_BLOCK_CLASSIFICATION = nameof(BemClassifier) + "_" + nameof(blockClassificationType);
        public const string BEM_ELEMENT_CLASSIFICATION = nameof(BemClassifier) + nameof(elementClassificationType);
        public const string BEM_MODIFIER_CLASSIFICATION = nameof(BemClassifier) + nameof(modifierClassificationType);
        public const string BEM_JS_CLASSIFICATION = nameof(BemClassifier) + nameof(jsClassificationType);
        public const string BEM_QA_CLASSIFICATION = nameof(BemClassifier) + nameof(qaClassificationType);
        
        private readonly IClassificationType blockClassificationType;
        private readonly IClassificationType elementClassificationType;
        private readonly IClassificationType modifierClassificationType;
        private readonly IClassificationType jsClassificationType;
        private readonly IClassificationType qaClassificationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="BemClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal BemClassifier(IClassificationTypeRegistryService registry)
        {
            this.blockClassificationType = registry.GetClassificationType(BEM_BLOCK_CLASSIFICATION);
            this.elementClassificationType = registry.GetClassificationType(BEM_ELEMENT_CLASSIFICATION);
            this.modifierClassificationType = registry.GetClassificationType(BEM_MODIFIER_CLASSIFICATION);
            this.jsClassificationType = registry.GetClassificationType(BEM_JS_CLASSIFICATION);
            this.qaClassificationType = registry.GetClassificationType(BEM_QA_CLASSIFICATION);
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>()
            {
                new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)), this.blockClassificationType)
            };

            return result;
        }

        #endregion
    }
}
