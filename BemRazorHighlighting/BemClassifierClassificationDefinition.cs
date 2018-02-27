using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BemRazorHighlighting
{
    internal static class BemClassifierClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BemClassifier.BEM_BLOCK_CLASSIFICATION)]
        private static ClassificationTypeDefinition blockDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BemClassifier.BEM_ELEMENT_CLASSIFICATION)]
        private static ClassificationTypeDefinition elementDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BemClassifier.BEM_JS_CLASSIFICATION)]
        private static ClassificationTypeDefinition jsDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BemClassifier.BEM_MODIFIER_CLASSIFICATION)]
        private static ClassificationTypeDefinition modifierDefinition;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BemClassifier.BEM_QA_CLASSIFICATION)]
        private static ClassificationTypeDefinition qaDefinition;

#pragma warning restore 169
    }
}
