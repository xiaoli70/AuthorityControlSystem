### Meiam.System - .NET 6 + Vue 2.0 + RBAC 企业级前后端分离权限框架


&nbsp;

## 框架介绍


- [x] 采用 服务 + 接口 的形式封装框架 （移除了仓库层）
- [x] 采用 REDIS 存储会话（用户信息 / 用户权限）- 更好的支持分布式应用、用户踢出、登录登出等功能
- [x] 采用 Autofac 依赖注入 IoC 容器, 实现批量自动注入所有服务
- [x] 采用 SqlSugar ORM 组件, DBFirst 开发模式, 封装 BaseService 数据库基础操作类
- [x] 采用 CSRedis 封装缓存层, 可结合 SqlSugar WithCache + RemoveDataCache 自动管理缓存
- [x] 采用 Swagger  自动生成 WebAPI 说明文档
- [x] 采用 Mapster 高性能对象映射, 代码书写更简洁 
- [x] 采用 NLog 日志组件, 集成  Microsoft.Extensions.Logging （ILogger 注入）操作日志
- [x] 采用 NLog.Web.AspNetCore 日志扩展组件, 增加 layout 更多属性
- [x] 采用 Quartz.Net 封装任务调度中心功能
- [x] 封装全局 会话 管理器 （用户 Token 会话管理）
- [x] 封装全局 权限 拦截器 （用户 Permission 权限管理）
- [x] 封装全局 异常 拦截器 （自动记录系统异常日志）
- [x] 封装全局 请求 拦截器 （自动记录接口请求日志）
- [x] 支持 用户功能权限、 用户数据权限分配
- [x] 支持 CORS 跨域配置
- [x] 支持代码生成器，自动生成 Model 、Service 、 IService 层
- [x] 支持跨方法事务方案,把事务提取到逻辑层进行统一事务提交,让你的代码更加漂亮
- [x] 支持多种数据库 Sqlite/SqlServer/MySql/PostgreSQL/Oracle 自由切换
- [x] 支持数据库 读写分离 和 多库 操作 
- [x] 计划 RabbitMQ 消息列队集成
- [ ] 计划 ImCore 及时通讯组件集成



&nbsp;

## 后端功能 （.Net Core）

| # | 模块功能                      |  项目文件                    | 说明
|---|-------------------------------|-------------------------------|-------------------------------
| 1 | 基础库 |Meiam.System.Common | 存放常用类库 、加密解密单元等
| 2 | 核心库 |Meiam.System.Core | 数据库、缓存相关类
| 3 | 扩展库 |Meiam.System.Extensions | 外部接口引用集成
| 4 | 服务层 |Meiam.System.Interfaces | Service 、 IServer, 基础服务类
| 5 | 模型层 |Meiam.System.Model | Entity、Dto、ViewModel
| 6 | 业务层 |Meiam.System.Hostd | WebAPI
| 7 | 工具库 |Meiam.System.Tools | 代码生成器
| 8 | 定时任务 |Meiam.System.Tasks | 任务调度中心


&nbsp;

## 使用文档 （相关配置）

### 导入数据库文件 

根据你选用的数据库, 导入 [Meiam.System-SQLServer.sql] 或 [Meiam.System-MySQL.sql] 数据库文件备份

### 替换日志配置文件

根据你选用的数据库, 复制 [NLog-SQLServer.config] 或 [NLog-MySQL.config] 到 Meiam.System.Hostd 目录，并重命名为 NLog.config

### 修改 `appsettings.json`  `Nlog.Config ` 中相关配置数  (数据库 REDIS 密码 等信息)


```c#
  // 数据库连接
  "DbConnection": {
    "ConnectionString": "Server=192.168.0.3;Database=MeiamSystem;UID=meiamsystem;Password=HApVpL8XhFFGz3Oy",
    "DbType": 1 //<= 配置你要选用的数据库 MySql = 0, SqlServer = 1, Sqlite = 2, Oracle = 3, PostgreSQL = 4 
  },
  // REDIS 配置 ， 默认使用了 3 个 DB
  "RedisServer": {
    "Cache": "192.168.0.3:6379,password=redis,preheat=5,idleTimeout=600,defaultDatabase=13,prefix=Cache",
    "Sequence": "192.168.0.3:6379,password=redis,preheat=5,idleTimeout=600,defaultDatabase=14,prefix=Sequence:",
    "Session": "192.168.0.3:6379,password=redis,preheat=5,idleTimeout=600,defaultDatabase=15,prefix=Session:"
  }, 
  // 跨域配置
  "Startup": {
    "ApiName": "Meiam.System",
    "ApiUrls": "http://*:19999",
    "AllowOrigins": "http://127.0.0.1:18888|http://localhost:18888"
  },
  // TOKEN 过期时间配置
  "AppSettings": {
    "Demo": true,  //<= 关闭演示模式 
    "WebSessionExpire": 24,
    "MiniProgramSessionExpire": 720
  },
  // 头像上传目录
  "AvatarUpload": {
    "AvatarDirectory": "D://wwwroot/avatars",
    "AvatarUrl": "/"
  }
```

