using System.Globalization;
using System.Xml;
using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public static class SVGHelpers
    {
        public static void Temp()
        {
            IGamepadListener listener;
        }

        public static byte[] GetFillArgb(XmlNode? node)
        {
            string? style = node?.Attributes?.GetNamedItem("style")?.Value;
            if (style == null) { throw new KeyNotFoundException(); }

            string search = "fill:#";
            int fillIdx = style.IndexOf(search) + search.Length;

            byte r = byte.Parse(style.Substring(fillIdx + 0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(style.Substring(fillIdx + 2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(style.Substring(fillIdx + 4, 2), NumberStyles.HexNumber);
            return [0xff, r, g, b];
        }

        public static XmlNode? FindChild(XmlNode? root, string targetId)
        {
            XmlNode? result = null;
            IterateNode(root, child =>
            {
                string? childId = child?.Attributes?.GetNamedItem("id")?.Value;
                if (childId == targetId)
                {
                    result = child;
                    return false;
                }

                return true;
            });

            return result;
        }

        private static void IterateNode(XmlNode? parent, Func<XmlNode, bool> onChild)
        {
            if (parent == null) { throw new KeyNotFoundException(); }
            for (int i = 0; i < parent.ChildNodes.Count; i++)
            {
                var child = parent.ChildNodes[i];
                if (child == null)
                {
                    throw new KeyNotFoundException();
                }

                bool keepGoing = onChild(child);
                if (!keepGoing) { break; }
            }
        }
    }
}
