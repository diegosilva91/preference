using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.OpenApi.Models;
using preference.Models;
using MongoDB.Driver;
using server.Hubs;
using Microsoft.AspNetCore.SignalR;



namespace preference
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            var config = new ServerConfig();
            Configuration.Bind(config);
            var todoContext = new TodoContext(config.MongoDB);
            var repo = new TodoRepository(todoContext);
            services.AddSingleton<ITodoRepository>(repo);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "ToDo API",
                        Description = "A simple example ASP.NET Core Web API",
                        TermsOfService = new Uri("https://example.com/terms"),
                        Contact = new OpenApiContact
                        {
                            Name = "Shayne Boyer",
                            Email = string.Empty,
                            Url = new Uri("https://twitter.com/spboyer"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Use under LICX",
                            Url = new Uri("https://example.com/license"),
                        }
                    });
                }
            );

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                            builder =>
                            {
                                builder.WithOrigins("http://localhost:8080",
                                                    "http://localhost:8082",
                                                    "http://localhost:5000")
                                                    .AllowCredentials()
                                                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                                                    .AllowAnyHeader()
                                        .AllowAnyMethod();
                    });
            });
             services.AddLogging(builder =>
            {
                builder.AddConsole()
                    .AddDebug()
                    .AddFilter<ConsoleLoggerProvider>(category: null, level: LogLevel.Debug)
                    .AddFilter<DebugLoggerProvider>(category: null, level: LogLevel.Debug);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*app.UseSignalR(route =>
            {
                route.MapHub<IssueHub>("/task-hub");
            });*/
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
#if NoOptions
            #region UseWebSockets
            app.UseWebSockets();
            #endregion
#endif
   #if UseOptions
            #region UseWebSocketsOptions
            var webSocketOptions = new WebSocketOptions() 
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);
            #endregion
#endif   

            #if UseOptionsAO
            #region UeseWebSocketsOptionsAO
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            #endregion
            webSocketOptions.AllowedOrigins.Add("http://localhost:5000");
            webSocketOptions.AllowedOrigins.Add("http://localhost:8082");
            webSocketOptions.AllowedOrigins.Add("http://localhost:8080");

            app.UseWebSockets(webSocketOptions);
            
            #endif
               #region AcceptWebSocket
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/socket.io/")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });
#endregion

            app.UseCors(MyAllowSpecificOrigins);
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<IssueHub>("/issue-hub");
            });
            
              app.UseFileServer();
        
        }
        #region Echo
        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        #endregion

    }
}
