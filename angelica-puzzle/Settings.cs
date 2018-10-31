using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace angelica_puzzle
{
    class Settings
    {
        private static XmlNode Get(string name)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Properties.Resources.CustomSettings);

            foreach (XmlNode node in xml.FirstChild.ChildNodes)
            {
                if (node.Name.CompareTo(name) == 0)
                {
                    return node;
                }
            }
            return null;
        }

        public static void SetPlayer()
        {

        }

        public static Player GetPlayer()
        {
            XmlNode node = Get("player");
            return new Player(node.ChildNodes[0].InnerText, node.ChildNodes[0].InnerText);
        }

        public static void SetGameRows()
        {

        }

        public static int GetGameRows()
        {
            XmlNode node = Get("game-rows");
            return int.Parse(node.InnerText);
        }

        public static void SetGameColumns()
        {

        }

        public static int GetGameColumns()
        {
            XmlNode node = Get("game-columns");
            return int.Parse(node.InnerText);
        }

        public static void SetGamePattern()
        {

        }

        public static string GetGamePattern()
        {
            XmlNode node = Get("game-pattern");
            return node.InnerText;
        }
    }
}
