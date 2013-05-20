using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;


using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;
using TallComponents.PDF.Layout.Fonts;
using TallComponents.PDF.Layout.Shapes;
using TallComponents.PDF.Layout.Shapes.Fields;

namespace eNotaryWebRole.Code
{
    public class PDFProvider: IPDFProvider
    {

        public  string replace_data(string value, string old_word, string new_word)
        {

            int index = value.IndexOf(old_word);
            value = index >= 0 ? value.Replace(old_word, new_word) : value;
            return value;
        }



        public  string replace_content(string value, Dictionary<string, string> dictionary)
        {
            foreach (var q in dictionary)
            {
                int index = value.IndexOf(q.Key);
                value = index >= 0 ? value.Replace(q.Key, q.Value) : value;
            }
            return value;
        }

        public Document create_divorce_pdf(string filename, string url,Dictionary<string,string> dictionary)
        {
            // generate a pdf form for divorce application
            TallComponents.PDF.Layout.Document pdf = new TallComponents.PDF.Layout.Document();
            using (FileStream file = new FileStream(url + "\\PDF\\" + filename, FileMode.Create, FileAccess.Write))
            {
            // read the xml configuration file for divorce
                Section section = pdf.Sections.Add();
                section.PageSize = TallComponents.PDF.Layout.PageSize.A4;
                section.DoNotBreak = true;
                TextParagraph text = new TextParagraph();

            XmlTextReader reader = new XmlTextReader(url+"\\ConfigFiles\\DivorceConfig.xml");

            string type = "";

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    text = new TextParagraph();
                    section.Paragraphs.Add(text);

                    string value = reader.Value;
                    if (type == "headerLine6")
                    {
                        value = replace_data(value, "[DataAzi]", DateTime.Now.ToShortDateString());
                    }
                    else if (type == "headerLine7")
                    {
                        value = replace_data(value, "[DataAzi]", DateTime.Now.AddDays(30).ToShortDateString());
                    }
                    else if (type == "bodyContent1")
                    {
                        value = replace_content(value,dictionary);
                    }

                    Fragment fragment = new Fragment(value, Font.TimesRoman, 10);
                    fragment.PreserveWhiteSpace = true;
                    text.Fragments.Add(fragment);
                }
            }
                 
        




                    return pdf;
                }


            

        }
    }
}