using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RdBuild.Shared.Enums;
using RdBuild.Shared.SectionData;

namespace RdBuild.Shared.Protocol
{
    public class Request<TCommandEnum> where TCommandEnum : Enum
    {
        private TCommandEnum command;
        private List<SectionHeader> m_headers;
        private List<Section> m_sections;


        public TCommandEnum Command { get; set; }

        public TObject GetObjectSection<TObject>()
        {
            m_headers.FirstOrDefault(item => item.SectionType == EHeaderSectionType.ObjectSection)
        }
    }

    public class Response
    {
        private List<SectionHeader> m_headers;
        private List<Section> m_sections;

        public enum EResponseCode
        {
            OK,
            InternalError,
            CommandNotFound
        }

        public EResponseCode Code { get; set; }

        public EResponseCode SetText(string res)
        {
            SetSection(res);
            return Response.EResponseCode.OK;
        }

        private void SetSection(string res)
        {
            m_sections ??= new List<Section>();
            m_sections.Add(new StringSection(res));
        }
    }

    class CommonRequestParametersReader
    {
        private static Dictionary<EHeaderSectionType, Func<BinaryReader, SectionHeader>> s_sectionTypeReader = new();

        public IEnumerable<SectionHeader> ReadHeaders(BinaryReader reader)
        {
            int headersCount = reader.ReadInt32();
            for (int i = 0; i < headersCount; i++)
            {
                var sectionType = (EHeaderSectionType)reader.ReadInt32();
                if (s_sectionTypeReader.TryGetValue(sectionType, out var sectionHeaderHandler))
                    yield return sectionHeaderHandler.Invoke(reader);
                else
                    throw new WrongSectionTypeException();
            }
        }

        public static void RegisterSectionHeaderReader(EHeaderSectionType sectionType,
            Func<BinaryReader, SectionHeader> handler)
        {
            if (s_sectionTypeReader.ContainsKey(sectionType))
                throw new SectionHeaderReaderAlreadyRegistered();
            s_sectionTypeReader[sectionType] = handler;
        }
    }

    class WrongSectionTypeException : Exception {}

    class SectionHeaderReaderAlreadyRegistered : Exception {}
}
