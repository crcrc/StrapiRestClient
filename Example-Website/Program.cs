using Microsoft.Extensions.Configuration;
using StrapiRestClient.Blocks.DataModels;
using StrapiRestClient.Extensions;

namespace Example_Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddStrapiRestClient(builder.Configuration, blocks =>
            {
                blocks.RegisterBlock<RichTextBlockComponent>("shared.rich-text")
                  .RegisterBlock<QuoteBlockComponent>("shared.quote")
                  .RegisterBlock<MediaBlockComponent>("shared.media")
                  .RegisterBlock<SliderBlockComponent>("shared.slider");
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{slug?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
