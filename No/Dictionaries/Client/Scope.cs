namespace Librarian.No.Dictionaries.Client;

[Flags]
public enum Scope
{
    None = 0,
    ExactLemma = 1,
    FullTextSearch = 2,
    InflectedForms = 4,
}
