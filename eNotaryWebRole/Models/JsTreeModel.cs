using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNotaryWebRole.Models
{
    public class JsTreeModel
    {
        public JsTreeData data { get; set; }
        public string state { get; set; }
        public string id { get; set; }
        public JsTreeAttribute attr { get; set; }
       

        public  JsTreeModel(JsTreeData d, string st, string i, JsTreeAttribute a, IList<JsTreeModel> ch )
        {
            data = d;
            state = st;
            id = i;
            attr = a;
    
        }
  
    }
  
  public class JsTreeAttribute
  {
      public string id { get; set; }
      public bool selected { get; set; }
      public string style { get; set; }

      public  JsTreeAttribute(string i, bool sel, string sty)
      {
          id = i;
          selected = sel;
          style = sty;
      }
  }
    public  class JsTreeData
    {
        public string title { get; set; }
        public string icon { get; set; }
        public  JsTreeData(string tit,string ico)
        {
            title = tit;
            icon = ico;
        }
        
    }
}