using I8080Translator;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;

namespace SomeAsmTranslator.Source.Listing.ListingWriters.Writers;

public class AssemblerListingWriterBinary(IEnumerable<AssemblyLine> assemblyLineList) : IAssemblerFileWriter
{
    public void WriteToFile(string outFilePathWithoutExtension)
    {
        var orderedAsmLine = assemblyLineList.OrderBy(x => x.Address).ToList();

        AssemblyLine? previousLine = null;

        using var fs = new FileStream($"{outFilePathWithoutExtension}.bin", FileMode.Create, FileAccess.Write);
        
        foreach (var line in orderedAsmLine)
        {
            var prevBytes = previousLine?.AssembledAssemblyStatement.MachineCode;
            var bytes = line.AssembledAssemblyStatement.MachineCode;

            if (bytes.Length == 0) continue;

            if (previousLine == null)
            {
                fs.Write(bytes);
                previousLine = line;
                continue;
            }

            if (line?.Address != null && previousLine?.Address != null && prevBytes != null)
            {
                int dif = (int)(line.Address - (previousLine.Address + prevBytes.Length));
                for (int i = 0; i < dif; i++)
                    fs.WriteByte(0);
            }
                
            fs.Write(bytes);
            previousLine = line;
        }
    }
}