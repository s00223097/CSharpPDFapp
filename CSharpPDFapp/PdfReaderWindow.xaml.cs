using Microsoft.Win32;
using PdfReader;
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
namespace CSharpPDFapp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PdfReaderWindow : Window
    {
        private PdfDocument pdfDocument;
        private FlowDocumentReader pdfReader;

        public PdfReaderWindow()
        {
            InitializeComponent();
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
                pdfDocument.DisplayPdfDocument(pdfReader);
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
    }
}
/*
using Microsoft.Win32;

namespace PdfReader
{
    public partial class PdfReaderWindow : Window
    {
        private PdfDocument pdfDocument;

        public PdfReaderWindow()
        {
            InitializeComponent();
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
                pdfDocument.DisplayPdfDocument(pdfReader);
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
    }
}



*/