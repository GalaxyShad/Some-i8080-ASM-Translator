using SomeAsmTranslator.Source.Listing;
using SomeAsmTranslator.Source.Listing.ListingWriters.Interfaces;
using SomeAsmTranslator.Source.Listing.ListingWriters.Writers;

namespace I8080Translator;

partial class Program
{
    public class AssemblerListingWriterFactory
    {
        private readonly IEnumerable<ListingLine> _listing;
        private readonly IEnumerable<AssemblyLine> _assemblyLineList;

        public AssemblerListingWriterFactory(
            IEnumerable<ListingLine> listing,
            IEnumerable<AssemblyLine> assemblyLineList)
        {
            _listing = listing;
            _assemblyLineList = assemblyLineList;
        }

        public IEnumerable<IAssemblerFileWriter> Get(ArgumentsOptions opt)
        {
            var formatParams = ListingLineFormatParameters.FromListing(_listing);

            yield return new AssemblerListingWriterText(
                _listing,
                formatParams,
                AssemblerListingWriterText.Extension.Txt
            );

            if (opt.IsGenerateWord)
                yield return new AssemblerListingWriterWord(_listing);

            if (opt.IsGenerateBinary)
                yield return new AssemblerListingWriterBinary(_assemblyLineList);

            if (opt.IsGenerateIntelHex)
                yield return new AssemblerListingWriterIntelHex(_assemblyLineList);

            if (opt.IsGenerateCsv)
                yield return new AssemblerListingWriterCsv(_listing);

            if (opt.IsGenerateMarkdown)
            {
                formatParams.CommentDividerChar = '|';

                yield return new AssemblerListingWriterText(
                    _listing,
                    formatParams,
                    AssemblerListingWriterText.Extension.MarkDown
                );
            }
        }
    }

}



