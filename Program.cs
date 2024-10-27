using Librarian;
using Librarian.Cards.Anki;
using Librarian.No.Dictionaries;
using Librarian.No.Pronunciations;
using Librarian.No.Translations;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromDays(1);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IRazorRenderer, RazorRenderer>();

builder.AddNorwegianDictionary();
builder.AddNorwegianPronunciation();
builder.AddNorwegianTranslation();
builder.AddAnki();

var app = builder.Build();

app.UseRouting();
app.UseSession();
app.UseStaticFiles();
app.MapControllers();

app.Run();
