namespace CodeEditor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using ScintillaNET;
    using System.Runtime.InteropServices;
    using Kohl.Framework.Logging;

    public partial class Edit : UserControl
    {
        #region Constants
        private const string NEW_DOCUMENT_TEXT = "Untitled";
        private const int LINE_NUMBERS_MARGIN_WIDTH = 35; 
        #endregion Constants

        #region Fields
        private int _newDocumentCount = 0;
        private string[] _args;
        private int _zoomLevel;
        #endregion Fields

        #region Constructors (3)      
        static Edit()
        {
        	try
        	{
	            string nativeDllName = IntPtr.Size == 4 ? "SciLexer.dll" : "SciLexer64.dll";
	            // would return the wrong path the the Exe's Location not to the plugin directory
	            //string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
	            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(Edit)).Location);
	            string name = System.IO.Path.Combine(path, nativeDllName);
		
	            if (LoadLibrary(name) == IntPtr.Zero)
	            {
	                Log.Fatal("Error loading native dll " + nativeDllName + " at '" + path + "' for AutoIt plugin.");
	            }
	            else
	            	Log.Debug("Loaded native dll '" + name + "' for AutoIt plugin dynamically.");
        	}
        	catch
        	{
        		Log.Fatal("Error loading native dll SciLexer.dll for AutoIt plugin.");
        	}
        }

        public Edit()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            try
            {
                NewDocument();
                aboutToolStripMenuItem.Text = String.Format(CultureInfo.CurrentCulture, "&About {0}", AboutForm.AssemblyTitle);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public Edit(string[] args) : this()
        {
            try
            {
                // Store the command line args
                this._args = args;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        #endregion

        #region Properties (6)
        public string Caption { get; set; }

        public bool Modified
        {
            get
            {
            	try
            	{
	                if (label1 == null)
	                    return false;
	
	                return label1.Modified;
            	}
            	catch
            	{
            		Log.Fatal("Unable to retrieve Scintialla control modification state.");
            		return false;
            	}
            }
        }
        
        /// <summary>
        /// Indicates that calls to the StyleNeeded event should use the custom INI lexer
        /// </summary>
		public bool IniLexer {
			get;
			set;
		}

		public string FilePath {
			get;
			set;
		}

        public new string Text
        {
            get
            {
            	try
            	{
	                if (label1 == null)
	                    return null;
	
	                return this.label1.Text;
            	}
            	catch
            	{
            		Log.Fatal("Unable to retrieve Scintialla text.");
            		return null;
            	}
            }
            set
            {
            	try
            	{
	                if (label1 == null)
	                    return;
	
	                Log.Info("Script document has been loaded.");
	                this.label1.Text = value;
	                label1.Modified = false;
            	}
            	catch
            	{
            		Log.Fatal("Unable to set Scintialla text.");
            	}
            }
        }

        public string Language
        {
            get
            {
            	try
            	{
	                if (label1 == null || label1.ConfigurationManager == null || label1.ConfigurationManager.Language == string.Empty)
	                    return null;
	
	                return label1.ConfigurationManager.Language;
            	}
            	catch
            	{
            		Log.Fatal("Unable to retrieve Scintialla language.");
            		return null;
            	}
            }
            set
            {
            	try
            	{
	                if (label1 == null)
	                    return;
	
	                SetLanguage(value);
            	}
            	catch
            	{
            		Log.Fatal("Unable to set Scintialla language.");
            	}
            }
        }
		#endregion
        
        #region Public Methods (7)
        public void SetLanguage(string extension)
        {
        	try
        	{
	        	if (label1 == null)
	                return;

                if (!string.IsNullOrEmpty(extension) && extension.StartsWith(".") && extension.Length > 1)
                    extension = extension.Substring(1, extension.Length - 1);

                switch (extension.ToUpper())
                {
                    case "CS":
                    case "HTML":
                    case "INI":
                    case "PYTHON":
                    case "XML":
                    case "VBSCRIPT":
                    case "MSSQL":
                        SetLang(extension);
                        break;
                    case "":
                    case null:
                        SetLang(string.Empty);
                        break;
                    default:
                        string location = label1.ConfigurationManager.CustomLocation;

                        // Set the directory of the main window's application.
                        label1.ConfigurationManager.CustomLocation = Path.Combine(Application.StartupPath, extension + ".xml");

                        // If this assembly is not located in the main window's application startup path, use this assembly path.
                        if (!File.Exists(label1.ConfigurationManager.CustomLocation))
                            label1.ConfigurationManager.CustomLocation = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), extension + ".xml");

                        // If there's still not a vaild path don't set the language.
                        if (File.Exists(label1.ConfigurationManager.CustomLocation))
                            SetLang(extension);

                        label1.ConfigurationManager.CustomLocation = location;
                        break;
                }
            }
            catch (Exception ex)
            {
            	Log.Fatal("Unable to set Scintialla language.", ex);
            }
        }

        public void NewDocument()
        {
        	try
        	{
	        	if (label1 == null)
	        		return;
	        	
	            SetScintillaToCurrentOptions(label1);
	
	            // Change label
	            Caption = String.Format(CultureInfo.CurrentCulture, "{0}{1}", NEW_DOCUMENT_TEXT, ++_newDocumentCount);
	            toolIncremental.Searcher.Scintilla = label1;
	
	            this.FilePath = null;
	            this.label1.Text = "";
	
	            Log.Info("New file has been loaded.");
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        public bool ExportAsHtml()
        {
        	try
        	{
	        	if (label1 == null)
	            	return false;
	        	
	            using (SaveFileDialog dialog = new SaveFileDialog())
	            {
	                string fileName = (Text.EndsWith(" *") ? Text.Substring(0, Text.Length - 2) : Text);
	                dialog.Filter = "HTML Files (*.html;*.htm)|*.html;*.htm|All Files (*.*)|*.*";
	                dialog.FileName = fileName + ".html";
	                if (dialog.ShowDialog() == DialogResult.OK)
	                {
	                    label1.Lexing.Colorize(); // Make sure the document is current
	                    using (StreamWriter sw = new StreamWriter(dialog.FileName))
	                        label1.ExportHtml(sw, fileName, false);
	
	                    return true;
	                }
	            }
	
	            return false;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        		return false;
        	}
        }

        public bool LoadFile(string fileName)
        {
        	try
        	{
	            if (string.IsNullOrEmpty(fileName))
	                return false;
	
	            // Open the document specified on the command line
	            FileInfo fi = new FileInfo(fileName);
	            if (fi.Exists)
	                OpenFile(fi.FullName);
	            else
	                return false;
	
	            return true;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        		return false;
        	}
        }

        public bool Save()
        {
        	try
        	{
		        if (String.IsNullOrEmpty(FilePath))
		            return SaveAs();
		
		        return Save(FilePath);
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        		return false;
        	}
        }

        public bool Save(string filePath)
        {
        	try
        	{
	        	if (label1 == null)
	            	return false;
	        	
	            using (FileStream fs = File.Create(filePath))
	            using (BinaryWriter bw = new BinaryWriter(fs))
	                bw.Write(label1.RawText, 0, label1.RawText.Length - 1); // Omit trailing NULL
	
	            label1.Modified = false;
	            return true;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        		return false;
        	}
        }

        public bool SaveAs()
        {
        	try
        	{
	            if (saveFileDialog.ShowDialog() == DialogResult.OK)
	            {
	                FilePath = saveFileDialog.FileName;
	                return Save(FilePath);
	            }
	
	            return false;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        		return false;
        	}
        }
		#endregion
        
        #region Private Methods (6)
        private void UpdateAllScintillaZoom()
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            // Update zoom level
	            label1.Zoom = _zoomLevel;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void OpenFile()
        {
        	try
        	{
	            if (openFileDialog.ShowDialog() != DialogResult.OK)
	                return;
	
	            foreach (string filePath in openFileDialog.FileNames)
	            {
	                // Ensure this file isn't already open
	                bool isOpen = false;
	
	                if (filePath.Equals(FilePath, StringComparison.OrdinalIgnoreCase))
	                {
	                    isOpen = true;
	                    break;
	                }
	
	                // Open the files
	                if (!isOpen)
	                    OpenFile(filePath);
	            }
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void OpenFile(string filePath)
        {
        	try
        	{
	        	if (label1 == null)
	        		return;
	        	
	            SetScintillaToCurrentOptions(label1);
	
	            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
	            {
	                using (StreamReader stream = new StreamReader(fs))
	                {
	                    label1.Text = stream.ReadToEnd();
	                }
	            }
	
	            label1.UndoRedo.EmptyUndoBuffer();
	            label1.Modified = false;
	            // show the filename
	            Caption = Path.GetFileName(filePath);
	            FilePath = filePath;
	            toolIncremental.Searcher.Scintilla = label1;
	            SetLanguage(Path.GetExtension(FilePath));
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void SetLang(string language)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            Log.Info("Trying to set style for " + language + " language.");
	
	            if ("ini".Equals(language, StringComparison.OrdinalIgnoreCase))
	            {
	                // Reset/set all styles and prepare _scintilla for custom lexing
	                CodeEditor.IniLexer.Init(label1);
	            }
	            else
	            {
	                // Use a built-in lexer and configuration
	                label1.ConfigurationManager.Language = language;
	
	                // Smart indenting...
	                if ("cs".Equals(language, StringComparison.OrdinalIgnoreCase))
	                    label1.Indentation.SmartIndentType = SmartIndent.CPP;
	                else
	                    label1.Indentation.SmartIndentType = SmartIndent.None;
	            }
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void SetScintillaToCurrentOptions(Scintilla scintilla)
        {
        	try
        	{
	        	if (scintilla == null)
	        		return;
	        	
	            // Turn on line numbers?
	            if (lineNumbersToolStripMenuItem.Checked)
	                scintilla.Margins.Margin0.Width = LINE_NUMBERS_MARGIN_WIDTH;
	            else
	                scintilla.Margins.Margin0.Width = 0;
	
	            // Turn on white space?
	            if (whitespaceToolStripMenuItem.Checked)
	                scintilla.Whitespace.Mode = WhitespaceMode.VisibleAlways;
	            else
	                scintilla.Whitespace.Mode = WhitespaceMode.Invisible;
	
	            // Turn on word wrap?
	            if (wordWrapToolStripMenuItem.Checked)
	                scintilla.LineWrapping.Mode = LineWrappingMode.Word;
	            else
	                scintilla.LineWrapping.Mode = LineWrappingMode.None;
	
	            // Show EOL?
	            scintilla.EndOfLine.IsVisible = endOfLineToolStripMenuItem.Checked;
	
	            // Set the zoom
	            scintilla.Zoom = _zoomLevel;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void AddOrRemoveAsteric()
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            if (label1.Modified)
	            {
	                if (!Text.EndsWith(" *"))
	                    Text += " *";
	            }
	            else
	            {
	                if (Text.EndsWith(" *"))
	                    Text = Text.Substring(0, Text.Length - 2);
	            }
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }
		#endregion
        
        #region Event reacting methods (60)
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (_args == null || _args.Length == 0 || !LoadFile(_args[0]))
                {
                    // Create a new document
                    NewDocument();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void au3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("au3");
        }

        private void toolStrip_MouseHover(object sender, EventArgs e)
        {
        	try
        	{
            	this.Focus();
        	}
        	catch
        	{
        		Log.Fatal("Unable to set Scintialla's focus behaviour.");
        	}
        }

        private void menuStrip_MouseHover(object sender, EventArgs e)
        {
        	try
        	{
            	this.Focus();
        	}
        	catch
        	{
        		Log.Fatal("Unable to set Scintialla's focus behaviour.");
        	}
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
                aboutForm.ShowDialog(this);
        }

        private void autoCompleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.AutoComplete.Show();
        	}
        	catch
        	{
        		Log.Fatal("Unable to show Scintialla's auto completion.");
        	}
        }

        private void clearBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
            {
            	if (label1 == null)
	            	return;

                label1.Markers.DeleteAll(0);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void collectToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
            {
            	if (label1 == null)
	            	return;

            	label1.DropMarkers.Collect();
        	}
        	catch (Exception ex)
        	{
        		Log.Error(ex);
        	}
        }

        private void commentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Commands.Execute(BindableCommand.LineComment);
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void commentStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Commands.Execute(BindableCommand.StreamComment);
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Clipboard.Copy();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
       	}

        private void csToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("cs");
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
				if (label1 == null)
	            	return;
				
	            label1.Clipboard.Cut();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void dropToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.DropMarkers.Drop();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void endOfLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            // Toggle EOL visibility for all open files
	            endOfLineToolStripMenuItem.Checked = !endOfLineToolStripMenuItem.Checked;
	            label1.EndOfLine.IsVisible = endOfLineToolStripMenuItem.Checked;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
            	this.Dispose();
    		}
        	catch (Exception ex)
        	{
        		Log.Warn(ex);
        	}
        }

        private void exportAsHtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportAsHtml();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.FindReplace.ShowFind();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void foldLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Lines.Current.FoldExpanded = true;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.GoTo.ShowGoToDialog();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void htmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("html");
        }

        private void iniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("ini");
        }

        private void insertSnippetToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Snippets.ShowSnippetList();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void lineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            // Toggle the line numbers margin for all documents
	            lineNumbersToolStripMenuItem.Checked = !lineNumbersToolStripMenuItem.Checked;
	
	            if (lineNumbersToolStripMenuItem.Checked)
	                label1.Margins.Margin0.Width = LINE_NUMBERS_MARGIN_WIDTH;
	            else
	                label1.Margins.Margin0.Width = 0;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}

        }

        private void makeLowerCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Commands.Execute(BindableCommand.LowerCase);
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void makeUpperCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Commands.Execute(BindableCommand.UpperCase);
            }
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void mssqlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("mssql");
        }

        private void navigateBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.DocumentNavigation.NavigateBackward();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void navigateForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
		    	if (label1 == null)
		        	return;
		    	
		        label1.DocumentNavigation.NavigateForward();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void nextBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            Line l = label1.Lines.Current.FindNextMarker(1);
	            if (l != null)
	                l.Goto();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void scintilla_ModifiedChanged(object sender, EventArgs e)
        {
            try
            {
                AddOrRemoveAsteric();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void scintilla_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            try
            {
                // Style the _text
                if (IniLexer)
                    CodeEditor.IniLexer.StyleNeeded((Scintilla)sender, e.Range);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Clipboard.Paste();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void plainTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage(String.Empty);
        }

        private void previosBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	            
	            Line l = label1.Lines.Current.FindPreviousMarker(1);
	            if (l != null)
	                l.Goto();
           	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Printing.PrintPreview();
    		}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Printing.Print();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void pythonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("python");
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.UndoRedo.Redo();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.FindReplace.ShowReplace();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _zoomLevel = 0;
            UpdateAllScintillaZoom();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Selection.SelectAll();
    		}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void surroundWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Snippets.ShowSurroundWithList();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void toggleBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            Line currentLine = label1.Lines.Current;
	            if (label1.Markers.GetMarkerMask(currentLine) == 0)
	            {
	                currentLine.AddMarker(0);
	            }
	            else
	            {
	                currentLine.DeleteMarker(0);
	            }
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void toolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle the visibility of the tool bar
            toolStrip.Visible = !toolStrip.Visible;
            toolBarToolStripMenuItem.Checked = toolStrip.Visible;
        }

        private void uncommentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Commands.Execute(BindableCommand.LineUncomment);
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.UndoRedo.Undo();
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void unfoldAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            foreach (Line l in label1.Lines)
	            {
	                l.FoldExpanded = false;
	            }
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void foldAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            foreach (Line l in label1.Lines)
	            {
	                l.FoldExpanded = true;
	            }
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void unfoldLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            label1.Lines.Current.FoldExpanded = false;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void vbScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("vbscript");
        }

        private void whitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            // Toggle the whitespace mode for all open files
	            whitespaceToolStripMenuItem.Checked = !whitespaceToolStripMenuItem.Checked;
	
	            if (whitespaceToolStripMenuItem.Checked)
	                label1.Whitespace.Mode = WhitespaceMode.VisibleAlways;
	            else
	                label1.Whitespace.Mode = WhitespaceMode.Invisible;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	try
        	{
	        	if (label1 == null)
	            	return;
	        	
	            // Toggle word wrap for all open files
	            wordWrapToolStripMenuItem.Checked = !wordWrapToolStripMenuItem.Checked;
	
	            if (wordWrapToolStripMenuItem.Checked)
	                label1.LineWrapping.Mode = LineWrappingMode.Word;
	            else
	                label1.LineWrapping.Mode = LineWrappingMode.None;
        	}
        	catch (Exception ex)
        	{
        		Log.Fatal(ex);
        	}
        }

        private void xmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("xml");
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Increase the zoom for all open files
            _zoomLevel++;
            UpdateAllScintillaZoom();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _zoomLevel--;
            UpdateAllScintillaZoom();
        }
		#endregion
        
        #region Windows API
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        #endregion
    }
}