using PDFReaderApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PdfBookmarkExtractor
{
    private readonly byte[] pdfBytes;

    public PdfBookmarkExtractor(byte[] pdfBytes)
    {
        this.pdfBytes = pdfBytes;
    }

    public IReadOnlyList<PdfBookmark> ExtractBookmarks()
    {
        var bookmarks = new List<PdfBookmark>();
        var bookmarkTree = ParseBookmarkTree();
        ExtractBookmarksRecursive(bookmarks, bookmarkTree, "");
        return bookmarks.AsReadOnly();
    }

    private void ExtractBookmarksRecursive(List<PdfBookmark> bookmarks, List<PdfBookmarkTreeNode> nodes, string parentTitle)
    {
        foreach (var node in nodes)
        {
            var title = parentTitle + node.Title;

            if (!string.IsNullOrWhiteSpace(title))
            {
                bookmarks.Add(new PdfBookmark(title, node.PageIndex));
            }

            ExtractBookmarksRecursive(bookmarks, node.Children, title + " > ");
        }
    }

    private List<PdfBookmarkTreeNode> ParseBookmarkTree()
    {
        var bookmarkTree = new List<PdfBookmarkTreeNode>();
        int bookmarkObjectIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("/Type /Outlines"));

        if (bookmarkObjectIndex >= 0)
        {
            int bookmarkTreeNodeIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("/First"), bookmarkObjectIndex);

            if (bookmarkTreeNodeIndex >= 0)
            {
                int firstNodeObjectNumber = GetNextInteger(pdfBytes, bookmarkTreeNodeIndex);

                if (firstNodeObjectNumber > 0)
                {
                    ParseBookmarkTreeNode(bookmarkTree, firstNodeObjectNumber);
                }
            }
        }

        return bookmarkTree;
    }

    private void ParseBookmarkTreeNode(List<PdfBookmarkTreeNode> nodes, int objectNumber)
    {
        int objectStartIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes($"{objectNumber} 0 obj"));
        int objectEndIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("endobj"), objectStartIndex);

        var node = new PdfBookmarkTreeNode();

        int titleIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("/Title"), objectStartIndex, objectEndIndex);

        if (titleIndex >= 0)
        {
            int titleStartIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("("), titleIndex, objectEndIndex);
            int titleEndIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes(")"), titleStartIndex, objectEndIndex);
            node.Title = DecodePdfString(pdfBytes, titleStartIndex + 1, titleEndIndex);
        }

        int destIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("/Dest"), objectStartIndex, objectEndIndex);

        if (destIndex >= 0)
        {
            int destStartIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("["), destIndex, objectEndIndex);
            int destEndIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("]"), destStartIndex, objectEndIndex);

            if (destEndIndex >= 0)
            {
                string destString = Encoding.ASCII.GetString(pdfBytes, destStartIndex + 1, destEndIndex - destStartIndex - 1);
                string[] destParts = destString.Split();

                if (destParts.Length >= 3)
                {
                    int pageIndex;

                    if (int.TryParse(destParts[0], out pageIndex))
                    {
                        node.PageIndex = GetPageIndex(pdfBytes, pageIndex);
                    }
                }
            }
        }

        int firstChildIndex = IndexOf(pdfBytes, Encoding.ASCII.GetBytes("/First"), objectStartIndex, objectEndIndex;
