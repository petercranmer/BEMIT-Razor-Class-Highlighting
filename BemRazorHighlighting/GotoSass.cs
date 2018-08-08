using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace BemRazorHighlighting
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GotoSass
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6fc2e6a1-7d2d-4c2b-8293-dac7a28316fd");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GotoSass"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GotoSass(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GotoSass Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in GotoSass's constructor requires
            // the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new GotoSass(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "GotoSass";

            EnvDTE80.DTE2 applicationObject = this.ServiceProvider.GetServiceAsync(typeof(DTE)).Result as EnvDTE80.DTE2;
            var activeFile = applicationObject.ActiveDocument.FullName;

            
            var selectedText = this.GetSelection(this.ServiceProvider);

            message += $" Selected {selectedText} Called from {activeFile}";

            var allProjectSassFiles = this.GetProjectSassFiles(applicationObject);

            var matchingFile = allProjectSassFiles
                .Where(f => f.Name.StartsWith(selectedText))
                .SingleOrDefault();

            if (matchingFile != null)
            {
                applicationObject.ItemOperations.OpenFile(matchingFile.FullName);

            }
            else
            {

                // Show a message box to prove we were here
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    "Unable to open: " + message,
                    title,
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private IEnumerable<FileInfo> GetProjectSassFiles(EnvDTE80.DTE2 applicationObject)
        {
            var currentProject = applicationObject.ActiveDocument.ProjectItem.ContainingProject;

            return this.GetSassFilesInProjectItems(currentProject.ProjectItems);
        }
    
        private IEnumerable<FileInfo> GetSassFilesInProjectItems(ProjectItems projectItems)
        {
            var filesFound = new List<FileInfo>();

            foreach (ProjectItem projectItem in projectItems)
            {
                if (projectItem.Name.EndsWith("scss"))
                {
                    filesFound.Add(new FileInfo(projectItem.FileNames[0]));
                }
                else if (projectItem.ProjectItems.Count > 0)
                {
                    filesFound.AddRange(
                        this.GetSassFilesInProjectItems(projectItem.ProjectItems)
                    );
                }
            }

            return filesFound;
        }

        private string GetSelection(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;
            var textManager = service as IVsTextManager2;
            IVsTextView view;
            int result = textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out view);

            //view.GetSelection(out int startLine, out int startColumn, out int endLine, out int endColumn);//end could be before beginning
            //var start = new TextViewPosition(startLine, startColumn);
            //var end = new TextViewPosition(endLine, endColumn);

            view.GetSelectedText(out string selectedText);

            //TextViewSelection selection = new TextViewSelection(start, end, selectedText);
            return selectedText;
        }
    }
}
