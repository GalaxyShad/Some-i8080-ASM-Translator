using I8080Translator;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;

namespace SomeAsmTranslator.Source.Listing.ListingWriters.Writers;

public class AssemblerListingWriterIntelHex(IEnumerable<AssemblyLine> assemblyLineList) : IAssemblerFileWriter
{
    public void WriteToFile(string outFilePathWithoutExtension)
    {
        var orderedAsmLine = assemblyLineList.OrderBy(x => x.Address).ToList();

        using var writer = new StreamWriter($"{outFilePathWithoutExtension}.hex");
        
        var currentAddress = 0;
        foreach (var line in orderedAsmLine)
        {
            if (line.Address.HasValue)
            {
                currentAddress = line.Address.Value;
            }

            var bytes = line.AssembledAssemblyStatement.MachineCode;
            if (bytes.Length == 0) continue;

            var recordLength = bytes.Length;
            
            if (recordLength >= 256)
                throw new NotImplementedException(
                    "For now records with a length of 256 or greater does not supported for Intel HEX generation. " +
                    "Please ensure that the assembled machine code for each instruction fits within a single record of length less than 256.");
            
            var recordType = "00";
            
            var checksum = (byte)-(recordLength + currentAddress / 256 + currentAddress % 256 + bytes.Sum(b => b));
            var dataString = bytes.Aggregate("", (a, b) => a + b.ToString("X2"));
            
            string record = $":{recordLength:X2}{currentAddress:X4}{recordType}{dataString}{checksum:X2}";
            writer.WriteLine(record);

            currentAddress += bytes.Length;
        }

        // Write the end of file record
        writer.WriteLine(":00000001FF");
    }
}