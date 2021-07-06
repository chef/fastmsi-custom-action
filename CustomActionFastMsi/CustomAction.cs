using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CustomActionFastMsi
{
    public class CustomActions
    {
        private static Session mySession = null;

        [CustomAction]
        public static ActionResult FastUnzip(Session session)
        {
            mySession = session;
            session.Log("Starting FastUnzip");

            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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

            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.FileName = Path.Combine(assemblyFolder , "7z.exe");
            proc.StartInfo.Arguments = string.Format("x \"{0}\" -aoa -o\"{1}\"", zipFile, Path.Combine(targetDir, appName));
            proc.EnableRaisingEvents = true;
            proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            proc.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);

            session.Log("Starting " + proc.StartInfo.FileName + " extraction using arguments: " + proc.StartInfo.Arguments);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            proc.Start();
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
            proc.WaitForExit();
            stopWatch.Stop();

            if (proc.ExitCode != 0) {
                session.Log(string.Format("7zip exited {0}", proc.ExitCode));
                return ActionResult.Failure;
            }

            session.Log(string.Format("Finished extraction (time taken: {0} ms)", stopWatch.ElapsedMilliseconds));

            File.Delete(zipFile);

            session.Log("Deleted zip file");

            return ActionResult.Success;
        }

        private static void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                mySession.Log(e.Data);
            }
        }

        private static void ErrorHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                mySession.Log(e.Data);
            }
        }
    }
}
