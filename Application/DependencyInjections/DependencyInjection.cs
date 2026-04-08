using Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DependencyInjections
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                // SỬA DÒNG NÀY: Trỏ type vào chính class DependencyInjection hiện tại
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                // Cắm chốt đo hiệu năng thời gian
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            });

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));

            return services;
        }
    }
}
