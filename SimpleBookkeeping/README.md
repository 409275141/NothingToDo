# 简易记账本 - MVP版本

这是一个面向小微商贩/社区团购团长的简易记账和赊账管理小程序，使用 .NET 8 + 微信小程序开发。

## 项目结构

```
SimpleBookkeeping/
├── Api/                          # .NET 8 WebAPI 后端
│   ├── Controllers/              # API控制器
│   │   ├── TransactionsController.cs   # 账本接口
│   │   └── CreditsController.cs        # 赊账接口
│   ├── Data/                     # 数据访问层
│   │   └── InMemoryDbContext.cs  # 内存数据库（MVP用）
│   ├── Models/                   # 数据模型
│   │   └── Transaction.cs        # 交易和赊账模型
│   ├── Program.cs                # 程序入口
│   └── SimpleBookkeeping.Api.csproj
│
└── MiniProgram/                  # 微信小程序前端
    ├── pages/
    │   ├── index/                # 首页（汇总面板）
    │   ├── transactions/         # 账本页面
    │   └── credits/              # 赊账页面
    ├── app.js                    # 小程序入口
    ├── app.json                  # 小程序配置
    └── project.config.json       # 项目配置
```

## 功能特性

### 1. 账本管理
- ✅ 记录收入/支出
- ✅ 分类管理（餐饮、交通、购物等）
- ✅ 收支明细列表
- ✅ 收支汇总统计

### 2. 赊账管理
- ✅ 记录客户赊账
- ✅ 部分还款/全部还款
- ✅ 赊账状态跟踪（未付/部分/已结清）
- ✅ 赊账汇总统计

### 3. 首页看板
- ✅ 本月结余展示
- ✅ 收入/支出统计
- ✅ 赊账未收提醒
- ✅ 快捷入口

## 快速开始

### 后端启动

```bash
cd SimpleBookkeeping/Api
dotnet run --urls="http://localhost:5000"
```

访问 Swagger: http://localhost:5000/swagger

### 小程序运行

1. 下载并安装 [微信开发者工具](https://developers.weixin.qq.com/miniprogram/dev/devtools/download.html)
2. 打开微信开发者工具，导入 `MiniProgram` 目录
3. 在 `app.js` 中修改 `apiUrl` 为你的后端地址
4. 编译运行即可

## API接口

### 账本接口
- `GET /api/transactions?userId={id}` - 获取交易列表
- `POST /api/transactions` - 新增交易
- `DELETE /api/transactions/{id}?userId={id}` - 删除交易
- `GET /api/transactions/summary?userId={id}` - 获取汇总

### 赊账接口
- `GET /api/credits?userId={id}` - 获取赊账列表
- `POST /api/credits` - 新增赊账
- `POST /api/credits/{id}/payment?userId={id}` - 记录还款
- `DELETE /api/credits/{id}?userId={id}` - 删除赊账
- `GET /api/credits/summary?userId={id}` - 获取汇总

## 后续优化建议

### 短期（MVP验证后）
1. **数据持久化**: 将内存存储替换为 SQLite/SQL Server
2. **用户系统**: 添加微信登录，实现多用户隔离
3. **数据导出**: 支持 Excel 导出，方便对账
4. **催款海报**: 生成赊账催款图片，方便分享

### 中期（产品完善）
1. **语音输入**: 快速记账，解放双手
2. **图片识别**: 拍照识别票据自动记账
3. **多账本**: 支持店铺/家庭等多账本切换
4. **数据统计**: 图表分析，经营报表

### 长期（商业化）
1. **会员体系**: 基础免费 + 高级功能付费
2. **云服务**: 数据云端同步，多设备使用
3. **行业模板**: 针对不同行业的定制模板
4. **营销工具**: 会员管理、优惠券等增值功能

## 技术栈

- **后端**: .NET 8 WebAPI
- **前端**: 微信小程序原生开发
- **数据存储**: 内存（MVP）→ SQLite/SQL Server（生产）
- **部署**: Docker + Linux/Windows Server

## 商业模式

- **目标用户**: 菜市场摊主、社区团购团长、小微商贩
- **定价策略**: 基础功能免费，高级功能 99 元/年
- **获客渠道**: 地推、行业社群、口碑传播

## 注意事项

⚠️ **当前是MVP版本**，使用内存存储，重启服务后数据会丢失。正式使用前请替换为持久化存储。

⚠️ **小程序域名配置**: 正式发布需要在微信公众平台配置合法域名。

⚠️ **AppID**: 请将 `project.config.json` 中的 `appid` 替换为你自己的小程序 AppID。
