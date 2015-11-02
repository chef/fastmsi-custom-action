using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

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

            session.Log("Starting extraction");

            fastzip.ExtractZip(zipFile, Path.Combine(targetDir, appName), null);

            session.Log("Finished extraction");

            File.Delete(zipFile);

            session.Log("Deleted zip file");

            return ActionResult.Success;
        }
    }
}