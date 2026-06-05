using Web_Payment.Hubs;

namespace Web_Payment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            // CORS cho WPF client (localhost + LAN)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWPF", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });

                // SignalR cần AllowCredentials — không thể dùng AllowAnyOrigin cùng lúc
                options.AddPolicy("SignalRPolicy", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Không dùng HTTPS redirect — cho phép điện thoại truy cập qua HTTP IP LAN
            // app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowWPF");
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Map SignalR Hub
            app.MapHub<InvoiceHub>("/invoiceHub").RequireCors("SignalRPolicy");

            app.Run();
        }
    }
}