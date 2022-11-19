using System;
using System.Collections.Generic;
using System.Text;

namespace RdBuild.Shared.Enums
{
    public class CommonEnums
    {
        public enum ESystemCommands
        {
            RUT // Are you there command
        }
        public enum CoordinatorCommands
        {
            RegisterServerData,
            GetJobProcessor
        }

        public enum ESectionTypes
        {
            StringSection,
            ObjectSection
        }

        public enum EParameterNames
        {
            ServerName,
        }
    }
}
