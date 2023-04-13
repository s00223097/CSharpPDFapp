using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PDFReaderApp
{
    public class PdfReader
    {
        private readonly string _pdfFilePath;
        private int _currentPageNumber;

        public PdfReader(string pdfFilePath)
        {
            _pdfFilePath = pdfFilePath;
            _currentPageNumber = 1;
        }

        public string GetText(int pageNumber)
        {
            // Check if the page number is valid
            if (pageNumber < 1 || pageNumber > GetNumberOfPages())
            {
                throw new ArgumentOutOfRangeException("pageNumber", "Invalid page number");
            }

            // Get the contents of the page
            string pageContents = GetPageContents(pageNumber);

            // Extract the text from the page contents
            string text = ExtractTextFromPageContents(pageContents);

            return text;
        }

        public int GetNumberOfPages()
        {
            // Open the PDF file
            using (FileStream stream = new FileStream(_pdfFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the PDF file into a byte array
                byte[] pdfBytes = new byte[stream.Length];
                stream.Read(pdfBytes, 0, (int)stream.Length);

                // Search for the /Count field in the PDF file trailer
                string trailer = Encoding.ASCII.GetString(pdfBytes, pdfBytes.Length - 512, 512);
                Match countMatch = Regex.Match(trailer, @"/Count\s+(\d+)");
                if (countMatch.Success)
                {
                    // Return the number of pages
                    return int.Parse(countMatch.Groups[1].Value);
                }
                else
                {
                    throw new InvalidOperationException("Could not find page count in PDF file trailer");
                }
            }
        }

        private string GetPageContents(int pageNumber)
        {
            // Open the PDF file
            using (FileStream stream = new FileStream(_pdfFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the PDF file into a byte array
                byte[] pdfBytes = new byte[stream.Length];
                stream.Read(pdfBytes, 0, (int)stream.Length);

                // Search for the specified page
                string pagePattern = $@"\d+\s+{pageNumber}\s+obj((.|\n|\r)*?)endobj";
                Match pageMatch = Regex.Match(Encoding.ASCII.GetString(pdfBytes), pagePattern);
                if (pageMatch.Success)
                {
                    // Return the contents of the page
                    return pageMatch.Groups[1].Value;
                }
                else
                {
                    throw new InvalidOperationException($"Could not find page {pageNumber} in PDF file");
                }
            }
        }

        private string ExtractTextFromPageContents(string pageContents)
        {
            // Remove any font information from the page contents
            string noFontsContents = Regex.Replace(pageContents, @"/(F|TT)[0-9]+ [0-9]+ Tf", "");

            // Remove any images from the page contents
            string noImagesContents = Regex.Replace(noFontsContents, @"\/Type \/XObject.*?\/Subtype \/Image.*?endobj", "");

            // Remove any other special characters from the page contents
            string cleanContents = Regex.Replace(noImagesContents, @"[\n\r\t]", "");

            // Extract the text from the cleaned page contents
            MatchCollection matches = Regex.Matches(cleanContents, @"\(.*?\)");
            StringBuilder textBuilder = new StringBuilder();
            foreach (Match match in matches)
            {
                textBuilder.Append(match.Value.Substring(1, match.Value.Length - 2));
            }

            return textBuilder.ToString();
        }
    }
}