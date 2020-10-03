using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cti.Dao;
using Cti.Dao.Base;
using Cti.Esl;
using Cti.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Beanstalk.Core;

namespace Cti
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CtiDaoContext>(options => options.UseMySQL(Configuration["Database:Cti"]));
            services.AddDbContext<FsDaoContext>(options => options.UseMySQL(Configuration["Database:Freeswitch"]));
            services.AddDbContext<SsoDaoContext>(options => options.UseMySQL(Configuration["Database:Sso"]));
            services.AddScoped<RootDao>();
            services.AddControllers();
            // 注入Redis服务
            services.AddSingleton(ConnectionMultiplexer.Connect(Configuration["Redis"]));

            // Beanstalk消息队列
            services.AddSingleton(new BeanstalkConnection(Configuration["MQ:Host"], Convert.ToUInt16(Configuration["MQ:Port"])));

            // 启动EevnetSocket
            services.AddHostedService<EslInbound>();
            
            // 任意域名跨域
            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials();
            }));
            // Websocket服务
            services.AddSignalR();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // 电话Websocket
                endpoints.MapHub<EslHub>("/wss/esl");
                // 监控Websocket
                endpoints.MapHub<Monitor.MonitorHub>("/wss/monitor", options => { });
            });
        }
    }
}
