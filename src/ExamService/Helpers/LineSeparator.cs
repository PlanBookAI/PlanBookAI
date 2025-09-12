using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

namespace ExamService.Helpers
{
    /// <summary>
    /// Helper class để tạo đường kẻ ngang trong PDF
    /// </summary>
    public class LineSeparator : IElement, IPdfPCellEvent
    {
        protected float lineWidth = 1;
        protected float percentage = 100;
        protected BaseColor lineColor = BaseColor.BLACK;
        protected int alignment = Element.ALIGN_CENTER;
        protected float offset = 0;
        
        // Implement IElement interface
        public bool Process(IElementListener listener)
        {
            return true;
        }
        
        public int Type
        {
            get { return Element.RECTANGLE; }
        }
        
        public bool IsContent()
        {
            return true;
        }
        
        public bool IsNestable()
        {
            return false;
        }
        
        public IList<Chunk> Chunks
        {
            get { return new List<Chunk>(); }
        }

        public LineSeparator() { }

        public LineSeparator(float lineWidth, float percentage, BaseColor lineColor, int alignment, float offset)
        {
            this.lineWidth = lineWidth;
            this.percentage = percentage;
            this.lineColor = lineColor;
            this.alignment = alignment;
            this.offset = offset;
        }

        public void CellLayout(PdfPCell cell, Rectangle rect, PdfContentByte[] canvas)
        {
            PdfContentByte cb = canvas[PdfPTable.LINECANVAS];
            cb.SaveState();
            cb.SetLineWidth(lineWidth);
            if (lineColor != null)
                cb.SetColorStroke(lineColor);
            
            float x1 = rect.Left;
            float x2 = rect.Right;
            
            if (percentage < 100)
            {
                float width = (rect.Right - rect.Left) * percentage / 100;
                switch (alignment)
                {
                    case Element.ALIGN_LEFT:
                        x2 = rect.Left + width;
                        break;
                    case Element.ALIGN_RIGHT:
                        x1 = rect.Right - width;
                        break;
                    case Element.ALIGN_CENTER:
                        float delta = (rect.Right - rect.Left - width) / 2;
                        x1 = rect.Left + delta;
                        x2 = rect.Right - delta;
                        break;
                }
            }
            
            cb.MoveTo(x1, rect.Top + offset);
            cb.LineTo(x2, rect.Top + offset);
            cb.Stroke();
            cb.RestoreState();
        }
    }
}
