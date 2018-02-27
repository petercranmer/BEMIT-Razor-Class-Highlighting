using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BemRazorHighlighting.BemClassifierFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    abstract class BemFormat : ClassificationFormatDefinition
    {
        public BemFormat(string displayName, Color color)
        {
            this.DisplayName = "BEMIT Class: " + displayName;
            this.BackgroundColor = color;
            this.BackgroundOpacity = 0.5;
        }
    }
}
