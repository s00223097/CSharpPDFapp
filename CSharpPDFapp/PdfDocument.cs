using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Xml;


namespace CSharpPDFapp
{
    public class PdfDocument
    {
        // Declare variables 
        private string filePath;
        private XpsDocument xpsDocument;
        private FixedDocumentSequence documentSequence;

        // constructor
        public PdfDocument(string filePath)
        {
            this.filePath = filePath;
        }

        public void LoadPdfDocument()
        {
            // Load PDF document using XpsDocument class
            xpsDocument = new XpsDocument(filePath, FileAccess.Read);
            documentSequence = xpsDocument.GetFixedDocumentSequence();
        }

        public void DisplayPdfDocument(FlowDocumentReader reader) // flowdocument is in system.windows as a window control option 
        {
            // Display PDF document using FlowDocumentReader control 
            var paginator = documentSequence.DocumentPaginator;
            var document = new FlowDocument(); 
            var block = new BlockUIContainer((System.Windows.UIElement)paginator.GetPage(0).Visual); // pick the Window to display the pdf in 
            document.Blocks.Add(block); // add 
            reader.Document = document;
        }

        public void ClosePdfDocument()
        {
            // Close PDF document
            if (xpsDocument != null) // if no document has been loaded...
            {
                xpsDocument.Close(); 
            }
        }

    }
}
/* 
 
 */