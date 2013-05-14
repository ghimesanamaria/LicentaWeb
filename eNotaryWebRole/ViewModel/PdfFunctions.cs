using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TallComponents.PDF.Rasterizer;

using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace eNotaryWebRole.ViewModel
{
    public class PdfFunctions
    {

        public void create_bitmap(Stream filestream, string png_name, string url)
        {
            
            
                 Document pdfDocument = new Document(filestream);

                // determine the width and height of the bitmap
                float resolution = 18;
                 float scale = resolution / 72f;
  
                 // determine width and height - we create a horizontal strip...
                 int MARGIN = 5;
                 int totalWidth = 0;
                 int totalHeight = 0;
                  for (int i = 0; i < pdfDocument.Pages.Count; i++)
                    {
                      Page pdfPage = pdfDocument.Pages[i];
                      totalWidth += (int)(scale * pdfPage.Width + 2 * MARGIN);
                      totalHeight = Math.Max(
                      totalHeight, (int)(scale * pdfPage.Height) + 2 * MARGIN);
                    }

                // Create the bitmap and render the pages


                  using (Bitmap bitmap = new Bitmap(totalWidth, totalHeight))
                  using (Graphics graphics = Graphics.FromImage(bitmap))
                  using (Pen pen = new Pen(Color.Black))
                  {
                      // we maintain a horizontal cursor to position the next page
                      int x = 0;
                      // draw the first page                    
                      
                          GraphicsState state = graphics.Save();

                          // get the next page
                          Page pdfPage = pdfDocument.Pages[0];

                          // draw the page at the given resolution and position
                          graphics.TranslateTransform(
                            (float)(x + MARGIN),
                            (float)(totalHeight - scale * pdfPage.Height) / 2);
                          graphics.ScaleTransform(scale, scale);
                          pdfPage.Draw(graphics);

                          // draw a black rectangle around the page
                          graphics.DrawRectangle(
                            pen, 0, 0, (int)pdfPage.Width, (int)pdfPage.Height);

                          // update the horizontal cursor 
                          x += (int)(scale * pdfPage.Width);
                          x += 2 * MARGIN;

                          graphics.Restore(state);

                         

                          bitmap.Save(url+"Content\\pdf_preview\\" + png_name, System.Drawing.Imaging.ImageFormat.Png);
                        

                         
                      
                 }
        }
    }
}