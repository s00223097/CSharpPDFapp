using Microsoft.Win32;
using CSharpPDFapp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mess

namespace CSharpPDFapp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public abstract class DocumentPaginator { }
    public partial class PdfReaderWindow : Window
    {
        // declare variables
        private PdfDocument pdfDocument;

        private Dictionary<int, string> bookmarks = new Dictionary<int, string>();

        private int currentPageNumber;
        public PdfReaderWindow()
        {
            InitializeComponent();
            pdfReader.PageNumberChanged += PdfReader_PageNumberChanged; // keep track of page number 
        }

        private void OpenPdfDocument_Click(object sender, RoutedEventArgs e)
        {
            // Open PDF document and display in FlowDocumentReader control
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                if (pdfDocument != null)
                {
                    pdfDocument.ClosePdfDocument();
                }
                pdfDocument = new PdfDocument(openFileDialog.FileName);
                pdfDocument.LoadPdfDocument();
                pdfDocument.DisplayPdfDocument(pdfReader); // this being the pdfReader declared in the window class
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Close PDF document and exit application
            if (pdfDocument != null)
            {
                pdfDocument.ClosePdfDocument();
            }
            Application.Current.Shutdown();
        }

        private void PdfReader_PageNumberChanged(object sender, EventArgs e)
        {
            FlowDocumentPaginator paginator = ((FlowDocument)pdfReader.Document).Paginator as FlowDocumentPaginator;
            currentPageNumber = (paginator.GetPageNumber(pdfReader.Document.ContentStart) + 1); // add 1 because page numbers start from 1, not 0

            //currentPageNumber = pdfReader.MasterPageNumber;
        }

        private void btnBookmark_Click(object sender, RoutedEventArgs e)
        {
            string bookmarkName = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for the bookmark:", "Bookmark", "");
            bookmarks[currentPageNumber] = bookmarkName;
        }
    }
}
