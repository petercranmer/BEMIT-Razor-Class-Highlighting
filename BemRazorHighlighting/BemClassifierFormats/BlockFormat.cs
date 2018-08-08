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
    [ClassificationType(ClassificationTypeNames = BemClassifier.BEM_BLOCK_CLASSIFICATION)]
    [Name(BemClassifier.BEM_BLOCK_CLASSIFICATION)]
    [UserVisible(true)]
    internal sealed class BlockFormat : BemFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockFormat"/> class.
        /// </summary>
        public BlockFormat()
            : base("Block", Colors.Red)
        {
        }
    }
}
