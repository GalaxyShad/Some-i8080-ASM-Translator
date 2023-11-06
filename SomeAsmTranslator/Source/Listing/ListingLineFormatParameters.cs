namespace SomeAsmTranslator.Source.Listing;

public class ListingLineFormatParameters
{
    private const int MIN_ADR_WIDTH = 4;
    private const int MIN_MACHINE_CODE_COLUMN_WIDTH = 2;
    private const int MIN_LABEL_COLUMN_WIDTH = 5;
    private const int MIN_COMMENT_COLUMN_WIDTH = 7;

    public int AddressColumnWidth { get; set; } = MIN_ADR_WIDTH;
    public int MachineCodeColumnWidth { get; set; } = MIN_MACHINE_CODE_COLUMN_WIDTH;
    public int LabelColumnWidth { get; set; } = MIN_LABEL_COLUMN_WIDTH;
    public int AsmCodeColumnWidth { get; set; }
    public int CommentColumnWidth { get; set; } = MIN_COMMENT_COLUMN_WIDTH;
    public char CommentDividerChar { get; set; } = ';';

    public static ListingLineFormatParameters FromListing(IEnumerable<ListingLine> listing)
    {
        return new ListingLineFormatParameters
        {
            MachineCodeColumnWidth = Math.Max(
                MIN_MACHINE_CODE_COLUMN_WIDTH, 
                listing.Max(x => x.MachineCode.Length)),

            LabelColumnWidth = Math.Max(
                MIN_LABEL_COLUMN_WIDTH, 
                listing.Max(x => x.Label.Length)),

            AsmCodeColumnWidth = listing.Max(y => y.AsmCode.Length),

            CommentColumnWidth = Math.Max(
                MIN_COMMENT_COLUMN_WIDTH,
                listing.Max(x => x.Comment.Length)),
        };
    }
}



