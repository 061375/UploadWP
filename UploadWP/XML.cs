using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace UploadWP
{
    sealed class XML
    {
        public List<string> GETTAG(string xml, string tagname)
        {
            List<string> _return = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeList elemList = doc.GetElementsByTagName(tagname);
            foreach (XmlNode node in elemList)
            {
                _return.Add(node.InnerText.ToString());
                Console.WriteLine(node.InnerText.ToString());
            }
            return _return;
        }
        public List<string> GETPath(string url, string xpath_expression)
        {
            List<string> _return = new List<string>();
            XPathDocument docNav = new XPathDocument(url);
            XPathNavigator nav = docNav.CreateNavigator();
            XPathNodeIterator NodeIter = nav.Select(xpath_expression);
            while (NodeIter.MoveNext())
            {
                _return.Add(NodeIter.Current.Value);
            };
            return _return;
        }
    }
}
