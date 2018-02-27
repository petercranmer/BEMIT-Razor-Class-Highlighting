using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BemRazorHighlighting
{
    /// <summary>
    /// Classification type definition export for EditorClassifier1
    /// </summary>
    internal static class BemClassifierClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        /// <summary>
        /// Defines the "EditorClassifier1" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BemClassifier.BEM_BLOCK_CLASSIFICATION)]
        private static ClassificationTypeDefinition blockDefinition;

#pragma warning restore 169
    }
}
