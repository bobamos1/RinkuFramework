namespace Rinku.Tools.Data.Parsers;
public struct DTInfo<TParserDel> {
    public required ParserBase JSON;
    public required string Name;
    public required Type Type;
    public required TParserDel Parser;
}