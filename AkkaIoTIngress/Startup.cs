﻿using System;
 using System.Linq;
 using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.NetCore;
 using AkkaIoTIngress.Actors.Device;
 using AkkaIoTIngress.Actors.Ingress;
 using AkkaIoTIngress.Actors.Machine;
 using AkkaIoTIngress.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaIoTIngress
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
            services.AddMvc();
            var actorSystem = ActorSystem.Create("ingress");
            services.AddSingleton<ITableStorage, TableStorage>();
            services.AddSingleton(actorSystem);

            foreach (var actorType in this.GetType().Assembly.GetTypes().Where(t => typeof(ActorBase).IsAssignableFrom(t) && ! t.IsAbstract))
            {
                services.AddTransient(actorType);
            }

            var resolver = new NetCoreDependencyResolver(services, actorSystem);
            actorSystem.AddDependencyResolver(resolver);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ActorSystem actorSystem)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            var ingressActor = actorSystem.ActorOf(actorSystem.DI().Props<IngressActor>(), "akka-iot-ingress");
            ingressActor.Tell(new IngressActor.StartListening());
        }
    }


}
