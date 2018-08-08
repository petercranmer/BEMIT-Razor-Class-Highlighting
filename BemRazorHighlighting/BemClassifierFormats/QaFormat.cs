using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BemRazorHighlighting.BemClassifierFormats
{
    /// <summary>
    /// Defines an editor format for the EditorClassifier1 type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = BemClassifier.BEM_QA_CLASSIFICATION)]
    [Name(BemClassifier.BEM_QA_CLASSIFICATION)]
    [UserVisible(true)] // This should be visible to the end user
    internal sealed class QaFormat : BemFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BemClassifierBlockFormat"/> class.
        /// </summary>
        public QaFormat()
            : base("QA", Colors.Orange)
        {
        }
    }
}
