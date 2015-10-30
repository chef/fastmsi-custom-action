using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace CustomActionFastMsi
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult FastUnzip(Session session)
        {
            session.Log("Begin CustomAction FastUnzip");

            return ActionResult.Success;
        }
    }
}
