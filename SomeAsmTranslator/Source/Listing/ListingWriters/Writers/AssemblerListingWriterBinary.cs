using I8080Translator;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;

namespace SomeAsmTranslator.Source.Listing.ListingWriters.Writers;

public class AssemblerListingWriterBinary : IAssemblerFileWriter
{
    private readonly IEnumerable<AssemblyLine> _assemblyLineList;
    public AssemblerListingWriterBinary(IEnumerable<AssemblyLine> assemblyLineList)
    {
        _assemblyLineList = assemblyLineList;
    }

    public void WriteToFile(string outFilePathWithoutExtension)
    {
        var orderedAsmLine = _assemblyLineList.OrderBy(x => x.Address).ToList();

        AssemblyLine? previousLine = null;

        using (var fs = new FileStream($"{outFilePathWithoutExtension}.bin", FileMode.Create, FileAccess.Write))
        {
            foreach (var line in orderedAsmLine)
            {
                if (line.Bytes == null) { continue; };

                if (previousLine == null)
                {
                    fs.Write(line.Bytes);
                    previousLine = line;
                    continue;
                }

                int dif = (int)(line.Address - (previousLine.Address + previousLine.Bytes.Length));
                for (int i = 0; i < dif; i++)
                    fs.WriteByte(0);

                fs.Write(line.Bytes);
                previousLine = line;
            }
        }
    }
}



