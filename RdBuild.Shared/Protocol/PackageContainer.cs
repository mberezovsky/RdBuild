using System.Collections.Generic;
using System.IO;
using RdBuild.Client;

namespace RdBuild.Shared.Protocol;

/// Package contains the data, sending to opposite site.
/// Package data consists of two parts - headers and body.
/// Headers are describing the body parts - its positions and size.
/// Bodies could be the following types:
///     - Binary stream
///     - Object data (including arrays)
///     - Parameters data
/// Headers are following:
///     - Header sections count
///     - Header Section List
///         - Header section type (enum data) - HeaderTypesEnum
///         - Header section length
public class PackageContainer
{
    class SectionData
    {
        public SectionHeader Header;
        public Section Body;
    }

    public IList<SectionHeader> SectionHeaderList { get; set; }

    public void SerializeTo(Stream stream)
    {

    }

}