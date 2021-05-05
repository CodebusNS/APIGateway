# APIGateway

`APIGateway`是一款基于`.NetCore + Ocelot`开发的一款智能API网关。在保留了`Ocelot`现有功能的情况下，我们新增了配置存储/读取方式、缓存配置存储/读取方式、自定义限流、鉴权、熔断等。 新增配置定时生效、配置实施修改生效、UI界面管理、调用链、集群功能等。`APIGateway` 简化了`Ocelot` 的配置方式（通过管理界面即可完成服务管理），如果说你们使用的技术栈为.NetCore，并且有想上微服务架构的意向，那么`APIGateway`将会是一个不错的选择。

# 框架组件介绍

- APIGateway.Core：Api核心和Ocelot扩展组件
  - Business：业务层，用于配置设置或读取
  - Common：通用层
  - DbRepository：网关配置仓库
  - Model：模型层
  - OcelotAddin：基于Ocelot开发的扩展插件
- APIGateway.Portal：提供网关管理WebApi

# 首次使用

## 数据迁移

> 当前项目配置存储方式为MySql，我们使用了EFCore组件，CodeFirst方式。所以在使用该项目时请先做数据迁移，将模型映射至你自己的数据库中
>
> `Add-Migration init`
>
> `Update-Database`

## DI和Middleware

```c#
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<GatewayDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseMySQL(Configuration["MySql:ConnectionString"]);
            }, ServiceLifetime.Scoped);

            // 注入Ocelot并添加AddConfigurationRepository网关自定义配置
            services.AddOcelot().AddConfigurationRepository(option =>
            {
                // 网关配置是否自动刷新
                option.AutoUpdate = false;
                
                // 网关配置刷新周期
                option.UpdateInterval = 10 * 1000;
            });

            // 注入网关管理中间件
            services.AddScoped<IGatewayContract, GatewayServices>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(opt =>
            {
                opt.MapControllers();
            });

            // 添加基于Ocelot的自定义中间件
            app.UseGateway().Wait();
        }
```

## 通过WebApi获取或设置网关配置

```c#
        [HttpGet]
        public async Task<AjaxResult> Get()
        {
            var result = await _gatewayContract.Get();
            return result.ToAjaxResult();
        }

        [HttpPost]
        public async Task<AjaxResult> Post([FromBody] FileConfiguration fileConfiguration)
        {
            var result = await _gatewayContract.Post(fileConfiguration);
            return result.ToAjaxResult();
        }
```



# 技术栈

- 后端
  - C#
  - .NetCore(3.x)
  - Ocelot(15.0.7)
  - EFCore
- 前端
  - JavaScript/JQuery
  - CSS
  - HTML

# 项目进度

关于网关配置参数请参Ocelot官方[文档](https://ocelot.readthedocs.io/en/latest/)。

截止到目前，codebus.apigateway框架已经可以满足API网关的基本功能了，计划中的功能点，均已得到较高水准的实现，体功能点完成进度如下所示：

- [x] 通过数据库动态配置网关
- [x] 通过API动态获取/配置网关
- [x] 路由
- [x] 聚合请求
- [x] 结合Consul或Eureka做服务发现
- [x] WebSocket
- [x] Kubernetes
- [x] 熔断限流
- [x] 服务治理
- [x] 负载均衡
- [x] 标头/方法/查询字符串/申明转换
- [ ] 记录/跟踪/关联
- [ ] UI管理界面
- [ ] 缓存配置集群部署
- [ ] 鉴权

# 贡献

我们很乐意接受来自社区的贡献，所以有任何建议或意见请及时评论，当然更欢迎你也能在`APIGateway`中留下代码。