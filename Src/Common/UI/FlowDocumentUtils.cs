using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;

namespace LinguaSpace.Common.UI
{
    public class FlowDocumentUtils
    {
        public static String Text2BracketsSignature(String text)
        {
            StringBuilder signature = new StringBuilder();
            foreach (char c in text)
            {
                switch (c)
                {
                    case '[':
                    case ']':
                        signature.Append(c);
                        break;
                }
            }
            return signature.ToString();
        }

        public static FlowDocument Text2FlowDocument(String text, bool bShowBrackets, Brush brushHighlight)
        {
            FlowDocument doc = new FlowDocument();

            Paragraph block = new Paragraph();

            block.TextAlignment = TextAlignment.Left;

            doc.Blocks.Add(block);

            StringBuilder token = new StringBuilder();
            bool bold = false;
			bool italic = false;

            foreach (Char c in text)
            {
                if (!bold)
                {
                    switch (c)
                    {
                        case '[':
                            if (token.Length > 0)
                            {
                                block.Inlines.Add(new Run(token.ToString()));
                                token.Length = 0;
                            }
                            if (bShowBrackets)
                            {
                                Bold b = new Bold(new Run("["));
                                if (brushHighlight != null)
									b.Foreground = brushHighlight;
                                block.Inlines.Add(b);
                            }
                            bold = true;
                            break;
                        default:
                            token.Append(c);
                            break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case ']':
                            if (token.Length > 0)
                            {
                                Bold b = new Bold(new Run(token.ToString()));
                                if (brushHighlight != null)
									b.Foreground = brushHighlight;
                                block.Inlines.Add(b);
                                token.Length = 0;
                            }
                            if (bShowBrackets)
                            {
                                Bold b = new Bold(new Run("]"));
								if (brushHighlight != null)
									b.Foreground = brushHighlight;
                                block.Inlines.Add(b);
                            }
                            bold = false;
                            break;
                        default:
                            token.Append(c);
                            break;
                    }
                }
            }

            if (token.Length > 0)
            {
                if (bold)
                {
                    Bold b = new Bold(new Run(token.ToString()));
					if (brushHighlight != null)
						b.Foreground = brushHighlight;
                    block.Inlines.Add(b);
                }
                else
                {
                    block.Inlines.Add(new Run(token.ToString()));
                }
            }

            return doc;
        }

        public static String FlowDocument2Text(FlowDocument doc)
        {
            String text = new TextRange(doc.ContentStart, doc.ContentEnd).Text;
            if (text.EndsWith("\r\n"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }
    }
}
