using SomeAsmTranslator.Source.Listing;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;

namespace SomeAsmTranslator.Source.Listing.ListingWriters.Writers;

public class AssemblerListingWriterCsv : IAssemblerFileWriter
{
    private readonly IEnumerable<ListingLine> _listing;

    public AssemblerListingWriterCsv(IEnumerable<ListingLine> listing)
    {
        _listing = listing;
    }

    public void WriteToFile(string outFilePathWithoutExtension)
    {
        using (var streamWritter = new StreamWriter($"{outFilePathWithoutExtension}.csv"))
        {
            foreach (var line in _listing)
            {
                streamWritter.WriteLine(line.Csv);
            }
        }
    }
}



