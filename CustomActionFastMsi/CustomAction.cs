using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;

namespace CustomActionFastMsi
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult FastUnzip(Session session)
        {
            session.Log("Starting FastUnzip");

            string targetDir = session.CustomActionData["FASTZIPDIR"];
            string appName = session.CustomActionData["FASTZIPAPPNAME"];

            session.Log("FASTZIPDIR = " + targetDir);
            session.Log("FASTZIPAPPNAME = " + appName);

            if ((targetDir == null) || (appName == null))
            {
                session.Log("Invalid parameters - exiting");
                return ActionResult.Failure;
            }

            string zipFile = Path.Combine(targetDir, appName + ".zip");
            if (!File.Exists(zipFile))
            {
                session.Log("Zip file does not exist - ignoring");
                return ActionResult.NotExecuted;
            }

            var fastzip = new FastZip();

            // Force zip library to use codepage 437 (IBM PC US) rather than autodetecting the system codepage.
            // ref: http://community.sharpdevelop.net/forums/t/19065.aspx
            // ref: https://stackoverflow.com/questions/46950386/sharpziplib-1-is-not-a-supported-code-page
            ZipConstants.DefaultCodePage = 437;

            session.Log("Starting extraction");

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            fastzip.ExtractZip(zipFile, Path.Combine(targetDir, appName), null);
            stopWatch.Stop();

            session.Log(string.Format("Finished extraction (time taken: {0} ms)", stopWatch.ElapsedMilliseconds));

            File.Delete(zipFile);

            session.Log("Deleted zip file");

            return ActionResult.Success;
        }
    }
}