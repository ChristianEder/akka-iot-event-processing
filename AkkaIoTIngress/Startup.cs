using Akka.Actor;
using AkkaIoTIngress.Actors.Ingress;
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

            services.AddSingleton<ITableStorage, TableStorage>();
            services.AddSingleton(_ => ActorSystem.Create("ingress"));
            services.AddSingleton<IngressActorProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IngressActorProvider ingressActorProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            var ingressActor = ingressActorProvider.Get();
            ingressActor.Tell(new IngressActor.StartListening());
        }
    }


}
