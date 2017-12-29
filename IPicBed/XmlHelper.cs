using System.Xml;

namespace IPicBed
{
    public static class XmlHelper
    {
        public static string Select(string xmlPath, string xpath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            //取根结点
            var root = xmlDoc.DocumentElement;//取到根结点
            //取指定的单个结点
            var node = xmlDoc.SelectSingleNode(xpath);
            return node?.InnerText ?? "";
        }
    }
}