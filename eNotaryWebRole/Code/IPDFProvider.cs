using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;
using TallComponents.PDF.Layout.Fonts;
using TallComponents.PDF.Layout.Shapes;
using TallComponents.PDF.Layout.Shapes.Fields;

namespace eNotaryWebRole.Code
{
    interface IPDFProvider
    {
        Document create_divorce_pdf(string filename, string url , Dictionary<string, string> dictionary,string url_config);
        string replace_content(string value, Dictionary<string, string> dictionary);
        string replace_data(string value, string old_word, string new_word);
    }
}
