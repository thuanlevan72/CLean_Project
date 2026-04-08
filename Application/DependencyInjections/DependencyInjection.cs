using Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DependencyInjections
{
    /// <summary>
    /// DependencyInjection: Application layer service registration
    /// 
    /// Mô tả:
    /// - Extension method: AddApplication()
    /// - Được gọi ở Program.cs: builder.Services.AddApplication()
    /// - Đăng ký tất cả Application layer dependencies
    /// - Hides implementation details, expose simple interface
    /// 
    /// Services được register:
    /// 1. MediatR: CQRS command/query processing
    /// 2. AutoMapper: Object-to-object mapping
    /// 3. Behaviors/Middleware: Pipeline behaviors
    /// - Validation: Validate commands before handler
    /// - Performance: Log execution time
    /// - Caching: Cache query results
    /// - Error handling: Global exception handling
    /// 
    /// Dependency Injection Scope:
    /// - AddMediatR: Scoped (per request)
    /// - AddAutoMapper: Singleton (global, thread-safe)
    /// - Behaviors: Scoped (per request)
    /// 
    /// Extension Method Pattern:
    /// - this IServiceCollection: Mở rộng IServiceCollection
    /// - Cho phép: builder.Services.AddApplication()
    /// - Return: IServiceCollection (fluent syntax)
    /// 
    /// Flow at startup:
    /// 1. Program.cs: builder.Services.AddApplication()
    /// 2. This method called
    /// 3. All services registered to DI container
    /// 4. Later: When request comes, MediatR resolves handlers
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// AddApplication: Register Application layer services
        /// 
        /// Mục đích:
        /// - Provide single extension method to register all app services
        /// - Keep Program.cs clean (không list out tất cả services)
        /// - Centralize configuration logic
        /// 
        /// Tham số:
        /// - services: IServiceCollection (DI container)
        /// 
        /// Trả về:
        /// - IServiceCollection: Return same container (fluent pattern)
        /// 
        /// Ví dụ sử dụng:
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.Services.AddApplication();  // Register tất cả app services
        /// builder.Services.AddInfrastructure();  // Register infrastructure
        /// 
        /// Gọi từ Program.cs:
        /// - Dòng: builder.Services.AddApplication();
        /// - Đăng ký MediatR, AutoMapper, behaviors
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            /// <summary>
            /// 1. Register MediatR - CQRS Pattern
            /// 
            /// Mục đích:
            /// - Enable command/query handling pattern
            /// - Scan assembly để tìm Commands, Queries, Handlers
            /// - Automatically register các handlers
            /// 
            /// Cấu hình:
            /// - cfg.RegisterServicesFromAssembly(): Tìm handlers trong assembly
            /// - typeof(DependencyInjection).Assembly: Current assembly (Application.csproj)
            /// - Sẽ tìm tất cả classes implement IRequestHandler<,>
            /// 
            /// Ví dụ handlers tìm được:
            /// - CreateTodoCommandHandler: IRequestHandler<CreateTodoCommand, Guid>
            /// - GetMyTodosQueryHandler: IRequestHandler<GetMyTodosQuery, List<TodoDto>>
            /// - UpdateTodoCommandHandler: IRequestHandler<UpdateTodoCommand, bool>
            /// 
            /// Behavior Pipeline:
            /// - AddBehavior(): Cắm chốt vào MediatR pipeline
            /// - PerformanceBehavior<,>: Log execution time
            /// - Có thể thêm: ValidationBehavior, CachingBehavior, etc.
            /// 
            /// Chú ý:
            /// - MediatR scope: Scoped (new instance per request)
            /// - Handlers registered by convention (implement IRequestHandler)
            /// - Auto-wiring: MediatR tự inject dependencies vào handlers
            /// </summary>
            services.AddMediatR(cfg => {
                /// <summary>
                /// Đăng ký services từ assembly
                /// 
                /// Cách hoạt động:
                /// 1. Reflection: Scan tất cả types trong assembly
                /// 2. Tìm classes implement IRequestHandler<TRequest, TResponse>
                /// 3. Register: services.AddScoped<IRequestHandler<CreateTodo, Guid>, CreateTodoHandler>()
                /// 4. Auto-wire: MediatR.Send(command) → resolve handler, inject dependencies
                /// 
                /// Types tìm được:
                /// - IRequest<T>: Commands, Queries
                /// - IRequestHandler<TRequest, TResponse>: Handlers
                /// - INotification, INotificationHandler: Domain events
                /// - Custom behaviors
                /// </summary>
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                /// <summary>
                /// Thêm performance monitoring behavior
                /// 
                /// Mục đích:
                /// - Đo thời gian execution của mỗi command/query
                /// - Log performance metrics (> 1s → warning)
                /// - Detect slow operations
                /// 
                /// Hoạt động:
                /// - Intercept mỗi request/response
                /// - Record start time
                /// - Execute handler
                /// - Calculate duration
                /// - Log: "CreateTodoHandler executed in 123ms"
                /// 
                /// Lợi ích:
                /// - Performance monitoring built-in
                /// - No need add logging to every handler
                /// - Identify bottlenecks
                /// 
                /// Note:
                /// - Behavior execute before handler (pre-processing)
                /// - Can add validation, caching, etc. behaviors similarly
                /// - Order matters: Validation → Performance → Caching → Handler
                /// </summary>
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            });

            /// <summary>
            /// 2. Register AutoMapper - Object Mapping
            /// 
            /// Mục đức:
            /// - Enable object-to-object mapping
            /// - Scan assembly để tìm mapping profiles
            /// - Compile mapping expressions
            /// 
            /// Cấu hình:
            /// - cfg.AddMaps(): Tìm Profile classes
            /// - typeof(DependencyInjection).Assembly: Current assembly
            /// - Tìm tất cả classes kế thừa Profile
            /// 
            /// Ví dụ profiles tìm được:
            /// - MappingProfile: Contains TodoItem→TodoDto, Category→CategoryDto, etc.
            /// 
            /// Scope:
            /// - AutoMapper singleton (same instance globally)
            /// - Thread-safe
            /// - Compiled mappings cached
            /// 
            /// Cách sử dụng:
            /// - Inject IMapper vào handlers
            /// - _mapper.Map<TodoDto>(todoItem)
            /// - AutoMapper execute pre-compiled mapping
            /// 
            /// Note:
            /// - AddMaps vs AddProfile:
            ///   + AddMaps: Auto-scan assemblies (current usage)
            ///   + AddProfile: Manually add specific profile
            /// - Both work, AddMaps more convenient
            /// </summary>
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));

            /// <summary>
            /// Return services collection
            /// 
            /// Fluent pattern:
            /// - Cho phép chaining multiple extensions
            /// - Ví dụ:
            ///   builder.Services
            ///     .AddApplication()
            ///     .AddInfrastructure()
            ///     .AddApiServices();
            /// </summary>
            return services;
        }
    }
}
