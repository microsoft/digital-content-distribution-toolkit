using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.IIS.Media.DASH.MPDParser
{
    class Constants
    {
        public static string pathToDASHXSD = "DASH-MPD.xsd";
        public static string pathToXlinkXSD = "xlink.xsd";
        public static string pathToXSLT = "val_schema.xsl";
        public static string pathToTempMPD = "MPD_temp.xml";
        public static string pathToLocalMPD = "MPD_XLink_Resolved.xml";

        public static String OPENTAG = "<svrl:failed-assert";
        public static String CLOSETAG = "</svrl:failed-assert>";
        public static String XLINK_NAMESPACE = "http://www.w3.org/1999/xlink";
        public static String HREF = "href";
        public static String PROTOCOL = "http://";
        public static String SECURE_PROTOCOL = "https://";

    }
}
